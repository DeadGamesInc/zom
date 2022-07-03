using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

public class SnapshotCamera : MonoBehaviour {
    private Camera _camera;
    private int _layer;

    public Vector3 DefaultPosition = new(0, 0, 3), DefaultRotation = new(0, 0, 0), DefaultScale = new(0.3f, 0.3f, 0.3f);

    private SnapshotCamera () { }

    public static SnapshotCamera Create(int layer) {
        var cameraObject = new GameObject("SnapshotCamera");
        var camera = cameraObject.AddComponent<Camera>();

        camera.cullingMask = 1 << layer;
        camera.orthographic = false;
        camera.orthographicSize = 1;
        camera.clearFlags = CameraClearFlags.SolidColor;
        camera.backgroundColor = Color.clear;
        camera.nearClipPlane = 0.1f;
        camera.enabled = false;
        
        var snapshotCamera = cameraObject.AddComponent<SnapshotCamera>();
        
        snapshotCamera._camera = camera;
        snapshotCamera._layer = layer;

        DontDestroyOnLoad(cameraObject);
        return snapshotCamera;
    }
    
    public Texture2D TakeObjectSnapshot(GameObject target, int width = 128, int height = 128) => 
        TakeObjectSnapshot(target, Color.clear, DefaultPosition, Quaternion.Euler(DefaultRotation), DefaultScale, width, height);

    public Texture2D TakeObjectSnapshot(GameObject target, Color backgroundColor, int width = 128, int height = 128) =>
        TakeObjectSnapshot(target, backgroundColor, DefaultPosition, Quaternion.Euler(DefaultRotation), DefaultScale, width, height);

    public Texture2D TakeObjectSnapshot(GameObject gameObject, Vector3 positionOffset, Quaternion rotation, Vector3 scale, 
        int width = 128, int height = 128) =>
        TakeObjectSnapshot(gameObject, Color.clear, positionOffset, rotation, scale, width, height);

    public Texture2D TakeObjectSnapshot(GameObject gameObject, Color backgroundColor, Vector3 positionOffset, Quaternion rotation, 
        Vector3 scale, int width = 128, int height = 128) {
        var previousState = PrepareObject(gameObject, positionOffset, rotation, scale);
        var snapshot = TakeSnapshot(backgroundColor, width, height);
        previousState.Restore();
        return snapshot;
    }
    
    public Texture2D TakePrefabSnapshot (GameObject prefab, int width = 128, int height = 128) =>
        TakePrefabSnapshot(prefab, Color.clear, DefaultPosition, Quaternion.Euler(DefaultRotation), DefaultScale, width, height);

    public Texture2D TakePrefabSnapshot (GameObject prefab, Color backgroundColor, int width = 128, int height = 128) =>
        TakePrefabSnapshot(prefab, backgroundColor, DefaultPosition, Quaternion.Euler(DefaultRotation), DefaultScale, width, height);

    public Texture2D TakePrefabSnapshot (GameObject prefab, Vector3 positionOffset, Quaternion rotation, Vector3 scale, 
        int width = 128, int height = 128) =>
        TakePrefabSnapshot(prefab, Color.clear, positionOffset, rotation, scale, width, height);

    public Texture2D TakePrefabSnapshot (GameObject prefab, Color backgroundColor, Vector3 positionOffset, Quaternion rotation, 
        Vector3 scale, int width = 128, int height = 128) {
        var instance = PreparePrefab(prefab, positionOffset, rotation, scale);
        var snapshot = TakeSnapshot(backgroundColor, width, height);
        DestroyImmediate(instance);
        return snapshot;
    }
    
    public static FileInfo SaveSnapshot(Texture2D texture, string filename) => 
        SaveSnapshot(texture.EncodeToPNG(), filename);
    
    public static FileInfo SaveSnapshot(byte[] data, string filename) {
        var directory = Directory.CreateDirectory(Path.Combine(Application.dataPath, "../snapshots")).FullName;
        filename = SanitizeFilename(filename) + ".png";
        var path = Path.Combine(directory, filename);
        File.WriteAllBytes(path, data);
        return new FileInfo(path);
    }

    private static string SanitizeFilename(string filename) {
        var invalidChars = Regex.Escape(new string(Path.GetInvalidFileNameChars()));
        var invalidRegEx = string.Format(@"([{0}]*\.+$)|([{0}]+)", invalidChars);
        return Regex.Replace(filename, invalidRegEx, "_");
    }

    private void SetLayersRecursively(GameObject target) {
        foreach (var child in target.GetComponentsInChildren<Transform>(true))
            child.gameObject.layer = _layer;
    }

    private GameObjectStateSnapshot PrepareObject(GameObject target, Vector3 positionOffset, Quaternion rotation, Vector3 scale) {
        var objectSnapshot = new GameObjectStateSnapshot(target);

        target.transform.position = transform.position + positionOffset;
        target.transform.rotation = rotation;
        target.transform.localScale = scale;
        SetLayersRecursively(target);

        return objectSnapshot;
    }

    private GameObject PreparePrefab(GameObject prefab, Vector3 positionOffset, Quaternion rotation, Vector3 scale) {
        var newObject = Instantiate(prefab, transform.position + positionOffset, rotation);
        newObject.transform.localScale = scale;
        SetLayersRecursively(newObject);
        return newObject;
    }

    private Texture2D TakeSnapshot(Color backgroundColor, int width, int height) {
        _camera.backgroundColor = backgroundColor;
        _camera.targetTexture = RenderTexture.GetTemporary(width, height, 24);
        _camera.Render();
        var previouslyActiveRenderTexture = RenderTexture.active;
        var targetTexture = _camera.targetTexture;
        RenderTexture.active = targetTexture;

        var texture = new Texture2D(targetTexture.width, targetTexture.height, TextureFormat.ARGB32, false);
        texture.ReadPixels(new Rect(0, 0, targetTexture.width, targetTexture.height), 0, 0);
        texture.Apply(false);

        RenderTexture.active = previouslyActiveRenderTexture;

        _camera.targetTexture = null;
        RenderTexture.ReleaseTemporary(_camera.targetTexture);

        return texture;
    }
    
    private readonly struct GameObjectStateSnapshot {
        private readonly GameObject _gameObject;
        private readonly Vector3 _position, _scale;
        private readonly Quaternion _rotation;
        private readonly Dictionary<GameObject, int> _layers;

        public GameObjectStateSnapshot(GameObject gameObject) {
            _gameObject = gameObject;
            _position = gameObject.transform.position;
            _rotation = gameObject.transform.rotation;
            _scale = gameObject.transform.localScale;
            _layers = new Dictionary<GameObject, int>();
            foreach (var transform in gameObject.GetComponentsInChildren<Transform>(true)) {
                var child = transform.gameObject;
                _layers.Add(child, child.layer);
            }
        }

        public void Restore() {
            _gameObject.transform.position = _position;
            _gameObject.transform.rotation = _rotation;
            _gameObject.transform.localScale = _scale;
            foreach (var entry in _layers) entry.Key.layer = entry.Value;
        }
    }
}
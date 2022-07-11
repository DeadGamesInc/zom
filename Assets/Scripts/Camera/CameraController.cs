using System;
using System.Collections;
using System.Linq;
using Cinemachine;
using Unity.VisualScripting;
using UnityEngine;

public class CameraController : MonoBehaviour {
    public static int ACTIVE = 20;
    public static int INACTIVE = 0;
    [SerializeField] public CinemachineVirtualCamera PrimaryCamera;
    [SerializeField] public CinemachineVirtualCamera FreeCamera;
    [SerializeField] public CinemachineVirtualCamera ActiveCamera;
    public int LocationIndex;
    
    public static CameraController Get() {
        GameObject cameraController = GameObject.Find("CameraController");
        if (cameraController != null) {
            return cameraController.GetComponent<CameraController>();
        }

        throw new Exception("CameraController not found in scene");
    }

    public void PrioritizeCamera(CinemachineVirtualCamera camera) {
        if (ActiveCamera != null) ActiveCamera.Priority = INACTIVE;
        if (camera == FreeCamera) {
            HandPosition.Get().IsFreeCamera = true;
        } else if (ActiveCamera == FreeCamera) {
            ActiveCamera.gameObject.GetComponentInParent<FreeCamera>().InControl = false;
            HandPosition.Get().IsFreeCamera = false;
        }

        camera.Priority = ACTIVE;
        ActiveCamera = camera;
    }

    public void TryRevokeFreeCameraControl() {
        if (ActiveCamera != FreeCamera) return;
        FreeCamera.gameObject.GetComponentInParent<FreeCamera>().InControl = false;
    }

    public void TryGiveFreeCameraControl() {
        if (ActiveCamera != FreeCamera) return;
        ActiveCamera.gameObject.GetComponentInParent<FreeCamera>().InControl = true;
    }

    public void Toggle() {
        if (ActiveCamera == PrimaryCamera) {
            PrioritizeFreeCamera();
        } else {
            PrioritizePrimary();
        }
    }

    public void PrioritizePrimary() {
        PrioritizeCamera(PrimaryCamera);
    }

    public void PrioritizeFreeCamera() {
        PrioritizeCamera(FreeCamera);
        FreeCamera.gameObject.GetComponentInParent<FreeCamera>().InControl = true;
    }

    // Start is called before the first frame update
    void Start() {
        // PrioritizeFreeCamera();
        StartCoroutine(InitializeFreeCamera());
    }

    public void FocusLocation(FreeCameraProperties props) {
        PrioritizeFreeCamera();
        var cameraRig = FreeCamera.GetComponentInParent<FreeCamera>();
        cameraRig.newPosition = props.NewPosition;
        cameraRig.newRotation = Quaternion.Euler(props.NewRotation);
        cameraRig.newZoom = props.NewZoom;
    }

    public void Forwards() {
        var lc = LevelController.Get();
        var locations = lc.Locations.Concat(lc.EmptyLocations).OrderBy(l => l.GetLocationBase().Index).ToList();
        var lastIndex = LocationIndex == locations.Count - 1;
        LocationIndex = ActiveCamera != FreeCamera || lastIndex ? 0 : LocationIndex + 1;
        FocusLocation(locations[LocationIndex].GetLocationBase().CameraPosition);
    }

    public void Backwards() {
        var lc = LevelController.Get();
        var locations = lc.Locations.Concat(lc.EmptyLocations).OrderBy(l => l.GetLocationBase().Index).ToList();
        var firstIndex = LocationIndex == 0;
        LocationIndex = ActiveCamera != FreeCamera || firstIndex ? locations.Count - 1 : LocationIndex - 1;
        FocusLocation(locations[LocationIndex].GetLocationBase().CameraPosition);
    }

    private IEnumerator InitializeFreeCamera() {
        var levelController = LevelController.Get();
        while (levelController._map == null) yield return null;
        var startLocation = LevelController.Get().Locations.First().GetLocationBase();
        FocusLocation(startLocation.CameraPosition);
    }

    // Update is called once per frame
    void Update() {
    }
}
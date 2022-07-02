using System;
using Cinemachine;
using UnityEngine;

public class CameraController : MonoBehaviour {
    public static int ACTIVE = 20;
    public static int INACTIVE = 0;
    [SerializeField] public CinemachineVirtualCamera PrimaryCamera;
    [SerializeField] public CinemachineVirtualCamera ActiveCamera;

    public static CameraController Get() {
        GameObject cameraController = GameObject.Find("CameraController");
        if (cameraController != null) {
            return cameraController.GetComponent<CameraController>();
        }

        throw new Exception("CameraController not found in scene");
    }

    public void PrioritizeCamera(CinemachineVirtualCamera camera) {
        ActiveCamera.Priority = INACTIVE;
        camera.Priority = ACTIVE;
    }

    public void PrioritizePrimary() {
        PrioritizeCamera(PrimaryCamera);
    }

    // Start is called before the first frame update
    void Start() {
        PrioritizePrimary();
    }

    // Update is called once per frame
    void Update() {
    }
}
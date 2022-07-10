using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class LocationCamera : MonoBehaviour {
    private CinemachineOrbitalTransposer orbitalTransposer;

    public static GameObject Create(GameObject character) {
        // Create object, attach script & cinemachine camera
        GameObject cameraObject = new GameObject("CharacterCamera");
        LocationCamera characterCamera = cameraObject.AddComponent<LocationCamera>();
        CinemachineVirtualCamera virtualCamera = cameraObject.AddComponent<CinemachineVirtualCamera>();
        // Add orbital body and set character as target
        characterCamera.orbitalTransposer = virtualCamera.AddCinemachineComponent<CinemachineOrbitalTransposer>();
        virtualCamera.AddCinemachineComponent<CinemachineHardLookAt>();
        virtualCamera.Follow = character.transform;
        virtualCamera.LookAt = character.transform;
        virtualCamera.Priority = CameraController.INACTIVE;

        return cameraObject;
    }
    
    // Start is called before the first frame update
    void Start() {
        orbitalTransposer.m_FollowOffset = new Vector3(0, 20, -100);
        orbitalTransposer.m_XAxis = new AxisState(); // Disables mouse affect on camera
    }

    // Update is called once per frame
    void FixedUpdate() {
        orbitalTransposer.m_Heading.m_Bias += 0.1f;
    }
}
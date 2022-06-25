using Cinemachine;
using UnityEngine;

public class DefenseCamera : MonoBehaviour {
    private CinemachineOrbitalTransposer orbitalTransposer;
    
    public static GameObject Create(GameObject defenseNode) {
        Vector3 from = defenseNode.transform.position;
        Vector3 to = Vector3.zero;
        Vector3 dir = (to - from).normalized;
        dir = new Vector3(dir.x, 0, dir.z);
        // Create object, attach script & cinemachine camera
        GameObject cameraObject = new GameObject("CharacterCamera");
        DefenseCamera defenseCamera = cameraObject.AddComponent<DefenseCamera>();
        CinemachineVirtualCamera virtualCamera = cameraObject.AddComponent<CinemachineVirtualCamera>();

        cameraObject.transform.position = defenseNode.transform.position + dir * 100f + new Vector3(0f, 40f, 0f);
        // Add orbital body and set character as target
        // defenseCamera.orbitalTransposer = virtualCamera.AddCinemachineComponent<CinemachineOrbitalTransposer>();
        virtualCamera.AddCinemachineComponent<CinemachineHardLookAt>();
        virtualCamera.Follow = defenseNode.transform;
        virtualCamera.LookAt = defenseNode.transform;
        virtualCamera.Priority = 1;

        return cameraObject;
    }
    
    // Start is called before the first frame update
    void Start() {
        // orbitalTransposer.m_FollowOffset = new Vector3(0, 20, -100);
        // orbitalTransposer.m_XAxis = new AxisState(); // Disables mouse affect on camera
    }

    // Update is called once per frame
    void FixedUpdate() {
        // orbitalTransposer.m_Heading.m_Bias += 0.1f;
    }
}
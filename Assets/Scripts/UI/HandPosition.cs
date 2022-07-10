using System;
using UnityEngine;

public class HandPosition : MonoBehaviour {
    [SerializeField] private GameObject _cameraController;
    [SerializeField] public Vector3 Offset;
    [SerializeField] public Vector3 FreeCameraOffset;
    public bool IsFreeCamera;

    public static HandPosition Get() {
        GameObject handPosition = GameObject.Find("HandPosition");
        if (handPosition != null) {
            return handPosition.GetComponent<HandPosition>();
        }

        throw new Exception("HandPosition not found in scene");
    }

    // Update is called once per frame
    void Update() {
        Vector3 cameraPos = _cameraController.transform.position;
        var cameraTransform = _cameraController.transform;
        var cameraRot = new Vector3(0f, cameraTransform.rotation.eulerAngles.y, 0f);
        transform.position = Functions.RotatePointAroundPivot(cameraPos + (IsFreeCamera ? FreeCameraOffset : Offset),
            cameraPos, cameraRot);
        transform.rotation = _cameraController.transform.rotation;
    }
}
using UnityEngine;

public struct FreeCameraProperties {
    public Vector3 NewPosition;
    public Vector3 NewRotation;
    public Vector3 NewZoom;

    public FreeCameraProperties(Vector3 newPosition, Vector3 newRotation, Vector3 newZoom) {
        NewPosition = newPosition;
        NewRotation = newRotation;
        NewZoom = newZoom;
    }
}
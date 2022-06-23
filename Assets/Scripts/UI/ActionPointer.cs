using UnityEngine;

public class ActionPointer : MonoBehaviour {
    private Vector3 targetPosition;
    private RectTransform pointerRectTransform;
    public QueuedCommand Command;
    
    private void Start() {
        Vector3 to = Command.Target.transform.position;
        Vector3 from =  Command.Source.transform.position;
        transform.rotation = Quaternion.FromToRotation(from, to);
        transform.localEulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
        Vector3 dir = (to - from).normalized;
        transform.position = Vector3.MoveTowards(from, to, 40f);
    }

    public void FixedUpdate() {
        transform.Rotate(1f, 0f, 0f);
    }
}

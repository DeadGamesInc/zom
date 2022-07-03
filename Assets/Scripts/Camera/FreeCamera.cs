using System;
using Cinemachine;
using UnityEngine;

public class FreeCamera : MonoBehaviour {
    public Transform CameraTransform;
    [SerializeField] public float MovementSpeed;
    [SerializeField] public float MovementTime;
    private Vector3 _dragStartPosition;
    private Vector3 _dragCurrentPosition;
    private Vector3 _rotateStartPosition;
    private Vector3 _rotateCurrentPosition;

    public Vector3 ZoomAmount;
    public float RotationAmount;

    public Vector3 newPosition;
    public Quaternion newRotation;
    public Vector3 newZoom;

    public bool InControl = false;

    public static FreeCamera Get() {
        GameObject freeCamera = GameObject.Find("FreeCamera");
        if (freeCamera != null) {
            return freeCamera.GetComponent<FreeCamera>();
        }

        throw new Exception("FreeCamera not found in scene");
    }

    private void Start() {
        newPosition = transform.position;
        newRotation = transform.rotation;
        newZoom = CameraTransform.localPosition;
    }

    // Update is called once per frame
    void Update() {
        if (!InControl) return;
        HandleMouseInput();
        HandleMovementInput();
    }

    private void HandleMouseInput() {
        if (Input.mouseScrollDelta.y != 0) {
            Debug.Log("scrolling");
            newZoom += Input.mouseScrollDelta.y * ZoomAmount;
        }

        if (Input.GetMouseButtonDown(0)) {
            Plane plane = new Plane(Vector3.up, Vector3.zero);

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            float entry;

            if (plane.Raycast(ray, out entry)) {
                _dragStartPosition = ray.GetPoint(entry);
            }
        }

        if (Input.GetMouseButton(0)) {
            Plane plane = new Plane(Vector3.up, Vector3.zero);

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            float entry;

            if (plane.Raycast(ray, out entry)) {
                _dragCurrentPosition = ray.GetPoint(entry);
                newPosition = transform.position + _dragStartPosition - _dragCurrentPosition;
            }
        }

        if (Input.GetMouseButtonDown(1)) {
            _rotateStartPosition = Input.mousePosition;
        }
        if (Input.GetMouseButton(1)) {
            _rotateCurrentPosition = Input.mousePosition;
            Vector3 difference = _rotateStartPosition - _rotateCurrentPosition;
            _rotateStartPosition = _rotateCurrentPosition;
            newRotation *= Quaternion.Euler(Vector3.up * (difference.x / 5f));
        }
    }

    private void HandleMovementInput() {
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) {
            newPosition += transform.forward * MovementSpeed;
        }

        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) {
            newPosition += transform.forward * -MovementSpeed;
        }

        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) {
            newPosition += transform.right * MovementSpeed;
        }

        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) {
            newPosition += transform.right * -MovementSpeed;
        }

        transform.position = Vector3.Lerp(transform.position, newPosition, Time.deltaTime * MovementTime);
        transform.rotation = Quaternion.Lerp(transform.rotation, newRotation, Time.deltaTime * MovementTime);
        CameraTransform.localPosition =
            Vector3.Lerp(CameraTransform.localPosition, newZoom, Time.deltaTime * MovementTime);
    }
}
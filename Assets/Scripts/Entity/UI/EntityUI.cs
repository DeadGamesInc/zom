using TMPro;
using UnityEngine;

public class EntityUI : MonoBehaviour {
    [SerializeField] protected GameObject statusText;
    [SerializeField] public GameObject HealthBar;
    [SerializeField] public GameObject Target;
    [SerializeField] public GameObject _camera;

    // Start is called before the first frame update
    void Start() {
        _camera = GameObject.Find("CameraController");
        HealthBar.GetComponent<HealthBar>().Target = Target.GetEntity();
    }
    
    public void SetCharacterText(string text) {
        statusText.GetComponent<TextMeshProUGUI>().text = text;
    }

    // Update is called once per frame
    private void FixedUpdate() {
        transform.position = Target.transform.position;
        transform.rotation = _camera.transform.rotation;
    }
}
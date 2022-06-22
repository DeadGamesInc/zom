using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocationControl : MonoBehaviour {
    [SerializeField] public bool BasicLocation;
    private LevelController _levelController;

    public void Start() {
        _levelController = GameObject.Find("LevelController")?.GetComponent<LevelController>();
    }

    public void OnMouseEnter() {
        if (_levelController == null) return;
        var basic = BasicLocation ? "BASIC " : "";
        _levelController.SetStatusText($"{basic}LOCATION SELECTED");
        _levelController.SelectedLocation = gameObject;
    }

    public void OnMouseExit() {
        if (_levelController == null) return;
        _levelController.SetStatusText("");
        _levelController.SelectedLocation = null;
    }
}

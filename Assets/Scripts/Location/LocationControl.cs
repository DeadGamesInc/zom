using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocationControl : MonoBehaviour {
    private LevelController _levelController;

    public void Start() {
        _levelController = GameObject.Find("LevelController")?.GetComponent<LevelController>();
    }

    public void OnMouseEnter() {
        if (_levelController == null) return;
        _levelController.SetStatusText("LOCATION SELECTED");
        _levelController.SelectedLocation = gameObject;
    }

    public void OnMouseExit() {
        if (_levelController == null) return;
        _levelController.SetStatusText("");
        _levelController.SelectedLocation = null;
    }
}

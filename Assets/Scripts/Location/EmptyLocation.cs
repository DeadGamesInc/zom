using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmptyLocation : MonoBehaviour {
    private LevelController _levelController;

    public void Start() {
        _levelController = GameObject.Find("LevelController")?.GetComponent<LevelController>();
    }

    public void OnMouseEnter() {
        if (_levelController == null) return;
        _levelController.SetStatusText("EMPTY LOCATION");
        _levelController.SelectedEmptyLocation = gameObject;
    }

    public void OnMouseExit() {
        if (_levelController == null) return;
        _levelController.SetStatusText("");
        _levelController.SelectedEmptyLocation = null;
    }
}

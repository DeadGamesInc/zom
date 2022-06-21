using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmptyLocation : MonoBehaviour {
    public MapNode Location;
    private LevelController _levelController;

    public void Setup(MapNode location) {
        Location = location;
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

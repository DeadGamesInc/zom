using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocationControl : LocationBase {
    [SerializeField] public bool BasicLocation;
    [SerializeField] public Sprite InfoCard;

    public void OnMouseEnter() {
        var basic = BasicLocation ? "BASIC " : "";
        var spawnTime = !Spawned ? $"{SpawnTime} turns before ready" : "";
        var controller = LevelController.Get();
        controller.SetStatusText($"{basic}LOCATION SELECTED");
        controller.SetInfoWindow(InfoCard, spawnTime);
        if (Spawned) controller.SelectedLocation = gameObject;
    }

    public void OnMouseExit() {
        var controller = LevelController.Get();
        controller.SetStatusText("");
        controller.SetInfoWindow(null, "");
        if (Spawned) controller.SelectedLocation = null;
    }
}

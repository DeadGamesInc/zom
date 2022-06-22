using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Brains : MonoBehaviour {
    [SerializeField] public int BrainsValue;
    [SerializeField] public int StoredBrains;
    [SerializeField] public Sprite InfoCard;
    public static Vector3 yOffset = new(0f, 5f, 0f);
    
    private LevelController _levelController;

    public void Setup(BrainsNode node, BaseMap map, int value) {
        BrainsValue = value;
        transform.localScale = new Vector3(10, 10, 10);
        transform.position = map.GetWorldPosition(node.x, node.z) + yOffset;
        _levelController = GameObject.Find("LevelController")?.GetComponent<LevelController>();
    }

    public void UpdateBrains() => StoredBrains += BrainsValue;
    
    public void OnMouseEnter() {
        _levelController.SetStatusText("BRAINS");
        _levelController.SetInfoWindow(InfoCard, $"{StoredBrains} BRAINS");
    }

    public void OnMouseExit() {
        _levelController.SetStatusText("");
        _levelController.SetInfoWindow(null, "");
    }

    public void OnMouseUp() {
        var phase = LevelController.Get().CurrentPhase;
        if (phase != PhaseId.STRATEGIC && phase != PhaseId.DEFENCE) return;
        
        if (StoredBrains > 0) LevelController.Get().AddBrains(StoredBrains);
        StoredBrains = 0;
        _levelController.SetStatusText("");
        LevelController.Get().SetInfoWindow(null, null);
    }
}

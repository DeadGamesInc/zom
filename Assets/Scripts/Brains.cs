using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Brains : MonoBehaviour {
    public static Vector3 yOffset = new(0f, 5f, 0f);
    
    private LevelController _levelController;

    public void Setup(BrainsNode node, BaseMap map) {
        transform.localScale = new Vector3(10, 10, 10);
        transform.position = map.GetWorldPosition(node.x, node.z) + yOffset;
        _levelController = GameObject.Find("LevelController")?.GetComponent<LevelController>();
    }
    
    public void OnMouseEnter() {
        _levelController.SetStatusText("BRAINS");
    }

    public void OnMouseExit() {
        _levelController.SetStatusText("");
    }
}

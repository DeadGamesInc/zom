using System;
using JetBrains.Annotations;
using UnityEngine;

public class MapNode : MonoBehaviour {
    private const int SIZE = 15;
    public int x, z;

    public static MapNode Create(int x, int z, MapGrid grid = null, bool draw = false) {
        GameObject newGameObject = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        MapNode node = newGameObject.AddComponent<MapNode>();
        node.x = x;
        node.z = z;
        if (draw) {
            newGameObject.transform.position = grid.GetWorldPosition(x, z);
            newGameObject.transform.localScale = new Vector3(SIZE, 0, SIZE);
        } else {
            newGameObject.transform.localScale = Vector3.zero;
        }

        return node;
    }
    
    public void Start() {
    }

    public void OnMouseEnter() {
        var renderer = GetComponent<Renderer>();
        renderer.material.SetColor("_Color", Color.green);
    }
    
    public void OnMouseExit() {
        var renderer = GetComponent<Renderer>();
        renderer.material.SetColor("_Color", Color.white);
    }

    public void OnMouseDown() {
        // This logic will move to the level controller & character will be selected before moving
        Character character = GameObject.Find("Character").GetComponent<Character>();
        character.MoveTowards(this);
    }

    public override bool Equals(object? obj) => obj is MapNode other && equals(other);
    private bool equals(MapNode n) => x == n.x && z == n.z;
}
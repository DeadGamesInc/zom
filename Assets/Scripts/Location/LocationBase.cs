using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocationBase : MonoBehaviour {
    [SerializeField] public MapNode ActiveNode;
    [SerializeField] public (int, int) MapPosition;
    
    public static GameObject CreateEmpty(MapGrid grid, (int, int) mapPosition, MapNode activeNode) {
        GameObject newGameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
        newGameObject.name = "Empty Location";
        LocationBase location = newGameObject.AddComponent<LocationBase>();
        location.MapPosition = mapPosition;
        location.ActiveNode = activeNode;
        newGameObject.transform.position = grid.GetWorldPosition(mapPosition.Item1, mapPosition.Item2);
        newGameObject.transform.localScale = new Vector3(30f, 0f, 30f);
        BoxCollider collider = newGameObject.AddComponent<BoxCollider>();
        return newGameObject;
    }
    
    // Start is called before the first frame update
    void Start() {
    }

    // Update is called once per frame
    void Update() {
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocationSpawner : MonoBehaviour {
    [SerializeField] public GameObject EmptyPrefab;
    
    public GameObject CreateEmpty(MapGrid grid, (int, int) mapPosition, MapNode activeNode) {
        GameObject locationObject = Instantiate(EmptyPrefab);
        LocationBase location = locationObject.GetComponent<LocationBase>();
        location.MapPosition = mapPosition;
        location.ActiveNode = activeNode;
        locationObject.transform.position = grid.GetWorldPosition(mapPosition.Item1, mapPosition.Item2);
        
        return locationObject;
    }
    
    // Start is called before the first frame update
    void Start() {
    }

    // Update is called once per frame
    void Update() {
    }
}
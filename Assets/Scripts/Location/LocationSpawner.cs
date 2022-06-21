using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocationSpawner : MonoBehaviour {
    [SerializeField] public GameObject EmptyPrefab;
    [SerializeField] public GameObject GraveyardPrefab;
    
    public GameObject CreateEmpty(MapGrid grid, (int, int) mapPosition, MapNode activeNode) {
        GameObject locationObject = Instantiate(EmptyPrefab);
        return configureLocation(locationObject, grid, mapPosition, activeNode);
    }
    
    public GameObject CreateGraveyard(MapGrid grid, (int, int) mapPosition, MapNode activeNode) {
        GameObject locationObject = Instantiate(GraveyardPrefab);
        return configureLocation(locationObject, grid, mapPosition, activeNode);
    }

    private GameObject configureLocation(GameObject locationObject, MapGrid grid, (int, int) mapPosition, MapNode activeNode) {
        LocationBase location = locationObject.GetComponent<LocationBase>();
        location.MapPosition = mapPosition;
        location.ActiveNode = activeNode;
        locationObject.transform.position = grid.GetWorldPosition(mapPosition.Item1, mapPosition.Item2);
        activeNode.location = locationObject;
        return locationObject;
    }
}
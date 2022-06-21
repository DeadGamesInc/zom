using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocationBase : MonoBehaviour {
    [SerializeField] public int Owner;
    [SerializeField] public int Health;
    [SerializeField] public MapNode ActiveNode;
    [SerializeField] public (int, int) MapPosition;

    // Start is called before the first frame update
    void Start() {
    }

    // Update is called once per frame
    void Update() {
    }
}
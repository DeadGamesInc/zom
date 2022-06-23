using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocationBase : MonoBehaviour {
    [SerializeField] public int Owner;
    [SerializeField] public int Health;
    [SerializeField] public MapNode ActiveNode;
    [SerializeField] public (int, int) MapPosition;
    [SerializeField] public int SpawnTime;
    [SerializeField] public bool Spawned;

    public GameObject Card;

    public void Setup(int owner) {
        Owner = owner;
        if (SpawnTime == 0) SetSpawned();
        else SetSpawning();
    }

    public void SpawnTick() {
        if (SpawnTime == 0) return;
        SpawnTime--;
        if (SpawnTime == 0) SetSpawned();
    }

    public void SetSpawning() {
        gameObject.GetComponent<Renderer>().material.ChangeAlpha(0.25f);
        Spawned = false;
    }

    public void SetSpawned() {
        gameObject.GetComponent<Renderer>().material.ChangeAlpha(1.0f);
        Spawned = true;
    }
    
    // Start is called before the first frame update
    void Start() {
    }

    // Update is called once per frame
    void Update() {
    }
}
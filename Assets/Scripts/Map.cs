using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour {
    public int width;
    public int height;
    public Func<int, int, LocationGrid> data = (int x, int y) => new LocationGrid(x, y);

    // public Grid grid;
    //
    // Start is called before the first frame update
    void Start() {
        // grid = new Grid(4, 2, 50f);
    }
    //
    // public void SetLocation(int x, int z, Location location) {
    //     // grid[x, z] = location;
    // }
}
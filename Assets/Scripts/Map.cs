using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour {
    private Location[,] gridArray;
    public string Name = "yoo";

    // Start is called before the first frame update
    void Start() {
        Grid grid = new Grid(4, 2, 50f);
    }

    public void SetLocation(int x, int z, Location location) {
        gridArray[x, z] = location;
    }
}
using UnityEngine;
using System;

[System.Serializable]
public class LocationGrid {
    [System.Serializable]
    public struct rowData{
        public bool[] row;
    }
    
    public int width;
    public int height;
    public rowData[] rows = Array.Empty<rowData>();

    public LocationGrid(int width, int height) {
        this.width = width;
        this.height = height;
    }
}
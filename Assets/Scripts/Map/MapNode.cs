using JetBrains.Annotations;
using UnityEngine;

public struct MapNode {
    public int x, z;

    public MapNode(int p1, int p2, MapGrid grid = null, bool draw = false) {
        x = p1;
        z = p2;
        if(draw) drawNode(grid);
    }
    
    private void drawNode(MapGrid grid) {
        GameObject nodeCylinder = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        nodeCylinder.transform.position = grid.GetWorldPosition(x, z);
        nodeCylinder.transform.localScale = new Vector3(10, 0, 10);
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class BaseMap : MonoBehaviour {
    [SerializeField] protected int length;
    [SerializeField] protected int width;
    [SerializeField] protected float cellSize;

    public MapPath[] paths = Array.Empty<MapPath>();

    // Expose readonly dimensions
    public int Width => width;
    public int Length => length;
    public MapGrid grid { get; private set; }

    protected abstract MapPath[] InitializePaths();

    private void DrawPaths() {
        foreach (MapPath mapPath in paths) {
            for (int i = 0; i < mapPath.path.Length; i++) {
                if (i == mapPath.path.Length - 1) break;
                MapNode node = mapPath.path[i];
                MapNode nextNode = mapPath.path[i + 1];

                Debug.DrawLine(
                    GetCoordWorldPosition(node.x, node.y),
                    GetCoordWorldPosition(nextNode.x, nextNode.y),
                    Color.red, 100f);
            }
        }
    }

    protected MapPath toPath(params (int, int)[] coords) {
        MapNode[] nodes = coords.Select(coord => new MapNode(coord.Item1, coord.Item2)).ToArray();
        return new MapPath(nodes);
    }

    public Vector3 GetCoordWorldPosition(int x, int z) {
        return grid.GetWorldPosition(x, z);
    }

    // Start is called before the first frame update
    void Start() {
        Vector3 globalPosition = gameObject.transform.position;
        grid = new MapGrid(width, length, cellSize, globalPosition);
        paths = InitializePaths();
        DrawPaths();
    }
}
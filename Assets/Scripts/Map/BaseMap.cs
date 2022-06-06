using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseMap : MonoBehaviour {
    [SerializeField] protected int length;
    [SerializeField] protected int width;
    [SerializeField] protected float cellSize;

    public (int, int)[][] paths = Array.Empty<(int, int)[]>();

    // Expose readonly dimensions
    public int Width => width;
    public int Length => length;
    public MapGrid grid { get; private set; }

    protected abstract (int, int)[][] InitializePaths();

    private void DrawPaths() {
        foreach ((int, int)[] path in paths) {
            for (int i = 0; i < path.Length; i++) {
                if (i == path.Length - 1) break;
                (int, int) node = path[i];
                (int, int) nextNode = path[i + 1];

                Debug.DrawLine(
                    GetCoordWorldPosition(node.Item1, node.Item2),
                    GetCoordWorldPosition(nextNode.Item1, nextNode.Item2),
                    Color.red, 100f);
            }
        }
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
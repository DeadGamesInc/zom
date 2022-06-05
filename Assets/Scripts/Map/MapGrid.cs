using UnityEngine;


public class MapGrid {
    private int width;
    private int length;
    private readonly float cellSize;
    private readonly Vector3 position;
    private Vector3[,] gridArray;

    public MapGrid(int width, int length, float cellSize, Vector3 position) {
        this.width = width;
        this.length = length;
        this.cellSize = cellSize;
        this.position = position;

        gridArray = new Vector3[width, length];

        for (int x = 0; x < gridArray.GetLength(0); x++) {
            for (int z = 0; z < gridArray.GetLength(1); z++) {
                Debug.DrawLine(GetWorldPosition(x, z), GetWorldPosition(x, z + 1), Color.white, 100f);
                Debug.DrawLine(GetWorldPosition(x, z), GetWorldPosition(x + 1, z), Color.white, 100f);
            }
        }
        Debug.DrawLine(GetWorldPosition(0, length), GetWorldPosition(width, length), Color.white, 100f);
        Debug.DrawLine(GetWorldPosition(width, 0), GetWorldPosition(width, length), Color.white, 100f);
    }

    public Vector3 GetWorldPosition(int x, int z) {
        return (new Vector3(x, 0, z) - new Vector3(width / 2f, 0, length / 2f)) * cellSize + position;
    }
}
using UnityEngine;

public class MapGrid {
    private readonly int _width, _length;
    private readonly float _cellSize;
    private readonly Vector3 _position;

    public MapGrid(int width, int length, float cellSize, Vector3 position) {
        _width = width;
        _length = length;
        _cellSize = cellSize;
        _position = position;

        var gridArray = new Vector3[width, length];

        for (var x = 0; x < gridArray.GetLength(0); x++) {
            for (var z = 0; z < gridArray.GetLength(1); z++) {
                Debug.DrawLine(GetWorldPosition(x, z), GetWorldPosition(x, z + 1), Color.white, 1000f);
                Debug.DrawLine(GetWorldPosition(x, z), GetWorldPosition(x + 1, z), Color.white, 1000f);
            }
        }
        Debug.DrawLine(GetWorldPosition(0, length), GetWorldPosition(width, length), Color.white, 1000f);
        Debug.DrawLine(GetWorldPosition(width, 0), GetWorldPosition(width, length), Color.white, 1000f);
    }

    public Vector3 GetWorldPosition(int x, int z) => (new Vector3(x, 0, z) - new Vector3(_width / 2f, 0, _length / 2f)) * _cellSize + _position;
}
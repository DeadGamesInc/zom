using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class MapNodeGrid : MapGrid {
    private MapNode _parent;
    private Vector3 dir;
    private Vector3 origin;
    private float angle;

    public MapNodeGrid(int width, int length, float cellSize, MapNode parent) {
        _width = width;
        _length = length;
        _cellSize = cellSize;
        _parent = parent;
        _position = LevelController.Get()._map.GetMapBase().GetNodeWorldPosition(parent);
        gridArray = new Vector3[width, length];
        
        origin = LevelController.Get()._map.transform.position;
        angle = Vector3.Angle(origin - _position, Vector3.forward);
        dir = (origin - _position).normalized * cellSize;
        // Vector3 perpendicularDir = Vector3.Cross(dir, Vector3.up);
        // Quaternion facing = Quaternion.identity; // zero rotation
        // Quaternion rotation = Quaternion.LookRotation(dir, Vector3.up);
        for (int x = 0; x < gridArray.GetLength(0); x++) {
            for (int z = 0; z < gridArray.GetLength(1); z++) {
                var node = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                node.transform.position = GetWorldPosition(x, z);
                node.transform.localScale = new Vector3(7.5f, 0f, 7.5f);

                if (z < _length - 1)
                    Debug.DrawLine(GetWorldPosition(x, z), GetWorldPosition(x, z + 1), Color.white, 1000f);
                if (x < _width - 1)
                    Debug.DrawLine(GetWorldPosition(x, z), GetWorldPosition(x + 1, z), Color.white, 1000f);
            }
        }
    }

    public (int, int) GetAvailableSpot(int owner, int? index = null) {
        LevelController levelController = LevelController.Get();
        var capacity = index ?? levelController.CharactersOnNode(_parent).Count();
        var characters = levelController.CharactersOnNode(_parent).Select(c => c.GetCharacter());
        var center = (_width / 2, _length / 2);

        int x = 0, y = 0;
        for (var offset = 0; offset < _length * _width; offset++) {
            x = (int)Math.Floor((capacity + offset) / Convert.ToDouble(MapNode.MAP_GRID_WIDTH));
            y = (capacity + offset) % MapNode.MAP_GRID_LENGTH;
            if (owner != 0) x = (MapNode.MAP_GRID_WIDTH - 1) - x;

            if ((x, y) == center) continue;
            if (!characters.Any(c => c.NodePosition == (x, y))) break;
        }

        switch (capacity) {
            case 0:
                return (center.Item1, center.Item2);
            default:
                return (x, y);
        }
    }
    
    public static Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Vector3 angles) {
        return RotatePointAroundPivot(point, pivot, Quaternion.Euler(angles));
    }
 
    public static Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Quaternion rotation) {
        return rotation * (point - pivot) + pivot;
    }

    public Vector3 GetWorldPosition(int x, int z) => LevelController.Get()._map.GetMapBase().LocationNodes.Contains((_parent.X, _parent.Z)) ? locationNodeWorldPosition(x, z) : worldPosition(x, z);

    private Vector3 worldPosition(int x, int z) =>
        (new Vector3(x, 0, z) - new Vector3(_width / 2f, 0, _length / 2f))
        * _cellSize
        + new Vector3(_cellSize / 2, 0, _cellSize / 2)
        + _position;

    private Vector3 locationNodeWorldPosition(int x, int z) {
        var point = (new Vector3(x, 0, z) - new Vector3(_width / 2f, 0, _length / 2f)) * _cellSize
                    + new Vector3(_cellSize / 2, 0, _cellSize / 2)
                    + _position;

        return RotatePointAroundPivot(
            point, _position, new Vector3(0f, angle, 0f));
    }
}
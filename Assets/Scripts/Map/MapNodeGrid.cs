using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MapNodeGrid : MapGrid {
    private MapNode parent;

    public MapNodeGrid(int width, int length, float cellSize, MapNode parent) {
        this.width = width;
        this.length = length;
        this.cellSize = cellSize;
        this.parent = parent;
        position = LevelController.Get()._map.GetMapBase().GetNodeWorldPosition(parent);

        gridArray = new Vector3[width, length];

        for (int x = 0; x < gridArray.GetLength(0); x++) {
            for (int z = 0; z < gridArray.GetLength(1); z++) {
                Debug.DrawLine(GetWorldPosition(x, z), GetWorldPosition(x, z + 1), Color.white, 1000f);
                Debug.DrawLine(GetWorldPosition(x, z), GetWorldPosition(x + 1, z), Color.white, 1000f);
            }
        }

        Debug.DrawLine(GetWorldPosition(0, length), GetWorldPosition(width, length), Color.white, 1000f);
        Debug.DrawLine(GetWorldPosition(width, 0), GetWorldPosition(width, length), Color.white, 1000f);
    }

    public (int, int) GetAvailableSpot(int owner) {
        LevelController levelController = LevelController.Get();
        var capacity = levelController.CharactersOnNode(parent).Count();
        var characters = levelController.CharactersOnNode(parent).Select(c => c.GetCharacter());
        var center = (width / 2, length / 2);
        
        int x = 0, y = 0;
        for (var offset = 0; offset < length * width; offset++) {
                x = (int)Math.Floor((capacity + offset) / Convert.ToDouble(MapNode.MAP_GRID_SIZE));
                y = (capacity + offset) % MapNode.MAP_GRID_SIZE;
                if (owner != 0) x = MapNode.MAP_GRID_SIZE - x;

            if ((x, y) == center) continue;
            if (!characters.Any(c => c.NodePosition == (x, y))) break;
        }
        Debug.Log((x, y));

        switch (capacity) {
            case 0:
                return (center.Item1, center.Item2);
            default:
                return (x, y);
        }
    }
}
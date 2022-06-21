using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public abstract class BaseMap : MonoBehaviour {
    [SerializeField] public int Length;
    [SerializeField] public int Width;
    [SerializeField] public float CellSize;

    protected LevelController _levelController;
    public MapPath[] paths = Array.Empty<MapPath>();
    public MapGrid grid { get; private set; }
    
    [CanBeNull]
    public static GameObject? Get() {
        GameObject map;

        if ((map = GameObject.Find("Map")) == null || map.GetComponent<BaseMap>().grid == null) {
            return null;
        }

        return map;
    }

    public Vector3 GetNodeWorldPosition(MapNode node) {
        return grid.GetWorldPosition(node.x, node.z);
    }

    public MapNode? GetNode(int x, int z) {
        foreach (MapPath mapPath in paths) {
            MapNode node = mapPath.path.FirstOrDefault(n => n.GetHashCode() == (x, z).GetHashCode());
            if (node) return node;
        }

        return null;
    }
    
    // Returns MapPath containing the specified nodes or null if one does not exist
    public MapPath? GetPathByNodes(params MapNode[] nodes) {
        foreach (MapPath mapPath in paths) {
            if (nodes.All(node => mapPath.path.Contains(node))) {
                return mapPath;
            }
        }

        return null;
    }

    // This assumes the map loops when determining the shortest route direction
    public MapPath? GetShortestPathBetweenNodes(MapNode node1, MapNode node2) {
        MapPath? mapPath = GetPathByNodes(node1, node2);
        if (mapPath == null) return null;
        MapNode[] path = mapPath.Value.path;
        int node1Index = Array.IndexOf(path, node1);
        int node2Index = Array.IndexOf(path, node2);
        bool innerMovingForward = node2Index > node1Index;
        bool outerMovingForward = node2Index < node1Index;
        int innerDistance = innerMovingForward ? node2Index - node1Index : node1Index - node2Index;
        int outerDistance =
            outerMovingForward ? node2Index + path.Length - node1Index : node1Index + path.Length - node2Index;

        MapNode[] shortestRoute;
        if (innerDistance <= outerDistance) {
            if (innerMovingForward) {
                shortestRoute = path[node1Index..(node2Index + 1)];
            } else {
                shortestRoute = path[node2Index..(node1Index + 1)].Reverse().ToArray();
            }
        } else {
            MapNode[] seg1, seg2;
            if (outerMovingForward) {
                seg1 = path[node1Index..path.Length];
                seg2 = path[1..(node2Index + 1)];
            } else {
                seg1 = path[0..(node1Index + 1)].Reverse().ToArray();
                seg2 = path[node2Index..].Reverse().ToArray();
            }

            shortestRoute = seg1.Concat(seg2).ToArray();
        }

        return new MapPath(shortestRoute);
    }

    private void drawPaths() {
        foreach (MapPath mapPath in paths) {
            for (int i = 0; i < mapPath.path.Length; i++) {
                if (i == mapPath.path.Length - 1) break;
                MapNode node = mapPath.path[i];
                MapNode nextNode = mapPath.path[i + 1];

                Debug.DrawLine(
                    GetNodeWorldPosition(node),
                    GetNodeWorldPosition(nextNode),
                    Color.red, 1000f);
            }
        }
    }

    protected MapPath toPath(params (int, int)[] coords) {
        MapNode[] nodes = coords.Select(coord => MapNode.Create(coord.Item1, coord.Item2, grid, true)).ToArray();
        return new MapPath(nodes);
    }

    protected abstract MapPath[] initializePaths();
    protected abstract void InitializeLocations();

    // Start is called before the first frame update
    protected void Initialize() {
        _levelController = GameObject.Find("LevelController")?.GetComponent<LevelController>();
        var globalPosition = gameObject.transform.position;
        grid = new MapGrid(Width, Length, CellSize, globalPosition);
        paths = initializePaths();
        drawPaths();
        InitializeLocations();
    }
}
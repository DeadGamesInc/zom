using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class BaseMap : MonoBehaviour {
    [SerializeField] public int Length;
    [SerializeField] public int Width;
    [SerializeField] public float CellSize;

    public MapPath[] paths = Array.Empty<MapPath>();
    public MapGrid grid { get; private set; }

    public Vector3 GetNodeWorldPosition(MapNode node) {
        return grid.GetWorldPosition(node.x, node.z);
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
        int posDistance = node2Index - node1Index;
        int negDistance = node1Index + (path.Length - 1) - node2Index;
        MapNode[] shortestRoute;
        if (posDistance <= negDistance) {
            var lastNode = new[] { node2 };
            shortestRoute = new ArraySegment<MapNode>(path, node1Index, node2Index - node1Index).Concat(lastNode).ToArray();
        } else {
            var seg1 = new ArraySegment<MapNode>(path, 0, node1Index).Reverse();
            var seg2 = new ArraySegment<MapNode>(path, node2Index, path.Length - node2Index).Reverse();
            shortestRoute = seg1.Concat(seg2).ToArray();
        }

        return new MapPath(shortestRoute);
    }

    private void DrawPaths() {
        foreach (MapPath mapPath in paths) {
            for (int i = 0; i < mapPath.path.Length; i++) {
                if (i == mapPath.path.Length - 1) break;
                MapNode node = mapPath.path[i];
                MapNode nextNode = mapPath.path[i + 1];

                Debug.DrawLine(
                    GetNodeWorldPosition(node),
                    GetNodeWorldPosition(nextNode),
                    Color.red, 100f);
            }
        }
    }

    protected MapPath toPath(params (int, int)[] coords) {
        MapNode[] nodes = coords.Select(coord => new MapNode(coord.Item1, coord.Item2)).ToArray();
        return new MapPath(nodes);
    }

    // Start is called before the first frame update
    public void Start() {
        Vector3 globalPosition = gameObject.transform.position;
        grid = new MapGrid(Width, Length, CellSize, globalPosition);
        DrawPaths();
    }
}
using System;
using System.Linq;
using UnityEngine;

public abstract class MapBase : MonoBehaviour {
    [SerializeField] public int Length, Width, DistanceUnit;
    [SerializeField] public float CellSize;

    public MapGrid Grid { get; private set; }
    
    protected abstract MapPath[] InitializePaths();
    protected abstract void InitializeLocations();
    protected abstract void InitializeBrainsNodes();
    
    private MapPath[] _paths = Array.Empty<MapPath>();
    
    public static MapBase Get() => GameObject.Find("Map").GetComponent<MapBase>();
    public Vector3 GetNodeWorldPosition(MapNode node) => Grid.GetWorldPosition(node.X, node.Z);
    public Vector3 GetWorldPosition(int x, int z) => Grid.GetWorldPosition(x, z);
    
    protected MapPath ToPath(params (int, int)[] coords) => new(coords.Select(coord => MapNode.Create(coord.Item1, coord.Item2, Grid, true)).ToArray());
    protected MapNode GetNode(int x, int z) => _paths.Select(mapPath => mapPath.path.FirstOrDefault(n => n.GetHashCode() == (x, z).GetHashCode())).FirstOrDefault(node => node);
    
    // Returns MapPath containing the specified nodes or null if one does not exist
    private MapPath? GetPathByNodes(params MapNode[] nodes) {
        foreach (var mapPath in _paths) 
            if (nodes.All(node => mapPath.path.Contains(node))) return mapPath;

        return null;
    }

    // This assumes the map loops when determining the shortest route direction
    public MapPath? GetShortestPathBetweenNodes(MapNode node1, MapNode node2) {
        var mapPath = GetPathByNodes(node1, node2);
        if (mapPath == null) return null;
        var path = mapPath.Value.path;
        var node1Index = Array.IndexOf(path, node1);
        var node2Index = Array.IndexOf(path, node2);
        var innerMovingForward = node2Index > node1Index;
        var outerMovingForward = node2Index < node1Index;
        var innerDistance = innerMovingForward ? node2Index - node1Index : node1Index - node2Index;
        var outerDistance = outerMovingForward ? node2Index + path.Length - node1Index : node1Index + path.Length - node2Index;
        
        MapNode[] shortestRoute;
        
        if (innerDistance <= outerDistance) {
            shortestRoute = innerMovingForward ? path[node1Index..(node2Index + 1)] : path[node2Index..(node1Index + 1)].Reverse().ToArray();
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
    
    protected void Initialize() {
        var globalPosition = gameObject.transform.position;
        Grid = new MapGrid(Width, Length, CellSize, globalPosition);
        _paths = InitializePaths();
        DrawPaths();
        InitializeLocations();
        InitializeBrainsNodes();
    }

    private void DrawPaths() {
        foreach (var mapPath in _paths) {
            for (var i = 0; i < mapPath.path.Length; i++) {
                if (i == mapPath.path.Length - 1) break;
                var node = mapPath.path[i];
                var nextNode = mapPath.path[i + 1];
                Debug.DrawLine(GetNodeWorldPosition(node), GetNodeWorldPosition(nextNode), Color.red, 1000f);
            }
        }
    }
}
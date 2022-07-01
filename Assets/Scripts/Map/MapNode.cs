using System.Collections.Generic;

using UnityEngine;

public class MapNode : MonoBehaviour {
    public const int MAP_GRID_LENGTH = 4;
    public const int MAP_GRID_WIDTH = 5;
    private const int SIZE = 15;
    public int X, Z;
    public MapNodeGrid PlayerGrid;
    [SerializeField] public GameObject Location;
    [SerializeField] public List<GameObject> EmptyBrainNodes = new(), BrainNodes = new();

    public static MapNode Create(int x, int z, MapGrid grid = null, bool draw = false) {
        var newGameObject = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        var node = newGameObject.AddComponent<MapNode>();
        node.X = x;
        node.Z = z;
        if (draw) {
            newGameObject.transform.position = grid.GetWorldPosition(x, z);
            newGameObject.transform.localScale = new Vector3(SIZE, 0.00000001f, SIZE);
        } else {
            newGameObject.transform.localScale = Vector3.zero;
        }

        node.PlayerGrid = new MapNodeGrid(MAP_GRID_WIDTH, MAP_GRID_LENGTH, 10, node);
        return node;
    }

    public override bool Equals(object obj) => obj is MapNode other && equals(other);
    private bool equals(MapNode n) => X == n.X && Z == n.Z;
    public override int GetHashCode() => (X, Z).GetHashCode();

    public void OnMouseEnter() => GetComponent<Renderer>().material.SetColor("_Color", Color.green);
    public void OnMouseExit() => GetComponent<Renderer>().material.SetColor("_Color", Color.white);
    public void OnMouseDown() => LevelController.Get().QueueCommand(PlayerCommand.MoveCharacter, gameObject);
}
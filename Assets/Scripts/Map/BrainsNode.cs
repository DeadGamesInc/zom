using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrainsNode : MonoBehaviour {
    private const int SIZE = 10;
    public int x, z;
    [SerializeField] public GameObject location;

    private LevelController _levelController;

    public static BrainsNode Create(int x, int z, MapGrid grid = null, bool draw = false) {
        var obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
        var node = obj.AddComponent<BrainsNode>();
        node.x = x;
        node.z = z;
        if (draw) {
            obj.transform.position = grid.GetWorldPosition(x, z);
            obj.transform.localScale = new Vector3(SIZE, 0, SIZE);
        } else {
            obj.transform.localScale = Vector3.zero;
        }
        
        return node;
    }

    public void Start() {
        _levelController = GameObject.Find("LevelController")?.GetComponent<LevelController>();
    }

    public void OnMouseEnter() {
        _levelController.SetStatusText("BRAINS NODE SELECTED");
        _levelController.SelectedBrainsNode = gameObject;
    }

    public void OnMouseExit() {
        _levelController.SetStatusText("");
        _levelController.SelectedBrainsNode = null;
    }

    public override bool Equals(object? obj) => obj is MapNode other && equals(other);
    private bool equals(MapNode n) => x == n.x && z == n.z;
    public override int GetHashCode() => (x, z).GetHashCode();
}

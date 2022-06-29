using UnityEngine;

public class BrainsNode : MonoBehaviour {
    [SerializeField] public MapNode MapNode;
    
    private const int SIZE = 10;
    public int x, z;
    
    public override bool Equals(object obj) => obj is MapNode other && equals(other);
    private bool equals(MapNode n) => x == n.X && z == n.Z;
    public override int GetHashCode() => (x, z).GetHashCode();

    public static BrainsNode Create(int x, int z, MapGrid grid, MapNode mapNode) {
        var obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
        var node = obj.AddComponent<BrainsNode>();
        node.MapNode = mapNode;
        node.x = x;
        node.z = z;
        obj.transform.position = grid.GetWorldPosition(x, z);
        obj.transform.localScale = new Vector3(SIZE, 0, SIZE);
        mapNode.EmptyBrainNodes.Add(obj);
        
        return node;
    }

    public void OnMouseEnter() {
        var controller = LevelController.Get();
        controller.SetStatusText("BRAINS NODE SELECTED");
        controller.SelectedBrainsNode = gameObject;
    }

    public void OnMouseExit() {
        var controller = LevelController.Get();
        controller.SetStatusText("");
        controller.SelectedBrainsNode = null;
    }
}

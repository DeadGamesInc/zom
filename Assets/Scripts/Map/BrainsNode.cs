using UnityEngine;

public class BrainsNode : MonoBehaviour {
    [SerializeField] public GameObject ParentLocation;
    
    private const int SIZE = 10;
    
    public static BrainsNode Create(GameObject parentLocation, bool side) {
        var obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
        var node = obj.AddComponent<BrainsNode>();
        node.ParentLocation = parentLocation;
        var location = parentLocation.GetLocationBase();
        var perpendicular = Vector3.Cross(location.DirectionVector, Vector3.up);
        obj.transform.position = parentLocation.transform.position + perpendicular * (side ? 40f : -40f);
        obj.transform.localScale = new Vector3(SIZE, 0.00000001f, SIZE);
        location.EmptyBrainNodes.Add(obj);
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

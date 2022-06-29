using UnityEngine;

public class Brains : MonoBehaviour {
    [SerializeField] public int BrainsValue, StoredBrains, Owner;
    [SerializeField] public Sprite InfoCard;
    public GameObject Card;
    public MapNode ActiveNode;
    
    private static Vector3 yOffset = new(0f, 5f, 0f);

    public void UpdateBrains() => StoredBrains += BrainsValue;
    
    public void Setup(BrainsNode node, MapBase map, int owner, int value) {
        Owner = owner;
        BrainsValue = value;
        ActiveNode = node.MapNode;
        transform.localScale = new Vector3(10, 10, 10);
        transform.position = map.GetWorldPosition(node.x, node.z) + yOffset;
    }

    public void OnMouseEnter() {
        var controller = LevelController.Get();
        controller.SetStatusText("BRAINS");
        controller.SetInfoWindow(InfoCard, $"{StoredBrains} BRAINS");
    }

    public void OnMouseExit() {
        var controller = LevelController.Get();
        controller.SetStatusText("");
        controller.SetInfoWindow(null, "");
    }

    public void OnMouseUp() {
        var controller = LevelController.Get();
        var phase = controller.CurrentPhase;
        if (phase != PhaseId.STRATEGIC && phase != PhaseId.DEFENCE) return;
        
        if (StoredBrains > 0) controller.AddBrains(StoredBrains);
        StoredBrains = 0;
        controller.SetStatusText("");
        controller.SetInfoWindow(null, null);
    }
}

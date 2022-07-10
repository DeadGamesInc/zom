using UnityEngine;

public class Brains : MonoBehaviour {
    [SerializeField] public int BrainsValue, StoredBrains, Owner;
    [SerializeField] public Sprite InfoCard;
    public GameObject Node;
    public GameObject Card;
    
    private static Vector3 yOffset = new(0f, 5f, 0f);

    public void UpdateBrains() => StoredBrains += BrainsValue;
    
    public void Setup(BrainsNode node, int owner, int value) {
        Node = node.gameObject;
        Owner = owner;
        BrainsValue = value;
        transform.localScale = new Vector3(10, 10, 10);
        transform.position = node.transform.position + yOffset;
    }

    public void Kill() {
        Node.SetActive(true);
        LevelController.Get().BrainLocations.Remove(gameObject);
        Destroy(gameObject);
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

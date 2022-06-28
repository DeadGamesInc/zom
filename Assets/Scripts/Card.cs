using UnityEngine;

public class Card : MonoBehaviour {
    [SerializeField] public CardId Id;
    [SerializeField] public CardType Type;
    [SerializeField] public int BrainsValue;
    [SerializeField] public string Name, Series, NervosTestnetNFT;
    [SerializeField] public Sprite CardPreview;
    [SerializeField] public GameObject CharacterPrefab, LocationPrefab, ItemPrefab, ResourcePrefab;

    public Vector3 StartScale;
    private Vector3 _startPosition;

    public void OnMouseExit() => LevelController.Get().SetCard(null, null, "");
    
    public void OnMouseEnter() {
        var info = Type == CardType.RESOURCE ? "Value: " : "Cost: ";
        LevelController.Get().SetCard(CardPreview, gameObject, $"{info}{BrainsValue}");
    }

    public void OnMouseDown() {
        LevelController.Get().SetCardLock(true);
        var transformInfo = transform;
        _startPosition = transformInfo.position;
        StartScale = transformInfo.localScale;
        var targetScale = new Vector3(StartScale.x / 3, StartScale.y / 3, StartScale.z / 3);
        transformInfo.localScale = targetScale;
    }

    public void OnMouseDrag() {
        if (Camera.main == null) return;
        var distance = Camera.main.WorldToScreenPoint(gameObject.transform.position).z;
        transform.position = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, distance));
    }

    public void OnMouseUp() {
        var controller = LevelController.Get();
        
        if (!controller.TryPlayCard()) {
            var setTransform = transform;
            setTransform.position = _startPosition;
            setTransform.localScale = StartScale;
        }
        
        controller.SetCardLock(false);
        controller.SetCard(null, null, "");
    }
}

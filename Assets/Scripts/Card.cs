using TMPro;

using UnityEngine;

public class Card : MonoBehaviour {
    [SerializeField] public CardId Id;
    [SerializeField] public CardType Type;
    [SerializeField] public int BrainsValue, SpawnTime, MovementSpeed;
    [SerializeField] public float Health, Attack;
    [SerializeField] public string Name, Description, Series, NervosTestnetNFT;
    [SerializeField] public bool InstantPlay;
    [SerializeField] public Sprite NFTImage, CardPreview, CardBlank, CardBack;
    [SerializeField] public GameObject CharacterPrefab, LocationPrefab, ItemPrefab, ResourcePrefab;
    [SerializeField] public SpriteRenderer NFTImageBox;
    [SerializeField] public TextMeshPro AttributeBox, NameBox, DescriptionBox, CostBox;

    public Vector3 StartScale, StartPosition;

    public void Start() {
        if (Type == CardType.NONE) return;
        
        NFTImageBox.sprite = NFTImage;
        NameBox.text = Name;
        DescriptionBox.text = Description;
        
        switch (Type) {
            case CardType.CHARACTER:
                AttributeBox.text = $"HP: {Health:0} - ATK: {Attack:0} - SPN: {SpawnTime} - MV: {MovementSpeed}";
                CostBox.text = BrainsValue.ToString();
                break;
            case CardType.RESOURCE:
                AttributeBox.text = $"GENERATES {BrainsValue} BRAINS PER TURN";
                CostBox.text = "";
                break;
            case CardType.LOCATION:
                AttributeBox.text = $"HEALTH: {Health:0} - SPAWN: {SpawnTime}";
                CostBox.text = BrainsValue.ToString();
                break;
            case CardType.ITEM:
                AttributeBox.text = "";
                CostBox.text = BrainsValue.ToString();
                break;
        }

        var texture = GameController.Get().SnapshotCamera.TakeObjectSnapshot(gameObject, 200, 266);
        CardPreview = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
    }

    public void SetBack(bool back) {
        GetComponent<SpriteRenderer>().sprite = back ? CardBack : CardBlank;
        var coll = GetComponent<BoxCollider2D>();
        coll.enabled = !back;
        NFTImageBox.enabled = !back;
        NameBox.enabled = !back;
        DescriptionBox.enabled = !back;
        AttributeBox.enabled = !back;
        CostBox.enabled = !back;
    }

    public void OnMouseExit() => LevelController.Get().SetCard(null, null, "");
    public void OnMouseEnter() => LevelController.Get().SetCard(CardPreview, gameObject, "");

    public void OnMouseDown() {
        LevelController.Get().SetCardLock(true);
        var transformInfo = transform;
        StartPosition = transformInfo.position;
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
            setTransform.position = StartPosition;
            setTransform.localScale = StartScale;
        }
        
        controller.SetCardLock(false);
        controller.SetCard(null, null, "");
    }
}

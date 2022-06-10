using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelController : MonoBehaviour {
    [field: SerializeField] public LevelId Id;
    
    protected GameController _gameController;
    protected DeckController _deckController;

    protected GameObject _handPosition;
    private GameObject _cardPreview;
    
    // Start is called before the first frame update
    public void Start() {
        _gameController = GameObject.Find("GameController").GetComponent<GameController>();
        _deckController = GameObject.Find("Player").GetComponent<DeckController>();
        _handPosition = GameObject.Find("HandPosition");
        _cardPreview = GameObject.Find("CardPreview");
        if (_cardPreview != null) _cardPreview.SetActive(false);
    }

    // Update is called once per frame
    public void Update() {
        
    }

    public void SetCardPreview(Sprite sprite) {
        if (_cardPreview == null) return;
        var image = _cardPreview.GetComponent<Image>();
        image.sprite = sprite;
        _cardPreview.SetActive(sprite != null);
    }
}

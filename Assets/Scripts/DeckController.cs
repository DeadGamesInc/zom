using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using Random = UnityEngine.Random;

public class DeckController : MonoBehaviour {
    private static DeckController _instance;
    
    [SerializeField] public Material CardBack; 
    [SerializeField] public List<GameObject> DeckCards, HandCards = new();
    
    public Transform HandPosition, DeckPosition;

    private readonly List<GameObject> PlacedDeckCards = new();
    
    public void PlayedCard(GameObject card) => HandCards.Remove(card);
    
    public void Start() {
        if (_instance != null) {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void Shuffle() {
        var cards = DeckCards.ToList();
        DeckCards.Clear();
        while (cards.Any()) {
            var index = Random.Range(0, cards.Count);
            DeckCards.Add(cards[index]);
            cards.RemoveAt(index);
        }
    }

    public void PlaceDeckCards() {
        if (DeckPosition == null) return;
        foreach (var card in PlacedDeckCards) Destroy(card);

        var position = DeckPosition.position;
        
        foreach (var card in DeckCards) {
            var placed = Instantiate(card, position, DeckPosition.rotation);
            var mesh = placed.GetComponent<MeshRenderer>();
            mesh.material = CardBack;
            var boxCollider = placed.GetComponent<BoxCollider2D>();
            boxCollider.enabled = false;
            PlacedDeckCards.Add(placed);
            position.y += 0.03f;
            position.z -= 0.03f;
        }
    }
    
    public bool DrawCard(bool freePlay = false, bool instant = false) {
        if (!DeckCards.Any()) return false;
        var card = DeckCards.First();
        HandleDrawnCard(card, freePlay, instant);
        return true;
    }

    public bool DrawCard(CardId id, bool freePlay = false, bool instant = false) {
        var card = DeckCards.Find(a => a.GetCard().Id == id);
        if (card == null) return false;
        HandleDrawnCard(card, freePlay, instant);
        return true;
    }

    private void HandleDrawnCard(GameObject card, bool freePlay, bool instant) {
        if (PlacedDeckCards.Any()) {
            var placed = PlacedDeckCards.Last();
            Destroy(placed);
        }
        
        var drawn = Instantiate(card, HandPosition.position, HandPosition.rotation);

        if (freePlay) drawn.GetCard().BrainsValue = 0;
        if (instant) drawn.GetCard().InstantPlay = true;
        
        HandCards.Add(drawn);
        DeckCards.Remove(card);
    }
}

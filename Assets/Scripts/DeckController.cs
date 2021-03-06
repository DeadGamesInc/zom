using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using Random = UnityEngine.Random;

public class DeckController : MonoBehaviour {
    [SerializeField] public List<GameObject> DeckCards, HandCards = new();
    
    public Transform HandPosition, DeckPosition;
    private readonly Vector3 _cardScale = new(0.3f, 0.3f, 0.3f);
    
    private readonly List<GameObject> PlacedDeckCards = new();
    
    public void PlayedCard(GameObject card) => HandCards.Remove(card);

    public void HandleReset() {
        DeckCards = new();
        HandCards = new();
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
            placed.transform.localScale = _cardScale;
            placed.GetCard().SetBack(true);
            var boxCollider = placed.GetComponent<BoxCollider2D>();
            boxCollider.enabled = false;
            PlacedDeckCards.Add(placed);
            position.y += 0.03f;
            position.z -= 0.03f;
        }
    }
    
    public bool DrawCard(bool freePlay = false, bool instant = false, bool hide = false) {
        if (!DeckCards.Any()) return false;
        var card = DeckCards.First();
        HandleDrawnCard(card, freePlay, instant, hide);
        return true;
    }

    public bool DrawCard(CardId id, bool freePlay = false, bool instant = false, bool hide = false) {
        var card = DeckCards.Find(a => a.GetCard().Id == id);
        if (card == null) return false;
        HandleDrawnCard(card, freePlay, instant, hide);
        return true;
    }

    private void HandleDrawnCard(GameObject card, bool freePlay, bool instant, bool hide) {
        if (PlacedDeckCards.Any()) {
            var placed = PlacedDeckCards.Last();
            Destroy(placed);
        }

        var position = hide ? new Vector3(0, 0, 0) : HandPosition.position + HandCards.Count * new Vector3(1f, 0.1f, -0.1f);
        var rotation = hide ? new Quaternion(0, 0, 0, 0) : HandPosition.rotation;
        
        var drawn = Instantiate(card, position, rotation);

        if (freePlay) drawn.GetCard().BrainsValue = 0;
        if (instant) drawn.GetCard().InstantPlay = true;
        
        drawn.transform.localScale = hide ? new Vector3(0, 0, 0) : _cardScale;
        
        HandCards.Add(drawn);
        DeckCards.Remove(card);
        drawn.transform.parent = HandPosition;
    }
}

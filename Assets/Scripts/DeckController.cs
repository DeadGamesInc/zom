using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

using Random = UnityEngine.Random;

public class DeckController : MonoBehaviour {
    public static DeckController Instance;
    [field: SerializeField] public List<GameObject> DeckCards = new();
    [field: SerializeField] public List<GameObject> HandCards = new();
    
    public Transform HandPosition;
    
    public void Start() {
        if (Instance != null) {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    
    public void Update() {
        
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

    public bool DrawCard() {
        var card = DeckCards.FirstOrDefault();
        if (card == null) return false;
        var drawn = Instantiate(card, HandPosition.position, HandPosition.rotation);
        HandCards.Add(drawn);
        DeckCards.Remove(card);
        return true;
    }

    public void DrawnCard(GameObject card) {
        HandCards.Remove(card);
    }
}

using System.Linq;
using UnityEngine;

public class Level0Controller : LevelController {
    protected override void Setup() {
        createMap();
        
        foreach (var card in _gameController.AvailableCards) 
            for (var i = 1; i <= card.Quantity; i++) _deckController.DeckCards.Add(card.Card);
        
        _deckController.Shuffle();
        
        for (var i = _deckController.HandCards.Count; i < HandCardsTarget; i++) {
            if (!_deckController.DeckCards.Any()) break;
            DrawCard();
        }
    }

    public void ClickShuffle() {
        if (CurrentPhase != PhaseId.STRATEGIC) return;
        _deckController.Shuffle();
    }

    public void ClickDraw() {
        if (CurrentPhase != PhaseId.STRATEGIC) return;
        DrawCard();
    }

    public void ClickEndTurn() {
        if (CurrentPhase != PhaseId.STRATEGIC) return;
        EndTurn();
    }
    
    private void createMap() {
        _map = new GameObject("Map");
        _map.AddComponent<Map0>();
    }
}

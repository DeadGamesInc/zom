using System.Linq;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class Level0Controller : LevelController {
    private Map0 _mapControl;
    
    protected override void Setup() {
        LocalTurn = true;
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
        if ((CurrentPhase == PhaseId.STRATEGIC && LocalTurn) || (CurrentPhase == PhaseId.DEFENCE && !LocalTurn)) 
            EndTurn();
    }
    
    private void createMap() {
        _map = new GameObject("Map");
        _map.transform.position = new Vector3(0, 75, 0);
        _mapControl = _map.AddComponent<Map0>();
        _mapControl.Initialize();
    }
    
    private Vector3 GetNodePosition(MapNode node) {
        return _mapControl.GetNodeWorldPosition(MapNode.Create(node.x, node.z));
    }
}

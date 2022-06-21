using System.Linq;
using UnityEngine;

public class Level0Controller : LevelController {
    private Map0 _mapControl;
    
    protected override void Setup() {
        createMap();
        
        foreach (var card in _gameController.AvailableCards) 
            for (var i = 1; i <= card.Quantity; i++) _deckController.DeckCards.Add(card.Card);
        
        _deckController.Shuffle();
        
        for (var i = _deckController.HandCards.Count; i < HandCardsTarget; i++) {
            if (!_deckController.DeckCards.Any()) break;
            DrawCard();
        }
        
        if (EmptyLocation != null) {
            var empty1Location = MapNode.Create(7, 1);
            var empty1 = Instantiate(EmptyLocation, new Vector3(0, 0, 0), new Quaternion());
            empty1.GetComponent<EmptyLocation>().Setup(empty1Location);
            empty1.transform.position = GetNodePosition(empty1Location) + yOffset;
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
        _mapControl = _map.AddComponent<Map0>();
        _mapControl.Initialize();
    }
    
    private Vector3 GetNodePosition(MapNode node) {
        return _mapControl.GetNodeWorldPosition(MapNode.Create(node.x, node.z));
    }
}

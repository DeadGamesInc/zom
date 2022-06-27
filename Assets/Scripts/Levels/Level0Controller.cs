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
        
        if ((CurrentPhase == PhaseId.STRATEGIC && !LocalTurn) || (CurrentPhase == PhaseId.DEFENCE && LocalTurn)) {
            Opponent.GetComponent<DevOpponent>().EndTurnClicked();
        }

        else if ((CurrentPhase == PhaseId.STRATEGIC && LocalTurn) || (CurrentPhase == PhaseId.DEFENCE && !LocalTurn)) 
            EndTurn();
    }

    public void ClickAddBrains() => AddBrains(10);

    protected override bool LevelSpecificCardHandling() {
        if (!(CurrentPhase == PhaseId.STRATEGIC && !LocalTurn) && !(CurrentPhase == PhaseId.DEFENCE && LocalTurn)) return false;
        if (SelectedCard == null) return false;
        
        var card = SelectedCard.GetComponent<Card>();
        var basicLocation = false;
        var ownedLocation = false;
        var ownedCharacter = false;
        var characterSpawned = false;
        
        if (SelectedLocation != null) {
            var script = SelectedLocation.GetComponent<LocationControl>();
            basicLocation = script.BasicLocation;
            ownedLocation = script.Owner == 1;
        }

        if (SelectedCharacter != null) {
            var script = SelectedCharacter.GetCharacter();
            ownedCharacter = script.Owner == 1;
            characterSpawned = script.Spawned;
        }
        
        if (SelectedCharacter != null && card.Type == CardType.ITEM && ownedCharacter && characterSpawned) {
            var character = SelectedCharacter.GetCharacter();
            character.EquippedItems.Add(card.ItemPrefab);
            CardPlayed(true);
            return true;
        }
        
        if (SelectedLocation != null && card.Type == CardType.CHARACTER && ownedLocation && SelectedLocation.GetLocationBase().Spawned && SubtractBrains(card.BrainsValue)) {
            var character = CreateCharacter(card.CharacterPrefab, SelectedLocation.GetLocationBase().ActiveNode, 1);
            var script = character.GetCharacter();
            script.Card = SelectedCard;
            CardPlayed(false);
            return true;
        }
        
        if (SelectedLocation != null && card.Type == CardType.LOCATION && basicLocation && ownedLocation && SubtractBrains(card.BrainsValue)) {
            var map = _map.GetComponent<MapBase>();
            var location = SelectedLocation.GetComponent<LocationBase>();
            var locationObject = CreateLocation(card.LocationPrefab, SelectedLocation, map.Grid, location.MapPosition, location.ActiveNode, 1);
            locationObject.GetLocationBase().Card = SelectedCard;
            CardPlayed(false);
            return true;
        }
        
        if (SelectedEmptyLocation != null && card.Type == CardType.LOCATION && SubtractBrains(card.BrainsValue)) {
            var map = _map.GetComponent<MapBase>();
            var location = SelectedEmptyLocation.GetComponent<LocationBase>();
            var locationObject = CreateLocation(card.LocationPrefab, SelectedEmptyLocation, map.Grid, location.MapPosition, location.ActiveNode, 1);
            locationObject.GetLocationBase().Card = SelectedCard;
            CardPlayed(false);
            return true;
        }
        
        if (SelectedBrainsNode != null && card.Type == CardType.RESOURCE) {
            var brains = CreateBrainsNode(card.ResourcePrefab, SelectedBrainsNode, 1, card.BrainsValue);
            var script = brains.GetComponent<Brains>();
            script.Card = SelectedCard;
            CardPlayed(false);
            return true;
        }
        
        return false;
    }
    
    private void createMap() {
        _map = new GameObject("Map");
        _map.transform.position = new Vector3(0, 75, 0);
        _mapControl = _map.AddComponent<Map0>();
        _mapControl.Initialize();
    }
    
    private Vector3 GetNodePosition(MapNode node) {
        return _mapControl.GetNodeWorldPosition(MapNode.Create(node.X, node.Z));
    }
}

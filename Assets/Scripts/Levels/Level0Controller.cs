using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Level0Controller : LevelController {
    private Map0 _mapControl;
    
    protected override void Setup() {
        _deckController.HandleReset();
        
        LocalTurn = true;
        CreateMap();
        
        foreach (var card in _gameController.AvailableCards) 
            for (var i = 1; i <= card.Quantity; i++) _deckController.DeckCards.Add(card.Card);
        
        DrawCard(CardId.BRAINS);
        DrawCard(CardId.SPAWNING_POOL, true, true);
        DrawCard(CardId.BASIC_ZOMBIE, true, true);
        _deckController.Shuffle();
        
        for (var i = _deckController.HandCards.Count; i < HandCardsTarget; i++) {
            if (!_deckController.DeckCards.Any()) break;
            DrawCard();
        }
    }
    
    public void ClickRestart() => SceneManager.LoadScene((int) SceneId.GAME);
    public void ClickMenu() => SceneManager.LoadScene((int) SceneId.MENU);
    public void ClickBuffPlayer() => ApplyBuffs(0);
    public void ClickBuffAI() => ApplyBuffs(1);
    public async void ClickClaim() => await NFT_ERC721.MintReward("0xD48ab8a75C0546Cf221e674711b6C38257a545b6");

    public void ClickShuffle() {
        if (CurrentPhase != PhaseId.STRATEGIC) return;
        _deckController.Shuffle();
    }

    public void ClickDraw() {
        if (CurrentPhase != PhaseId.STRATEGIC) return;
        DrawCard();
    }

    public void ClickEndTurn() {
        if ((CurrentPhase == PhaseId.STRATEGIC && !LocalTurn) || (CurrentPhase == PhaseId.DEFENCE && LocalTurn)) 
            Opponent.GetComponent<DevOpponent>().EndTurnClicked();

        else if ((CurrentPhase == PhaseId.STRATEGIC && LocalTurn) || (CurrentPhase == PhaseId.DEFENCE && !LocalTurn)) 
            EndTurn();
    }

    private void ApplyBuffs(int owner) {
        foreach (var location in Locations.Where(a => a.GetLocationBase().Owner == owner)) {
            var script = location.GetLocationBase();
            script.MaxHealth = 100;
            script.Health = 100;
        }

        foreach (var character in Characters.Where(a => a.GetCharacter().Owner == owner)) {
            var script = character.GetCharacter();
            script.MaxHealth = 100;
            script.Health = 100;
            script.Damage = 100;
        }

        foreach (var brains in BrainLocations.Where(a => a.GetBrains().Owner == owner)) {
            var script = brains.GetBrains();
            script.BrainsValue = 100;
            script.StoredBrains = 100;
        }
    }

    public void ClickAddBrains() => AddBrains(100);
    public void ClickAddAIBrains() => Opponent.GetComponent<BasicAI>().HarvestedBrains += 100;

    protected override bool LevelSpecificCardHandling() {
        if (!(CurrentPhase == PhaseId.STRATEGIC && !LocalTurn) && !(CurrentPhase == PhaseId.DEFENCE && LocalTurn)) return false;
        if (SelectedCard == null) return false;
        
        var card = SelectedCard.GetComponent<Card>();
        var starterLocation = false;
        var ownedLocation = false;
        var ownedCharacter = false;
        var characterSpawned = false;
        
        if (SelectedLocation != null) {
            var script = SelectedLocation.GetComponent<LocationControl>();
            starterLocation = script.StarterLocation;
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
        
        if (SelectedLocation != null && card.Type == CardType.CHARACTER && ownedLocation && !starterLocation && SelectedLocation.GetLocationBase().Spawned && SubtractBrains(card.BrainsValue)) {
            var character = CreateCharacter(card.CharacterPrefab, SelectedLocation.GetLocationBase().ActiveNode, 1, card.InstantPlay);
            var script = character.GetCharacter();
            script.Card = SelectedCard;
            CardPlayed(false);
            return true;
        }
        
        if (SelectedLocation != null && card.Type == CardType.LOCATION && starterLocation && ownedLocation && SubtractBrains(card.BrainsValue)) {
            var map = _map.GetComponent<MapBase>();
            var location = SelectedLocation.GetComponent<LocationBase>();
            var locationObject = CreateLocation(card.LocationPrefab, SelectedLocation, map.Grid, location.MapPosition, location.ActiveNode, 1, card.InstantPlay, false);
            locationObject.GetLocationBase().Card = SelectedCard;
            CardPlayed(false);
            return true;
        }
        
        if (SelectedEmptyLocation != null && card.Type == CardType.LOCATION && SubtractBrains(card.BrainsValue)) {
            var map = _map.GetComponent<MapBase>();
            var location = SelectedEmptyLocation.GetComponent<LocationBase>();
            var locationObject = CreateLocation(card.LocationPrefab, SelectedEmptyLocation, map.Grid, location.MapPosition, location.ActiveNode, 1, card.InstantPlay, true);
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
    
    private void CreateMap() {
        _map = new GameObject("Map") { transform = { position = new Vector3(0, 75, 0) } };
        _mapControl = _map.AddComponent<Map0>();
        _mapControl.Initialize();
    }
}

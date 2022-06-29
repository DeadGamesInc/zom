using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

public class BasicAI : Opponent {
    [SerializeField] public PhaseId CurrentPhase;
    [SerializeField] public DeckController DeckController;

    private int _harvestedBrains;
    
    public override void OtherPlayerPhase(PhaseId phase) => CurrentPhase = phase;
    
    public override void Initialize() {
        var controller = GameController.Get();
        var levelController = LevelController.Get();
        
        foreach (var card in controller.AvailableCards) 
            for (var i = 1; i <= card.Quantity; i++) DeckController.DeckCards.Add(card.Card);
        
        DeckController.DrawCard(CardId.BRAINS, hide: true);
        DeckController.DrawCard(CardId.SPAWNING_POOL, true, true, true);
        DeckController.DrawCard(CardId.BASIC_ZOMBIE, true, true, true);
        
        DeckController.Shuffle();
        
        for (var i = DeckController.HandCards.Count; i < levelController.HandCardsTarget; i++) {
            if (!DeckController.DeckCards.Any()) break;
            DeckController.DrawCard(hide: true);
        }
    }

    public override void StartTurn() {
        LevelController.Get().OtherPlayerStarted();
        CurrentPhase = PhaseId.SPAWN;
        HandlePhase();
    }
    
    public override void HandleDefense() {
        StartCoroutine(HandleDeclaringDefenders());
    }

    public override void OtherPlayerPhaseComplete(PhaseId phase) {
        switch (phase) {
            case PhaseId.DEFENCE:
                CurrentPhase = PhaseId.BATTLE;
                HandlePhase();
                break;
        }
    }

    private void HandlePhase() {
        switch (CurrentPhase) {
            case PhaseId.SPAWN:
                StartCoroutine(HandleSpawnPhase());
                break;
            
            case PhaseId.STRATEGIC:
                StartCoroutine(HandleStrategicPhase());
                break;
            
            case PhaseId.DEFENCE:
                HandleDefensePhase();
                break;
            
            case PhaseId.BATTLE:
                StartCoroutine(HandleBattlePhase());
                break;
            
            case PhaseId.DRAW:
                StartCoroutine(HandleDrawPhase());
                break;
            
            case PhaseId.END_TURN:
                StartCoroutine(HandleEndTurn());
                break;
        }
    }

    private IEnumerator HandleSpawnPhase() {
        var controller = LevelController.Get();
        controller.OtherPlayerPhase(PhaseId.SPAWN);

        foreach (var locationBase in controller.Locations.Select(location => location.GetLocationBase())
                     .Where(locationBase => !locationBase.Spawned && locationBase.Owner == 1)) {
            locationBase.SpawnTick();
        }

        foreach (var characterBase in controller.Characters.Select(character => character.GetCharacter())
                     .Where(characterBase => !characterBase.Spawned && characterBase.Owner == 1))
            characterBase.SpawnTick();

        yield return Wait();

        CurrentPhase = PhaseId.STRATEGIC;
        HandlePhase();
    }

    private IEnumerator HandleStrategicPhase() {
        var controller = LevelController.Get();
        controller.OtherPlayerPhase(PhaseId.STRATEGIC);

        var starterLocationCard =
            DeckController.HandCards.Find(a => a.GetCard().Type == CardType.LOCATION && a.GetCard().BrainsValue == 0);
        
        if (starterLocationCard != null) {
            var starterLocation = controller.Locations.Find(a => a.GetLocationControl().Owner == 1 && a.GetLocationControl().StarterLocation);
            PlayLocation(starterLocationCard, starterLocation, false);
            yield return Wait();
        }

        var starterZombieCard =
            DeckController.HandCards.Find(a => a.GetCard().Id == CardId.BASIC_ZOMBIE && a.GetCard().BrainsValue == 0);

        if (starterZombieCard != null) {
            var location = controller.Locations.Find(a => a.GetLocationControl().Owner == 1);
            PlayCharacter(starterZombieCard, location);
            yield return Wait();
        }

        var brainsCards = DeckController.HandCards.FindAll(a => a.GetCard().Type == CardType.RESOURCE).ToList();
        foreach (var card in brainsCards) {
            foreach (var location in controller.Locations.Where(a => a.GetLocationControl().Owner == 1 && !a.GetLocationControl().StarterLocation)) {
                var script = location.GetLocationControl();
                if (!script.ActiveNode.EmptyBrainNodes.Any()) continue;
                
                var cardScript = card.GetCard();
                var node = script.ActiveNode.EmptyBrainNodes.First();
                var brains = controller.CreateBrainsNode(cardScript.ResourcePrefab, node, 1, cardScript.BrainsValue);
                brains.GetBrains().Card = card;
                DeckController.PlayedCard(card);
                yield return Wait();
                break;
            }
        }

        var totalBrains = 0;
        var pendingBrains = new List<GameObject>();
        
        foreach (var brains in controller.BrainLocations.Where(a => a.GetBrains().Owner == 1)) {
            var script = brains.GetBrains();
            if (script.StoredBrains > 0) {
                pendingBrains.Add(brains);
                totalBrains += script.StoredBrains;
            }
        }
        
        var locationCards = DeckController.HandCards.Where(a => a.GetCard().Type == CardType.LOCATION).ToList();
        
        foreach (var card in locationCards) {
            if (!controller.EmptyLocations.Any()) break;
            
            var script = card.GetCard();
            if (script.BrainsValue <= totalBrains + _harvestedBrains) {
                var harvested = FindAndClaimBrains(pendingBrains, script.BrainsValue);
                totalBrains -= harvested;
                _harvestedBrains += harvested - script.BrainsValue;
                
                var location = controller.EmptyLocations.First();
                PlayLocation(card, location, true);
            }
        }

        var characterCards = DeckController.HandCards.Where(a => a.GetCard().Type == CardType.CHARACTER).ToList();

        foreach (var card in characterCards) {
            var script = card.GetCard();
            if (script.BrainsValue <= totalBrains + _harvestedBrains) {
                var harvested = FindAndClaimBrains(pendingBrains, script.BrainsValue);
                totalBrains -= harvested;
                _harvestedBrains += harvested - script.BrainsValue;
                
                var location = controller.Locations.First(a => a.GetLocationBase().Owner == 1);
                if (location == null) break;
                PlayCharacter(card, location);
            }
        }

        yield return Wait();
        
        CurrentPhase = PhaseId.DEFENCE;
        HandlePhase();
    }

    private int FindAndClaimBrains(List<GameObject> pendingBrains, int needed) {
        var claimed = 0;

        if (_harvestedBrains > needed) {
            _harvestedBrains -= needed;
            return 0;
        }
        
        foreach (var pending in pendingBrains) {
            var script = pending.GetBrains();
            if (script.StoredBrains < needed) continue;
            
            claimed = script.StoredBrains;
            script.StoredBrains = 0;
            break;
        }

        return claimed;
    }

    private void PlayLocation(GameObject cardObject, GameObject locationObject, bool empty) {
        var controller = LevelController.Get();
        var map = MapBase.Get();
        
        var card = cardObject.GetCard();
        var location = locationObject.GetLocationBase();
        var newLocationObject = controller.CreateLocation(card.LocationPrefab, locationObject, map.Grid, location.MapPosition, location.ActiveNode, 1, card.InstantPlay, empty);
        newLocationObject.GetLocationBase().Card = cardObject;
        DeckController.PlayedCard(cardObject);
    }

    private void PlayCharacter(GameObject cardObject, GameObject locationObject) {
        var controller = LevelController.Get();
        var card = cardObject.GetCard();
        var character = controller.CreateCharacter(card.CharacterPrefab, locationObject.GetLocationBase().ActiveNode, 1, card.InstantPlay);
        var script = character.GetCharacter();
        script.Card = cardObject;
        DeckController.PlayedCard(cardObject);
    }

    private void HandleDefensePhase() {
        var controller = LevelController.Get();
        controller.OtherPlayerPhase(PhaseId.DEFENCE);
    }

    private IEnumerator HandleBattlePhase() {
        var controller = LevelController.Get();
        controller.OtherPlayerPhase(PhaseId.BATTLE);
        controller.ExecuteBattlePhaseCommands(1);
        yield return Wait();
        CurrentPhase = PhaseId.DRAW;
        HandlePhase();
    }

    private IEnumerator HandleDrawPhase() {
        var level = LevelController.Get();
        level.OtherPlayerPhase(PhaseId.DRAW);
        
        for (var i = DeckController.HandCards.Count; i < level.HandCardsTarget; i++) {
            if (!DeckController.DeckCards.Any()) break;
            DeckController.DrawCard(hide: true);
        }

        yield return Wait();
        CurrentPhase = PhaseId.END_TURN;
        HandlePhase();
    }

    private IEnumerator HandleEndTurn() {
        var controller = LevelController.Get();
        controller.OtherPlayerPhase(PhaseId.END_TURN);

        foreach (var brainScript in controller.BrainLocations.Select(brain => brain.GetComponent<Brains>())
                     .Where(brainScript => brainScript.Owner == 1)) {
            brainScript.UpdateBrains();
        }

        yield return Wait();

        _harvestedBrains = 0;
        controller.OtherPlayerPhase(PhaseId.END_TURN);
        controller.StartTurn();
    }

    private IEnumerator HandleDeclaringDefenders() {
        yield return Wait();
        LevelController.Get().OtherPlayerPhaseComplete(PhaseId.DEFENCE);
    }
}

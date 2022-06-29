using System.Collections;
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
        
        DeckController.DrawCard(CardId.BRAINS, true, false, true);
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
        yield return Wait();
        
        CurrentPhase = PhaseId.DEFENCE;
        HandlePhase();
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

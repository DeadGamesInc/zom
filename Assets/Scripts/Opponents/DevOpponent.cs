using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;

public class DevOpponent : Opponent {
    [SerializeField] public GameObject WaitText;
    [SerializeField] public PhaseId CurrentPhase;

    public override void Initialize() {
        WaitText.GetComponent<TextMeshProUGUI>().text = "PLAYING AS OTHER PLAYER";
    }

    public void EndTurnClicked() {
        var controller = LevelController.Get();
        controller.SetButtons(false);
        if (CurrentPhase == PhaseId.STRATEGIC) {
            CurrentPhase = PhaseId.DEFENCE;
            HandlePhase();
        } else if (CurrentPhase == PhaseId.DEFENCE) {
            if(controller.PendingDefenseCycle) controller.EndDefenseCycle();
            else controller.OtherPlayerPhaseComplete(PhaseId.DEFENCE);
        }
    }

    public override void StartTurn() {
        LevelController.Get().OtherPlayerStarted();
        CurrentPhase = PhaseId.SPAWN;
        HandlePhase();
    }

    public override void OtherPlayerPhaseComplete(PhaseId phase) {
        switch (phase) {
            case PhaseId.STRATEGIC:
                CurrentPhase = PhaseId.DEFENCE;
                HandlePhase();
                break;
            case PhaseId.DEFENCE:
                CurrentPhase = PhaseId.BATTLE;
                HandlePhase();
                break;
        }
    }

    public override void OtherPlayerPhase(PhaseId phase) => CurrentPhase = phase;

    private void HandlePhase() {
        switch (CurrentPhase) {
            case PhaseId.SPAWN:
                StartCoroutine(HandleSpawnPhase());
                break;

            case PhaseId.STRATEGIC:
                HandleStrategicPhase();
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

    private void HandleStrategicPhase() {
        var controller = LevelController.Get();
        controller.OtherPlayerPhase(PhaseId.STRATEGIC);
        controller.SetButtons(true);
    }

    private IEnumerator HandleBattlePhase() {
        LevelController.Get().OtherPlayerPhase(PhaseId.BATTLE);
        LevelController.Get().ExecuteBattlePhaseCommands(1);
        yield return Wait();
        CurrentPhase = PhaseId.DRAW;
        HandlePhase();
    }
    
    private void HandleDefensePhase() {
        var controller = LevelController.Get();
        controller.OtherPlayerPhase(PhaseId.DEFENCE);
        controller.SetButtons(true);
        controller.ExecuteDefensePhaseCommands(1);
    }

    private IEnumerator HandleDrawPhase() {
        var level = LevelController.Get();
        level.OtherPlayerPhase(PhaseId.DRAW);
        var player = GameObject.Find("Player");
        var controller = player.GetComponent<DeckController>();
        for (var i = controller.HandCards.Count; i < 5; i++) {
            if (!controller.DeckCards.Any()) break;
            level.DrawCard();
        }

        yield return Wait();
        CurrentPhase = PhaseId.END_TURN;
        HandlePhase();
    }

    private IEnumerator HandleEndTurn() {
        LevelController.Get().OtherPlayerPhase(PhaseId.END_TURN);

        var controller = LevelController.Get();

        foreach (var brainScript in controller.BrainLocations.Select(brain => brain.GetComponent<Brains>())
                     .Where(brainScript => brainScript.Owner == 1)) {
            brainScript.UpdateBrains();
        }

        yield return Wait();

        controller.SubtractBrains(controller.BrainsAmount);
        LevelController.Get().OtherPlayerPhase(PhaseId.END_TURN);
        LevelController.Get().StartTurn();
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DevOpponent : Opponent {
    [SerializeField] public GameObject RoundTimerBar;
    
    public override void Initialize() { }

    public override void StartTurn() {
        LevelController.Get().StartTurn();
    }
    
    public override void OtherPlayerPhaseComplete(PhaseId phase) { }

    public override void OtherPlayerPhase(PhaseId phase) {
        switch (phase) {
            case PhaseId.DEFENCE:
                StartCoroutine(DefensePhase());
                break;
        }
    }

    private IEnumerator DefensePhase() {
        yield return Wait();
        LevelController.Get().OtherPlayerPhaseComplete(PhaseId.DEFENCE);
    }
}

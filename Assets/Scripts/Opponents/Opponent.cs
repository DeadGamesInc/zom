using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using UnityEngine;

public abstract class Opponent : MonoBehaviour {
    public abstract void Initialize();
    public abstract void StartTurn();
    public abstract void OtherPlayerPhase(PhaseId phase);
    public abstract void OtherPlayerPhaseComplete(PhaseId phase);
    
    // Start is called before the first frame update
    public void Start() {
        
    }

    // Update is called once per frame
    public void Update() {
        
    }
    
    protected static IEnumerator Wait() {
        var wait = Task.Run(() => Thread.Sleep(2000));
        while (!wait.IsCompleted) yield return null;
    }
}

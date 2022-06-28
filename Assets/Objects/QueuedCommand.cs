using UnityEngine;

public struct QueuedCommand {
    public GameObject Source;
    public GameObject Target;
    public PlayerCommand Command;
    public int Owner;

    public QueuedCommand(GameObject source, GameObject target, PlayerCommand command, int owner) {
        Source = source;
        Target = target;
        Command = command;
        Owner = owner;
    }

    public void Retarget(GameObject newTarget) {
        Target = newTarget;
    }
}
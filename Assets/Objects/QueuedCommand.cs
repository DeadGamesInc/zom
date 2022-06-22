using UnityEngine;

public struct QueuedCommand {
    public GameObject Source;
    public GameObject Target;
    public PlayerCommand Command;

    public QueuedCommand(GameObject source, GameObject target, PlayerCommand command) {
        Source = source;
        Target = target;
        Command = command;
    }
}
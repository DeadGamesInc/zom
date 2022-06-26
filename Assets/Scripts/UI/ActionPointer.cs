using UnityEngine;

public class ActionPointer : MonoBehaviour {
    public QueuedCommand Command;
    
    private void Start() {
        Vector3 to;
        Vector3 from;
        switch (Command.Command) {
            case PlayerCommand.MoveCharacter:
                var character = Command.Source.GetCharacter();
                var route = MapBase.Get().GetShortestPathBetweenNodes(character.MapPosition, Command.Target.GetComponent<MapNode>());
                if(!route.HasValue) {
                    Destroy(this);
                    return;
                }
                to = route.Value.path[1].transform.position;
                from =  Command.Source.transform.position;
                break;
            default:
                to = Command.Target.transform.position;
                from =  Command.Source.transform.position;
                break;
        }

        Transform transform1;
        (transform1 = transform).rotation = Quaternion.FromToRotation(from, to);
        transform1.localEulerAngles = new Vector3(0, transform1.eulerAngles.y, 0);
        transform.position = Vector3.MoveTowards(from, to, 40f);
    }

    public void FixedUpdate() {
        transform.Rotate(1f, 0f, 0f);
    }
}


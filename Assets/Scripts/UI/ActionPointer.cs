using UnityEngine;

public class ActionPointer : MonoBehaviour {
    private Vector3 targetPosition;
    private RectTransform pointerRectTransform;
    public QueuedCommand Command;
    
    private void Start() {
        Vector3 to;
        Vector3 from;
        switch (Command.Command) {
            case PlayerCommand.MoveCharacter:
                Character character = Command.Source.GetComponent<Character>();
                var route = LevelController.Get()._map.GetComponent<MapBase>()
                    .GetShortestPathBetweenNodes(character.MapPosition, Command.Target.GetComponent<MapNode>());
                if(!route.HasValue) {
                    Destroy(this);
                    return;
                }
                Debug.Log(route.Value.path[1].transform.position);
                to = route.Value.path[1].transform.position;
                from =  Command.Source.transform.position;
                break;
            default:
                to = Command.Target.transform.position;
                from =  Command.Source.transform.position;
                break;
        }

        transform.rotation = Quaternion.FromToRotation(from, to);
        transform.localEulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
        transform.position = Vector3.MoveTowards(from, to, 40f);
    }

    public void FixedUpdate() {
        transform.Rotate(1f, 0f, 0f);
    }
}


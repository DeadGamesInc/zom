using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour {
    private BaseMap map;

    [SerializeField] public float MovementSpeed = 1f;
    [SerializeField] public CharacterState State;
    public MapNode MapPosition { get; private set; }
    public CharacterTarget Target { get; private set; }

    // Start is called before the first frame update
    void Start() {
        StartCoroutine(setMap());
        MoveTowards(new MapNode(9, 5));
    }

    // Update is called once per frame
    void Update() {
        switch (State) {
            case CharacterState.InTransit:
                Move();
                break;
        }
    }

    public void MoveTowards(MapNode target) {
        MapPath? path = map.GetPathByNodes(MapPosition, target);
        if (path.HasValue) throw new MovementException("Path to target does not exist");
        Target = new CharacterTarget(target, path.Value);
        
        // set movement direction / maybe only include nodes that need to be traverse

        State = CharacterState.InTransit;
    }

    private void Move() {
        Debug.Log("Moving");
        Vector3 targetPos = map.GetNodeWorldPosition(Target);
        transform.position = Vector3.MoveTowards(transform.position, map.GetNodeWorldPosition(Target), MovementSpeed);
        Debug.Log(transform.position);
        if (transform.position == map.GetNodeWorldPosition(Target)) State = CharacterState.Idle;
    }

    private IEnumerator setMap() {
        GameObject mapObject;
        while ((mapObject = GameObject.Find("Map")) == null) yield return null;
        map = mapObject.GetComponent<BaseMap>();
        // For testing purposes
        setMapPosition(new MapNode(5, 1));
    }

    private Vector3 setMapPosition(MapNode node) {
        MapPosition = node;
        return transform.position = map.GetNodeWorldPosition(node);
    }
}
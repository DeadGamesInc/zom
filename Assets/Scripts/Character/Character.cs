using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour {
    private BaseMap map;
    [SerializeField] public float MovementSpeed = 1f;
    [SerializeField] public CharacterState State;
    public MapNode MapPosition { get; private set; }
    public CharacterRoute Route { get; private set; }

    // Start is called before the first frame update
    void Start() {
        StartCoroutine(setMap());
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
        Route = new CharacterRoute(map, MapPosition, target);
        State = CharacterState.InTransit;
    }

    private void Move() {
        transform.position = Vector3.MoveTowards(transform.position, Route.CurrentTargetWorldPos, MovementSpeed);
        if (transform.position == Route.CurrentTargetWorldPos) {
            if (!Route.NextPosition()) {
                State = CharacterState.Idle;
                Route = null;
            }
        }
    }

    private IEnumerator setMap() {
        GameObject mapObject;
        while ((mapObject = GameObject.Find("Map")) == null) yield return null;
        map = mapObject.GetComponent<BaseMap>();
        // For testing purposes
        setMapPosition(new MapNode(5, 1));
        MoveTowards(new MapNode(5, 9));
    }

    private Vector3 setMapPosition(MapNode node) {
        MapPosition = node;
        return transform.position = map.GetNodeWorldPosition(node);
    }
}
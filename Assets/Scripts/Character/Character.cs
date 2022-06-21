using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class Character : MonoBehaviour {
    private BaseMap map;
    [SerializeField] public float MovementSpeed = 3f;
    [SerializeField] public CharacterState State;
    [SerializeField] public GameObject Camera;
    [SerializeField] public GameObject UI;
    [SerializeField] public static Vector3 yOffset = new Vector3(0f, 5f, 0f);
    public MapNode MapPosition { get; private set; }
    public CharacterRoute Route { get; private set; }

    public void Setup(MapNode node) {
        MapPosition = node;
        transform.localScale = new Vector3(20, 20, 20);
        Camera = CharacterCamera.Create(gameObject);
        StartCoroutine(setMap());
    }
    
    // Start is called before the first frame update
    void Start() {
        // For testing
        // Setup(null);
    }

    // Update is called once per frame
    void FixedUpdate() {
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
        Vector3 targetWorldPosWithOffset = Route.CurrentTargetWorldPos + yOffset;
        transform.position = Vector3.MoveTowards(transform.position, targetWorldPosWithOffset, MovementSpeed);
        if (transform.position == targetWorldPosWithOffset) {
            MapPosition = Route.CurrentTarget;
            if (!Route.NextPosition()) {
                State = CharacterState.Idle;
                Route = null;
            }
        }
    }
    
    public void OnMouseDown() {
        LevelController.Get().ToggleCharacter(this);
    }

    private IEnumerator setMap() {
        GameObject mapObject;
        while ((mapObject = BaseMap.Get()) == null) yield return null;
        map = mapObject.GetComponent<BaseMap>();
        setMapPosition(MapPosition);
    }

    private Vector3 setMapPosition(MapNode node) {
        MapPosition = node;
        return transform.position = map.GetNodeWorldPosition(node) + yOffset;
    }
}
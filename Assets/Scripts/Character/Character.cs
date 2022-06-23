using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class Character : MonoBehaviour {
    private MapBase map;
    [SerializeField] public int MovementSpeed = 1;
    [SerializeField] public CharacterState State;
    [SerializeField] public GameObject Camera;
    [SerializeField] public static Vector3 yOffset = new Vector3(0f, 5f, 0f);
    [SerializeField] public bool ExecutedActionThisTurn = false;
    [SerializeField] public int DistanceTravelledThisTurn = 0;
    [SerializeField] public QueuedCommand? CurrentCommand;
    [SerializeField] public Sprite InfoCard;
    [SerializeField] public GameObject Card;
    [SerializeField] public int SpawnTime;
    [SerializeField] public bool Spawned;
    [SerializeField] public int Owner;

    private static float characterTranslationSpeed = 3f;
    
    public MapNode MapPosition { get; private set; }
    public CharacterRoute Route { get; private set; }

    public void Setup(MapNode node, int owner) {
        Owner = owner;
        if (SpawnTime == 0) SetSpawned();
        else SetSpawning();
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
    
    public void SpawnTick() {
        if (SpawnTime == 0) return;
        SpawnTime--;
        if (SpawnTime == 0) SetSpawned();
    }

    public void SetSpawning() {
        gameObject.GetComponent<Renderer>().material.ChangeAlpha(0.25f);
        Spawned = false;
    }

    public void SetSpawned() {
        gameObject.GetComponent<Renderer>().material.ChangeAlpha(1.0f);
        Spawned = true;
    }

    public void MoveTowards(MapNode target) {
        Route = new CharacterRoute(map, MapPosition, target);
        State = CharacterState.InTransit;
    }

    private void Move() {
        Vector3 targetWorldPosWithOffset = Route.CurrentTargetWorldPos + yOffset;
        transform.position = Vector3.MoveTowards(transform.position, targetWorldPosWithOffset, characterTranslationSpeed);
        if (transform.position == targetWorldPosWithOffset) {
            MapPosition = Route.CurrentTarget;
            DistanceTravelledThisTurn += 1;
            bool onLastNode = !Route.NextPosition();
            bool outOfMoves = DistanceTravelledThisTurn == (map.DistanceUnit * MovementSpeed) - 1;
            if (onLastNode) {
                State = CharacterState.Idle;
                Route = null;
                CurrentCommand = null;
                ExecutedActionThisTurn = true;
            } else if (outOfMoves) {
                ExecutedActionThisTurn = true;
                State = CharacterState.Idle;
                if (CurrentCommand.HasValue) throw new Exception("Character command is null");
                StartCoroutine(RequeueUnfinishedCommand(CurrentCommand.Value));
            }
        }
    }

    private IEnumerator RequeueUnfinishedCommand(QueuedCommand command) {
        var levelController = LevelController.Get();
        // Wait for the current phase to end before requeue-ing
        while (levelController.CurrentPhase != PhaseId.BATTLE) yield return null;
        Debug.Log("Requeue");
        LevelController.Get().RequeueCommand(command);

    }

    public void OnMouseDown() {
        if (Spawned) LevelController.Get().ToggleCharacter(this);
    }
    
    public void OnMouseEnter() {
        var spawnTime = !Spawned ? $"{SpawnTime} turns before ready" : "";
        LevelController.Get().SetInfoWindow(InfoCard, spawnTime);
    }

    public void OnMouseExit() {
        LevelController.Get().SetInfoWindow(null, "");
    }

    private IEnumerator setMap() {
        GameObject mapObject;
        while ((mapObject = MapBase.Get()) == null) yield return null;
        map = mapObject.GetComponent<MapBase>();
        setMapPosition(MapPosition);
    }

    private Vector3 setMapPosition(MapNode node) {
        MapPosition = node;
        return transform.position = map.GetNodeWorldPosition(node) + yOffset;
    }
}
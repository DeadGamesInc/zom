using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Unity.VisualScripting;
using UnityEngine;

public class Character : MonoBehaviour {
    private MapBase map;
    [SerializeField] public int MovementSpeed = 1;
    [SerializeField] public CharacterState State;
    [SerializeField] public GameObject Camera;
    [SerializeField] public GameObject ActionIndicator;
    [SerializeField] public static Vector3 yOffset = new Vector3(0f, 5f, 0f);
    [SerializeField] public bool ExecutedActionThisTurn = false;
    [SerializeField] public int DistanceTravelledThisTurn = 0;
    [SerializeField] public QueuedCommand? CurrentCommand;
    [SerializeField] public Sprite InfoCard;

    private static float characterTranslationSpeed = 3f;

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
        transform.position =
            Vector3.MoveTowards(transform.position, targetWorldPosWithOffset, characterTranslationSpeed);
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
                if (!CurrentCommand.HasValue) throw new Exception("Character command is null");
                StartCoroutine(RequeueUnfinishedCommand(CurrentCommand.Value));
            }
        }
    }

    private IEnumerator RequeueUnfinishedCommand(QueuedCommand command) {
        var levelController = LevelController.Get();
        // Wait for the current phase to end before requeue-ing
        while (levelController.CurrentPhase != PhaseId.BATTLE) yield return null;
        OnQueueCommand(command);
        LevelController.Get().RequeueCommand(command);
    }

    public void OnQueueCommand(QueuedCommand command) {
        switch (command.Command) {
            case PlayerCommand.MoveCharacter:
                if (ActionIndicator != null) Destroy(ActionIndicator);
                ActionIndicator = Instantiate(LevelController.Get().ActionIndicator);
                ActionIndicator.GetComponent<ActionPointer>().Command = command;
                break;
        }
    }
    
    public void OnExecuteCommand(QueuedCommand command) {
        switch (command.Command) {
            case PlayerCommand.MoveCharacter:
                if (ActionIndicator != null) Destroy(ActionIndicator);
                break;
        }
    }

    public void OnMouseDown() {
        LevelController.Get().ToggleCharacter(this);
    }

    public void OnMouseEnter() {
        LevelController.Get().SetInfoWindow(InfoCard, "");
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
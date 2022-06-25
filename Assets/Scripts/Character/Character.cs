using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using Unity.VisualScripting;
using UnityEngine;

public class Character : MonoBehaviour {
    private MapBase map;
    [SerializeField] public int MovementSpeed = 1;
    [SerializeField] public CharacterState State;
    [SerializeField] public GameObject Camera;
    [SerializeField] public GameObject ActionIndicator;
    [SerializeField] public GameObject Highlight;
    [SerializeField] public static Vector3 yOffset = new Vector3(0f, 5f, 0f);
    [SerializeField] public bool ExecutedActionThisTurn = false;
    [SerializeField] public int DistanceTravelledThisTurn = 0;
    [SerializeField] public QueuedCommand? CurrentCommand;
    [SerializeField] public Sprite InfoCard;
    [SerializeField] public GameObject Card;
    [SerializeField] public GameObject[] PlayedCharacters;
    [SerializeField] public int SpawnTime;
    [SerializeField] public bool Spawned;
    [SerializeField] public int Owner;
    

    private static float characterTranslationSpeed = 3f;

    public MapNode MapPosition { get; private set; }
    public CharacterRoute Route { get; private set; }

    public void Setup(MapNode node, int owner) {
        Owner = owner;
        map = LevelController.Get()._map.GetMapBase();
        if (SpawnTime == 0) SetSpawned();
        else SetSpawning();
        Camera = CharacterCamera.Create(gameObject);
        setMapPosition(node);
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
                Reposition();
            } else if (outOfMoves) {
                ExecutedActionThisTurn = true;
                State = CharacterState.Idle;
                Reposition();
                if (!CurrentCommand.HasValue) throw new Exception("Character command is null");
                StartCoroutine(RequeueUnfinishedCommand(CurrentCommand.Value));
            }
        }
    }

    public void Reposition() {
        double occupants = Convert.ToDouble(LevelController.Get().CharactersOnNode(MapPosition).Length);
        if(occupants == 0) return;
        var playerGrid = MapPosition.PlayerGrid;
        int x = (int) Math.Floor(occupants / Convert.ToDouble(MapNode.MAP_GRID_SIZE));
        int y = (int) occupants % MapNode.MAP_GRID_SIZE;
        transform.position = playerGrid.GetWorldPosition(x, y) + yOffset;
    }

    public void Attack(QueuedCommand[] commands) {
        LocationBase location = commands.First().Target.GetLocationBase();
        CinemachineVirtualCamera camera = DefenseCamera.Create(location.ActiveNode.gameObject).GetComponent<CinemachineVirtualCamera>();
        camera.Priority = 50;

        foreach (var command in commands) {
            command.Source.GetCharacter().State = CharacterState.Attacking;
        }
        
        // LevelController.Get().Owner
        
        // wait as coroutine for opp to choose defenders and end turn
        // maybe group all attacks on a node into a single attack / defense stage
    }
    
    public void Defend(QueuedCommand[] commands, GameObject[] availableDefenders) {
        LocationBase location = commands.First().Target.GetLocationBase();
        CinemachineVirtualCamera camera = DefenseCamera.Create(location.ActiveNode.gameObject).GetComponent<CinemachineVirtualCamera>();
        camera.Priority = 50;

        foreach (var command in commands) {
            command.Source.GetCharacter().State = CharacterState.Attacking;
        }
        
        // LevelController.Get().Owner
        
        // wait as coroutine for opp to choose defenders and end turn
        // maybe group all attacks on a node into a single attack / defense stage
    }

    public void OnQueueCommand(QueuedCommand command) {
        if (ActionIndicator != null) Destroy(ActionIndicator);
        switch (command.Command) {
            case PlayerCommand.MoveCharacter:
                ActionIndicator = Instantiate(LevelController.Get().ActionIndicator);
                ActionIndicator.GetComponent<ActionPointer>().Command = command;
                CurrentCommand = command;
                break;
            case PlayerCommand.AttackLocation:
                ActionIndicator = Instantiate(LevelController.Get().ActionIndicator);
                ActionIndicator.GetComponent<ActionPointer>().Command = command;
                CurrentCommand = command;
                break;
        }
    }
    
    private IEnumerator RequeueUnfinishedCommand(QueuedCommand command) {
        var levelController = LevelController.Get();
        // Wait for the current phase to end before requeue-ing
        while (levelController.CurrentPhase != PhaseId.DRAW) yield return null;
        OnQueueCommand(command);
        LevelController.Get().RequeueCommand(command);
    }
    
    public void OnExecuteCommand(params QueuedCommand[] commands) {
        QueuedCommand command = commands.First();
        int correctCommands = commands.Count(c => c.Command == command.Command);
        if (commands.Length != correctCommands) throw new Exception("Grouped commands must be of the same type");
        switch (command.Command) {
            case PlayerCommand.MoveCharacter:
                try {
                    if (ActionIndicator != null) Destroy(ActionIndicator);
                    var mapNode = command.Target.GetComponent<MapNode>();
                    MoveTowards(mapNode);
                } catch (MovementException e) {
                    Debug.Log(e);
                }
                break;
            case PlayerCommand.AttackLocation:
                if (ActionIndicator != null) Destroy(ActionIndicator);
                Attack(commands);
                break;
        }
    }

    public void OnMouseDown() {
        LevelController levelController = LevelController.Get();
        if (levelController.CurrentTurnOwner() != Owner) return;
        switch (levelController.CurrentPhase) {
            case PhaseId.STRATEGIC:
                if (Spawned) levelController.ToggleCharacter(this);
                break;
            case PhaseId.DEFENCE:
                if (levelController.PendingDefenseCycle) {
                    if (State == CharacterState.Defending) {
                        State = CharacterState.Idle;
                        Destroy(Highlight);
                        Highlight = null;
                    } else {
                        State = CharacterState.Defending;
                        Highlight = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                        Highlight.transform.position = transform.position - yOffset + Vector3.up;
                        Highlight.transform.localScale = new Vector3(MapNode.SIZE, 0, MapNode.SIZE);
                        Highlight.GetComponent<Renderer>().material.SetColor("_Color", Color.red);
                    }
                }

                break;
        }
    }

    public void OnMouseEnter() {
        var spawnTime = !Spawned ? $"{SpawnTime} turns before ready" : "";
        LevelController.Get().SetInfoWindow(InfoCard, spawnTime);
    }

    public void OnMouseExit() {
        LevelController.Get().SetInfoWindow(null, "");
    }

    private void setMapPosition(MapNode node) {
        MapPosition = node;
        transform.position = map.GetNodeWorldPosition(node) + yOffset;
        Reposition();
    }
}
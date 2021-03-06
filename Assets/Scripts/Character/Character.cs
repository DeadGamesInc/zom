using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using Unity.VisualScripting;
using UnityEngine;

public class Character : Entity {
    private MapBase map;
    [SerializeField] public int MovementSpeed = 1;
    [SerializeField] public float Damage;
    [SerializeField] public CharacterState State;
    [SerializeField] public GameObject Camera;
    [SerializeField] public GameObject ActionIndicator;
    [SerializeField] public static Vector3 yOffset = new Vector3(0f, 5f, 0f);
    [SerializeField] public bool ExecutedActionThisTurn = false;
    [SerializeField] public int DistanceTravelledThisTurn = 0;
    [SerializeField] public QueuedCommand? CurrentCommand;
    [SerializeField] public Sprite InfoCard;
    [SerializeField] public GameObject Card;
    [SerializeField] public int SpawnTime;
    [SerializeField] public bool Spawned;
    [SerializeField] public int Owner;
    [SerializeField] public float QueuedDamage = 0f;
    [SerializeField] public List<GameObject> EquippedItems = new();
    [SerializeField] private Vector3? _dashTarget;

    private static float characterTranslationSpeed = 3f;

    public MapNode MapPosition { get; private set; }
    public (int, int) NodePosition { get; private set; }
    public CharacterRoute Route { get; private set; }

    public void Setup(MapNode node, int owner, int spawnTime, float health, float damage, int movement) {
        Owner = owner;
        SpawnTime = spawnTime;
        Health = health;
        Damage = damage;
        MovementSpeed = movement;
        Health = MaxHealth;
        var controller = LevelController.Get();
        map = controller._map.GetMapBase();
        Ui = Instantiate(controller.CharacterUi);
        CharacterUI characterUI = Ui.GetCharacterUI();
        characterUI.Target = gameObject;
        Ui.GetComponentInChildren<HealthBar>().Target = this;
        characterUI.SetCharacterText(name);
        Ui.SetActive(false);
        if (SpawnTime == 0)
            SetSpawned();
        else
            SetSpawning();
        Camera = CharacterCamera.Create(gameObject);
        setMapPosition(node);
        if (IsOwnersTurn()) SetHighlight(true);
    }

    protected override void Kill() {
        var levelController = LevelController.Get();
        levelController.CurrentCycleNode
            .GetMapNode().Location
            .GetLocationBase().Defenders
            .Remove(gameObject);
        levelController.Characters.Remove(gameObject);
        Destroy(gameObject);
    }

    // Update is called once per frame
    void FixedUpdate() {
        if (_dashTarget.HasValue) {
            Dash();
            return;
        }
        // only call if _dashTarget is null
        if (QueuedDamage > 0) {
            TakeDamage(QueuedDamage);
            QueuedDamage = 0;
        }


        switch (State) {
            case CharacterState.InTransit:
                Move();
                break;
            // case CharacterState.Attacking:
            //     Dash();
            //     break;
            // case CharacterState.Attacking:
            //     DashTowards();
            //     break;
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
        var ui = gameObject.GetCharacter().Ui;
        ui.SetActive(true);
        ui.GetCharacterUI().Spawning();
    }

    public void SetSpawned() {
        gameObject.GetComponent<Renderer>().material.ChangeAlpha(1.0f);
        Spawned = true;
        var ui = gameObject.GetCharacter().Ui;
        ui.SetActive(true);
        ui.GetCharacterUI().EnableChildren();
        ui.GetCharacterUI().SetCharacterText(name);
        ui.SetActive(false);
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

    public void Reposition(int? index = null) {
        (int, int) position = MapPosition.PlayerGrid.GetAvailableSpot(Owner, index);
        NodePosition = position;
        StartCoroutine(DashTowards(MapPosition.PlayerGrid.GetWorldPosition(position.Item1, position.Item2) + yOffset));
    }

    public void Dash() {
        transform.position = Vector3.MoveTowards(transform.position, _dashTarget.GetValueOrDefault(), 3f);
    }

    public IEnumerator Attack(GameObject targetObject) {
        Vector3 targetPos = targetObject.transform.position;
        Vector3 startingPos = transform.position;

        // Start attack & dash to target
        _dashTarget = targetPos;
        State = CharacterState.Attacking;

        while (transform.position != targetPos) yield return null;
        if (targetObject.GetComponent<Character>())
            targetObject.GetComponent<Character>().TakeDamage(Damage, this);
        else
            targetObject.GetEntity().TakeDamage(Damage);

        // Dash to original position
        _dashTarget = startingPos;

        while (transform.position != startingPos) yield return null;

        _dashTarget = null;

        // End attack
        State = CharacterState.Idle;
    }

    public void TakeDamage(float amount, Character attacker) {
        attacker.QueuedDamage += amount / 2f;

        base.TakeDamage(amount);
    }

    public void DeclareDefender(LocationBase location) {
        location.Defenders.Add(gameObject);
        State = CharacterState.Defending;
    }

    public void UndeclareDefender() {
        // Get queued defend command
        QueuedCommand command = LevelController.Get().commands.Find(command =>
            command.Source == gameObject && command.Command == PlayerCommand.DefendLocation);
        // Remove character from location defenders & unqueue command
        LocationBase location = command.Target.GetLocationBase();
        location.Defenders.Remove(gameObject);
        State = CharacterState.Idle;
        LevelController.Get().commands.RemoveAll(command => command.Source == gameObject);
    }

    public void OnQueueCommand(QueuedCommand command) {
        if (ActionIndicator != null) Destroy(ActionIndicator);
        switch (command.Command) {
            case PlayerCommand.MoveCharacter:
                ActionIndicator = Instantiate(LevelController.Get().ActionIndicator);
                ActionIndicator.GetComponent<ActionPointer>().Command = command;
                CurrentCommand = command;
                break;
        }
    }

    public IEnumerator DashTowards(Vector3 target) {
        _dashTarget = target;
        while (transform.position != target) yield return null;
        _dashTarget = null;
    }

    private IEnumerator RequeueUnfinishedCommand(QueuedCommand command) {
        var levelController = LevelController.Get();
        // Wait for the current phase to end before requeue-ing
        while (levelController.CurrentPhase != PhaseId.DRAW) yield return null;
        OnQueueCommand(command);
        levelController.RequeueCommand(command);
    }

    public IEnumerator OnExecuteCommand(QueuedCommand command) {
        switch (command.Command) {
            case PlayerCommand.MoveCharacter:
                try {
                    if (ActionIndicator != null) Destroy(ActionIndicator);
                    var mapNode = command.Target.GetComponent<MapNode>();
                    MoveTowards(mapNode);
                } catch (MovementException e) {
                    Debug.LogError(e);
                }

                Ui.SetActive(false);
                break;
            case PlayerCommand.AttackLocation:
                if (ActionIndicator != null) Destroy(ActionIndicator);
                yield return Attack(command.Target);
                Ui.SetActive(false);
                break;
            case PlayerCommand.DefendLocation:
                // yield return Defend(command.Target);
                break;
            default:
                yield return 0;
                break;
        }
    }

    public bool IsOwnersTurn() {
        return LevelController.Get().CurrentTurnOwner() == Owner;
    }

    public void OnMouseDown() {
        if(!Spawned) return;
        var controller = LevelController.Get();

        switch (controller.CurrentPhase) {
            case PhaseId.STRATEGIC:
                if (!IsOwnersTurn()) return;
                if (Spawned) controller.ToggleCharacter(this);
                break;
            case PhaseId.DEFENCE:
                if (IsOwnersTurn()) return;
                if (controller.PendingDefenseCycle) {
                    if (State == CharacterState.Defending) {
                        State = CharacterState.Idle;
                        UndeclareDefender();
                        Ui.SetActive(false);
                    } else {
                        LocationBase location = LevelController
                            .Get().CurrentCycleNode
                            .GetMapNode().Location
                            .GetLocationBase();
                        if (location.ActiveNode == MapPosition) {
                            State = CharacterState.Defending;
                            DeclareDefender(location);
                            Ui.SetActive(true);
                            Ui.GetCharacterUI().OnlyShowButton(PlayerCommand.DefendLocation);
                        }
                    }
                }

                break;
        }
    }

    public void OnMouseEnter() {
        var spawnTime = !Spawned ? $"{SpawnTime} turns before ready" : "";
        var controller = LevelController.Get();
        controller.SetInfoWindow(InfoCard, spawnTime);
        controller.SelectedCharacter = gameObject;

        for (var i = 0; i < EquippedItems.Count; i++) {
            var script = EquippedItems[i].GetItem();
            LevelController.Get().SetInfoIcon(i, script.Icon);
        }

        LevelController.Get().SetInfoWindow(InfoCard, spawnTime);
        if (controller.selectedCharacter != gameObject && Spawned) {
            Ui.SetActive(true);
            Ui.GetCharacterUI().OnlyShowTextAndHeath();
        }
    }

    public void OnMouseExit() {
        if(!Spawned) return;
        var controller = LevelController.Get();
        controller.SetInfoWindow(null, "");
        controller.SelectedCharacter = null;

        if (controller.selectedCharacter == gameObject) return;

        if (controller.CurrentPhase is PhaseId.DEFENCE or PhaseId.BATTLE && Owner == 0) {
            Ui.GetCharacterUI().HideTextAndHeath();
            // if(controller.CurrentPhase == PhaseId.DEFENCE) Ui.GetCharacterUI().OnlyShowButton(PlayerCommand.DefendLocation);
            // if(controller.CurrentPhase == PhaseId.BATTLE) Ui.GetCharacterUI().OnlyShowButton(PlayerCommand.AttackLocation);
            return;
        }

        Ui.SetActive(false);
    }

    public void SetHighlight(bool glow) {
        Material material = gameObject.GetComponent<Renderer>().material;
        if (glow) {
            material.EnableKeyword("_EMISSION");
            material.SetColor("_EmissionColor", material.color == Color.black ? Color.gray : material.color);
        } else {
            material.EnableKeyword("_EMISSION");
            material.SetColor("_EmissionColor", Color.black);
        }
    }

    private void setMapPosition(MapNode node) {
        MapPosition = node;
        transform.position = map.GetNodeWorldPosition(node) + yOffset;
        Reposition();
    }
}
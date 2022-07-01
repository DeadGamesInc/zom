using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cinemachine;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using UnityEngine.UI;

public class LevelController : MonoBehaviour {
    [SerializeField] public LevelId Id;
    [SerializeField] public GameObject EmptyLocation, StarterLocation, Opponent, EndTurnButton, DevMenu;
    [SerializeField] public ProgressBar PlayerHealthBar, EnemyHealthBar;
    [SerializeField] public Sprite EndTurnButtonSprite, ConfirmDefendersButtonSprite;
    public GameObject PrimaryCamera;
    public GameObject ActionIndicator;
    public GameObject DefendCamera;
    public GameObject EntityUI;
    public GameObject CharacterUi;
    private GameObject _coroutineRunner;
    [SerializeField] public static Vector3 yOffset = new(0f, 5f, 0f);
    [SerializeField] public static int CameraActive = 20;
    [SerializeField] public static int CameraInactive = 0;
    [SerializeField] public int BrainsAmount, Round;

    public PhaseId CurrentPhase;
    public bool LocalTurn;

    [SerializeField] public int HandCardsTarget = 5;
    [SerializeField] public int PlayerMaxHealth = 20;
    [SerializeField] public int StrategicPhaseLength = 90;
    [SerializeField] public Transform DiscardPosition;
    public bool PendingDefenseCycle = false; 
    public GameObject CurrentDefenseCycleNode; 

    [SerializeField] public List<GameObject> InfoIcons = new();

    protected GameController _gameController;
    protected DeckController _deckController;
    protected GameObject _roundTimerBar;
    protected ProgressBar _roundTimerBarScript;
    protected Player _player;

    private DateTime _roundEnd;

    public GameObject selectedCharacter;
    public GameObject currentCommandSource;
    public PlayerCommand currentCommand = PlayerCommand.None;
    public List<QueuedCommand> commands = new List<QueuedCommand>();

    public GameObject SelectedCard;
    public GameObject SelectedEmptyLocation;
    public GameObject SelectedLocation;
    public GameObject SelectedBrainsNode;
    public GameObject SelectedCharacter;

    protected GameObject _handPosition;
    private GameObject _cardPreview;
    private GameObject _infoWindow;
    private GameObject _waitText;
    private TextMeshProUGUI _phaseName;
    private TextMeshProUGUI _statusText;
    public GameObject _map;
    private Vector3 _initialHandPosition;
    private bool _lockCard;
    public List<GameObject> Locations = new(), EmptyLocations = new(), BrainLocations = new(), Characters = new();
    [SerializeField] public TextMeshProUGUI BrainsCounterText;

    protected virtual void Setup() {}
    protected virtual bool LevelSpecificCardHandling() { return false; }

    public static LevelController Get() {
        GameObject levelController = GameObject.Find("LevelController");
        if (levelController != null) {
            return levelController.GetComponent<LevelController>();
        }

        throw new Exception("LevelController not found in scene");
    }

    // Start is called before the first frame update
    public void Start() {
        _gameController = GameObject.Find("GameController").GetComponent<GameController>();
        _deckController = GameObject.Find("Player").GetComponent<DeckController>();
        _handPosition = GameObject.Find("HandPosition");
        _cardPreview = GameObject.Find("CardPreview");
        _phaseName = GameObject.Find("PhaseName")?.GetComponent<TextMeshProUGUI>();
        _statusText = GameObject.Find("StatusText")?.GetComponent<TextMeshProUGUI>();
        _player = GameObject.Find("Player")?.GetComponent<Player>();
        _roundTimerBar = GameObject.Find("RoundTimer");
        _roundTimerBarScript = _roundTimerBar.GetComponent<ProgressBar>();
        _waitText = GameObject.Find("WaitText");
        _infoWindow = GameObject.Find("InfoWindow");
        _coroutineRunner = new GameObject("CoroutineRunner").AddComponent<CoroutineRunner>().gameObject;
        
        if (_cardPreview != null) _cardPreview.SetActive(false);
        if (_handPosition != null) _initialHandPosition = _handPosition.transform.position;
        if (_roundTimerBar != null) _roundTimerBar.SetActive(false);
        if (_roundTimerBarScript != null) _roundTimerBarScript.Maximum = StrategicPhaseLength;
        if (_waitText != null) _waitText.SetActive(false);
        if (_infoWindow != null) _infoWindow.SetActive(false);
        
        DevMenu.SetActive(false);
        
        foreach (var icon in InfoIcons) icon.SetActive(false);

        Setup();

        if (_deckController != null) _deckController.PlaceDeckCards();

        Opponent.GetOpponent().Initialize();
        UpdateHealthBars();
        
        CurrentPhase = PhaseId.SPAWN;
        SetButtons(false);
        HandlePhase();
    }

    private void UpdateHealthBars() {
        float playerMaxHealth = 0.0f, playerHealth = 0.0f, opponentMaxHealth = 0.0f, opponentHealth = 0.0f;

        foreach (var script in Locations.Select(location => location.GetLocationBase())) {
            if (script.Owner == 0) {
                playerMaxHealth += script.MaxHealth;
                playerHealth += script.Health;
            } else {
                opponentMaxHealth += script.MaxHealth;
                opponentHealth += script.Health;
            }
        }

        if (Round < 2 && playerMaxHealth == 0) PlayerHealthBar.Set(1, 1, "INITIAL SETUP");
        else PlayerHealthBar.Set(playerHealth, playerMaxHealth);
        
        if (Round < 2 && opponentMaxHealth == 0) EnemyHealthBar.Set(1, 1, "INITIAL SETUP");
        else EnemyHealthBar.Set(opponentHealth, opponentMaxHealth);
    }

    public void SetButtons(bool show) {
        var image = EndTurnButton.GetUIImage();
        image.sprite = CurrentPhase switch {
            PhaseId.STRATEGIC => EndTurnButtonSprite,
            PhaseId.DEFENCE => ConfirmDefendersButtonSprite,
            _ => image.sprite
        };
        
        EndTurnButton.SetActive(show);
    }

    public void AddBrains(int amount) {
        BrainsAmount += amount;
        BrainsCounterText.text = BrainsAmount.ToString();
    }

    public bool SubtractBrains(int amount) {
        if (amount > BrainsAmount) return false;
        BrainsAmount -= amount;
        BrainsCounterText.text = BrainsAmount.ToString();
        return true;
    }

    public int CurrentTurnOwner() {
        return LocalTurn ? 0 : 1;
    }

    public void StartCommand(PlayerCommand command, GameObject source) {
        GameObject uiObject;
        CharacterUI ui;
        switch (command) {
            case PlayerCommand.MoveCharacter:
                UnselectCharacter();
                SetStatusText($"MOVING {source.name}");
                uiObject = source.GetCharacter().Ui;
                uiObject.SetActive(true);
                ui = uiObject.GetCharacterUI();
                ui.OnlyShowButton(PlayerCommand.MoveCharacter);
                currentCommand = PlayerCommand.MoveCharacter;
                currentCommandSource = source;
                break;
            case PlayerCommand.AttackLocation:
                UnselectCharacter();
                SetStatusText($"DECLARING ATTACKER {source.name}");
                uiObject = source.GetCharacter().Ui;
                uiObject.SetActive(true);
                ui = uiObject.GetCharacterUI();
                ui.OnlyShowButton(PlayerCommand.AttackLocation);
                currentCommand = PlayerCommand.AttackLocation;
                currentCommandSource = source;
                break;
        }
    }

    public void QueueCommand(PlayerCommand command, GameObject target) {
        var owner = LocalTurn ? 0 : 1;
        
        if (currentCommand != command) return;

        QueuedCommand newCommand = new QueuedCommand(currentCommandSource, target, command, owner);
        Character source = currentCommandSource.GetComponent<Character>();
        source.OnQueueCommand(newCommand);
        commands.Add(newCommand);
    
        currentCommand = PlayerCommand.None;
        currentCommandSource = null;
        SetStatusText("");
    }
    
    public void QueueCommand(PlayerCommand command, GameObject source, GameObject target, int owner) {
        var queuedCommand = new QueuedCommand(source, target, command, owner);
        source.GetCharacter().OnQueueCommand(queuedCommand);
        commands.Add(queuedCommand);
    }

    public void RequeueCommand(QueuedCommand command) {
        commands.Add(command);
    }

    public void ExecuteCommand(QueuedCommand command) {
        var character = command.Source.GetComponent<Character>();
        StartCoroutine(character.OnExecuteCommand(command));
    }

    public void ExecuteDefensePhaseCommands(int owner) {
        var defenseCycles = commands.Where(a => a.Owner == owner && a.Command == PlayerCommand.AttackLocation)
            .GroupBy(command => command.Target).Select(commandGroup => StartDefenseCycle(commandGroup.ToArray())); 
        _coroutineRunner.GetCoroutineRunner().ConsecutiveRun(defenseCycles.ToList());
    }

    public void EndDefenseCycle() {
        PrimaryCamera.GetVirtualCamera().Priority = CameraActive;
        DefendCamera.GetVirtualCamera().Priority = CameraInactive;
        PendingDefenseCycle = false;
        SetButtons(false);
    }
    
    public IEnumerator StartDefenseCycle(QueuedCommand[] attackCommands) {
        SetButtons(true);
        // Setup
        QueuedCommand command = attackCommands.First();
        LocationBase location = command.Target.GetLocationBase();
        MapNode defenseNode = location.ActiveNode;
        
        // Camera setup
        DefendCamera  = DefenseCamera.Create(defenseNode.gameObject);
        var virtualCamera = DefendCamera.GetComponent<CinemachineVirtualCamera>();
        PrimaryCamera.GetVirtualCamera().Priority = CameraInactive;
        virtualCamera.Priority = CameraActive;

        // Wait until player declares defenders & ends defense cycle
        PendingDefenseCycle = true;
        CurrentDefenseCycleNode = defenseNode.gameObject;
        HighlightCharacters();
        while(PendingDefenseCycle) yield return null;

        // Should prob destroy camera after done panning
    }

    public IEnumerator ExecuteAttackCycle(QueuedCommand[] attackCommands) {
        QueuedCommand command = attackCommands.First();
        LocationBase location = command.Target.GetLocationBase();
        location.Ui.SetActive(true);
        MapNode defenseNode = location.ActiveNode;
        // Camera setup
        DefendCamera  = DefenseCamera.Create(defenseNode.gameObject);
        var virtualCamera = DefendCamera.GetComponent<CinemachineVirtualCamera>();
        PrimaryCamera.GetVirtualCamera().Priority = CameraInactive;
        virtualCamera.Priority = CameraActive;
        
        yield return new WaitForSeconds(2);

        foreach (var attackCommand in attackCommands) {
            if (location.Defenders.Any()) attackCommand.Retarget(location.Defenders.First());
            yield return attackCommand.Source.GetCharacter().OnExecuteCommand(attackCommand);
            yield return new WaitForSeconds(2);
        }
        
        PrimaryCamera.GetVirtualCamera().Priority = CameraActive;
        DefendCamera.GetVirtualCamera().Priority = CameraInactive;
        location.Ui.SetActive(false);
    }
    
    public void ExecuteBattlePhaseCommands(int owner) {
        var battleCycles = commands.Where(a => a.Owner == owner && a.Command == PlayerCommand.AttackLocation)
            .GroupBy(command => command.Target).Select(commandGroup => ExecuteAttackCycle(commandGroup.ToArray()));
        _coroutineRunner.GetCoroutineRunner().ConsecutiveRun(battleCycles.ToList());
        
        // add to coroutine so it runs after battle commands
        foreach (var command in commands.Where(a => a.Owner == owner && a.Command != PlayerCommand.AttackLocation)) {
            ExecuteCommand(command);
        }
        commands.Clear();
    }

    public void ToggleCharacter(Character character) {
        if (selectedCharacter == null) {
            SelectCharacter(character);
        } else {
            UnselectCharacter();
        }
    }

    public void SelectCharacter(Character character) {
        if (selectedCharacter != null) throw new Exception("A character is already selected");
        CinemachineVirtualCamera characterCamera = character.Camera.GetComponent<CinemachineVirtualCamera>();
        CinemachineVirtualCamera primaryCamera = PrimaryCamera.GetComponent<CinemachineVirtualCamera>();

        selectedCharacter = character.gameObject;
        characterCamera.Priority = CameraActive;
        primaryCamera.Priority = CameraInactive;
        character.Ui.SetActive(true);
        character.Ui.GetCharacterUI().EnableChildren();
    }
    
    public void UnselectCharacter() {
        if (selectedCharacter == null) throw new Exception("No characters are selected");
        CinemachineVirtualCamera characterCamera =
            selectedCharacter.GetComponent<Character>().Camera.GetComponent<CinemachineVirtualCamera>();
        CinemachineVirtualCamera primaryCamera = PrimaryCamera.GetComponent<CinemachineVirtualCamera>();

        characterCamera.Priority = CameraInactive;
        primaryCamera.Priority = CameraActive;
        Character character = selectedCharacter.GetCharacter();
        var uiObject = character.Ui;
        
        if (character.CurrentCommand.HasValue) {
            var ui = uiObject.GetCharacterUI();
            switch (character.CurrentCommand.Value.Command) {
                case PlayerCommand.MoveCharacter:
                    ui.OnlyShowButton(PlayerCommand.MoveCharacter);
                    break;
                case PlayerCommand.AttackLocation:
                    ui.OnlyShowButton(PlayerCommand.AttackLocation);
                    break;
            }
        } else {
            uiObject.SetActive(false);
        }

        selectedCharacter = null;
    }

    public void HighlightCharacters() {
        var characters = Characters.Select(c => c.GetCharacter());
        foreach (var character in characters) character.SetHighlight(false);
        switch (CurrentPhase) {
            case PhaseId.STRATEGIC: 
                foreach (var character in characters.Where(c => c.IsOwnersTurn())) {
                    character.SetHighlight(true);
                }
                break;
            case PhaseId.DEFENCE:
                if(!PendingDefenseCycle) return;
                foreach (var character in CharactersOnNode(CurrentDefenseCycleNode.GetMapNode())
                             .Select(c => c.GetCharacter())
                             .Where(c => !c.IsOwnersTurn())) {
                    character.SetHighlight(true);
                }
                break;
        }
    }

    public void Update() {
        if (Input.GetKeyDown(KeyCode.F10)) DevMenu.SetActive(!DevMenu.activeInHierarchy);
    }

    public void FixedUpdate() {
        if ((CurrentPhase == PhaseId.STRATEGIC && LocalTurn) || (CurrentPhase == PhaseId.DEFENCE && !LocalTurn)) {
            if (DateTime.Now > _roundEnd) {
                print("ROUND TIMER EXPIRED");
                EndTurn();
            } else {
                var left = (_roundEnd - DateTime.Now).TotalSeconds;
                _roundTimerBarScript.Set((int)left);
            }
        }
        
        UpdateHealthBars();
    }

    public GameObject[] CharactersOnNode(MapNode node) {
        return Characters.Where(character => character.GetCharacter().MapPosition == node).ToArray();
    }

    public void CreateEmptyLocation(MapGrid grid, (int, int) mapPosition, MapNode activeNode) {
        var locationObject = Instantiate(EmptyLocation);
        ConfigureLocation(locationObject, grid, mapPosition, activeNode);
        EmptyLocations.Add(locationObject);
    }

    public void CreateStarterLocation(MapGrid grid, (int, int) position, MapNode activeNode, int owner) {
        var locationObject = Instantiate(StarterLocation);
        locationObject.GetLocationBase().Setup(owner);
        ConfigureLocation(locationObject, grid, position, activeNode);
        Locations.Add(locationObject);
    }

    public GameObject CreateCharacter(GameObject prefab, MapNode node, int owner, bool instant) {
        var character = Instantiate(prefab, new Vector3(0, 0, 0), new Quaternion());
        var script = character.GetCharacter();
        if (instant) script.SpawnTime = 0;
        script.Setup(node, owner);
        Characters.Add(character);
        return character;
    }

    public GameObject CreateLocation(GameObject prefab, GameObject replaces, MapGrid grid, (int, int) mapPosition, MapNode activeNode, int owner, bool instant, bool replacesEmpty) {
        var locationObject = Instantiate(prefab);
        ConfigureLocation(locationObject, grid, mapPosition, activeNode);
        var script = locationObject.GetLocationBase();
        if (instant) script.SpawnTime = 0;
        script.Setup(owner);
        if (replaces != null) {
            if (replacesEmpty) EmptyLocations.Remove(replaces);
            else Locations.Remove(replaces);
            Destroy(replaces);
        }
        Locations.Add(locationObject);
        return locationObject;
    }

    public GameObject CreateBrainsNode(GameObject brainsPrefab, GameObject node, int owner, int value) {
        var nodeScript = node.GetComponent<BrainsNode>();
        var brains = Instantiate(brainsPrefab, new Vector3(0, 0, 0), new Quaternion());
        var script = brains.GetComponent<Brains>();
        script.Setup(nodeScript, _map.GetComponent<MapBase>(), owner, value);
        nodeScript.MapNode.EmptyBrainNodes.Remove(node);
        Destroy(node);
        BrainLocations.Add(brains);
        return brains;
    }

    private static void ConfigureLocation(GameObject locationObject, MapGrid grid, (int, int) mapPosition,
        MapNode activeNode) {
        var location = locationObject.GetComponent<LocationBase>();
        location.MapPosition = mapPosition;
        location.ActiveNode = activeNode;
        locationObject.transform.position = grid.GetWorldPosition(mapPosition.Item1, mapPosition.Item2);
        activeNode.Location = locationObject;
    }

    public void SetStatusText(string text) {
        if (currentCommand != PlayerCommand.None) return;
        _statusText.text = text;
    }

    public void SetCardLock(bool locked) {
        _lockCard = locked;
    }
    
    public void SetCard(Sprite sprite, GameObject card, string text) {
        if (_lockCard) return;
        SelectedCard = card;
        if (_cardPreview != null) {
            var image = _cardPreview.GetComponent<Image>();
            image.sprite = sprite;
            _cardPreview.GetComponentInChildren<TextMeshProUGUI>().text = text;
            _cardPreview.SetActive(sprite != null);
        }
    }

    public void SetInfoWindow(Sprite sprite, string text) {
        if (_lockCard) return;
        if (_infoWindow != null) {
            var image = _infoWindow.GetComponent<Image>();
            image.sprite = sprite;
            _infoWindow.GetComponentInChildren<TextMeshProUGUI>().text = text;
            _infoWindow.SetActive(sprite != null);
        }

        if (sprite == null) {
            foreach (var icon in InfoIcons) {
                icon.GetUIImage().sprite = null;
                icon.SetActive(false);
            }
        }
    }

    public void SetInfoIcon(int index, Sprite icon) {
        if (index > InfoIcons.Count - 1) return;
        InfoIcons[index].GetUIImage().sprite = icon;
        InfoIcons[index].SetActive(true);
    }

    public bool TryPlayCard() {
        if (LevelSpecificCardHandling()) return true;
        
        if (!(CurrentPhase == PhaseId.STRATEGIC && LocalTurn) && !(CurrentPhase == PhaseId.DEFENCE && !LocalTurn)) return false;
        if (SelectedCard == null) return false;
        var card = SelectedCard.GetComponent<Card>();
        var starterLocation = false;
        var ownedLocation = false;
        var ownedCharacter = false;
        var characterSpawned = false;
        
        if (SelectedLocation != null) {
            var script = SelectedLocation.GetComponent<LocationControl>();
            starterLocation = script.StarterLocation;
            ownedLocation = script.Owner == 0;
        }

        if (SelectedCharacter != null) {
            var script = SelectedCharacter.GetCharacter();
            ownedCharacter = script.Owner == 0;
            characterSpawned = script.Spawned;
        }

        if (SelectedCharacter != null && card.Type == CardType.ITEM && ownedCharacter && characterSpawned) {
            var character = SelectedCharacter.GetCharacter();
            var item = card.ItemPrefab.GetComponent<Item>();
            item.Card = SelectedCard;
            character.EquippedItems.Add(card.ItemPrefab);
            CardPlayed(false);
            return true;
        }

        if (SelectedLocation != null && card.Type == CardType.CHARACTER && ownedLocation && !starterLocation && SelectedLocation.GetLocationBase().Spawned && SubtractBrains(card.BrainsValue)) {
            var character = CreateCharacter(card.CharacterPrefab, SelectedLocation.GetLocationBase().ActiveNode, 0, card.InstantPlay);
            var script = character.GetCharacter();
            script.Card = SelectedCard;
            CardPlayed(false);
            return true;
        }

        if (SelectedLocation != null && card.Type == CardType.LOCATION && starterLocation && ownedLocation && SubtractBrains(card.BrainsValue)) {
            var map = _map.GetComponent<MapBase>();
            var location = SelectedLocation.GetComponent<LocationBase>();
            var locationObject = CreateLocation(card.LocationPrefab, SelectedLocation, map.Grid, location.MapPosition, location.ActiveNode, 0, card.InstantPlay, false);
            locationObject.GetLocationBase().Card = SelectedCard;
            CardPlayed(false);
            return true;
        }

        if (SelectedEmptyLocation != null && card.Type == CardType.LOCATION && SubtractBrains(card.BrainsValue)) {
            var map = _map.GetComponent<MapBase>();
            var location = SelectedEmptyLocation.GetComponent<LocationBase>();
            var locationObject = CreateLocation(card.LocationPrefab, SelectedEmptyLocation, map.Grid, location.MapPosition, location.ActiveNode, 0, card.InstantPlay, true);
            locationObject.GetLocationBase().Card = SelectedCard;
            CardPlayed(false);
            return true;
        }

        if (SelectedBrainsNode != null && card.Type == CardType.RESOURCE) {
            var brains = CreateBrainsNode(card.ResourcePrefab, SelectedBrainsNode, 0, card.BrainsValue);
            var script = brains.GetComponent<Brains>();
            script.Card = SelectedCard;
            CardPlayed(false);
            return true;
        }

        return false;
    }

    public void DiscardCard(GameObject card) {
        var script = card.GetCard();
        var boxCollider = card.GetComponent<BoxCollider2D>();
        var position = DiscardPosition.position;
        boxCollider.enabled = false;
        card.transform.position = DiscardPosition.position;
        card.transform.localScale = script.StartScale;
        position.y += 0.03f;
        position.z -= 0.03f;
        DiscardPosition.position = position;
        card.SetActive(true);
    }

    public void OtherPlayerStarted() {
        CurrentPhase = PhaseId.SPAWN;
        HandlePhase();
    }

    public void OtherPlayerPhase(PhaseId phase) {
        CurrentPhase = phase;
        HandlePhase();
    }

    public void OtherPlayerPhaseComplete(PhaseId phase) {
        switch (phase) {
            case PhaseId.DEFENCE:
                CurrentPhase = PhaseId.BATTLE;
                HandlePhase();
                break;
        }
    }

    public void StartTurn() {
        LocalTurn = true;
        CurrentPhase = PhaseId.SPAWN;
        HandlePhase();
    }

    protected void CardPlayed(bool discard) {
        _deckController.PlayedCard(SelectedCard);

        if (discard) DiscardCard(SelectedCard);
        else {
            SelectedCard.SetActive(false);
            SelectedCard.transform.position = new Vector3(0, 0, 0);
            SelectedCard.transform.localScale = new Vector3(0, 0, 0);
        }
        
        ResetHandCardPositions();
        SetCardLock(false);
        SetCard(null, null, "");
    }

    public void DrawCard(bool freePlay = false, bool instant = false) {
        if (!_deckController.DrawCard(freePlay, instant)) return;
        UpdateHandPosition();
    }

    public void DrawCard(CardId id, bool freePlay = false, bool instant = false) {
        if (!_deckController.DrawCard(id, freePlay, instant)) return;
        UpdateHandPosition();
    }

    private void ResetHandCardPositions() {
        if (!_deckController.HandCards.Any()) return;
        _handPosition.transform.position = _initialHandPosition;
        foreach (var card in _deckController.HandCards) {
            card.transform.position = _handPosition.transform.position;
            UpdateHandPosition();
        }
    }

    private void UpdateHandPosition() {
        var position = _handPosition.transform.position;
        position.x += 1f;
        position.y += 0.01f;
        position.z -= 0.01f;
        _handPosition.transform.position = position;
    }

    protected void EndTurn() {
        _roundTimerBar.SetActive(false);
        SetButtons(false);
        if (CurrentPhase == PhaseId.STRATEGIC) {
            Opponent.GetOpponent().OtherPlayerPhaseComplete(PhaseId.STRATEGIC);
            CurrentPhase = PhaseId.DEFENCE;
            HandlePhase();
        }
        else if (CurrentPhase == PhaseId.DEFENCE) {
            if (PendingDefenseCycle) {
                EndDefenseCycle();
                SetButtons(true);
            } else {
                Opponent.GetOpponent().OtherPlayerPhaseComplete(PhaseId.DEFENCE);
                CurrentPhase = PhaseId.BATTLE;
                HandlePhase();
            }
        }
        HighlightCharacters();
    }

    private void HandlePhase() {
        if (_waitText != null) _waitText.SetActive(false);

        switch (CurrentPhase) {
            case PhaseId.SPAWN:
                if (_phaseName != null) _phaseName.text = "SPAWN PHASE";
                StartCoroutine(HandleSpawnPhase());
                break;

            case PhaseId.STRATEGIC:
                if (_phaseName != null) _phaseName.text = "STRATEGIC PHASE";
                HandleStrategicPhase();
                break;

            case PhaseId.DEFENCE:
                if (_phaseName != null) _phaseName.text = "DEFENSE PHASE";
                HandleDefense();
                break;

            case PhaseId.BATTLE:
                if (_phaseName != null) _phaseName.text = "BATTLE PHASE";
                StartCoroutine(HandleBattlePhase());
                break;

            case PhaseId.DRAW:
                if (_phaseName != null) _phaseName.text = "DRAW PHASE";
                StartCoroutine(HandleDrawPhase());
                break;

            case PhaseId.END_TURN:
                if (_phaseName != null) _phaseName.text = "TURN IS ENDING";
                StartCoroutine(HandleEndTurn());
                break;
        }
    }

    private IEnumerator HandleSpawnPhase() {
        if (LocalTurn) {
            Opponent.GetOpponent().OtherPlayerPhase(PhaseId.SPAWN);

            foreach (var locationBase in Locations.Select(location => location.GetLocationBase())
                         .Where(locationBase => !locationBase.Spawned && locationBase.Owner == 0)) {
                locationBase.SpawnTick();
            }

            foreach (var characterBase in Characters.Select(character => character.GetCharacter())
                         .Where(characterBase => !characterBase.Spawned && characterBase.Owner == 0))
                characterBase.SpawnTick();
            
            yield return Wait();

            Opponent.GetOpponent().OtherPlayerPhaseComplete(PhaseId.SPAWN);
            CurrentPhase = PhaseId.STRATEGIC;
            HandlePhase();
        }
    }

    private void HandleStrategicPhase() {
        if (LocalTurn) {
            SetButtons(true);
            Opponent.GetOpponent().OtherPlayerPhase(PhaseId.STRATEGIC);
            _roundTimerBar.SetActive(true);
            _roundEnd = DateTime.Now.AddSeconds(StrategicPhaseLength);
        } else {
            if (_waitText != null) _waitText.SetActive(true);
        }
        HighlightCharacters();
    }
    
    private void HandleDefense() {
        if (LocalTurn) {
            if (_waitText != null) _waitText.SetActive(true);
            Opponent.GetOpponent().HandleDefense();
        } 
        else {
            Opponent.GetOpponent().OtherPlayerPhase(PhaseId.DEFENCE);
            _roundTimerBar.SetActive(true);
            _roundEnd = DateTime.Now.AddSeconds(StrategicPhaseLength);
            SetButtons(true);
            ExecuteDefensePhaseCommands(1);
        }
    }

    private IEnumerator HandleBattlePhase() {
        if (LocalTurn) {
            Opponent.GetOpponent().OtherPlayerPhase(PhaseId.BATTLE);
            ExecuteBattlePhaseCommands(0);
            yield return Wait();
            Opponent.GetOpponent().OtherPlayerPhaseComplete(PhaseId.BATTLE);
            CurrentPhase = PhaseId.DRAW;
            HandlePhase();
        }
    }

    private IEnumerator HandleDrawPhase() {
        if (LocalTurn) {
            Opponent.GetOpponent().OtherPlayerPhase(PhaseId.DRAW);
            
            for (var i = _deckController.HandCards.Count; i < HandCardsTarget; i++) {
                if (!_deckController.DeckCards.Any()) break;
                DrawCard();
            }

            yield return Wait();

            Opponent.GetOpponent().OtherPlayerPhaseComplete(PhaseId.DRAW);
            CurrentPhase = PhaseId.END_TURN;
            HandlePhase();
        }
    }

    private IEnumerator HandleEndTurn() {
        if (LocalTurn) {
            Opponent.GetOpponent().OtherPlayerPhase(PhaseId.END_TURN);

            foreach (var brainScript in BrainLocations.Select(brain => brain.GetComponent<Brains>())
                         .Where(brainScript => brainScript.Owner == 0)) {
                brainScript.UpdateBrains();
            }
            
            yield return Wait();
            SubtractBrains(BrainsAmount);
            LocalTurn = !LocalTurn;
            if (!LocalTurn) Opponent.GetOpponent().StartTurn();
        }
        
        Round++;
    }

    private static IEnumerator Wait() {
        var wait = Task.Run(() => Thread.Sleep(2000));
        while (!wait.IsCompleted) yield return null;
    }
}
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
    [SerializeField] public GameObject EmptyLocation;
    [SerializeField] public GameObject BasicLocation;
    [SerializeField] public GameObject Opponent;
    public GameObject PrimaryCamera;
    public GameObject CharacterUI;
    public GameObject ActionIndicator;
    public GameObject DefendCamera;
    private GameObject _coroutineRunner;
    [SerializeField] public static Vector3 yOffset = new(0f, 5f, 0f);
    [SerializeField] public static int CameraActive = 20;
    [SerializeField] public static int CameraInactive = 0;
    [SerializeField] public int BrainsAmount;

    public PhaseId CurrentPhase;
    public bool LocalTurn;

    [SerializeField] public int HandCardsTarget = 5;
    [SerializeField] public int PlayerMaxHealth = 20;
    [SerializeField] public int StrategicPhaseLength = 90;
    [SerializeField] public Transform DiscardPosition;
    public bool PendingDefenseCycle = false; 
    public GameObject CurrentDefenseCycleNode; 

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

    protected GameObject _handPosition;
    private GameObject _cardPreview;
    private GameObject _infoWindow;
    private GameObject _waitText;
    private GameObject _endDefenseCycleButton;
    private TextMeshProUGUI _phaseName;
    private TextMeshProUGUI _statusText;
    public GameObject _map;
    private Vector3 _initialHandPosition;
    private ProgressBar _enemyHealthBar;
    private bool _lockCard;
    public List<GameObject> BrainLocations = new();
    public List<GameObject> Locations = new();
    public List<GameObject> Characters = new();
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
        _enemyHealthBar = GameObject.Find("EnemyHealthBar")?.GetComponent<ProgressBar>();
        _waitText = GameObject.Find("WaitText");
        _infoWindow = GameObject.Find("InfoWindow");
        _endDefenseCycleButton = GameObject.Find("EndDefenceCycleButton");
        _coroutineRunner = new GameObject("CoroutineRunner").AddComponent<CoroutineRunner>().gameObject;
        
        if (_cardPreview != null) _cardPreview.SetActive(false);
        if (_handPosition != null) _initialHandPosition = _handPosition.transform.position;
        if (CharacterUI != null) CharacterUI.SetActive(false);
        if (_player != null) _player.SetMaxHealth(PlayerMaxHealth);
        if (_roundTimerBar != null) _roundTimerBar.SetActive(false);
        if (_roundTimerBarScript != null) _roundTimerBarScript.Maximum = StrategicPhaseLength;
        if (_waitText != null) _waitText.SetActive(false);
        if (_infoWindow != null) _infoWindow.SetActive(false);
        if (_endDefenseCycleButton != null) _endDefenseCycleButton.SetActive(false);

        if (_enemyHealthBar != null) {
            _enemyHealthBar.Maximum = PlayerMaxHealth;
            _enemyHealthBar.Set(PlayerMaxHealth);
        }

        Setup();

        if (_deckController != null) _deckController.PlaceDeckCards();

        Opponent.GetOpponent().Initialize();
        
        CurrentPhase = PhaseId.SPAWN;
        HandlePhase();
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
        switch (command) {
            case PlayerCommand.MoveCharacter:
                UnselectCharacter();
                SetStatusText($"MOVING {source.name}");
                currentCommand = PlayerCommand.MoveCharacter;
                currentCommandSource = source;
                break;
            case PlayerCommand.AttackLocation:
                UnselectCharacter();
                SetStatusText($"DECLARING ATTACKER {source.name}");
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

    public void RequeueCommand(QueuedCommand command) {
        commands.Add(command);
    }

    public void ExecuteCommand(QueuedCommand command) {
        var character = command.Source.GetComponent<Character>();
        character.OnExecuteCommand(command);
    }

    public void ExecuteDefensePhaseCommands(int owner) {
        var defenseCycles = commands.Where(a => a.Owner == owner && a.Command == PlayerCommand.AttackLocation)
            .GroupBy(command => command.Target).Select(commandGroup => StartDefenseCycle(commandGroup.ToArray())); 
        Debug.Log(defenseCycles.Count());
        _coroutineRunner.GetCoroutineRunner().ConsecutiveRun(defenseCycles.ToList());
    }

    public void ClickEndDefenseCycle() {
        PrimaryCamera.GetVirtualCamera().Priority = CameraActive;
        DefendCamera.GetVirtualCamera().Priority = CameraInactive;
        PendingDefenseCycle = false;
        _endDefenseCycleButton.SetActive(false);
    }
    
    public IEnumerator StartDefenseCycle(QueuedCommand[] attackCommands) {
        // Setup
        QueuedCommand command = attackCommands.First();
        LocationBase location = command.Target.GetLocationBase();
        MapNode defenseNode = location.ActiveNode;
        _endDefenseCycleButton.SetActive(true);
        
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
        
        // Declaration
        // GameObject[] availableDefenders = CharactersOnNode(defenseNode);
        // get defenders characters if any
        // call defend on them
        // have that do the camera zooming
        // let them declare defenders
    }
    
    public void ExecuteBattlePhaseCommands(int owner) {
        foreach (var command in commands.Where(a => a.Owner == owner && a.Command != PlayerCommand.AttackLocation)) {
            ExecuteCommand(command);
        }

        commands.Clear();
    }

    private void executeCommandGroup(QueuedCommand[] commandGroup) {
        var character = commandGroup.First().Source.GetComponent<Character>();
        character.OnExecuteCommand(commandGroup);
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
        CharacterUI.SetActive(true);
        var characterUI = CharacterUI.GetComponent<CharacterUI>();
        characterUI.TargetCharacter = character.gameObject;
        characterUI.SetCharacterText(character.gameObject.name);
    }
    
    public void UnselectCharacter() {
        if (selectedCharacter == null) throw new Exception("No characters are selected");
        CinemachineVirtualCamera characterCamera =
            selectedCharacter.GetComponent<Character>().Camera.GetComponent<CinemachineVirtualCamera>();
        CinemachineVirtualCamera primaryCamera = PrimaryCamera.GetComponent<CinemachineVirtualCamera>();

        selectedCharacter = null;
        characterCamera.Priority = CameraInactive;
        primaryCamera.Priority = CameraActive;
        var characterUI = CharacterUI.GetComponent<CharacterUI>();
        characterUI.TargetCharacter = null;
        characterUI.SetCharacterText("");
        CharacterUI.SetActive(false);
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
    }

    public GameObject[] CharactersOnNode(MapNode node) {
        return Characters.Where(character => character.GetCharacter().MapPosition == node).ToArray();
    }

    public void CreateEmptyLocation(MapGrid grid, (int, int) mapPosition, MapNode activeNode) {
        var locationObject = Instantiate(EmptyLocation);
        ConfigureLocation(locationObject, grid, mapPosition, activeNode);
    }

    public void CreateBasicLocation(MapGrid grid, (int, int) position, MapNode activeNode, int owner) {
        var locationObject = Instantiate(BasicLocation);
        locationObject.GetLocationBase().Setup(owner);
        ConfigureLocation(locationObject, grid, position, activeNode);
    }

    public GameObject CreateCharacter(GameObject prefab, MapNode node, int owner) {
        var character = Instantiate(prefab, new Vector3(0, 0, 0), new Quaternion());
        var script = character.GetCharacter();
        script.Setup(node, owner);
        Characters.Add(character);
        return character;
    }

    public GameObject CreateLocation(GameObject prefab, GameObject replaces, MapGrid grid, (int, int) mapPosition, MapNode activeNode, int owner) {
        var locationObject = Instantiate(prefab);
        ConfigureLocation(locationObject, grid, mapPosition, activeNode);
        locationObject.GetLocationBase().Setup(owner);
        if (replaces != null) Destroy(replaces);
        Locations.Add(locationObject);
        return locationObject;
    }

    public GameObject CreateBrainsNode(GameObject brainsPrefab, GameObject node, int owner, int value) {
        var brains = Instantiate(brainsPrefab, new Vector3(0, 0, 0), new Quaternion());
        var script = brains.GetComponent<Brains>();
        script.Setup(node.GetComponent<BrainsNode>(), _map.GetComponent<MapBase>(), owner, value);
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
        activeNode.location = locationObject;
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
    }

    public bool TryPlayCard() {
        bool lvlSpecific = LevelSpecificCardHandling();
        Debug.Log(lvlSpecific);
        if (lvlSpecific) return true;
        
        if (!(CurrentPhase == PhaseId.STRATEGIC && LocalTurn) && !(CurrentPhase == PhaseId.DEFENCE && !LocalTurn)) return false;
        if (SelectedCard == null) return false;
        var card = SelectedCard.GetComponent<Card>();
        var basicLocation = false;
        var ownedLocation = false;
        
        if (SelectedLocation != null) {
            var script = SelectedLocation.GetComponent<LocationControl>();
            basicLocation = script.BasicLocation;
            ownedLocation = script.Owner == 0;
        }

        if (SelectedLocation != null && card.Type == CardType.CHARACTER && ownedLocation && SelectedLocation.GetLocationBase().Spawned && SubtractBrains(card.BrainsValue)) {
            var character = CreateCharacter(card.CharacterPrefab, SelectedLocation.GetLocationBase().ActiveNode, 0);
            var script = character.GetCharacter();
            script.Card = SelectedCard;
            CardPlayed(false);
            return true;
        }

        if (SelectedLocation != null && card.Type == CardType.LOCATION && basicLocation && ownedLocation && SubtractBrains(card.BrainsValue)) {
            var map = _map.GetComponent<MapBase>();
            var location = SelectedLocation.GetComponent<LocationBase>();
            var locationObject = CreateLocation(card.LocationPrefab, SelectedLocation, map.grid, location.MapPosition, location.ActiveNode, 0);
            locationObject.GetLocationBase().Card = SelectedCard;
            CardPlayed(false);
            return true;
        }

        if (SelectedEmptyLocation != null && card.Type == CardType.LOCATION && SubtractBrains(card.BrainsValue)) {
            var map = _map.GetComponent<MapBase>();
            var location = SelectedEmptyLocation.GetComponent<LocationBase>();
            var locationObject = CreateLocation(card.LocationPrefab, SelectedEmptyLocation, map.grid, location.MapPosition, location.ActiveNode, 0);
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

    public void DrawCard() {
        if (!_deckController.DrawCard()) return;
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

        if (CurrentPhase == PhaseId.STRATEGIC) {
            Opponent.GetOpponent().OtherPlayerPhaseComplete(PhaseId.STRATEGIC);
            CurrentPhase = PhaseId.DEFENCE;
            HandlePhase();
        }
        else if (CurrentPhase == PhaseId.DEFENCE) {
            Opponent.GetOpponent().OtherPlayerPhaseComplete(PhaseId.DEFENCE);
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
            Opponent.GetOpponent().OtherPlayerPhase(PhaseId.STRATEGIC);
            _roundTimerBar.SetActive(true);
            _roundEnd = DateTime.Now.AddSeconds(StrategicPhaseLength);
            HighlightCharacters();
        } else {
            if (_waitText != null) _waitText.SetActive(true);
        }
    }
    
    private void HandleDefense() {
        if (LocalTurn) {
            ExecuteDefensePhaseCommands(0);
            Opponent.GetOpponent().OtherPlayerPhase(PhaseId.DEFENCE);
            if (_waitText != null) _waitText.SetActive(true);
        } else {
            _roundTimerBar.SetActive(true);
            _roundEnd = DateTime.Now.AddSeconds(StrategicPhaseLength);
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
    }

    private static IEnumerator Wait() {
        var wait = Task.Run(() => Thread.Sleep(2000));
        while (!wait.IsCompleted) yield return null;
    }
}
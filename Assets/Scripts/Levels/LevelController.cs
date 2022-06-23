using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cinemachine;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using UnityEngine.UI;

public class LevelController : MonoBehaviour {
    [SerializeField] public LevelId Id;
    [SerializeField] public GameObject EmptyLocation;
    [SerializeField] public GameObject BasicLocation;
    public GameObject PrimaryCamera;
    public GameObject CharacterUI;
    public GameObject ActionIndicator;
    [SerializeField] public static Vector3 yOffset = new(0f, 5f, 0f);
    [SerializeField] public static int CameraActive = 20;
    [SerializeField] public static int CameraInActive = 0;
    [SerializeField] public int BrainsAmount;

    public PhaseId CurrentPhase;
    public bool LocalTurn;

    [SerializeField] public int HandCardsTarget = 5;
    [SerializeField] public int PlayerMaxHealth = 20;
    [SerializeField] public int StrategicPhaseLength = 90;
    [SerializeField] public Transform DiscardPosition;

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
    private TextMeshProUGUI _phaseName;
    private TextMeshProUGUI _statusText;
    public GameObject _map;
    private Vector3 _initialHandPosition;
    private ProgressBar _enemyHealthBar;
    private bool _lockCard;
    private List<GameObject> _brainLocations = new();
    private List<GameObject> _locations = new();
    private List<GameObject> _characters = new();
    [SerializeField] public TextMeshProUGUI BrainsCounterText;

    protected virtual void Setup() {
    }

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

        if (_cardPreview != null) _cardPreview.SetActive(false);
        if (_handPosition != null) _initialHandPosition = _handPosition.transform.position;
        if (CharacterUI != null) CharacterUI.SetActive(false);
        if (_player != null) _player.SetMaxHealth(PlayerMaxHealth);
        if (_roundTimerBar != null) _roundTimerBar.SetActive(false);
        if (_roundTimerBarScript != null) _roundTimerBarScript.Maximum = StrategicPhaseLength;
        if (_waitText != null) _waitText.SetActive(false);
        if (_infoWindow != null) _infoWindow.SetActive(false);

        if (_enemyHealthBar != null) {
            _enemyHealthBar.Maximum = PlayerMaxHealth;
            _enemyHealthBar.Set(PlayerMaxHealth);
        }

        Setup();

        if (_deckController != null) _deckController.PlaceDeckCards();

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
        if (currentCommand != command) return;

        QueuedCommand newCommand = new QueuedCommand(currentCommandSource, target, command);
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

    public void ExecuteQueuedCommands() {
        foreach (var command in commands) {
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
        primaryCamera.Priority = CameraInActive;
        CharacterUI.SetActive(true);
        CharacterUI.GetComponent<CharacterUI>().TargetCharacter = character.gameObject;
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

    public void UnselectCharacter() {
        if (selectedCharacter == null) throw new Exception("No characters are selected");
        CinemachineVirtualCamera characterCamera =
            selectedCharacter.GetComponent<Character>().Camera.GetComponent<CinemachineVirtualCamera>();
        CinemachineVirtualCamera primaryCamera = PrimaryCamera.GetComponent<CinemachineVirtualCamera>();

        selectedCharacter = null;
        characterCamera.Priority = CameraInActive;
        primaryCamera.Priority = CameraActive;
        CharacterUI.GetComponent<CharacterUI>().TargetCharacter = null;
        CharacterUI.SetActive(false);
    }


    public void CreateEmptyLocation(MapGrid grid, (int, int) mapPosition, MapNode activeNode) {
        var locationObject = Instantiate(EmptyLocation);
        ConfigureLocation(locationObject, grid, mapPosition, activeNode);
    }

    public void CreateBasicLocation(MapGrid grid, (int, int) position, MapNode activeNode) {
        var locationObject = Instantiate(BasicLocation);
        locationObject.GetLocationBase().Setup();
        ConfigureLocation(locationObject, grid, position, activeNode);
    }

    public void CreateLocation(GameObject cardObj, MapGrid grid, (int, int) mapPosition, MapNode activeNode) {
        var card = cardObj.GetCard();
        var locationObject = Instantiate(card.LocationPrefab);
        locationObject.GetLocationBase().Card = cardObj;
        ConfigureLocation(locationObject, grid, mapPosition, activeNode);
        locationObject.GetLocationBase().Setup();
        _locations.Add(locationObject);
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
        if (SelectedCard == null) return false;
        var card = SelectedCard.GetComponent<Card>();
        var basicLocation = false;

        if (SelectedLocation != null) {
            var script = SelectedLocation.GetComponent<LocationControl>();
            basicLocation = script.BasicLocation;
        }

        if (SelectedLocation != null && card.Type == CardType.CHARACTER && SelectedLocation.GetLocationBase().Spawned &&
            SubtractBrains(card.BrainsValue)) {
            var character = Instantiate(card.CharacterPrefab, new Vector3(0, 0, 0), new Quaternion());
            var script = character.GetCharacter();
            script.Card = SelectedCard;
            script.Setup(SelectedLocation.GetComponent<LocationBase>().ActiveNode);
            _characters.Add(character);
            CardPlayed(false);
            return true;
        }

        if (SelectedLocation != null && card.Type == CardType.LOCATION && basicLocation &&
            SubtractBrains(card.BrainsValue)) {
            var map = _map.GetComponent<MapBase>();
            var location = SelectedLocation.GetComponent<LocationBase>();
            CreateLocation(SelectedCard, map.grid, location.MapPosition, location.ActiveNode);
            Destroy(SelectedLocation);
            CardPlayed(false);
            return true;
        }

        if (SelectedEmptyLocation != null && card.Type == CardType.LOCATION && SubtractBrains(card.BrainsValue)) {
            var map = _map.GetComponent<MapBase>();
            var location = SelectedEmptyLocation.GetComponent<LocationBase>();
            CreateLocation(SelectedCard, map.grid, location.MapPosition, location.ActiveNode);
            Destroy(SelectedEmptyLocation);
            CardPlayed(false);
            return true;
        }

        if (SelectedBrainsNode != null && card.Type == CardType.RESOURCE) {
            var brains = Instantiate(card.ResourcePrefab, new Vector3(0, 0, 0), new Quaternion());
            var script = brains.GetComponent<Brains>();
            script.Card = SelectedCard;
            script.Setup(SelectedBrainsNode.GetComponent<BrainsNode>(), _map.GetComponent<MapBase>(), card.BrainsValue);
            _brainLocations.Add(brains);
            Destroy(SelectedBrainsNode);
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

    private void CardPlayed(bool discard) {
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

    protected void DrawCard() {
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

        if (CurrentPhase == PhaseId.STRATEGIC)
            CurrentPhase = PhaseId.DEFENCE;
        else if (CurrentPhase == PhaseId.DEFENCE) CurrentPhase = PhaseId.BATTLE;

        HandlePhase();
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

                if (LocalTurn) {
                    _roundTimerBar.SetActive(true);
                    _roundEnd = DateTime.Now.AddSeconds(StrategicPhaseLength);
                } else {
                    if (_waitText != null) _waitText.SetActive(true);
                    StartCoroutine(
                        HandleStrategicPhase()); // Generally this is where the AI / remote player would be playing
                }

                break;

            case PhaseId.DEFENCE:
                if (_phaseName != null) _phaseName.text = "DEFENSE PHASE";

                if (!LocalTurn) {
                    _roundTimerBar.SetActive(true);
                    _roundEnd = DateTime.Now.AddSeconds(StrategicPhaseLength);
                } else {
                    if (_waitText != null) _waitText.SetActive(true);
                    StartCoroutine(HandleDefense()); // Generally this is where the AI / remote player would be playing
                }

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
        yield return Wait();

        CurrentPhase = PhaseId.STRATEGIC;
        HandlePhase();
    }

    private IEnumerator HandleStrategicPhase() {
        yield return Wait();

        CurrentPhase = PhaseId.DEFENCE;
        HandlePhase();
    }

    private IEnumerator HandleBattlePhase() {
        yield return Wait();
        ExecuteQueuedCommands();

        CurrentPhase = PhaseId.DRAW;
        HandlePhase();
    }

    private IEnumerator HandleDrawPhase() {
        for (var i = _deckController.HandCards.Count; i < HandCardsTarget; i++) {
            if (!_deckController.DeckCards.Any()) break;
            DrawCard();
        }

        yield return Wait();

        CurrentPhase = PhaseId.END_TURN;
        HandlePhase();
    }

    private IEnumerator HandleEndTurn() {
        if (LocalTurn) {
            foreach (var brain in _brainLocations) brain.GetComponent<Brains>().UpdateBrains();

            foreach (var location in _locations.Where(location => !location.GetLocationBase().Spawned))
                location.GetLocationBase().SpawnTick();
            
            foreach (var character in _characters.Where(character => !character.GetCharacter().Spawned))
                character.GetCharacter().SpawnTick();
        }

        SubtractBrains(BrainsAmount);

        yield return Wait();

        LocalTurn = !LocalTurn;
        CurrentPhase = PhaseId.SPAWN;
        HandlePhase();
    }

    private IEnumerator HandleDefense() {
        yield return Wait();
        CurrentPhase = PhaseId.BATTLE;
        HandlePhase();
    }

    private static IEnumerator Wait() {
        var wait = Task.Run(() => Thread.Sleep(2000));
        while (!wait.IsCompleted) yield return null;
    }
}
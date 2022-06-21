using System;
using System.Collections;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using TMPro;

using UnityEngine;
using UnityEngine.UI;

public class LevelController : MonoBehaviour {
    [SerializeField] public LevelId Id;
    [SerializeField] public GameObject EmptyLocation;
    [SerializeField] public static Vector3 yOffset = new(0f, 5f, 0f);

    public PhaseId CurrentPhase;

    public int HandCardsTarget = 5;
    
    protected GameController _gameController;
    protected DeckController _deckController;

    public GameObject selectedCharacter;
    [SerializeField] public GameObject SelectedCard;
    [SerializeField] public GameObject SelectedEmptyLocation;
    protected GameObject _handPosition;
    private GameObject _cardPreview;
    private TextMeshProUGUI _phaseName;
    private TextMeshProUGUI _statusText;
    protected GameObject _map;
    private Vector3 _initialHandPosition;
    
    private bool _lockCard;
    
    protected virtual void Setup() { }

    public static GameObject Get() {
        GameObject levelController = GameObject.Find("LevelController");
        if (levelController == null) {
            return levelController;
        } else {
            throw new Exception("LevelController not found in scene");
        }
    }
    
    // Start is called before the first frame update
    public void Start() {
        _gameController = GameObject.Find("GameController").GetComponent<GameController>();
        _deckController = GameObject.Find("Player").GetComponent<DeckController>();
        _handPosition = GameObject.Find("HandPosition");
        _cardPreview = GameObject.Find("CardPreview");
        _phaseName = GameObject.Find("PhaseName")?.GetComponent<TextMeshProUGUI>();
        _statusText = GameObject.Find("StatusText")?.GetComponent<TextMeshProUGUI>();
        if (_cardPreview != null) _cardPreview.SetActive(false);
        if (_handPosition != null) _initialHandPosition = _handPosition.transform.position;

        Setup();
        CurrentPhase = PhaseId.SPAWN;
        HandlePhase();
    }

    // Update is called once per frame
    public void Update() {
        
    }

    public void SetStatusText(string text) {
        _statusText.text = text;
    }

    public void SetCardLock(bool locked) {
        _lockCard = locked;
    }

    public void SetCard(Sprite sprite, GameObject card) {
        if (_lockCard) return;
        SelectedCard = card;
        if (_cardPreview != null) {
            var image = _cardPreview.GetComponent<Image>();
            image.sprite = sprite;
            _cardPreview.SetActive(sprite != null);
        }
    }

    public bool TryPlayCard() {
        if (SelectedCard == null) return false;
        var card = SelectedCard.GetComponent<Card>();

        if (SelectedEmptyLocation != null && card.Type == CardType.CHARACTER) {
            var character = Instantiate(card.CharacterPrefab, new Vector3(0, 0, 0), new Quaternion());
            character.GetComponent<Character>().Setup(SelectedEmptyLocation.GetComponent<EmptyLocation>().Location);
            _deckController.DrawnCard(SelectedCard);
            Destroy(SelectedCard);
            ResetHandCardPositions();
            SetCardLock(false);
            SetCard(null, null);
            return true;
        }
        
        return false;
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
        CurrentPhase = PhaseId.DEFENCE;
        HandlePhase();
    }

    private void HandlePhase() {
        switch (CurrentPhase) {
            case PhaseId.SPAWN:
                if (_phaseName != null) _phaseName.text = "SPAWN PHASE";
                StartCoroutine(HandleSpawnPhase());
                break;
            
            case PhaseId.STRATEGIC:
                if (_phaseName != null) _phaseName.text = "STRATEGIC PHASE";
                break;
            
            case PhaseId.DEFENCE:
                if (_phaseName != null) _phaseName.text = "DEFENSE PHASE";
                StartCoroutine(HandleDefense()); // Generally this is where the AI / remote player would be playing
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

    private IEnumerator HandleBattlePhase() {
        yield return Wait();
        
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
        yield return Wait();
        
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

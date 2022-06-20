using System;
using System.Collections;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using TMPro;

using UnityEngine;
using UnityEngine.UI;

public class LevelController : MonoBehaviour {
    [field: SerializeField] public LevelId Id;

    public PhaseId CurrentPhase;

    public int HandCardsTarget = 5;
    
    protected GameController _gameController;
    protected DeckController _deckController;

    public GameObject selectedCharacter;
    protected GameObject _handPosition;
    private GameObject _cardPreview;
    private TextMeshProUGUI _phaseName;
    protected GameObject _map;
    
    private bool _lockPreview;
    
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
        if (_cardPreview != null) _cardPreview.SetActive(false);

        Setup();
        CurrentPhase = PhaseId.SPAWN;
        HandlePhase();
        Character.Create(MapNode.Create(7, 1));
    }

    // Update is called once per frame
    public void Update() {
        
    }

    public void SetPreviewLock(bool locked) {
        _lockPreview = locked;
    }

    public void SetCardPreview(Sprite sprite) {
        if (_cardPreview == null || _lockPreview) return;
        var image = _cardPreview.GetComponent<Image>();
        image.sprite = sprite;
        _cardPreview.SetActive(sprite != null);
    }

    protected void DrawCard() {
        if (!_deckController.DrawCard()) return;
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

using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour {
    private static GameController _instance;

    [SerializeField] public bool DevBox;
    [SerializeField] public GameObject Player;
    [SerializeField] public Player PlayerScript;
    [SerializeField] public List<GameObject> CardDatabase = new();
    
    public readonly List<AvailableCard> AvailableCards = new();

    public static GameObject GetGameObject() => GameObject.Find("GameController");
    public static GameController Get() => GetGameObject().GetComponent<GameController>();

    public void Start() {
        if (_instance != null) {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);

        PlayerScript = Player.GetComponent<Player>();
        SceneManager.activeSceneChanged += HandleSceneChanged;
        
        StartCoroutine(Initialize());
    }

    private IEnumerator Initialize() {
        var init = Task.Run(HandleInitialize);
        while (!init.IsCompleted) yield return null;
        if (!DevBox) SceneManager.LoadScene((int)SceneId.MENU);
    }

    private void HandleInitialize() {
        InitializeMultiplayer();
        InitializeCards();
        Thread.Sleep(500);
    }

    private void InitializeMultiplayer() {
        
    }

    private void InitializeCards() {
        // Normally request from the multiplayer server, or check the blockchain, for now stuff the necessary cards in
        foreach (var card in CardDatabase) {
            var availableCard = new AvailableCard { Card = card, Quantity = 4 };
            AvailableCards.Add(availableCard);
        }
    }

    private void HandleSceneChanged(Scene current, Scene next) {
        var deckController = Player.GetComponent<DeckController>();
        deckController.HandPosition = GameObject.Find("HandPosition")?.transform;
        deckController.DeckPosition = GameObject.Find("DeckPosition")?.transform;
        PlayerScript.HealthBar = GameObject.Find("HealthBar")?.GetComponentInChildren<ProgressBar>();
    }
}

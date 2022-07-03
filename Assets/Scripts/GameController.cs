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
    [SerializeField] public readonly List<AvailableCard> AvailableCards = new();
    [SerializeField] public SnapshotCamera SnapshotCamera;

    public static GameObject GetGameObject() => GameObject.Find("GameController");
    public static GameController Get() => GetGameObject().GetComponent<GameController>();

    public async void Start() {
        if (_instance != null) {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);

        PlayerScript = Player.GetComponent<Player>();
        SnapshotCamera = SnapshotCamera.Create(31);
        SceneManager.activeSceneChanged += HandleSceneChanged;

        await Initialize();
    }

    private async Task Initialize() {
        await HandleInitialize();
        if (!DevBox) SceneManager.LoadScene((int)SceneId.MENU);
    }

    private async Task HandleInitialize() {
        InitializeMultiplayer();
        await InitializeCards();
        Thread.Sleep(500);
    }

    private void InitializeMultiplayer() {
        
    }

    private async Task InitializeCards() {
        var balance = await NFT_ERC721.BalanceOf("0xD48ab8a75C0546Cf221e674711b6C38257a545b6");
        PlayerPrefs.SetInt("NFT_BALANCE_NERVOS_REWARD_1", balance);
        
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
    }
}

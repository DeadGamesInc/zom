using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour {
    public static GameController Instance;
    
    [field: SerializeField] public GameObject Player;
    [field: SerializeField] public List<GameObject> CardDatabase = new();
    public List<AvailableCard> AvailableCards = new();

    public void Start() {
        if (Instance != null) {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        SceneManager.activeSceneChanged += HandleSceneChanged;
        
        StartCoroutine(Initialize());
    }

    public void ExitGame() {
        Application.Quit();
    }

    private IEnumerator Initialize() {
        var init = Task.Run(HandleInitialize);
        
        while (!init.IsCompleted) yield return null;
        SceneManager.LoadScene((int)SceneId.MENU);
    }

    private void HandleInitialize() {
        InitializeMultiplayer();
        InitializeCards();
        Thread.Sleep(500);
    }

    public void Update() { }

    private void InitializeMultiplayer() {
        
    }

    private void InitializeCards() {
        // Normally request from the multiplayer server, or check the blockchain, for now stuff the necessary cards in
        foreach (var card in CardDatabase) {
            var availableCard = new AvailableCard { Card = card, Quantity = 2 };
            AvailableCards.Add(availableCard);
        }
    }

    private void HandleSceneChanged(Scene current, Scene next) {
        Player.GetComponent<DeckController>().HandPosition = GameObject.Find("HandPosition")?.transform;
    }
}

using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour {
    public static GameController Instance;
    public List<Card> CardDatabase;

    public void Start() {
        if (Instance != null) {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        StartCoroutine(Initialize());
    }

    private IEnumerator Initialize() {
        var init = Task.Run(HandleInitialize);
        while (!init.IsCompleted) yield return null;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    private void HandleInitialize() {
        InitializeCards();
        InitializeMultiplayer();
        Thread.Sleep(3000);
    }

    public void Update() { }

    private void InitializeCards() {
        CardDatabase = new List<Card>();
    }

    private void InitializeMultiplayer() {
        
    }
}

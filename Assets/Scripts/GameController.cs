using System.Collections;
using System.Collections.Generic;
using System.Threading;
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
        
        InitializeCards();
        InitializeMultiplayer();
        Thread.Sleep(3000);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void Update() { }

    private void InitializeCards() {
        CardDatabase = new List<Card>();
    }

    private void InitializeMultiplayer() {
        
    }
}

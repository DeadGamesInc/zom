using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    }

    public void Update() { }

    private void InitializeCards() {
        CardDatabase = new List<Card>();
    }

    private void InitializeMultiplayer() {
        
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelController : MonoBehaviour {
    [field: SerializeField] public LevelId Id;
    
    protected GameController _gameController;
    protected DeckController _deckController;

    protected GameObject _handPosition;
    
    // Start is called before the first frame update
    public void Start() {
        _gameController = GameObject.Find("GameController").GetComponent<GameController>();
        _deckController = GameObject.Find("Player").GetComponent<DeckController>();
        _handPosition = GameObject.Find("HandPosition");
    }

    // Update is called once per frame
    public void Update() {
        
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarryDevBox : MonoBehaviour {
    private GameController _gameController;
    private DeckController _deckController;
    
    public void Start() {
        _gameController = GameObject.Find("GameController").GetComponent<GameController>();
        
        var availableCards = _gameController.AvailableCards;
        foreach (var card in availableCards) {
            for (var i = 1; i <= card.Quantity; i++) _deckController.DeckCards.Add(card.Card);
        }
    }

    public void ClickExit() {
        _gameController.ExitGame();
    }

    public void ClickShuffle() {
        _deckController.Shuffle();
    }

    public void ClickDraw() {
        _deckController.DrawCard();
    }
}

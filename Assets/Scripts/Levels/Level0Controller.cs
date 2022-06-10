using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level0Controller : LevelController {
    // Start is called before the first frame update
    public new void Start() {
        base.Start();
        foreach (var card in _gameController.AvailableCards) 
            for (var i = 1; i <= card.Quantity; i++) _deckController.DeckCards.Add(card.Card);
    }

    // Update is called once per frame
    public new void Update() {
        base.Update();
    }

    public void ClickShuffle() {
        _deckController.Shuffle();
    }

    public void ClickDraw() {
        if (!_deckController.DrawCard()) return;
        var position = _handPosition.transform.position;
        position.x += 1f;
        position.y += 0.01f;
        position.z -= 0.01f;
        _handPosition.transform.position = position;
    }
}

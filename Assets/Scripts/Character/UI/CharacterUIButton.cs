using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CharacterUIButton : MonoBehaviour {
    [SerializeField] public GameObject Ui;
    [SerializeField] public PlayerCommand Type;

    public void Start() {
        GetComponent<Button>().onClick.AddListener(OnClick);
    }

    public void OnClick() {
        var levelController = LevelController.Get();
        var characterUi = Ui.GetCharacterUI();
        if (characterUi.Target != levelController.selectedCharacter) return;
        switch (Type) {
            case PlayerCommand.MoveCharacter:
                levelController.StartCommand(PlayerCommand.MoveCharacter, characterUi.Target);
                break;
            case PlayerCommand.AttackLocation:
                levelController.StartCommand(PlayerCommand.AttackLocation, characterUi.Target);
                break;
        }
    }
}
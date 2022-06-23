using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CharacterUIButton : MonoBehaviour {
    [SerializeField] public PlayerCommand Type;

    public void Start() {
        GetComponent<Button>().onClick.AddListener(OnClick);
    }

    public void OnClick() {
        var levelController = LevelController.Get();
        var characterUI = CharacterUI.Get();
        
        switch (Type) {
            case PlayerCommand.MoveCharacter:
                levelController.StartCommand(PlayerCommand.MoveCharacter, characterUI.TargetCharacter);
                break;
            case PlayerCommand.AttackLocation:
                levelController.StartCommand(PlayerCommand.AttackLocation, characterUI.TargetCharacter);
                break;
        }
    }
}
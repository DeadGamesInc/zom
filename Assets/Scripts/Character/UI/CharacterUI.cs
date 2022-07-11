using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterUI : EntityUI {
    [SerializeField] private GameObject moveButton;
    [SerializeField] private GameObject attackButton;
    [SerializeField] private GameObject defendButton;

    // Start is called before the first frame update
    void Start() {
        _camera = GameObject.Find("CameraController");
        moveButton.GetComponent<CharacterUIButton>().Ui = gameObject;
        attackButton.GetComponent<CharacterUIButton>().Ui = gameObject;
        defendButton.GetComponent<CharacterUIButton>().Ui = gameObject;
    }
    
    public void SetCharacterText(string text) {
        statusText.GetComponent<TextMeshProUGUI>().text = text;
    }
    
    public void Spawning() {
        Health.SetActive(false);
        HealthBar.SetActive(false);
        moveButton.SetActive(false);
        attackButton.SetActive(false);
        defendButton.SetActive(false);
        statusText.GetComponent<TextMeshProUGUI>().text = "Spawning";
    }
    
    public void OnEnable() {
        EnableChildren();
    }

    public void EnableChildren() {
        statusText.SetActive(true);
        attackButton.SetActive(true);
        moveButton.SetActive(true);
        defendButton.SetActive(false);
        attackButton.transform.localPosition = new Vector3(13, 15f, 0);
        moveButton.transform.localPosition = new Vector3(-13f, 15f, 0);
    }

    public void OnlyShowTextAndHeath() {
        statusText.SetActive(true);
        attackButton.SetActive(false);
        moveButton.SetActive(false);
        defendButton.SetActive(false);
    }
    
    public void HideTextAndHeath() {
        Health.SetActive(false);
        HealthBar.SetActive(false);
        statusText.SetActive(false);
    }
    
    public void OnlyShowButton(PlayerCommand type) {
        switch (type) {
            case PlayerCommand.AttackLocation:
                defendButton.SetActive(false);
                moveButton.SetActive(false);
                attackButton.SetActive(true);
                attackButton.transform.localPosition = new Vector3(0, 15f, 0);
                break;
            case PlayerCommand.MoveCharacter:
                defendButton.SetActive(false);
                attackButton.SetActive(false);
                moveButton.SetActive(true);
                moveButton.transform.localPosition = new Vector3(0, 15f, 0);
                break;
            case PlayerCommand.DefendLocation:
                attackButton.SetActive(false);
                moveButton.SetActive(false);
                defendButton.SetActive(true);
                break;
        }
        statusText.SetActive(false);
    }
}
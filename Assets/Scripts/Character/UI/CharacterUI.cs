using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterUI : MonoBehaviour {
    [SerializeField] private GameObject statusText;
    [SerializeField] private GameObject moveButton;
    [SerializeField] private GameObject attackButton;
    [SerializeField] private GameObject defendButton;
    [SerializeField] private GameObject healthBar;
    [SerializeField] private GameObject _healthBar;
    [SerializeField] public GameObject TargetCharacter;
    [SerializeField] public GameObject _camera;

    // Start is called before the first frame update
    void Start() {
        _camera = GameObject.Find("MainCamera");
        moveButton.GetComponent<CharacterUIButton>().Ui = gameObject;
        attackButton.GetComponent<CharacterUIButton>().Ui = gameObject;
        defendButton.GetComponent<CharacterUIButton>().Ui = gameObject;
        GetComponentInChildren<HealthBar>().TargetCharacter = TargetCharacter.GetCharacter();
    }
    
    public void SetCharacterText(string text) {
        statusText.GetComponent<TextMeshProUGUI>().text = text;
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

    // Update is called once per frame
    private void FixedUpdate() {
        transform.position = TargetCharacter.transform.position;
        transform.rotation = _camera.transform.rotation;
    }
}
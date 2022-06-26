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
    [SerializeField] public GameObject TargetCharacter;
    [SerializeField] public GameObject _camera;

    // Start is called before the first frame update
    void Start() {
        _camera = GameObject.Find("MainCamera");
        moveButton.GetComponent<CharacterUIButton>().Ui = gameObject;
        attackButton.GetComponent<CharacterUIButton>().Ui = gameObject;
    }
    
    public void SetCharacterText(string text) {
        statusText.GetComponent<TextMeshProUGUI>().text = text;
    }
    
    public void OnEnable() {
        EnableChildren();
    }

    public void EnableChildren() {
        attackButton.SetActive(true);
        moveButton.SetActive(true);
        statusText.SetActive(true);
    }
    
    

    public void OnlyShowButton(PlayerCommand type) {
        switch (type) {
            case PlayerCommand.AttackLocation:
                attackButton.SetActive(true);
                moveButton.SetActive(false);
                break;
            case PlayerCommand.MoveCharacter:
                moveButton.SetActive(true);
                attackButton.SetActive(false);
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
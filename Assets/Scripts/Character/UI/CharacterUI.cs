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

    // Update is called once per frame
    private void FixedUpdate() {
        transform.position = TargetCharacter.transform.position;
        transform.rotation = _camera.transform.rotation;
    }
}
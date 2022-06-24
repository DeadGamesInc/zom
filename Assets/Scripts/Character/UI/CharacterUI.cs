using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterUI : MonoBehaviour {
    [SerializeField] public GameObject TargetCharacter;
    [SerializeField] public GameObject Camera;
    [SerializeField] public TextMeshProUGUI _statusText;

    public static CharacterUI Get() {
        GameObject characterUI = GameObject.Find("CharacterUI");
        if (characterUI != null) {
            return characterUI.GetComponent<CharacterUI>();
        }

        throw new Exception("CharacterUI not found in scene"); 
    }

    // Start is called before the first frame update
    void Start() {
        _statusText = GameObject.Find("CharacterText").GetComponent<TextMeshProUGUI>();
    }
    
    public void SetCharacterText(string text) {
        _statusText.text = text;
    }

    // Update is called once per frame
    private void FixedUpdate() {
        gameObject.transform.position = TargetCharacter.transform.position;
        transform.rotation = Camera.transform.rotation;
    }
}
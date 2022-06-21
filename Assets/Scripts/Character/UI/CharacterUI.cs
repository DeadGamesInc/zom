using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterUI : MonoBehaviour {
    [SerializeField] public GameObject TargetCharacter;
    [SerializeField] public GameObject Camera;

    public static CharacterUI Get() {
        GameObject characterUI = GameObject.Find("CharacterUI");
        if (characterUI != null) {
            return characterUI.GetComponent<CharacterUI>();
        }

        throw new Exception("CharacterUI not found in scene");
    }

    // Start is called before the first frame update
    void Start() {
    }

    // Update is called once per frame
    void Update() {
        gameObject.transform.position = TargetCharacter.transform.position;
        transform.rotation = Camera.transform.rotation;
    }
}
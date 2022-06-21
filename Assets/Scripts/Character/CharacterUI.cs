using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterUI : MonoBehaviour {
    [SerializeField] public GameObject TargetCharacter;
    [SerializeField] public GameObject Camera;

    // private void Awake() {
    //     image = GetComponent<Image>();
    // }

    // Start is called before the first frame update
    void Start() {
        gameObject.transform.position = TargetCharacter.transform.position;
    }

    // Update is called once per frame
    void Update() {
        transform.rotation = Camera.transform.rotation;
    }
}
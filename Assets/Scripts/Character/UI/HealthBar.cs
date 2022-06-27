using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour {
    private Image _healthBar;
    public Character TargetCharacter;
    
    // Start is called before the first frame update
    void Start() {
        _healthBar = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update() {
        _healthBar.fillAmount = TargetCharacter.Health / TargetCharacter.MaxHealth;
    }
}
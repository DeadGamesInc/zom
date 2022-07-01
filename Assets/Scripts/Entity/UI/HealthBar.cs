using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour {
    private Image _healthBar;
    public Entity Target;
    
    // Start is called before the first frame update
    void Start() {
        _healthBar = GetComponent<Image>();
    }

    public void Refresh() {
        if (_healthBar != null) _healthBar.fillAmount = Target.Health / Target.MaxHealth;
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Entity : MonoBehaviour {
    public float MaxHealth;
    public float Health;
    public GameObject Ui;

    protected void Start() {
        Health = MaxHealth;
    }

    public void Damage(float amount) {
        Health -= amount;
        Ui.GetEntityUI().HealthBar.GetComponent<HealthBar>().Refresh();
        if(Health <= 0) Destroy(gameObject);
    }

    protected void OnDestroy() {
        Destroy(Ui);
    }
}

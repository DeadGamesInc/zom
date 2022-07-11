using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Entity : MonoBehaviour {
    public float MaxHealth;
    public float Health;
    public GameObject Ui;

    protected abstract void Kill();

    protected void Start() {
        Health = MaxHealth;
    }

    public void TakeDamage(float amount) {
        Health -= amount;
        Ui.GetEntityUI().Health.GetComponent<HealthBar>().Refresh();
        if(Health <= 0) Kill();
    }

    protected void OnDestroy() {
        Destroy(Ui);
    }
}

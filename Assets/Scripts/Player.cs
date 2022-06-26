using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {
    private static Player _instance;
    
    [SerializeField] public int MaxHealth, Health;

    public ProgressBar HealthBar;

    public static GameObject GetGameObject() => GameObject.Find("Player");
    public static Player Get() => GetGameObject().GetComponent<Player>();
    
    // Start is called before the first frame update
    public void Start() {
        if (_instance != null) {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void SetMaxHealth(int health) {
        MaxHealth = health;
        Health = health;
        HealthBar.Maximum = health;
        HealthBar.Set(health);
    }
}

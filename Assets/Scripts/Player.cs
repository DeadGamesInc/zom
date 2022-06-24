using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {
    [SerializeField] public int MaxHealth;
    [SerializeField] public int Health;

    public ProgressBar HealthBar;

    private static Player Instance;

    public static Player Get() {
        var player = GameObject.Find("Player");
        if (player == null) return null;
        return player.GetComponent<Player>();
    }
    
    // Start is called before the first frame update
    public void Start() {
        if (Instance != null) {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void SetMaxHealth(int health) {
        MaxHealth = health;
        Health = health;
        HealthBar.Maximum = health;
        HealthBar.Set(health);
    }

    // Update is called once per frame
    public void Update() {
        
    }
}

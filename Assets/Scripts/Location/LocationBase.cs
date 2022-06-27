using UnityEngine;

public class LocationBase : MonoBehaviour {
    [SerializeField] public int Owner, Health, SpawnTime;
    [SerializeField] public bool Spawned;
    public MapNode ActiveNode;
    public (int, int) MapPosition;
    public GameObject Card;

    public void Setup(int owner) {
        Owner = owner;
        if (SpawnTime == 0) SetSpawned();
        else SetSpawning();
    }

    public void SpawnTick() {
        if (SpawnTime == 0) return;
        SpawnTime--;
        if (SpawnTime == 0) SetSpawned();
    }
    
    private void SetSpawning() {
        gameObject.GetComponent<Renderer>().material.ChangeAlpha(0.25f);
        Spawned = false;
    }

    private void SetSpawned() {
        gameObject.GetComponent<Renderer>().material.ChangeAlpha(1.0f);
        Spawned = true;
    }

    public void OnMouseDown() {
        var levelController = LevelController.Get();
        var command = levelController.currentCommand;
        switch (command) {
            case PlayerCommand.AttackLocation:
                levelController.QueueCommand(PlayerCommand.AttackLocation, gameObject);
                break;
        }
    }
}
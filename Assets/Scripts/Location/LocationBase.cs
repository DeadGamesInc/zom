using System.Collections.Generic;
using UnityEngine;

public class LocationBase : Entity {
    [SerializeField] public int Owner, SpawnTime;
    [SerializeField] public bool Spawned;
    public MapNode ActiveNode;
    public (int, int) MapPosition;
    public GameObject Card;
    public List<GameObject> Defenders;
    
    public void Setup(int owner) {
        Owner = owner;
        Ui = Instantiate(LevelController.Get().EntityUI);
        Ui.GetEntityUI().Target = gameObject;
        Ui.SetActive(false);
        if (SpawnTime == 0)
            SetSpawned();
        else
            SetSpawning();
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
        var source = levelController.currentCommandSource.GetCharacter();
        var currentTurnOwner = levelController.CurrentTurnOwner();
        switch (command) {
            case PlayerCommand.AttackLocation:
                if (currentTurnOwner == source.Owner && currentTurnOwner != Owner)
                    levelController.QueueCommand(PlayerCommand.AttackLocation, gameObject);
                break;
        }
    }
}
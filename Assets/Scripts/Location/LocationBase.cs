using System;
using System.Collections.Generic;
using UnityEngine;

public class LocationBase : Entity {
    [SerializeField] public int Owner, SpawnTime;
    [SerializeField] public bool Spawned;
    [SerializeField] public Vector3 DirectionVector;
    [SerializeField] public List<GameObject> EmptyBrainNodes = new(), BrainNodes = new();
    public FreeCameraProperties CameraPosition;
    public int Index;

    public MapNode ActiveNode;
    public (int, int) MapPosition;
    public GameObject Card;
    public List<GameObject> Defenders;
    
    public void Setup(int owner, int spawnTime, float health, int index) {
        Owner = owner;
        SpawnTime = spawnTime;
        Health = health;
        Index = index;
        Ui = Instantiate(LevelController.Get().EntityUI);
        Ui.GetEntityUI().Target = gameObject;
        Ui.GetEntityUI().SetCharacterText(gameObject.name);
        Ui.SetActive(false);
        if (SpawnTime == 0)
            SetSpawned();
        else
            SetSpawning();
    }
    
    protected override void Kill() {
        var controller = LevelController.Get();
        var map = MapBase.Get();
        ActiveNode.Location = null;
        foreach (var node in BrainNodes) node.GetBrains().Kill();
        controller.CreateEmptyLocation(map.Grid, MapPosition, ActiveNode, Index);
        controller.Locations.Remove(gameObject);
        Destroy(gameObject);
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
                var source = levelController.currentCommandSource.GetCharacter();
                var currentTurnOwner = levelController.CurrentTurnOwner();
                if (currentTurnOwner == source.Owner && currentTurnOwner != Owner )
                    levelController.QueueCommand(PlayerCommand.AttackLocation, gameObject);
                break;
        }
    }
}
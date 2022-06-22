using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour {
    [SerializeField] public CardId Id;
    [SerializeField] public CardType Type;
    [SerializeField] public int BrainsValue;
    [SerializeField] public string Name;
    [SerializeField] public string Series;
    [SerializeField] public float MaxHealth;
    [SerializeField] public float Damage;
    [SerializeField] public int MaxRange;
    [SerializeField] public string NervosTestnetNFT;

    [SerializeField] public Sprite CardPreview;
    [SerializeField] public GameObject CharacterPrefab;
    [SerializeField] public GameObject LocationPrefab;
    [SerializeField] public GameObject ItemPrefab;
    [SerializeField] public GameObject ResourcePrefab;

    private Vector3 _startPosition;
    public Vector3 StartScale;
    private LevelController _levelController;
    
    public void Start() {
        _levelController = GameObject.Find("LevelController")?.GetComponent<LevelController>();
    }

    public void Update() {
        
    }

    public void OnMouseEnter() {
        if (_levelController == null) return;
        _levelController.SetCard(CardPreview, gameObject, $"Cost: {BrainsValue}");
    }

    public void OnMouseExit() {
        if (_levelController == null) return;
        _levelController.SetCard(null, null, "");
    }

    public void OnMouseDown() {
        if (Camera.main == null || _levelController == null || _levelController.CurrentPhase != PhaseId.STRATEGIC) return;
        _levelController.SetCardLock(true);
        var transformInfo = transform;
        _startPosition = transformInfo.position;
        StartScale = transformInfo.localScale;
        var targetScale = new Vector3(StartScale.x / 3, StartScale.y / 3, StartScale.z / 3);
        transformInfo.localScale = targetScale;
    }

    public void OnMouseDrag() {
        if (Camera.main == null || _levelController == null || _levelController.CurrentPhase != PhaseId.STRATEGIC) return;
        var distance = Camera.main.WorldToScreenPoint(gameObject.transform.position).z;
        transform.position = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, distance));
    }

    public void OnMouseUp() {
        if (_levelController == null || _levelController.CurrentPhase != PhaseId.STRATEGIC) return;
        
        if (!_levelController.TryPlayCard()) {
            var setTransform = transform;
            setTransform.position = _startPosition;
            setTransform.localScale = StartScale;
        }
        
        _levelController.SelectedCard = null;
        _levelController.SetCardLock(false);
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour {
    [field: SerializeField] public CardId Id;
    [field: SerializeField] public string Name;
    [field: SerializeField] public string Series;
    [field: SerializeField] public float MaxHealth;
    [field: SerializeField] public float Damage;
    [field: SerializeField] public int MaxRange;
    [field: SerializeField] public string NervosTestnetNFT;

    [field: SerializeField] public Sprite CardPreview;

    private Vector3 _startPosition;
    private LevelController _levelController;
    
    public void Start() {
        _levelController = GameObject.Find("LevelController")?.GetComponent<LevelController>();
    }

    public void Update() {
        
    }

    public void OnMouseEnter() {
        if (_levelController == null) return;
        _levelController.SetCardPreview(CardPreview);
    }

    public void OnMouseExit() {
        if (_levelController == null) return;
        _levelController.SetCardPreview(null);
    }

    public void OnMouseDown() {
        if (Camera.main == null || _levelController == null || _levelController.CurrentPhase != PhaseId.STRATEGIC) return;
        _levelController.SetPreviewLock(true);
        _startPosition = transform.position;
    }

    public void OnMouseDrag() {
        if (Camera.main == null || _levelController == null || _levelController.CurrentPhase != PhaseId.STRATEGIC) return;
        var distance = Camera.main.WorldToScreenPoint(gameObject.transform.position).z;
        transform.position = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, distance));
    }

    public void OnMouseUp() {
        if (_levelController == null || _levelController.CurrentPhase != PhaseId.STRATEGIC) return;
        _levelController.SetPreviewLock(false);
        transform.position = _startPosition;
    }
}

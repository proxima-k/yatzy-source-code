using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Vector2 = UnityEngine.Vector2;

public class PlayerUI : MonoBehaviour{
    
    public static PlayerUI Instance { get; private set; }
    
    [SerializeField] private ScorecardUIHolder _scorecardUIHolder;
    
    // private Image _blockRaycastImage;
    
    private ScorecardUI _scorecardUI;
    private Rect _scorecardRect;
    private Vector2 _defaultPos;

    private Coroutine _moveScorecardCoroutine;
    // private bool _isScorecardFocused = false;
    
    private void Awake() {
        if (Instance != null) {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void Initialize() {
        _scorecardUIHolder.Initialize();
    }
    
    public void InitializePlayerInput(Player localPlayer) {
        // setup inputs
        _scorecardUIHolder.InitializeInput(localPlayer);
        
        RollDieUI.Instance.InitializePlayer(localPlayer);
    }
}

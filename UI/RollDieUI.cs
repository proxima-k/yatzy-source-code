using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RollDieUI : MonoBehaviour {
    // [SerializeField] private _rollDiePrefab;
    public static RollDieUI Instance { get; private set; }
    
    public event EventHandler OnRollButtonClicked;
    
    [SerializeField] private Button _rollButton;
    [SerializeField] private TextMeshProUGUI _rollButtonText;
    [SerializeField] private Color _textEnabledColor;
    [SerializeField] private Color _textDisabledColor;

    [SerializeField] private TextMeshProUGUI _rollCountText;
    
    private void Awake() {
        if (Instance != null) {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        
        _rollButton.onClick.AddListener(RollButton_OnClick);
        DisableButton();
    }

    public void InitializePlayer(Player player) {
        // player.OnStartTurn += Player_OnStartTurn;
        // player.OnEndTurn += Player_OnEndTurn;
        player.PlayerStateManager.RollDieState.OnRollCountChanged += Player_OnRollCountChanged;
    }
    
    private void Player_OnStartTurn(object sender, EventArgs e) {
        EnableButton();
    }
    
    private void Player_OnEndTurn(object sender, EventArgs e) {
        DisableButton();
    }
    
    private void Player_OnRollCountChanged(object sender, PlayerRollDieState.OnRollCountChangedEventArgs e) {
        _rollCountText.text = $"{e.RollsLeftCount}/{PlayerRollDieState.MAX_ROLLS}";
        if (e.RollsLeftCount == 0) {
            DisableButton();
        }
    }
    
    private void RollButton_OnClick() {
        OnRollButtonClicked?.Invoke(this, EventArgs.Empty);
    }
    
    public void EnableButton() {
        _rollButton.interactable = true;
        _rollButtonText.color = _textEnabledColor;
        Debug.Log("Enabled roll button");
    }
    
    public void DisableButton() {
        _rollButton.interactable = false;
        _rollButtonText.color = _textDisabledColor;
        Debug.Log("Disabled roll button");
    }
}

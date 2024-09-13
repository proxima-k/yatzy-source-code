using System;
using UnityEngine;

public class PlayerRollDieState : PlayerBaseState {

    private bool _isAbleToRoll = false;
    private PlayerInput _playerInput;
    
    
    public const int MAX_ROLLS = 3;
    private int _rollsLeftCount = 0;
    DiceSelect _diceSelect;

    public event EventHandler<OnRollCountChangedEventArgs> OnRollCountChanged;
    public class OnRollCountChangedEventArgs : EventArgs {
        public int RollsLeftCount;
        public OnRollCountChangedEventArgs(int rollsLeftCount) {
            RollsLeftCount = rollsLeftCount;
        }
    }
    
    public override void EnterState() {
        Debug.Log($"[PlayerRollDieState] Player {playerStateManager.Player.Name} entered roll die state.");
        
        _playerInput = playerStateManager.Player.PlayerInput;
        _diceSelect = playerStateManager.Player.DiceSelect;
        _isAbleToRoll = true;
        _rollsLeftCount = MAX_ROLLS;
        OnRollCountChanged?.Invoke(this, new OnRollCountChangedEventArgs(_rollsLeftCount));
        
        _playerInput.OnPlayerRoll += PlayerInput_OnPlayerRoll;
        DiceRoller.Instance.OnAllDieRolled += DiceRoller_OnAllDieRolled;
        
        RollDieUI.Instance.OnRollButtonClicked += PlayerInput_OnPlayerRoll;
        RollDieUI.Instance.EnableButton();
    }

    
    public override void UpdateState() {
    }
    
    
    public override void ExitState() {
        _playerInput.OnPlayerRoll -= PlayerInput_OnPlayerRoll;
        DiceRoller.Instance.OnAllDieRolled -= DiceRoller_OnAllDieRolled;
        
        RollDieUI.Instance.OnRollButtonClicked -= PlayerInput_OnPlayerRoll;
        RollDieUI.Instance.DisableButton();
        
        _diceSelect.enabled = false;
    }
    
    // this is only running on the client side
    private void DiceRoller_OnAllDieRolled(object sender, DiceRoller.OnAllDieRolledEventArgs e) {
        
        // call getoptions in the client side
        playerStateManager.Player.PlayerScorecard.GetOptions();

        if (_rollsLeftCount <= 0) {
            _diceSelect.enabled = false;
            Debug.LogWarning("Player must submit a result.");
            return;
        }
        
        // if (_rollCount >= 1) {
            _diceSelect.enabled = true;
            _isAbleToRoll = true;
        // }
    }
    
    
    private void PlayerInput_OnPlayerRoll(object sender, EventArgs e) {
        TryRoll();
    }

    private void TryRoll() {
        if (_isAbleToRoll) {
            playerStateManager.Player.PlayerScorecard.ClearOptions();
            
            if (_rollsLeftCount == 3)
                DiceRoller.Instance.RollAllDieServerRpc();
            else {
                DiceRoller.Instance.RollSelectedDieServerRpc();
            }
            
            _rollsLeftCount--;
            _isAbleToRoll = false;
            
            OnRollCountChanged?.Invoke(this, new OnRollCountChangedEventArgs(_rollsLeftCount));
        }
    }
    
    private void AllowRoll() {
        _isAbleToRoll = true;
        // event for when player is allowed to roll
        // for ui to subscribe?
    }
    
    public PlayerRollDieState(PlayerStateManager playerStateManager) : base(playerStateManager) { }
}

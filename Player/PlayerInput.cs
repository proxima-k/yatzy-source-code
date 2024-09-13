using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour {

    public event EventHandler OnPlayerSelect;
    public event EventHandler OnPlayerRoll;
    
    private PlayerInputActions _playerInputActions;
    
    private void Awake() {
        _playerInputActions = new PlayerInputActions();
    }
    
    private void OnEnable() {
        _playerInputActions.Player.Enable();
        _playerInputActions.Player.Select.performed += Player_OnSelect;
        _playerInputActions.Player.Roll.performed += Player_OnRoll;
    }

    private void Player_OnRoll(InputAction.CallbackContext obj) {
        OnPlayerRoll?.Invoke(this, EventArgs.Empty);
    }

    private void Player_OnSelect(InputAction.CallbackContext obj) {
        OnPlayerSelect?.Invoke(this, EventArgs.Empty);
    }
    
    private void OnDisable() {
        _playerInputActions.Player.Select.performed -= Player_OnSelect;
        _playerInputActions.Player.Roll.performed -= Player_OnRoll;
        _playerInputActions.Player.Disable();
    }
}

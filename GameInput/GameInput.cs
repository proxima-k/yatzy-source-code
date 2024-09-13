using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameInput : MonoBehaviour {
    public event EventHandler OnToggleMenu;
    public event EventHandler OnToggleBack;

    private GameInputActions _gameInputActions;

    private void Awake() {
        _gameInputActions = new GameInputActions();
    }

    private void OnEnable() {
        _gameInputActions.Enable();
        _gameInputActions.Menu.ToggleMenu.performed += GameInput_OnToggleMenu;
    }

    private void GameInput_OnToggleMenu(InputAction.CallbackContext obj) {
        OnToggleMenu?.Invoke(this, EventArgs.Empty);
    }

    private void OnDisable() {
        _gameInputActions.Menu.ToggleMenu.performed -= GameInput_OnToggleMenu;
        _gameInputActions.Disable();
    }
}

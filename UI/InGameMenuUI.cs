using System;
using UnityEngine;
using UnityEngine.UI;

public class InGameMenuUI : MonoBehaviour {
    [SerializeField] private GameInput _gameInput;
    
    [SerializeField] private Transform _inGameMenuTransform;
    [SerializeField] private Button _openMenuButton;
    [SerializeField] private Button _resumeButton;
    [SerializeField] private Button _settingsButton;
    [SerializeField] private Button _mainMenuButton;
    
    [SerializeField] private SettingsMenuUI _settingsMenuUI;

    private bool _isActive;
    
    private void Awake() {
        _openMenuButton.onClick.AddListener(Show);
        _resumeButton.onClick.AddListener(Hide);
        _settingsButton.onClick.AddListener(SettingsButton_OnClick);
        _mainMenuButton.onClick.AddListener(MainMenuButton_OnClick);
        // _settingsMenuUI.OnExit += SettingsMenuUI_OnExit;
        
        _gameInput.OnToggleMenu += GameInput_OnToggleMenu;
        
        Hide();
    }
    
    private void GameInput_OnToggleMenu(object sender, EventArgs e) {
        if (_isActive) {
            Hide();
            return;
        }

        Show();
    }
    
    private void SettingsMenuUI_OnExit(object sender, EventArgs e) {
        Show();
    }

    private void SettingsButton_OnClick() {
        _settingsMenuUI.Show();
        Hide();
    }

    private void MainMenuButton_OnClick() {
        ConnectionHandler.Instance.Disconnect();
        SceneLoader.Load(SceneLoader.Scene.MainMenuScene);
        // Hide();
    }

    public void Show() {
        _inGameMenuTransform.gameObject.SetActive(true);
        _isActive = true;
    }
    
    public void Hide() {
        _inGameMenuTransform.gameObject.SetActive(false);
        _isActive = false;
    }
}

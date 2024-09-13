using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectGameModeUI : MonoBehaviour {

    [SerializeField] private GameObject _UIContainer;
    [SerializeField] private Button _localButton;
    [SerializeField] private Button _onlineButton;
    [SerializeField] private Button _backButton;

    [SerializeField] private LocalGameModeUI _localGameModeUI;
    
    
    private void Awake() {
        _localButton.onClick.AddListener(LocalButton_OnClick);
        _onlineButton.onClick.AddListener(OnlineButton_OnClick);
        _backButton.onClick.AddListener(BackButton_OnClick);
    }
    
    private void LocalButton_OnClick() {
        ShowLocalGameModeUI();
    }
    
    private void OnlineButton_OnClick() {
        SceneLoader.Load(SceneLoader.Scene.LobbyScene);
    }
    
    private void BackButton_OnClick() {
        Hide();
    }
    
    public void Show() {
        _UIContainer.SetActive(true);
    }
    
    public void Hide() {
        _UIContainer.SetActive(false);
    }
    
    private void ShowLocalGameModeUI() {
        _localGameModeUI.Show();
    }
    
    private void HideLocalGameModeUI() {
        _localGameModeUI.Hide();
    }
}

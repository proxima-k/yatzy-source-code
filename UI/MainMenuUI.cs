using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour {
    [SerializeField] private Button _playButton;
    // [SerializeField] private Button _settingsMenuButton;
    [SerializeField] private Button _quitButton;
    
    [SerializeField] private SelectGameModeUI _selectGameModeUI;

    private void Awake() {
        _playButton.onClick.AddListener(PlayButton_OnClick);
        // _settingsMenuButton.onClick.AddListener(SettingsMenuButton_OnClick);
        _quitButton.onClick.AddListener(QuitButton_OnClick);
        
    }

    private void PlayButton_OnClick() {
        // SceneLoader.Load(SceneLoader.Scene.GameScene);
        _selectGameModeUI.Show();
    }


    // private void SettingsMenuButton_OnClick() {
    //     // opens 
    // }
    
    
    private void QuitButton_OnClick() {
        Application.Quit();
    }
}

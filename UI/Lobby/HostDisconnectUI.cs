using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HostDisconnectUI : MonoBehaviour {

    
    [SerializeField] private Transform _hostDisconnectUI;
    [SerializeField] private Button _closeUIButton;
    
    private void Awake() {
        _closeUIButton.onClick.AddListener(CloseUI);
        Hide();
    }

    private void Start() {
        ConnectionHandler.Instance.OnHostDisconnected += MultiplayerHandler_OnHostDisconnected;
    }
    
    private void MultiplayerHandler_OnHostDisconnected(object sender, EventArgs e) {
        // MultiplayerHandler.Instance.Disconnect();
        Show();
    }
    
    private void CloseUI() {
        ConnectionHandler.Instance.Disconnect();
        
        // if current scene is not lobby scene, load lobby scene
        if (SceneManager.GetActiveScene().ToString() != SceneLoader.Scene.LobbyScene.ToString()) {
            SceneLoader.Load(SceneLoader.Scene.LobbyScene);
            return;
        }
        
        Hide();
    }
    
    public void Show() {
        _hostDisconnectUI.gameObject.SetActive(true);
    }
    
    public void Hide() {
        _hostDisconnectUI.gameObject.SetActive(false);
    }

    private void OnDestroy() {
        ConnectionHandler.Instance.OnHostDisconnected -= MultiplayerHandler_OnHostDisconnected;
    }
}

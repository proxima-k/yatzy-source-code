using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class LobbyConnectionUI : MonoBehaviour {
    
    [SerializeField] private Transform _lobbyConnectionUI;
    [SerializeField] private Button _hostGameButton;
    [SerializeField] private Button _joinGameButton;
    [SerializeField] private Button _mainMenuButton;
    
    [SerializeField] private TMP_InputField _playerNameInputField;
    
    private TextMeshProUGUI _hostGameButtonText;
    private TextMeshProUGUI _joinGameButtonText;
    
    [SerializeField] private LobbyCodeInputUI _lobbyCodeInputUI;
    [SerializeField] private ConnectionFailedUI _connectionFailedUI;
    [SerializeField] private LobbyUI _lobbyUI;
    
    [SerializeField] private Transform _connectingUI;
    
    private void Start() {
        _hostGameButton.onClick.AddListener(HostButton_OnClicked);
        _joinGameButton.onClick.AddListener(JoinButton_OnClicked);
        _mainMenuButton.onClick.AddListener(() => SceneLoader.Load(SceneLoader.Scene.MainMenuScene));
        _hostGameButtonText = _hostGameButton.GetComponentInChildren<TextMeshProUGUI>();
        _joinGameButtonText = _joinGameButton.GetComponentInChildren<TextMeshProUGUI>();
        // _closeConnectionFailedButton.onClick.AddListener();
        
        _lobbyCodeInputUI.OnLobbyCodeSubmitted += LobbyCodeInputUI_OnLobbyCodeSubmitted;
        _lobbyCodeInputUI.OnCancelButtonClicked += LobbyCodeInputUI_OnCancelButtonClicked;
        
        ConnectionHandler.Instance.OnConnectionSucceeded += ConnectionHandler_OnConnectionSucceeded;
        ConnectionHandler.Instance.OnConnectionFailed += ConnectionHandler_OnConnectionFailed;
        
        ConnectionHandler.Instance.OnSelfDisconnected += ConnectionHandler_OnSelfDisconnected;
        
        PlayerDataHandler.Instance.OnLocalClientSyncedData += PlayerDataHandler_OnLocalClientSyncedData;
        
        Show();
    }
    
    
    private void LobbyCodeInputUI_OnCancelButtonClicked(object sender, EventArgs e) {
        _lobbyCodeInputUI.Hide();
        Show();
    }
    

    private void ConnectionHandler_OnSelfDisconnected(object sender, EventArgs e) {
        Show();
    }

    // HOST FUNCTIONALITY
    private async void HostButton_OnClicked() {
        DisableButtons();
        ShowConnectingUI();
        
        bool createdRelay = await RelayHandler.Instance.CreateRelay();
        
        if (createdRelay) {
            _lobbyUI.Show();
            Hide();
        }
        else {
            _connectionFailedUI.Show("Failed to create relay");
            HideConnectingUI();
            Show();
        }
    }

    // CLIENT FUNCTIONALITY
    // Todo: make a pop up saying loading, then make the pop up go away when connection is successful
    private void JoinButton_OnClicked() {
        DisableButtons();
        _lobbyCodeInputUI.Show();
    }
    
    private async void LobbyCodeInputUI_OnLobbyCodeSubmitted(object sender, LobbyCodeInputUI.OnLobbyCodeSubmittedEventArgs e) {
        ShowConnectingUI();
        
        bool joinedRelay = await RelayHandler.Instance.JoinRelay(e.LobbyCode);
        
        // todo: the function will always return true since the start client function will always return true.
        // If there was a problem with the connection, the connection failed event will be called later 
        if (joinedRelay) {

        }
        else {
            _lobbyCodeInputUI.UpdateInstructionText("Invalid code or something went wrong", true);
            HideConnectingUI();
        }
    }

    public void Show() {
        _lobbyConnectionUI.gameObject.SetActive(true);
        EnableButtons();
    }
    
    public void Hide() {
        _lobbyConnectionUI.gameObject.SetActive(false);
    }

    private void DisableButtons() {
        _hostGameButton.interactable = false;
        if (_hostGameButtonText != null) {
            Color color = _hostGameButtonText.color;
            color.a = 0.23f;
            _hostGameButtonText.color = color;
        }
        
        _joinGameButton.interactable = false;
        if (_joinGameButtonText != null) {
            Color color = _joinGameButtonText.color;
            color.a = 0.23f;
            _joinGameButtonText.color = color;
        }
    }
    
    private void EnableButtons() {
        _hostGameButton.interactable = true;
        if (_hostGameButtonText != null) {
            Color color = _hostGameButtonText.color;
            color.a = 1f;
            _hostGameButtonText.color = color;
        }
        
        _joinGameButton.interactable = true;
        if (_joinGameButtonText != null) {
            Color color = _joinGameButtonText.color;
            color.a = 1f;
            _joinGameButtonText.color = color;
        }
    }
    
    private void ConnectionHandler_OnConnectionSucceeded(object sender, EventArgs e) {
        Debug.Log("Connection succeeded");
        
        
        _lobbyCodeInputUI.Hide();
        _lobbyUI.Show();
        Hide();
        
        HideConnectingUI();
    }
    
    private void ConnectionHandler_OnConnectionFailed(object sender, ConnectionHandler.OnConnectionFailedEventArgs e) {
        Debug.Log("Connection failed");
        _connectionFailedUI.Show(e.Reason);
        Show();
        
        HideConnectingUI();
    }
    
    private void PlayerDataHandler_OnLocalClientSyncedData(object sender, EventArgs e) {
        // if (!NetworkManager.Singleton.IsClient)
        //     return;
        
        // submit player name
        // limit player name to 20 characters
        if (_playerNameInputField.text.Length > 20) {
            _playerNameInputField.text = _playerNameInputField.text.Substring(0, 20);
        }
        
        PlayerDataHandler.Instance.SubmitPlayerNameServerRpc(_playerNameInputField.text);
    }
    
    private void ShowConnectingUI() {
        _connectingUI.gameObject.SetActive(true);
    }
    
    private void HideConnectingUI() {
        _connectingUI.gameObject.SetActive(false);
    }

    private void OnDestroy() {
        ConnectionHandler.Instance.OnConnectionSucceeded -= ConnectionHandler_OnConnectionSucceeded;
        ConnectionHandler.Instance.OnConnectionFailed -= ConnectionHandler_OnConnectionFailed;
        ConnectionHandler.Instance.OnSelfDisconnected -= ConnectionHandler_OnSelfDisconnected;
    }
}

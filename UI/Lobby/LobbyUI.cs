using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUI : MonoBehaviour {
    
    [SerializeField] private LobbyCodeUI _lobbyCodeUI;
    [SerializeField] private TextMeshProUGUI _joinCodeText;
    
    [SerializeField] private LobbyConnectionUI _lobbyConnectionUI;
    
    [SerializeField] private Lobby _lobby;
    [SerializeField] private Transform _lobbyUI;
    
    [Header("Leave Button")]
    [SerializeField] private Button _leaveButton;
    
    [Header("Ready Button")]
    [SerializeField] private Button _readyButton;
    [SerializeField] private TextMeshProUGUI _readyText;
    [SerializeField] private Color _readyColor;
    [SerializeField] private Color _notReadyColor;
    bool _isReady = false;
    
    [Header("Start Game Button")]
    [SerializeField] private Button _startGameButton;
    [SerializeField] private TextMeshProUGUI _startGameText;
    
    private void Start() {
        ConnectionHandler.Instance.OnSelfDisconnected += MultiplayerHandlerOnSelfDisconnected;
        
        _leaveButton.onClick.AddListener(LeaveLobby);
        _readyButton.onClick.AddListener(ToggleReady);
        
        _startGameButton.onClick.AddListener(StartGame);
        
        _lobby.OnAllPlayersReady += Lobby_OnAllPlayersReady;
        _lobby.OnNotAllPlayersReady += Lobby_OnNotAllPlayersReady;
        _lobby.OnClientJoinedLobby += Lobby_OnClientJoinedLobby;
        
        _lobby.OnPlayerToggleReady += Lobby_OnPlayerToggleReady;
        
        Hide();
    }

    private void MultiplayerHandlerOnSelfDisconnected(object sender, EventArgs e) {
        
        _isReady = false;
        UpdateReadyButton(_isReady);
        
        Hide();
    }
    
    // button functions
    private void LeaveLobby() {
        
        
        ConnectionHandler.Instance.Disconnect();
        // _lobbyConnectionUI.Show();
        
        _isReady = false;
        UpdateReadyButton(_isReady);
        
        Hide();
    }
    
    private void ToggleReady() {
        _lobby.TogglePlayerReady();
    }
    
    private void StartGame() {
        // SceneLoader.LoadNetwork(SceneLoader.Scene.GameScene);
        SceneLoader.LoadOnlineMultiplayer();
    }
    
    
    // ui handling functions
    public void Show() {
        
        _lobbyUI.gameObject.SetActive(true);

        bool isHost = NetworkManager.Singleton.IsHost;
        _startGameButton.gameObject.SetActive(isHost);
        
        // _joinCodeText.text = $"{RelayHandler.Instance.JoinCode}";
        _lobbyCodeUI.SetLobbyCode(RelayHandler.Instance.JoinCode);
        
        DisableStartGameButton();
    }

    public void Hide() {
        _lobbyUI.gameObject.SetActive(false);
    }
    
    public void UpdateReadyButton(bool isReady) {
        _readyText.color = isReady ? _readyColor : _notReadyColor;
    }
    
    private void EnableStartGameButton() {
        Color color = _startGameText.color;
        color.a = 1f;
        _startGameText.color = color;
        
        _startGameButton.interactable = true;
    }
    
    private void DisableStartGameButton() {
        Color color = _startGameText.color;
        color.a = 0.23f;
        _startGameText.color = color;
        
        _startGameButton.interactable = false;
    }
    
    
    private void Lobby_OnClientJoinedLobby(ulong obj, bool isReady) {
        DisableStartGameButton();
    }
    
    private void Lobby_OnPlayerToggleReady(object sender, Lobby.OnPlayerToggleReadyEventArgs e) {
        if (e.ClientID != NetworkManager.Singleton.LocalClientId)
            return;
        
        _isReady = e.IsReady;
        UpdateReadyButton(_isReady);
    }
    
    private void Lobby_OnAllPlayersReady(object sender, EventArgs e) {
        EnableStartGameButton();
    }
    
    private void Lobby_OnNotAllPlayersReady(object sender, EventArgs e) {
        DisableStartGameButton();
    }
    
    private void OnDestroy() {
        ConnectionHandler.Instance.OnSelfDisconnected -= MultiplayerHandlerOnSelfDisconnected;
        
        _leaveButton.onClick.RemoveListener(LeaveLobby);
        _readyButton.onClick.RemoveListener(ToggleReady);
        
        _startGameButton.onClick.RemoveListener(StartGame);
        
        _lobby.OnAllPlayersReady -= Lobby_OnAllPlayersReady;
        _lobby.OnNotAllPlayersReady -= Lobby_OnNotAllPlayersReady;
        _lobby.OnClientJoinedLobby -= Lobby_OnClientJoinedLobby;
    }
}

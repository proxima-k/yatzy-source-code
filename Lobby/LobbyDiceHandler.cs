using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class LobbyDiceHandler : MonoBehaviour {
    
    [SerializeField] private LobbyDice _lobbyDicePrefab;
    
    private LobbyDice _localClientLobbyDice;
    
    private List<LobbyDice> _allLobbyDiceList = new List<LobbyDice>();
    private List<LobbyDice> _otherLobbyDiceList = new List<LobbyDice>();
    
    [SerializeField] private Transform _frontSlot;
    
    [SerializeField] private Transform _leftSlot;
    [SerializeField] private Transform _rightSlot;
    [SerializeField] private Transform _backSlot;
    
    private void Start() {
        Lobby.Instance.OnClientJoinedLobby += Lobby_OnClientJoinedLobby;
        Lobby.Instance.OnClientLeftLobby += Lobby_OnClientLeftLobby;
        Lobby.Instance.OnPlayerToggleReady += Lobby_OnPlayerToggleReady;
        
        ConnectionHandler.Instance.OnSelfDisconnected += MultiplayerHandlerOnSelfDisconnected;
        
        PlayerDataHandler.Instance.OnPlayerNameChanged += PlayerDataHandler_OnPlayerNameChanged;
        PlayerDataHandler.Instance.OnPlayerDataRemoved += PlayerDataHandler_OnPlayerDataRemoved;
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.F))
        {
            UpdatePlayerOrder();
        }
    }

    private void PlayerDataHandler_OnPlayerNameChanged(object sender, PlayerDataHandler.OnPlayerNameChangedEventArgs e) {
        foreach (var lobbyDice in _allLobbyDiceList) {
            if (lobbyDice.ClientID == (int)e.ClientID) {
                
                // if the lobby dice is the local client's dice, add " (you)" to the end of the name
                if (lobbyDice == _localClientLobbyDice)
                    e.NewPlayerName += " (you)";
                
                lobbyDice.SetName(e.NewPlayerName);
                return;
            }
        }
    }
    
    private void PlayerDataHandler_OnPlayerDataRemoved(object sender, PlayerDataHandler.OnPlayerDataRemovedEventArgs e) {
        RemoveLobbyDice((int)e.ClientID);
    }
    
    private void MultiplayerHandlerOnSelfDisconnected(object sender, EventArgs e) {
        foreach (var lobbyDice in _allLobbyDiceList) {
            Destroy(lobbyDice.gameObject);
        }
        _allLobbyDiceList.Clear();
        _otherLobbyDiceList.Clear();
    }

    private void Lobby_OnPlayerToggleReady(object sender, Lobby.OnPlayerToggleReadyEventArgs e) {
        SetReady((int)e.ClientID, e.IsReady);
    }

    private void Lobby_OnClientJoinedLobby(ulong clientId, bool isReady) {
        Debug.Log($"[LobbyDiceHandler] Client {clientId} joined the lobby. Is ready: {isReady}");
        SpawnLobbyDice((int)clientId, isReady);
    }
    
    
    private void Lobby_OnClientLeftLobby(ulong clientId) {
        // RemoveLobbyDice((int)clientId);
    }
    
    public void SpawnLobbyDice(int clientId, bool isReady) {
        if (_allLobbyDiceList.Count >= ConnectionHandler.MAX_PLAYER_COUNT) {
            Debug.LogError("Too many lobby dice");
            return;
        }
        
        bool isLocalClient = (ulong)clientId == NetworkManager.Singleton.LocalClientId;

        LobbyDice newLobbyDice;
        PlayerData playerData = PlayerDataHandler.Instance.GetPlayerData((ulong)clientId);
        string playerName = playerData.PlayerName.ToString();
        
        if (isLocalClient) {
            Transform spawnPoint = _frontSlot;
            newLobbyDice = Instantiate(_lobbyDicePrefab, spawnPoint.position, Quaternion.identity);
            
            _localClientLobbyDice = newLobbyDice;
            
            playerName += " (you)";
        }
        else {

            newLobbyDice = Instantiate(_lobbyDicePrefab, transform.position, Quaternion.identity);
            _otherLobbyDiceList.Add(newLobbyDice);
            
            UpdateDicePositions();
        }
        
        newLobbyDice.SetClientID(clientId);
        newLobbyDice.SetName(playerName);
        newLobbyDice.SetReady(isReady);
        newLobbyDice.SetColor(playerData.PlayerColor);
        
        _allLobbyDiceList.Add(newLobbyDice);
        
        UpdatePlayerOrder();
    }

    public void RemoveLobbyDice(int clientId) {
        foreach (var lobbyDice in _allLobbyDiceList) {
            if (lobbyDice.ClientID != (int)clientId) 
                continue;
            
            _allLobbyDiceList.Remove(lobbyDice);
            _otherLobbyDiceList.Remove(lobbyDice);

            Destroy(lobbyDice.gameObject);
            break;
        }
        
        UpdateDicePositions();
        UpdatePlayerOrder();
    }

    public void SetReady(int clientId, bool isReady) {
        foreach (var lobbyDice in _allLobbyDiceList) {
            if (lobbyDice.ClientID == clientId) {
                lobbyDice.SetReady(isReady);
                return;
            }
        }
    }

    private void UpdateDicePositions() {
        // check the current other dice list
        switch (_otherLobbyDiceList.Count) {
            case 0:
                break;
            case 1:
                _otherLobbyDiceList[0].MoveToPosition(_backSlot.position);
                break;
            case 2:
                _otherLobbyDiceList[0].MoveToPosition(_leftSlot.position);
                _otherLobbyDiceList[1].MoveToPosition(_rightSlot.position);
                break;
            case 3:
                _otherLobbyDiceList[0].MoveToPosition(_leftSlot.position);
                _otherLobbyDiceList[1].MoveToPosition(_rightSlot.position);
                _otherLobbyDiceList[2].MoveToPosition(_backSlot.position);
                break;
            default:
                Debug.LogError("Too many lobby dice");
                break;
        }
    }
    
    // set the dice value that is facing up based on order according to player data list
    public void UpdatePlayerOrder() {
        foreach (var lobbyDice in _allLobbyDiceList) {
            int playerIndex = PlayerDataHandler.Instance.GetPlayerIndexFromClientID((ulong)lobbyDice.ClientID);
            
            int diceValue = playerIndex + 1;
            lobbyDice.SetUpwardFacingValue(diceValue);
        }
    }
    
    private void OnDestroy() {
        Lobby.Instance.OnClientJoinedLobby -= Lobby_OnClientJoinedLobby;
        Lobby.Instance.OnClientLeftLobby -= Lobby_OnClientLeftLobby;
        Lobby.Instance.OnPlayerToggleReady -= Lobby_OnPlayerToggleReady;
        
        ConnectionHandler.Instance.OnSelfDisconnected -= MultiplayerHandlerOnSelfDisconnected;
        
        PlayerDataHandler.Instance.OnPlayerNameChanged -= PlayerDataHandler_OnPlayerNameChanged;
        PlayerDataHandler.Instance.OnPlayerDataRemoved -= PlayerDataHandler_OnPlayerDataRemoved;
        
    }
}

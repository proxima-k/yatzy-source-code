using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

// This class depends on PlayerData to be synced first
public class Lobby : NetworkBehaviour {
    
    
    public static Lobby Instance { get; private set; }
    
    private Dictionary<ulong, bool> _playerReadyDictionary = new Dictionary<ulong, bool>();

    
    private bool _allPlayersReady = false;

    // currently used by LobbyUI for disabling the start button
    // currently used by LobbyDiceHandler to spawn dice
    public event Action<ulong, bool> OnClientJoinedLobby;
    public event Action<ulong> OnClientLeftLobby;
    
    public event EventHandler OnAllPlayersReady;
    public event EventHandler OnNotAllPlayersReady;
    public event EventHandler<OnPlayerToggleReadyEventArgs> OnPlayerToggleReady;
    public class OnPlayerToggleReadyEventArgs : EventArgs {
        public ulong ClientID;
        public bool IsReady;
        public OnPlayerToggleReadyEventArgs(ulong clientId, bool isReady) {
            ClientID = clientId;
            IsReady = isReady;
        }
    }
    
    
    private void Awake() {
        if (Instance != null) {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start() {
        
        PlayerDataHandler.Instance.OnClientPlayerDataListSynced += PlayerDataHandler_OnClientPlayerDataListSynced;
        NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_OnClientDisconnect;
        
        ConnectionHandler.Instance.OnSelfDisconnected += ConnectionHandler_OnSelfDisconnected;
    }
    
    // will be called when client has finished syncing player data with the server
    // this also means the client syncing the data is the one that just joined the lobby
    private void PlayerDataHandler_OnClientPlayerDataListSynced(object sender, PlayerDataHandler.OnClientPlayerDataListSyncedEventArgs e) {
        if (!IsServer)
            return;
        
        // this section only notifies the new joining client of any existing clients that are in the lobby
        ClientRpcParams clientRpcParams = new ClientRpcParams {
            Send = new ClientRpcSendParams {
                TargetClientIds = new ulong[] {e.ClientID}
            }
        };
        Debug.Log($"{_playerReadyDictionary.Count} players in lobby");
        foreach (var playerReadyState in _playerReadyDictionary) {
            NotifyClientExistingStateClientRpc(playerReadyState.Key, playerReadyState.Value, clientRpcParams);
        }
        
        // this section notifies all existing clients (including the newly joined client) of the new joining client
        
        _playerReadyDictionary[e.ClientID] = false;
        // OnClientJoinedLobby?.Invoke(e.ClientID, false);
        OnClientJoinedLobbyClientRpc(e.ClientID);
    }
    
    private void NetworkManager_OnClientDisconnect(ulong clientId) {
        if (!IsServer)
            return;
     
        _playerReadyDictionary.Remove(clientId);
        // OnClientLeftLobby?.Invoke(clientId);
        
        OnClientLeftLobbyClientRpc(clientId);
    }
    
    
    private void ConnectionHandler_OnSelfDisconnected(object sender, EventArgs e) {
        _playerReadyDictionary.Clear();
    }
    
    
    
    // this is targeted towards the new joining client
    [ClientRpc]
    private void NotifyClientExistingStateClientRpc(ulong clientId, bool isReady, ClientRpcParams clientRpcParams = default) {
        // host doesn't need to be notified since it is a server
        // but it still needs to know so the lobby dice can be spawned
        if (IsHost)
            return;
        
        
        _playerReadyDictionary[clientId] = isReady;
        OnClientJoinedLobby?.Invoke(clientId, isReady);
        // Debug.Log($"[Client] {clientId} is ready: {isReady}");
    }
    
    // existing client will be notified of the new joining client
    // the new joining client will receive this rpc as well, which is the client itself
    [ClientRpc]
    private void OnClientJoinedLobbyClientRpc(ulong clientId) {
        // if it's the host don't alter the dictionary as it already has the correct value
        if (!IsHost) {
            _playerReadyDictionary[clientId] = false;
        }
        OnClientJoinedLobby?.Invoke(clientId, false);
    }
    
    [ClientRpc]
    private void OnClientLeftLobbyClientRpc(ulong clientId) {
        if (!IsHost) {
            _playerReadyDictionary.Remove(clientId);
        }
        
        OnClientLeftLobby?.Invoke(clientId);
    }
    
    
    // toggle ready functions
    public void TogglePlayerReady() {
        ToggleReadyServerRpc();
    }
    
    // todo: potential bugs here
    [ServerRpc(RequireOwnership = false)]
    private void ToggleReadyServerRpc(ServerRpcParams serverRpcParams = default) {
        bool isReady = _playerReadyDictionary[serverRpcParams.Receive.SenderClientId];
        _playerReadyDictionary[serverRpcParams.Receive.SenderClientId] = !isReady;
        
        OnPlayerToggleReadyClientRpc(serverRpcParams.Receive.SenderClientId, !isReady);
        
        
        _allPlayersReady = true;
        foreach (var playerReadyState in _playerReadyDictionary) {
            if (!playerReadyState.Value) {
                _allPlayersReady = false;
                break;
            }
        }

        if (_allPlayersReady) {
            Debug.Log("All players ready!");
            OnAllPlayersReady?.Invoke(this, EventArgs.Empty);
        }
        else {
            Debug.Log("Not all players ready.");
            OnNotAllPlayersReady?.Invoke(this, EventArgs.Empty);
        }
    }

    [ClientRpc]
    private void OnPlayerToggleReadyClientRpc(ulong clientId, bool isReady) {
        _playerReadyDictionary[clientId] = isReady;
        
        
        // send event to all clients so they know which dice to change the material
        OnPlayerToggleReady?.Invoke(this, new OnPlayerToggleReadyEventArgs(clientId, isReady));
    }
    
    public override void OnDestroy() {
        
        base.OnDestroy();
        
        // NetworkManager.Singleton.OnClientConnectedCallback -= NetworkManager_OnClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback -= NetworkManager_OnClientDisconnect;
        
        ConnectionHandler.Instance.OnSelfDisconnected -= ConnectionHandler_OnSelfDisconnected;
    }
}

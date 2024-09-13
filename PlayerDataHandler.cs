using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerDataHandler : NetworkBehaviour {
    
    public static PlayerDataHandler Instance { get; private set; }

    // this is for client
    public event EventHandler OnLocalClientSyncedData;
    public event EventHandler<OnPlayerDataRemovedEventArgs> OnPlayerDataRemoved;
    public class OnPlayerDataRemovedEventArgs : EventArgs {
        public ulong ClientID;
        public OnPlayerDataRemovedEventArgs(ulong clientID) {
            ClientID = clientID;
        }
    }
    
    // this is for server
    public event EventHandler<OnClientPlayerDataListSyncedEventArgs> OnClientPlayerDataListSynced;
    public class OnClientPlayerDataListSyncedEventArgs : EventArgs {
        public ulong ClientID;
        public OnClientPlayerDataListSyncedEventArgs(ulong clientID) {
            ClientID = clientID;
        }
    }
    
    public event EventHandler<OnPlayerNameChangedEventArgs> OnPlayerNameChanged;
    public class OnPlayerNameChangedEventArgs : EventArgs {
        public ulong ClientID;
        public string NewPlayerName;
        public OnPlayerNameChangedEventArgs(ulong clientID, string newPlayerName) {
            ClientID = clientID;
            NewPlayerName = newPlayerName;
        }
    }
    
    public NetworkList<PlayerData> PlayerDataNetworkList => _playerDataNetworkList;
    private NetworkList<PlayerData> _playerDataNetworkList;
    
    [SerializeField] private GameSettings _gameSettings;
    [SerializeField] private List<string> _playerNames = new List<string>();
    
    private void Awake() {
        if (Instance != null) {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        
        _playerDataNetworkList = new NetworkList<PlayerData>();
        _playerDataNetworkList.OnListChanged += PlayerDataNetworkList_OnListChanged;
        
        Debug.Log("Initializing PlayerData NetworkList.");
    }

    private void Start() {
        NetworkManager.Singleton.OnClientConnectedCallback += NetworkManager_OnClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_OnClientDisconnect;
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Keypad0))
        {
            // Debug.Log(_playerDataNetworkList.Count);
            foreach (var playerData in _playerDataNetworkList) {
                Debug.Log($"ClientID: {playerData.ClientID}; PlayerName: {playerData.PlayerName}");
            }
        }
    }
    
    // Handles creating the player data for the newly joined client
    private void NetworkManager_OnClientConnected(ulong clientId) {
        // only server should handle this
        if (!IsServer)
            return;
        
        // if the client is the host, clear the player data list from previous session
        if (clientId == NetworkManager.ServerClientId) {
            // Debug.Log("Host connected.");
            if (_playerDataNetworkList != null)
                _playerDataNetworkList.Clear();
        }

        // loop through all players and check if they already have the same color
        
        List<Color> availableColors = _gameSettings.PlayerPresetColors.GetRange(0, ConnectionHandler.MAX_PLAYER_COUNT);

        if (_playerDataNetworkList != null) {
            foreach (var playerData in _playerDataNetworkList) {
                availableColors.Remove(playerData.PlayerColor);
            }

            Color newPlayerColor = availableColors[Random.Range(0, availableColors.Count)];

            PlayerData newPlayerData = new PlayerData(clientId, newPlayerColor);
            _playerDataNetworkList.Add(newPlayerData);
        }
    }
    
    private void NetworkManager_OnClientDisconnect(ulong clientId) {
        Debug.Log($"Player {clientId} disconnected.");
        if (!ConnectionHandler.Instance.IsConnected)
            return;
        
        if (!IsServer)
            return;
        
        Debug.Log("[PlayerDataHandler] Removing player from list.");
        for (int i = 0; i < _playerDataNetworkList.Count; i++) {
            Debug.Log($"Current index's player: {_playerDataNetworkList[i].ClientID}; Player to remove: {clientId}");
            if (_playerDataNetworkList[i].ClientID == clientId) {
                _playerDataNetworkList.RemoveAt(i);
                Debug.Log($"Removed player {clientId} at index {i}");
                return;
            }
        }
    }
    
    
    private void PlayerDataNetworkList_OnListChanged(NetworkListEvent<PlayerData> changeEvent) {
        // Debug.Log($"Type:{changeEvent.Type}; Index: {changeEvent.Index}; clientID: {changeEvent.Value.ClientID}");

        // ANYTHING AFTER THIS POINT IS ONLY FOR CLIENTS
        if (!IsClient)
            return;
        
        
        // if the list is added, that means the newly joined client will also have the player data list synced
        if (changeEvent.Type == NetworkListEvent<PlayerData>.EventType.Add) {
            // if it's not the local client, ignore
            // otherwise it will send an rpc to the server, which the server can utilize the OnClientPlayerDataListSynced event
            if (changeEvent.Value.ClientID != NetworkManager.Singleton.LocalClientId)
                return;
            
            // for server to know
            OnClientSyncedDataServerRpc(changeEvent.Value.ClientID);
            
            // for local client that joined to know
            OnLocalClientSyncedData?.Invoke(this, EventArgs.Empty);
        } 
        else if (changeEvent.Type 
                 is NetworkListEvent<PlayerData>.EventType.RemoveAt 
                 or NetworkListEvent<PlayerData>.EventType.Remove) {
            
            // for local client to know
            OnPlayerDataRemoved?.Invoke(this, new OnPlayerDataRemovedEventArgs(changeEvent.Value.ClientID));
        }
    }
    
    [ServerRpc(RequireOwnership = false)]
    private void OnClientSyncedDataServerRpc(ulong clientID) {
        Debug.Log($"Client {clientID} synced data.");
        OnClientPlayerDataListSynced?.Invoke(this, new OnClientPlayerDataListSyncedEventArgs(clientID));
    }
    
    
    // UPDATE PLAYER NAME ---------------------------------------------------------------------
    
    [ServerRpc(RequireOwnership = false)]
    public void SubmitPlayerNameServerRpc(string playerName, ServerRpcParams serverRpcParams = default) {
        // Debug.Log("SubmitPlayerNameServerRpc");
        int playerIndex = GetPlayerIndexFromClientID(serverRpcParams.Receive.SenderClientId);
        PlayerData playerData = _playerDataNetworkList[playerIndex];
        
        
        // if string is empty, set to player order
        if (string.IsNullOrEmpty(playerName)) {
            string name1 = _playerNames[Random.Range(0, _playerNames.Count)];
            string name2 = _playerNames[Random.Range(0, _playerNames.Count)];
            playerName = $"{name1}{name2}";
        }
        
        // if the name is already taken, add a number to the end
        for (int i = 0; i < _playerDataNetworkList.Count; i++) {
            if (_playerDataNetworkList[i].PlayerName == playerName) {
                playerName += $"{i + 1}";
            }
        }
        
        playerData.SetPlayerName(playerName);
        _playerDataNetworkList[playerIndex] = playerData;
        
        NotifyPlayerNameChangedClientRpc(serverRpcParams.Receive.SenderClientId, playerName);
    }
    
    [ClientRpc]
    private void NotifyPlayerNameChangedClientRpc(ulong clientID, string playerName) {
        OnPlayerNameChanged?.Invoke(this, new OnPlayerNameChangedEventArgs(clientID, playerName));
    }
    
    
    // GETTERS ---------------------------------------------------------------------
    
    public int GetPlayerIndexFromClientID(ulong clientID) {
        for (int i = 0; i < _playerDataNetworkList.Count; i++) {
            if (_playerDataNetworkList[i].ClientID == clientID)
                return i;
        }

        return -1;
    }
    
    public string GetPlayerName(ulong clientID) {
        int playerIndex = GetPlayerIndexFromClientID(clientID);
        return _playerDataNetworkList[playerIndex].PlayerName.ToString();
    }
    
    public Color GetPlayerColor(ulong clientID) {
        int playerIndex = GetPlayerIndexFromClientID(clientID);
        return _playerDataNetworkList[playerIndex].PlayerColor;
    }
    
    public PlayerData GetPlayerData(ulong clientID) {
        int playerIndex = GetPlayerIndexFromClientID(clientID);
        return _playerDataNetworkList[playerIndex];
    }
    
    public PlayerData GetLocalPlayerData() {
        int playerIndex = GetPlayerIndexFromClientID(NetworkManager.Singleton.LocalClientId);
        return _playerDataNetworkList[playerIndex];
    }
}

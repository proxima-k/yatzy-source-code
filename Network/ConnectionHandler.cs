
using System;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.SceneManagement;

// should only run on server
public class ConnectionHandler : NetworkBehaviour {
    
    public static ConnectionHandler Instance { get; private set; }
    
    public const int MAX_PLAYER_COUNT = 4;

    // public event EventHandler OnAttemptToConnect;
    public event EventHandler OnConnectionSucceeded;
    public event EventHandler<OnConnectionFailedEventArgs> OnConnectionFailed;
    public class OnConnectionFailedEventArgs : EventArgs {
        public string Reason { get; set; }
    }
    
    public event EventHandler OnHostDisconnected;
    public event EventHandler OnSelfDisconnected;

    public event EventHandler OnOtherClientConnected;
    public event EventHandler OnOtherClientDisconnected;
    
    public bool IsConnected => _isConnected;
    private bool _isConnected = false;

    private void Awake() {
        if (Instance != null) {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    

    public bool StartHost() {
        NetworkManager.Singleton.ConnectionApprovalCallback = NetworkManager_ConnectionApprovalCallback;
        
        NetworkManager.Singleton.OnClientConnectedCallback += NetworkManager_OnClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_OnClientDisconnect;

        if (NetworkManager.Singleton.StartHost()) {
            return true;
        }
        
        NetworkManager.Singleton.ConnectionApprovalCallback = null;
        NetworkManager.Singleton.OnClientConnectedCallback -= NetworkManager_OnClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback -= NetworkManager_OnClientDisconnect;
        return false;
    }
    
    public bool StartClient() {
        NetworkManager.Singleton.OnClientConnectedCallback += NetworkManager_OnClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_OnClientDisconnect;

        if (NetworkManager.Singleton.StartClient()) {
            return true;
        }
        
        NetworkManager.Singleton.OnClientConnectedCallback -= NetworkManager_OnClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback -= NetworkManager_OnClientDisconnect;
        return false;
    }


    private void NetworkManager_ConnectionApprovalCallback(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response) {

        if (SceneManager.GetActiveScene().name != SceneLoader.Scene.LobbyScene.ToString()) {
            response.Approved = false;
            response.Reason = "Game already in progress.";
            return;
        }
        
        if (NetworkManager.Singleton.ConnectedClientsList.Count >= MAX_PLAYER_COUNT) {
            response.Approved = false;
            response.Reason = "Game is full.";
            return;
        }
        
        response.Approved = true;
    }

    private void NetworkManager_OnClientConnected(ulong clientID) {

        if (clientID == NetworkManager.Singleton.LocalClientId) {
            _isConnected = true;
            OnConnectionSucceeded?.Invoke(this, EventArgs.Empty);
            
        } else {
            OnOtherClientConnected?.Invoke(this, EventArgs.Empty);
        }
    }

    
    // todo: tidy up this function
    private void NetworkManager_OnClientDisconnect(ulong clientID) {
        
        Debug.Log($"[ConnectionHandler] {clientID} disconnected from server.");

        if (_isConnected) {
            if (clientID == NetworkManager.ServerClientId) {
                // Debug.Log("Host has disconnected");
                OnHostDisconnected?.Invoke(this, EventArgs.Empty);
                _isConnected = false;
            } 
            
            else if (clientID == NetworkManager.Singleton.LocalClientId) {
                OnSelfDisconnected?.Invoke(this, EventArgs.Empty);
                _isConnected = false;
            } 
            
            else {
                OnOtherClientDisconnected?.Invoke(this, EventArgs.Empty);
            }
            
            return;
        }
        
        // will trigger when client can't connect to server
        if (!NetworkManager.Singleton.IsServer && NetworkManager.Singleton.DisconnectReason != string.Empty) {
            OnConnectionFailed?.Invoke(this, new OnConnectionFailedEventArgs { Reason = NetworkManager.Singleton.DisconnectReason });
            return;
        }
        
        
        if (clientID == NetworkManager.Singleton.LocalClientId) {
            OnConnectionFailed?.Invoke(this, new OnConnectionFailedEventArgs { Reason = "Failed to connect to server." });
        }
        
    }

    public void Disconnect() {
        // add an event OnDisconnected
        OnSelfDisconnected?.Invoke(this, EventArgs.Empty);
        _isConnected = false;
        
        NetworkManager.Singleton.Shutdown();
        NetworkManager.Singleton.ConnectionApprovalCallback = null;
        NetworkManager.Singleton.OnClientConnectedCallback -= NetworkManager_OnClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback -= NetworkManager_OnClientDisconnect;
    }
}

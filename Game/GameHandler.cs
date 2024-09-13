using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameHandler : NetworkBehaviour {
    public static GameHandler Instance { get; private set; }

    // public event EventHandler OnNextPlayerTurn;
    public event EventHandler OnGameStart;
    public event EventHandler OnGameEnd;
    public event EventHandler OnPlayerEndTurn;
    public event EventHandler<OnPlayerStartTurnEventArgs> OnPlayerStartTurn; 
    public class OnPlayerStartTurnEventArgs : EventArgs {
        public int PlayerIndex;
        public OnPlayerStartTurnEventArgs(int playerIndex) {
            PlayerIndex = playerIndex;
        }
    }
    public event EventHandler<OnPlayerDisconnectedEventArgs> OnPlayerDisconnected;
    public class OnPlayerDisconnectedEventArgs : EventArgs {
        public int PlayerIndex; 
        public OnPlayerDisconnectedEventArgs(int playerIndex) {
            PlayerIndex = playerIndex;
        }
    }
    
    
    // Player settings
    public List<Player> Players => _players;
    private int _currentPlayerIndex = 0;
    [SerializeField] private Transform _playerPrefab;
    [SerializeField] private List<Player> _players = new List<Player>();
    public void AddPlayer(Player player) => _players.Add(player);
    public void RemovePlayer(Player player) => _players.Remove(player);
    
    // Game settings
    public GameSettings GameSettings => _gameSettings;
    [SerializeField] private GameSettings _gameSettings;
    
    [SerializeField] private ScorecardUICreator _scorecardUICreator;
    
    public bool IsGameStarted => _isGameStarted;
    public bool IsGameEnded => _isGameEnded;
    private bool _isGameStarted = false;
    private bool _isGameEnded = false;
    
    private void Awake() {
        if (Instance != null) {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        
        NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += SceneManager_OnLoadEventCompleted;
    }

    private void SceneManager_OnLoadEventCompleted(string sceneName, LoadSceneMode loadSceneMode, List<ulong> clientsCompleted, List<ulong> clientsTimedOut) {
        if (sceneName != SceneLoader.Scene.GameScene.ToString()) {
            return;
        }
        
        if (!IsServer) {
            return;
        }
        
        switch (SceneLoader.CurrentGameMode) {
            case SceneLoader.GameMode.OnlineMultiplayer:
                
                SetupOnlineGame();
                
                break;
            case SceneLoader.GameMode.LocalMultiplayer:
                
                SetupLocalGame();
                
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }        
        
        StartCoroutine(StartGame());
    }
    
    
    
    private IEnumerator StartGame() {
        yield return new WaitForSeconds(3f);
        
        _isGameStarted = true;
        
        OnGameStartClientRpc();
        
        PlayerTurnStart();
    }
    
    [ClientRpc]
    private void OnGameStartClientRpc() {
        OnGameStart?.Invoke(this, EventArgs.Empty);
    }
    
    private void PlayerTurnStart() {
        Player player = _players[_currentPlayerIndex];
        
        if (_currentPlayerIndex == 0 && player.PlayerScorecard.AllRulesCompleted()) {
            Debug.LogWarning("GAME HAS ENDED");
            // go to end game state
            
            EndGame();
            return;
        }
        
        player.StartTurnClientRpc();
        // OnPlayerStartTurn?.Invoke(this, new OnPlayerStartTurnEventArgs(player, _currentPlayerIndex+1));
        OnPlayerStartTurnClientRpc(_currentPlayerIndex);
    }
    
    
    [ClientRpc]
    private void OnPlayerStartTurnClientRpc(int playerIndex) {
        OnPlayerStartTurn?.Invoke(this, new OnPlayerStartTurnEventArgs(playerIndex));
    }
    
    private void Player_OnEndTurn(object sender, EventArgs e) {
        _currentPlayerIndex++;
        if (_currentPlayerIndex >= _players.Count) {
            _currentPlayerIndex = 0;
        }
        
        OnPlayerEndTurn?.Invoke(this, EventArgs.Empty);
        PlayerTurnStart();
    }
    
    private void EndGame() {
        _isGameEnded = true;
        
        Player winner = _players[0];
        foreach (var player in _players) {
            if (player.PlayerScorecard.GetTotalScore() > winner.PlayerScorecard.GetTotalScore()) {
                winner = player;
            }
        }
        Debug.Log($"Winner is {winner.Name} with a score of {winner.PlayerScorecard.GetTotalScore()}.");
        OnGameEnd?.Invoke(this, EventArgs.Empty);
        NotifyGameEndClientRpc();
    }
    
    [ClientRpc]
    private void NotifyGameEndClientRpc() {
        if (IsServer) {
            return;
        }
        
        OnGameEnd?.Invoke(this, EventArgs.Empty);
    }
    
    [ClientRpc]
    private void PlayerDisconnectedClientRpc(int playerIndex) {
        OnPlayerDisconnected?.Invoke(this, new OnPlayerDisconnectedEventArgs(playerIndex));
    }
    
    private void SetupOnlineGame() {
        if (!IsServer) {
            return;
        }
        
        Debug.Log("Server is setting up game.");
        _players.Clear();

        var clients = NetworkManager.Singleton.ConnectedClientsList;
        int clientCount = clients.Count;
        if (clientCount <= 0) {
            Debug.LogError("No clients connected.");
            return;
        }
        
        for (int i = 0; i < clientCount; i++) {
            var player = Instantiate(_playerPrefab, transform).GetComponent<Player>();
            player.gameObject.GetComponent<NetworkObject>().SpawnAsPlayerObject(clients[i].ClientId);
            
            player.OnEndTurn += Player_OnEndTurn;
            
            PlayerData playerData = PlayerDataHandler.Instance.GetPlayerData(clients[i].ClientId);
            string playerName = playerData.PlayerName.ToString();
            Color playerColor = playerData.PlayerColor;
            player.InitializeClientRpc(playerName, playerColor);

        }
        
        // todo: timing of disconnection will cause bug
        NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_OnClientDisconnect;

        
        SetupLocalPlayerUIClientRpc();
    }

    [ClientRpc]
    private void SetupLocalPlayerUIClientRpc() {
        
        PlayerUI.Instance.Initialize();
        
        Player localPlayer = NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<Player>();
        
        PlayerUI.Instance.InitializePlayerInput(localPlayer);
    }
    
    private void NetworkManager_OnClientDisconnect(ulong clientId) {
        if (!IsServer) {
            return;
        }
        
        Player disconnectingPlayer = NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject.GetComponent<Player>();
        disconnectingPlayer.OnEndTurn -= Player_OnEndTurn;
        int playerIndex = _players.IndexOf(disconnectingPlayer);
        
        PlayerDisconnectedClientRpc(playerIndex);
        
        Player currentTurnPlayer = _players[_currentPlayerIndex];
        
        _players.Remove(disconnectingPlayer);
        
        // if the player disconnecting is the current player, start next player's turn
        if (disconnectingPlayer == currentTurnPlayer) {
            _currentPlayerIndex--;
            Player_OnEndTurn(disconnectingPlayer, EventArgs.Empty);
        }
        
    }
    
    private void SetupLocalGame() {
        // NetworkManager.Singleton.StartHost();
        
        // create players
        if (_gameSettings.NumberOfPlayers <= 0) {
            Debug.LogError("Number of players is 0 or less.");
            return;
        }
        
        _players.Clear();
        for (int i = 0; i < SceneLoader.NumPlayers; i++) {
            var player = Instantiate(_playerPrefab, transform).GetComponent<Player>();
            player.gameObject.GetComponent<NetworkObject>().Spawn();
            
            player.OnEndTurn += Player_OnEndTurn;
            
            string playerName = $"Player {i + 1}";
            player.InitializeClientRpc(playerName, _gameSettings.PlayerPresetColors[i]);
        }
     
        // initialize scorecard UI
        PlayerUI.Instance.Initialize();
        for (int i = 0; i < _players.Count; i++) {
            PlayerUI.Instance.InitializePlayerInput(_players[i]);
            RollDieUI.Instance.InitializePlayer(_players[i]);
        }
    }
}

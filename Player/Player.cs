using System;
using Unity.Netcode;
using UnityEngine;

public class Player : NetworkBehaviour {
    
    public string Name => _name;
    [SerializeField] private string _name = "New Player";
    
    public Color PlayerColor => _playerColor;
    public PlayerScorecard PlayerScorecard => playerScorecard;
    public PlayerStateManager PlayerStateManager => _playerStateManager;
    public PlayerInput PlayerInput => _playerInput;
    public DiceSelect DiceSelect => _diceSelect;
    
    [SerializeField] private Color _playerColor;
    [SerializeField] private PlayerScorecard playerScorecard;
    [SerializeField] private PlayerStateManager _playerStateManager;
    [SerializeField] private PlayerInput _playerInput;
    [SerializeField] private DiceSelect _diceSelect; // todo: this could be an instance and just pass in the player input

    public event EventHandler OnStartTurn;
    public event EventHandler OnEndTurn;
    
    [ClientRpc]
    public void InitializeClientRpc(string name, Color playerColor) {
        _name = name;
        _playerColor = playerColor;
        
        gameObject.name = _name;
    }

    public override void OnNetworkSpawn() {
        GameHandler.Instance.AddPlayer(this);
    }

    private void Awake() {
        _playerInput = GetComponent<PlayerInput>();
        _playerStateManager = GetComponent<PlayerStateManager>();
        playerScorecard = GetComponent<PlayerScorecard>();
        _diceSelect = GetComponent<DiceSelect>();
        _diceSelect.enabled = false;
    }

    [ClientRpc]
    public void StartTurnClientRpc() {
        if (!IsOwner) {
            return;
        }
        
        Debug.Log("Starting turn");
        StartTurn();
    }
    
    [ClientRpc]
    public void EndTurnClientRpc() {
        if (!IsOwner && !IsServer) {
            return;
        }
        
        Debug.Log("Ending turn");
        EndTurn();
    }

    private void StartTurn() {
        _playerStateManager.StartTurn();
        OnStartTurn?.Invoke(this, EventArgs.Empty);
    }

    private void EndTurn() {
        _playerStateManager.EndTurn();
        
        OnEndTurn?.Invoke(this, EventArgs.Empty);
    }

    public override void OnNetworkDespawn() {
        base.OnNetworkDespawn();
        GameHandler.Instance.RemovePlayer(this);
    }
}

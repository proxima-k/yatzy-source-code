using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using Random = UnityEngine.Random;

public class DiceRoller : NetworkBehaviour {
    public static DiceRoller Instance { get; private set; }
    
    public event EventHandler<OnAllDieRolledEventArgs> OnAllDieRolled;
    public class OnAllDieRolledEventArgs : EventArgs {
        public List<int> DieValues;
        public OnAllDieRolledEventArgs(List<int> dieValues) {
            DieValues = dieValues;
        }
    }
    
    public event EventHandler<OnDiceToggleSelectEventArgs> OnDiceToggleSelect;
    public class OnDiceToggleSelectEventArgs : EventArgs {
        public Dice Dice;
        public OnDiceToggleSelectEventArgs(Dice dice) {
            Dice = dice;
        }
    }
    
    [SerializeField] private Transform _throwPointOrientation;

    [SerializeField] private float _throwForce = 6f;
    [SerializeField] private float _rollForce = 10f;

    [SerializeField] private List<Dice> _allDieList;
    [SerializeField] private List<Dice> _selectedDiceList = new List<Dice>();
    [SerializeField] private List<Dice> _dieToRollList = new List<Dice>();
    
    public List<int> DieValues => _dieValues;
    [SerializeField] private List<int> _dieValues = new List<int>();
    
    private NetworkList<int> _dieValuesNetworkList;
    
    private Coroutine _rollDicesCoroutine;
    private int _diceRolled = 0;

    private void Awake() {
        if (Instance != null) {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        
        _dieValuesNetworkList = new NetworkList<int>(null, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    }

    private void Start() {
        foreach (var dice in _allDieList) {
            dice.DiceLocomotion.OnStoppedRolling += Dice_OnStoppedRolling;
            _dieValues.Add(-1);
        }
        
        if (GameHandler.Instance != null)
            GameHandler.Instance.OnPlayerEndTurn += GameHandler_OnPlayerEndTurn;
        else
            Debug.LogWarning("GameHandler is null!");
    }

    public override void OnNetworkSpawn() {
        _dieValuesNetworkList.OnListChanged += On_DieValuesNetworkList_Changed;
        Debug.Log("DiceRoller spawned!");
    }

    private void On_DieValuesNetworkList_Changed(NetworkListEvent<int> changeEvent) {
        // Debug.Log($"DieValuesNetworkList changed: {changeevent.Type}");
        //
        // if (changeevent.Type == NetworkListEvent<int>.EventType.Add) {
        //     Debug.Log($"DieValuesNetworkList added: {changeevent.Value}");
        // }
        // else if (changeevent.Type == NetworkListEvent<int>.EventType.Remove) {
        //     Debug.Log($"DieValuesNetworkList removed: {changeevent.Value}");
        // }
        // else if (changeevent.Type == NetworkListEvent<int>.EventType.Insert) {
        //     Debug.Log($"DieValuesNetworkList inserted: {changeevent.Value}");
        // }
        // else if (changeevent.Type == NetworkListEvent<int>.EventType.Clear) {
        //     Debug.Log($"DieValuesNetworkList cleared: {changeevent.Value}");
        // }
        // else if (changeevent.Type == NetworkListEvent<int>.EventType.Value) {
        //     Debug.Log("Assigning die values...");
        // }
        
        if (_dieValuesNetworkList.Count == _allDieList.Count) {
            _dieValues.Clear();
            foreach (var dieValue in _dieValuesNetworkList) {
                _dieValues.Add(dieValue);
            }
            Debug.Log("All die values assigned!");
            OnAllDieRolled?.Invoke(this, new OnAllDieRolledEventArgs(_dieValues));
        }
    }
    
    private void Dice_OnStoppedRolling(object sender, DiceLocomotion.OnStoppedRollingEventArgs e) {
        AddDiceRolled();
    }

    private void AddDiceRolled() {
        _diceRolled++;
        
        if (_diceRolled == _dieToRollList.Count) {
            // use indexing would help
            List<Dice> dieWithValues = new List<Dice>();

            // get the die to roll index
            foreach (var dice in _dieToRollList) {
                if (dice.TryGetValue(out int diceValue)) {
                    _dieValues[_allDieList.IndexOf(dice)] = diceValue;
                    
                    dieWithValues.Add(dice);
                }
            }

            foreach (Dice dice in dieWithValues) {
                _dieToRollList.Remove(dice);
            }
            
            if (_dieToRollList.Count > 0) {
                RollDie(_dieToRollList);
                return;
            }
            
            Debug.Log("All die rolled!");
            
            // network list implementation
            _dieValuesNetworkList.Clear();
            for (var index = 0; index < _dieValues.Count; index++) {
                var dieValue = _dieValues[index];
                _dieValuesNetworkList.Add(dieValue);
            }

            // OnAllDieRolled?.Invoke(this, new OnAllDieRolledEventArgs(_dieValues));
        }
    }
    
    
    [ServerRpc(RequireOwnership = false)]
    public void RollAllDieServerRpc() {
        Debug.Log("Rolling all die!");
        RollDie(_allDieList);
    }
    
    [ServerRpc(RequireOwnership = false)]
    public void RollSelectedDieServerRpc() {
        // todo: check if the client calling this rpc is the current player
        
        Debug.Log("Rolling selected die!");
        RollDie(_selectedDiceList);
    }
    
    private void RollDie(List<Dice> dieToRollList) {
        
        _dieToRollList = new List<Dice>();
        if (dieToRollList.Count == 0) {
            _dieToRollList.AddRange(_allDieList);
        }
        else {
            _dieToRollList.AddRange(dieToRollList);
        }
        
        _diceRolled = 0;
        
        if (_rollDicesCoroutine != null)
            StopCoroutine(_rollDicesCoroutine);
        _rollDicesCoroutine = StartCoroutine(RollDicesRoutine());

    }
    
    private IEnumerator RollDicesRoutine() {
        // set rolls completed value
        _diceRolled = 0;
        
        foreach (var dice in _dieToRollList) {
            RollDice(dice);
 
            yield return new WaitForSeconds(0.05f);
        }

        yield return null;
    }

    private void RollDice(Dice dice) {
        dice.DiceLocomotion.TriggerRoll();
        Transform diceTf = dice.transform;
        // set position
        diceTf.position = _throwPointOrientation.position;
        
        // set random rotation
        diceTf.rotation = Random.rotation;
        
        Rigidbody diceRigidbody = diceTf.GetComponent<Rigidbody>();
        diceRigidbody.velocity = Vector3.zero;
        
        // add force
        diceRigidbody.AddForce(_throwPointOrientation.forward * (_throwForce * Random.Range(0.5f, 1.5f)), ForceMode.Impulse);
        
        // add torque
        Vector3 torque = new Vector3(
            Random.Range(-1f, 1f), 
            Random.Range(-1f, 1f), 
            Random.Range(-1f, 1f));
        
        diceRigidbody.AddTorque(torque * _rollForce, ForceMode.Impulse);
    }
    
    private void GameHandler_OnPlayerEndTurn(object sender, EventArgs e) {
        foreach (var dice in _selectedDiceList) {
            OnDiceToggleSelect?.Invoke(this, new OnDiceToggleSelectEventArgs(dice));
        }
        _selectedDiceList.Clear();
        Debug.LogWarning("Clearing selected dice list!");
    }
    
    // [ServerRpc]
    public void ToggleSelect(Dice dice) {
        int diceIndex = _allDieList.IndexOf(dice);

        ToggleSelectServerRpc(diceIndex);
        
        // if (_selectedDiceList.Contains(dice)) {
        //     _selectedDiceList.Remove(dice);
        //     Debug.Log($"{dice.name} deselected!");
        // }
        // else {
        //     _selectedDiceList.Add(dice);
        //     Debug.Log($"{dice.name} selected!");
        // }
        // OnDiceToggleSelect?.Invoke(this, new OnDiceToggleSelectEventArgs(dice));
    }
    
    [ServerRpc(RequireOwnership = false)]
    private void ToggleSelectServerRpc(int diceIndex) {
        
        Dice dice = _allDieList[diceIndex];
        
        if (_selectedDiceList.Contains(dice)) {
            _selectedDiceList.Remove(dice);
            Debug.Log($"{dice.name} deselected!");
        }
        else {
            _selectedDiceList.Add(dice);
            Debug.Log($"{dice.name} selected!");
        }
        
        OnDiceToggleSelect?.Invoke(this, new OnDiceToggleSelectEventArgs(dice));
    }
    
#if UNITY_EDITOR
    private void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(_throwPointOrientation.position, _throwPointOrientation.forward * 3f);
    }
#endif
}

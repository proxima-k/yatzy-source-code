using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class DiceVisual : NetworkBehaviour {
    
    [SerializeField] private DiceRoller _diceRoller;
    [SerializeField] private Dice _dice;
    [SerializeField] private MeshRenderer _meshRenderer;
    [SerializeField] private Material _highlightMaterial;
    [SerializeField] private Material _defaultMaterial;
    
    // private bool _isSelected = false;

    private NetworkVariable<bool> _isSelectedNetworkVar = new NetworkVariable<bool>(false);
    
    private void Awake() {
        _defaultMaterial = _meshRenderer.sharedMaterial;
    }
    
    private void Start() {
        _diceRoller = DiceRoller.Instance;
        if (_diceRoller == null) {
            // Debug.LogWarning("DiceRoller is null.");
            return;
        }
        _diceRoller.OnDiceToggleSelect += DiceRoller_OnDiceToggleSelect;
    }

    public override void OnNetworkSpawn() {
        _isSelectedNetworkVar.OnValueChanged += On_IsSelectedNetworkVar_Changed;
    }


    private void DiceRoller_OnDiceToggleSelect(object sender, DiceRoller.OnDiceToggleSelectEventArgs e) {
        if (e.Dice != _dice) {
            return;
        }
        
        _isSelectedNetworkVar.Value = !_isSelectedNetworkVar.Value;
    }
    
    private void On_IsSelectedNetworkVar_Changed(bool previousValue, bool newValue) {
        Debug.Log($"DiceVisual: {gameObject.name} isSelected: {newValue}");
        
        if (newValue) {
            _meshRenderer.sharedMaterial = _highlightMaterial;
        }
        else {
            _meshRenderer.sharedMaterial = _defaultMaterial;
        }
    }

    // for player's visual feedback
    public void Highlight(bool isHighlighted) {
        if (_isSelectedNetworkVar.Value)
            return;
        
        if (isHighlighted) {
            _meshRenderer.sharedMaterial = _highlightMaterial;
        }
        else {
            _meshRenderer.sharedMaterial = _defaultMaterial;
        }
    }

    
    public void ToggleSelect() {
        bool isSelected = _isSelectedNetworkVar.Value;
        
        isSelected = !isSelected;
        Highlight(isSelected);
    }
    
    public void ResetMaterial() {
        _meshRenderer.sharedMaterial = _defaultMaterial;
    }
}

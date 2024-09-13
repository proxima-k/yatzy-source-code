using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DiceSelect : MonoBehaviour {
    
    [SerializeField] PlayerInput _playerInput;
    [SerializeField] LayerMask _diceLayerMask;
    
    public List<Dice> SelectedDiceList => _selectedDiceList;
    private List<Dice> _selectedDiceList = new List<Dice>();

    private DiceVisual _highlightedDice;
    private Camera _camera;
    
    private void OnEnable() {
        _camera = Camera.main;
        if (_playerInput == null) {
            Debug.LogWarning("Player input is null.");
            return;
        }
        _playerInput.OnPlayerSelect += PlayerInput_OnPlayerSelect;
        _selectedDiceList.Clear();
    }

    private void OnDisable() {
        _playerInput.OnPlayerSelect -= PlayerInput_OnPlayerSelect;
        _selectedDiceList.Clear();
    }
    
    private void Update() {
        HighlightDice();
    }

    private void PlayerInput_OnPlayerSelect(object sender, EventArgs e) {
        Ray ray = _camera.ScreenPointToRay(Mouse.current.position.ReadValue());
        
        if (Physics.Raycast(ray, out var hit, 100, _diceLayerMask)) {
            
            Dice dice = hit.rigidbody.GetComponent<Dice>();
            if (dice == null) {
                return;
            }
            
            DiceRoller.Instance.ToggleSelect(dice);
        }
    }

    
    // this part of code doesn't work with logic, it's only purpose is to give visual feedback to the player
    private void HighlightDice() {
        Ray ray = _camera.ScreenPointToRay(Mouse.current.position.ReadValue());

        // if hit something
        if (Physics.Raycast(ray, out var hit, 100, _diceLayerMask)) {
            // if found dice
            if (hit.rigidbody.TryGetComponent<DiceVisual>(out var diceVisual)) {
                
                
                if (_highlightedDice != diceVisual) {
                    if (_highlightedDice != null) {
                        _highlightedDice.Highlight(false);
                        // Debug.Log($"Unhighlighting {_highlightedDice.name}!");
                    }
                    _highlightedDice = diceVisual;
                    _highlightedDice.Highlight(true);
                    // _selectedDice.Highlight();
                    // Debug.Log($"Highlighting {_highlightedDice.name}!");
                }
            }
            // if found something else
            else {
                if (_highlightedDice != null) {
                    _highlightedDice.Highlight(false);
                    // Debug.Log($"Unhighlighting {_highlightedDice.name}!");
                    _highlightedDice = null;
                }
            }
        }
        // if hit nothing
        else {
            if (_highlightedDice != null) {
                _highlightedDice.Highlight(false);
                // Debug.Log($"Unhighlighting {_highlightedDice.name}!");
                _highlightedDice = null;
            }
        }
    }
}

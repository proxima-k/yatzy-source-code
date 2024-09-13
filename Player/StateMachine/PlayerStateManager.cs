using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PlayerStateManager : StateManager<PlayerBaseState> {
    
    public Player Player => _player;
    private Player _player;
    
    // all the states here
    public PlayerPassiveState PassiveState;
    public PlayerRollDieState RollDieState;
    
    private void Awake() {
        PassiveState = new PlayerPassiveState(this);
        RollDieState = new PlayerRollDieState(this);
        ChangeState(PassiveState);
        _player = GetComponent<Player>();
    }
    
    public void StartTurn() {
        Debug.Log($"{_player.Name} is starting their turn.");
        ChangeState(RollDieState);
    }
    
    public void EndTurn() {
        Debug.Log($"{_player.Name} is ending their turn.");
        ChangeState(PassiveState);
    }
    
    // handles what player can do during a turn
    // states
        // passive
        // roll dice
            // this will keep checking the dice until they stopped rolling
        // choose options
            // choice 1: reroll die
            // choice 2: submit choice
                // move to a different 
        // end turn
            // do necessary operations
}

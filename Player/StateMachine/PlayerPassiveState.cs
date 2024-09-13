using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPassiveState : PlayerBaseState {
    
    public override void EnterState() {
        // Debug.Log($"{_playerStateMachine.Player.Name} is in passive state.");
    }

    public override void UpdateState() {
    }

    public override void ExitState() {
    }

    public PlayerPassiveState(PlayerStateManager playerStateManager) : base(playerStateManager) { }
}

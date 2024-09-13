using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStartTurnSound : MonoBehaviour {
    private void Start() {
        if (SoundHandler.Instance == null)
            return;
        
        GameHandler.Instance.OnPlayerStartTurn += GameHandler_OnPlayerStartTurn;
    }
    
    private void GameHandler_OnPlayerStartTurn(object sender, GameHandler.OnPlayerStartTurnEventArgs e) {
        if (SoundHandler.Instance == null)
            return;
        
        SoundHandler.Instance.PlaySound(SoundHandler.Instance.AudioClipRefsSo.DiceGrab, transform.position);
    }
}

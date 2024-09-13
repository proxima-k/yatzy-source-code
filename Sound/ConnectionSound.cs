using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ConnectionSound : MonoBehaviour {
    private void Start() {
        ConnectionHandler.Instance.OnConnectionSucceeded += ConnectionHandler_OnConnectionSucceeded;
        ConnectionHandler.Instance.OnConnectionFailed += ConnectionHandler_OnConnectionFailed;
     
        ConnectionHandler.Instance.OnOtherClientConnected += ConnectionHandler_OnOtherClientConnected;
        ConnectionHandler.Instance.OnOtherClientDisconnected += ConnectionHandler_OnOtherClientDisconnected;
        
        ConnectionHandler.Instance.OnHostDisconnected += ConnectionHandler_OnConnectionFailed;
    }
    
    private void ConnectionHandler_OnConnectionFailed(object sender, EventArgs e) {
        SoundHandler.Instance.PlaySound(SoundHandler.Instance.AudioClipRefsSo.ConnectionFail, Vector3.zero, 0.5f);
    }
    
    private void ConnectionHandler_OnConnectionSucceeded(object sender, EventArgs e) {
        SoundHandler.Instance.PlaySound(SoundHandler.Instance.AudioClipRefsSo.ConnectionSuccess, Vector3.zero);
    }

    private void ConnectionHandler_OnOtherClientConnected(object sender, EventArgs e) {
        SoundHandler.Instance.PlaySound(SoundHandler.Instance.AudioClipRefsSo.PlayerJoin, Vector3.zero);
    }
    
    private void ConnectionHandler_OnOtherClientDisconnected(object sender, EventArgs e) {
        SoundHandler.Instance.PlaySound(SoundHandler.Instance.AudioClipRefsSo.PlayerLeave, Vector3.zero);
    }
}

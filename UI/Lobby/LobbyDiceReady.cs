using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyDiceReady : MonoBehaviour {

    [SerializeField] private MeshRenderer _meshRenderer;
    [SerializeField] private Material _notReadyMaterial;
    [SerializeField] private Material _readyMaterial;
    
    // listens to lobby event
    // if player is ready, show ready material
    
    // if the player that is ready is the local player, show ready material
    
    public void SetReady(bool isReady) {
        _meshRenderer.material = isReady ? _readyMaterial : _notReadyMaterial;
    }
    
    // possibility to add a juicy animation
}

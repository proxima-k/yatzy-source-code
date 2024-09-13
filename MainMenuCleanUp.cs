using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class MainMenuCleanUp : MonoBehaviour {
    [SerializeField] private NetworkManager NetworkManagerPrefab;
    
    private void Awake() {
        if (NetworkManager.Singleton != null) {
            Destroy(NetworkManager.Singleton.gameObject);
            NetworkManager networkManager = Instantiate(NetworkManagerPrefab);
            networkManager.SetSingleton();
        } else {
            NetworkManager networkManager = Instantiate(NetworkManagerPrefab);
            networkManager.SetSingleton();
        }
        
        // if (ConnectionHandler.Instance != null) {
        //     Destroy(ConnectionHandler.Instance.gameObject);
        // }
        
        if (RelayHandler.Instance != null) {
            Destroy(RelayHandler.Instance.gameObject);
        }
        
        if (PlayerDataHandler.Instance != null) {
            Destroy(PlayerDataHandler.Instance.gameObject);
        }
    }
}

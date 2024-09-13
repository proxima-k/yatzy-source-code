using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using UnityEngine;
using UnityEngine.UI;

public class LocalGameModeUI : MonoBehaviour {
    [SerializeField] private GameObject _UIContainer;
    
    [SerializeField] private Button _subtractButton;
    [SerializeField] private Button _addButton;
    [SerializeField] private Button _startButton;
    [SerializeField] private Button _backButton;
    [SerializeField] private TextMeshProUGUI _numPlayersText;
    
    private const int MIN_PLAYERS = 1;
    private const int MAX_PLAYERS = 4;
    private int _numPlayers = 2;
    
    
    private void Awake() {
        _subtractButton.onClick.AddListener(SubtractButton_OnClick);
        _addButton.onClick.AddListener(AddButton_OnClick);
        _startButton.onClick.AddListener(StartButton_OnClick);
        _backButton.onClick.AddListener(BackButton_OnClick);
        
        _numPlayersText.text = _numPlayers.ToString();
    }
    
    private void SubtractButton_OnClick() {
        _numPlayers--;
        if (_numPlayers <= MIN_PLAYERS) {
            _numPlayers = MIN_PLAYERS;
            DisableButton(_subtractButton);
        }
        else {
            EnableButton(_addButton);
        }
        
        _numPlayersText.text = _numPlayers.ToString();
    }
    
    private void AddButton_OnClick() {
        _numPlayers++;
        if (_numPlayers >= MAX_PLAYERS) {
            _numPlayers = MAX_PLAYERS;
            DisableButton(_addButton);
        }
        else {
            EnableButton(_subtractButton);
        }
        
        _numPlayersText.text = _numPlayers.ToString();
    }
    
    private void StartButton_OnClick() {
        // SceneLoader.Load(SceneLoader.Scene.GameScene);
        // load it with extra data like num players and game mode

        // RelayServerData relayServerData = new RelayServerData();
        
        // NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);
        UnityTransport unityTransport = NetworkManager.Singleton.GetComponent<UnityTransport>();
        unityTransport.SetConnectionData("127.0.0.1", 7777);
        unityTransport.UseEncryption = false;
        
        ConnectionHandler.Instance.StartHost();
        SceneLoader.LoadLocalMultiplayer(_numPlayers);
    }
    
    private void BackButton_OnClick() {
        Hide();
    }

    public void Show() {
        _UIContainer.SetActive(true);
    }
    
    public void Hide() {
        _UIContainer.SetActive(false);
    }
    
    private void EnableButton(Button button) {
        button.interactable = true;
    }
    
    private void DisableButton(Button button) {
        button.interactable = false;
    }
}

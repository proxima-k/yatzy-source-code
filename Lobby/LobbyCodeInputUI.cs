using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

public class LobbyCodeInputUI : MonoBehaviour {

    public event EventHandler<OnLobbyCodeSubmittedEventArgs> OnLobbyCodeSubmitted;
    public class OnLobbyCodeSubmittedEventArgs : EventArgs {
        public string LobbyCode;
        public OnLobbyCodeSubmittedEventArgs(string lobbyCode) {
            LobbyCode = lobbyCode;
        }
    }
    
    public event EventHandler OnCancelButtonClicked;
    
    [SerializeField] private TMP_InputField _lobbyCodeInputField;
    [SerializeField] private RelayHandler relayHandler;
    [SerializeField] private Transform _container;
    
    [SerializeField] private Button _joinButton;
    [SerializeField] private Button _cancelButton;
    
    [SerializeField] private TextMeshProUGUI _instructionText;
    [SerializeField] private Color _normalColor;
    [SerializeField] private Color _errorColor;
    
    private void Awake() {
        _lobbyCodeInputField.onSubmit.AddListener(InputField_OnSubmit);
        
        _joinButton.onClick.AddListener(JoinButton_OnClick);
        _cancelButton.onClick.AddListener(CancelButton_OnClicked);
    }
    
    private void InputField_OnSubmit(string inputFieldString) {
        
        if (string.IsNullOrEmpty(inputFieldString)) {
            UpdateInstructionText("No relay code entered.", true);
            
            Debug.Log("No relay code entered.");
            return;
        }
        
        // Debug.Log($"Joining lobby with code: {inputFieldString}");
        // _testRelay.JoinRelay(inputFieldString);
        
        OnLobbyCodeSubmitted?.Invoke(this, new OnLobbyCodeSubmittedEventArgs(inputFieldString));
        // _lobbyCodeInputField.text = "";
    }

    private void JoinButton_OnClick() {
        InputField_OnSubmit(_lobbyCodeInputField.text);
    }
    
    private void CancelButton_OnClicked() {
        OnCancelButtonClicked?.Invoke(this, EventArgs.Empty);
    }
    
    public void Show() {
        _container.gameObject.SetActive(true);
        UpdateInstructionText("Get code from a host", false);
        _lobbyCodeInputField.text = "";
    }
    
    public void Hide() {
        _container.gameObject.SetActive(false);
    }

    public void UpdateInstructionText(string text, bool isError) {
        _instructionText.text = text;

        _instructionText.color = isError ? _errorColor : _normalColor;
    }
}

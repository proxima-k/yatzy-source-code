using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ConnectionFailedUI : MonoBehaviour {
    
    [SerializeField] private Transform _connectionFailedUI;
    [SerializeField] private TextMeshProUGUI _connectionFailedText;
    [SerializeField] private Button _closeUIButton;

    private void Awake() {
        _closeUIButton.onClick.AddListener(Hide);
        Hide();
    }

    public void Show(string message) {
        _connectionFailedUI.gameObject.SetActive(true);
        _connectionFailedText.text = message;
    }
    
    public void Hide() {
        _connectionFailedUI.gameObject.SetActive(false);
    }
}

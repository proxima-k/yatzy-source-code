using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenuUI : MonoBehaviour {

    public event EventHandler OnExit;
    
    [SerializeField] private Transform _settingsUITransform;
    [SerializeField] private Button _exitButton;

    private void Awake() {
        // Show();
        _exitButton.onClick.AddListener(ExitButton_OnClick);
    }



    private void ExitButton_OnClick() {
        OnExit?.Invoke(this, EventArgs.Empty);
    }

    public void Show() {
        _settingsUITransform.gameObject.SetActive(true);
    }

    public void Hide() {
        _settingsUITransform.gameObject.SetActive(false);
    }
}

using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

// handles the score option selection for a rule
public class RuleScoreOptionUI : MonoBehaviour {
    
    // events
    public event EventHandler<OnScoreButtonClickedEventArgs> OnScoreButtonClicked;
    public class OnScoreButtonClickedEventArgs : EventArgs {
        public int RuleIndex;
        public int OptionIndex;
        public OnScoreButtonClickedEventArgs(int ruleIndex, int optionIndex) {
            RuleIndex = ruleIndex;
            OptionIndex = optionIndex;
        }
    }
    
    private ScoreUI _scoreUI;
    private Button _scoreButton;
    
    // for multiple options
    // [SerializeField] private Button _nextOptionButton;
    private int _ruleIndex;
    private List<int> _options = new List<int>();
    private int _currentOptionIndex = 0;
    
    
    public void Initialize(int ruleIndex, ScoreUI scoreUI, Button scoreButton) {
        _ruleIndex = ruleIndex;
        _scoreUI = scoreUI;
        _scoreButton = scoreButton;

        _scoreButton.onClick.AddListener(Button_OnClick);
        DisableButton();
    }

    private void OnEnable() {
        if (_scoreButton == null) {
            // Debug.LogWarning("Score button is null.");
            return;
        }
        _scoreButton.onClick.AddListener(Button_OnClick);
        // _nextOptionButton.onClick.AddListener(NextOptionButton_OnClick);
    }
    
    private void OnDisable() {
        _scoreButton.onClick.RemoveListener(Button_OnClick);
        // _nextOptionButton.onClick.RemoveListener(NextOptionButton_OnClick);
    }
    
    public void ShowOptions(List<int> options) {
        // enables the button navigation if there's more than one option
        // Debug.Log($"{_ruleNameText.text} has {scores.Count} options.");
        _options = options;

        if (options.Count < 1) {
            return;
        }
        
        EnableButton();
        

        int largestScore = 0;
        for (var i = 0; i < options.Count; i++) {
            
            var option = options[i];
            if (option > largestScore) {
                largestScore = option;
                
                _currentOptionIndex = i;
            }
        }

        if (options[_currentOptionIndex] != 0) {
            _scoreUI.UpdateText(largestScore.ToString());
            return;
        }
        
        _scoreUI.UpdateText("");
    }
    
    private void Button_OnClick() {
        if (_options.Count <= 0) {
            Debug.LogWarning("No options available.");
            Debug.Log(_ruleIndex);
            return;
        }
        
        _scoreUI.UpdateText(_options[_currentOptionIndex].ToString());
        
        OnScoreButtonClicked?.Invoke(this, new OnScoreButtonClickedEventArgs(_ruleIndex, _currentOptionIndex));
    }

    // private void NextOptionButton_OnClick() {
    //     _currentOptionIndex++;
    //     if (_currentOptionIndex >= _options.Count) {
    //         _currentOptionIndex = 0;
    //     }
    //     _scoreUI.UpdateScore(_options[_currentOptionIndex]);
    // }


    public void EnableButton() {
        _scoreButton.enabled = true;
        _scoreUI.ChangeImageColor(Color.white);
        _scoreUI.ChangeTextColor(Color.black);
    }
    
    public void DisableButton() {
        // _nextOptionButton.gameObject.SetActive(false);
        _options.Clear();
        _currentOptionIndex = 0;
        
        _scoreButton.enabled = false;
        
        _scoreUI.ResetColor();
    }
    
    public void ClearUI() {
        DisableButton();
        _scoreUI.UpdateText("");
    }
    
    public void DisplayScore(int score) {
        _scoreUI.UpdateText(score.ToString());
    }
}

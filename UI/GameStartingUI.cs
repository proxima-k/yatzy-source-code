using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameStartingUI : MonoBehaviour {

    [SerializeField] private GameObject _UIContainer;
    [SerializeField] private TextMeshProUGUI _gameStartingText;
    
    private Coroutine _gameStartingTextCoroutine;
    private float _waitTime = 0.2f;
    
    private void Start() {
        GameHandler.Instance.OnGameStart += GameHandler_OnGameStart;
        
        Show();
    }

    private void GameHandler_OnGameStart(object sender, System.EventArgs e) {
        Hide();
    }

    private IEnumerator GameStartingText() {
        while (true) {
            _gameStartingText.text = "Game Starting";
            yield return new WaitForSeconds(_waitTime);
            _gameStartingText.text = "Game Starting.";
            yield return new WaitForSeconds(_waitTime);
            _gameStartingText.text = "Game Starting..";
            yield return new WaitForSeconds(_waitTime);
            _gameStartingText.text = "Game Starting...";
            yield return new WaitForSeconds(_waitTime);
        }
    }

    private void Show() {
        _UIContainer.SetActive(true);
        
        if (_gameStartingTextCoroutine != null)
            StopCoroutine(_gameStartingTextCoroutine);

        _gameStartingTextCoroutine = StartCoroutine(GameStartingText());
    }

    private void Hide() {
        _UIContainer.SetActive(false);
        
        if (_gameStartingTextCoroutine != null)
            StopCoroutine(_gameStartingTextCoroutine);
    }
}

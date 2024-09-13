using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// plain ui script that handles displaying a score for a cell
public class ScoreUI : MonoBehaviour {
    public Image ScoreImage => _scoreImage;
    
    [SerializeField] private TextMeshProUGUI _scoreText;
    [SerializeField] private Image _scoreImage;
    private Color _defaultTextColor;
    private Color _defaultImageColor;

    private bool _isDefaultImageActive;
    
    private Coroutine _flashScoreCoroutine;
    
    private void Awake() {
        _scoreText.raycastTarget = false;
        
        _scoreText.text = "";
        
        _defaultTextColor = _scoreText.color;
        _defaultImageColor = _scoreImage.color;
    }

    private void Start() {
        _isDefaultImageActive = _scoreImage.enabled;
    }

    public void UpdateText(string text) {
        _scoreText.text = text;
    }
    
    public void UpdateText(int text) {
        _scoreText.text = text.ToString();
    }
    
    public void ChangeTextColor(Color color) {
        _scoreText.color = color;
    }
    
    public void ChangeImageColor(Color color) {
        _scoreImage.color = color;
    }
    
    public void ResetColor() {
        _scoreText.color = _defaultTextColor;
        _scoreImage.color = _defaultImageColor;
    }
    
    public void FlashScore(int score, Color color, float duration = 4f) {
        if (_flashScoreCoroutine != null) {
            StopCoroutine(_flashScoreCoroutine);
        }
        _flashScoreCoroutine = StartCoroutine(FlashScoreRoutine(score, color, duration));
    }
    
    public void Highlight(Color color) {
        if (_flashScoreCoroutine != null) {
            StopCoroutine(_flashScoreCoroutine);
        }
        _flashScoreCoroutine = StartCoroutine(HighlightRoutine(color));
    }
    
    private IEnumerator FlashScoreRoutine(int score, Color color, float highlightDuration) {

        StartCoroutine(HighlightRoutine(color));
        
        // pause for a bit
        yield return new WaitForSeconds(highlightDuration);
        
        // slowly change color of text to default
        StartCoroutine(UnhighlightRoutine());
        
    }
    
    private IEnumerator HighlightRoutine(Color backgroundColor) {
        float speed = 0.5f;
        
        if (!_isDefaultImageActive) {
            _scoreImage.enabled = true;
        }
        
        // slowly change color of text to black
        // slowly change color of image to player color
        float tValue = 0;
        while (tValue < 1) {
            _scoreText.color = Color.Lerp(_scoreText.color, Color.black, tValue);
            _scoreImage.color = Color.Lerp(_scoreImage.color, backgroundColor, tValue);
            
            tValue += Time.deltaTime * speed;
            yield return null;
        }
    }
    
    private IEnumerator UnhighlightRoutine() {
        float speed = 0.5f;
        
        // slowly change color of text to black
        // slowly change color of image to player color
        Color defaultImageColor = _defaultImageColor;
        
        if (!_isDefaultImageActive) {
            defaultImageColor = Color.clear;
        }
        
        float tValue = 0;
        while (tValue < 1) {
            _scoreText.color = Color.Lerp(_scoreText.color, _defaultTextColor, tValue);
            _scoreImage.color = Color.Lerp(_scoreImage.color, defaultImageColor, tValue);
            
            tValue += Time.deltaTime * speed;
            yield return null;
        }
        
        ResetColor();
        
        if (!_isDefaultImageActive) {
            _scoreImage.enabled = false;
        }
        yield return null;
    }
}

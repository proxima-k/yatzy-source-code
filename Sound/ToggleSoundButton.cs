using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleSoundButton : MonoBehaviour {
    [SerializeField] private Button _button;
    
    [SerializeField] private Image _image;
    [SerializeField] private Sprite _soundOnSprite;
    [SerializeField] private Sprite _soundOffSprite;
    
    
    private void Start() {
        _button.onClick.AddListener(ToggleSound);
        
        if (SoundHandler.Instance == null)
            return;
        
        UpdateImage();
    }
    
    private void ToggleSound() {
        if (SoundHandler.Instance == null)
            return;
        SoundHandler.Instance.ToggleMute();
        
        UpdateImage();
    }
    
    private void UpdateImage() {
        
        if (SoundHandler.Instance.IsMuted) {
            _image.sprite = _soundOffSprite;
        } else {
            _image.sprite = _soundOnSprite;
        }
    }
}

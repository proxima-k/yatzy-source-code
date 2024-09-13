using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonSound : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler {
    private Button _button;
    
    private void Start() {
        _button = GetComponent<Button>();
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (SoundHandler.Instance == null)
            return;
        
        if (HasButton() && (!_button.interactable || !_button.enabled))
            return;

        SoundHandler.Instance.PlaySound(SoundHandler.Instance.AudioClipRefsSo.ButtonHover, Camera.main.transform.position, 0.1f);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (SoundHandler.Instance == null)
            return;

        
        if (HasButton() && (!_button.interactable || !_button.enabled))
            return;
        
        SoundHandler.Instance.PlaySound(SoundHandler.Instance.AudioClipRefsSo.ButtonClick, Camera.main.transform.position, 0.1f);
    }
    
    private bool HasButton() {
        return _button != null;
    }
}

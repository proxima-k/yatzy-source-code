using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LobbyCodeUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler {
    [SerializeField] private TextMeshProUGUI _lobbyCodeText;
    [SerializeField] private Color _textNormalColor;
    [SerializeField] private Color _textHoverColor;
    [SerializeField] private Color _textClickColor;
    
    private bool _isClickingOnText = false;


    public void OnPointerEnter(PointerEventData eventData) {
        _lobbyCodeText.color = _textHoverColor;
    }
    
    public void OnPointerExit(PointerEventData eventData) {
        // if pointer is still down, don't change color
        if (_isClickingOnText) {
            return;
        }
        
        _lobbyCodeText.color = _textNormalColor;
    }
    
    public void OnPointerDown(PointerEventData eventData) {
        _isClickingOnText = true;
        
        _lobbyCodeText.color = _textClickColor;
    }
    
    public void OnPointerUp(PointerEventData eventData) {
        _isClickingOnText = false;
        
        if (eventData.hovered.Count > 0) {
            GUIUtility.systemCopyBuffer = _lobbyCodeText.text;
    
            _lobbyCodeText.color = _textHoverColor;
            return;
        }
        _lobbyCodeText.color = _textNormalColor;
    }
    
    public void SetLobbyCode(string lobbyCode) {
        _lobbyCodeText.text = lobbyCode;
        
        LayoutRebuilder.ForceRebuildLayoutImmediate(gameObject.GetComponent<RectTransform>());
    }
}

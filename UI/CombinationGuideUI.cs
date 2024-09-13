using UnityEngine;
using UnityEngine.UI;

public class CombinationGuideUI : MonoBehaviour {
    [SerializeField] private Button _openButton;
    [SerializeField] private Button _closeButton;
    [SerializeField] private GameObject _UIContainer;
    
    private void Start() {
        _openButton.onClick.AddListener(Show);
        _closeButton.onClick.AddListener(Hide);
        
        Hide();
    }
    
    private void Hide() {
        _UIContainer.SetActive(false);
    }
    
    private void Show() {
        _UIContainer.SetActive(true);
    }
}

using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DiceNameTagUI : MonoBehaviour {
    
    [SerializeField] private Vector3 _relativePosition = Vector3.zero;
    [SerializeField] private TextMeshPro _nameText;
    [SerializeField] private RectTransform _backgroundRectTf;
    [SerializeField] private Image _playerColorImage;
    
    private Transform rootTf;
    
    private void Awake() {
        rootTf = transform.root;
        _relativePosition = transform.position - rootTf.position;
    }
    
    private void LateUpdate() {
        transform.position = rootTf.position + _relativePosition;
     
        transform.forward = Camera.main.transform.forward;
        // Vector3 directionFromCamera = transform.position - Camera.main.transform.position;
        // transform.LookAt(transform.position + directionFromCamera);
    }
    
    public void SetName(string name) {
        _nameText.text = name;
        LayoutRebuilder.ForceRebuildLayoutImmediate(_backgroundRectTf);
    }
    
    public void SetColor(Color color) {
        _playerColorImage.color = color;
    }
}

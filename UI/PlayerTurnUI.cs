using System;
using System.Collections;
using TMPro;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlayerTurnUI : NetworkBehaviour {
    [SerializeField] private TextMeshProUGUI _playerTurnText;
    [SerializeField] private Image _playerColorImage;
    private RectTransform _rectTransform;

    private float _rotateAmount = 5f;
    
    private Vector2 _defaultPivot;
    
    private Coroutine _juiceUpUICoroutine;

    private void Awake() {
        _rectTransform = GetComponent<RectTransform>();
        
        _defaultPivot = _rectTransform.pivot;
    }

    private void Start() {
        // if (!IsServer)
            // return;
        
        GameHandler.Instance.OnPlayerStartTurn += GameHandler_OnPlayerStartTurn;
        // GameHandler.Instance.OnPlayerEndTurn += GameHandler_OnPlayerEndTurn;
        
        Hide();
    }

    private void GameHandler_OnPlayerStartTurn(object sender, GameHandler.OnPlayerStartTurnEventArgs e) {
        // UpdateUIClientRpc(e.Player.PlayerColor, e.Player.Name);
        Player player = GameHandler.Instance.Players[e.PlayerIndex];
        
        UpdateUIClientRpc(player.PlayerColor, player.Name);
    }
    
    [ClientRpc]
    private void UpdateUIClientRpc(Color playerColor, string playerName) {
        if (gameObject.activeSelf == false)
            Show();
        
        _playerColorImage.color = playerColor;
        _playerTurnText.text = $"{playerName}'s Turn";
        
        LayoutRebuilder.ForceRebuildLayoutImmediate(_rectTransform);
        
        if (_juiceUpUICoroutine != null)
            return;
        
        _juiceUpUICoroutine = StartCoroutine(JuiceUpUI());
    }
    

    private IEnumerator JuiceUpUI() {
        
        yield return null;
        
        _rectTransform.SetPivot(new Vector2(0.5f, 0.5f));
        
        // shake rotate for 4 times
        for (int i = 0; i < 5; i++) {
            
            _rectTransform.localRotation = Quaternion.Euler(0, 0, _rotateAmount);
            // _rectTransform.Rotate(0, 0, _rotateAmount);
            yield return new WaitForSeconds(0.1f);
            
            _rectTransform.localRotation = Quaternion.Euler(0, 0, -_rotateAmount);
            // _rectTransform.Rotate(0, 0, -_rotateAmount);
            yield return new WaitForSeconds(0.1f);
            
        }
        
        _rectTransform.localRotation = Quaternion.identity;
        
        yield return null;
        
        _rectTransform.SetPivot(_defaultPivot);
        
        yield return null;
        
        _juiceUpUICoroutine = null;
    }
    
    
    private void Show() {
        gameObject.SetActive(true);
    }
    
    private void Hide() {
        gameObject.SetActive(false);
    }
}

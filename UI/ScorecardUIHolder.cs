using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class ScorecardUIHolder : MonoBehaviour {
    
    [SerializeField] private Button _toggleScorecardButtonPrefab;
    [SerializeField] private ScorecardUICreator _scorecardUICreator;
    
    [Range(0f,100f)]
    // [SerializeField] private float _percentage = 5f;
    [SerializeField] private float _moveSpeed = 3f;
    // maybe instead of using percentage, use a pixel value?
    
    private RectTransform _holderRectTransform;
    
    private Button _toggleFocusButton;
    private Image _arrowImage;
    private Coroutine _arrowRotateCoroutine;
    
    public ScorecardUI ScorecardUI => _scorecardUI;
    private ScorecardUI _scorecardUI;
    private RectTransform _scorecardUITransform;
    private Rect _holderRect;
    private Image _blockRaycastImage;
    
    private Vector2 _defaultPos;
    private bool _isScorecardFocused = false;
    
    private Coroutine _moveScorecardCoroutine;
    
    public void Initialize() {
        _scorecardUI = _scorecardUICreator.CreateScorecardUI(GameHandler.Instance.Players,
            GameHandler.Instance.GameSettings.RuleSections);
        
        // todo: perhaps this function can be done within the scorecard creation 
        _scorecardUI.InitializePlayers(GameHandler.Instance.Players);
        
        _scorecardUI.transform.SetParent(transform);
        
        _scorecardUITransform = _scorecardUI.GetComponent<RectTransform>();
        
        LayoutRebuilder.ForceRebuildLayoutImmediate(_scorecardUITransform);
        // Debug.Log(_scorecardUITransform.rect);
        
        
        // block raycast instantiation
        RectTransform blockRaycastRectTf = new GameObject("BlockRaycast", typeof(RectTransform)).GetComponent<RectTransform>();
        blockRaycastRectTf.transform.SetParent(transform);
        
        blockRaycastRectTf.gameObject.AddComponent<LayoutElement>().ignoreLayout = true;
        _blockRaycastImage = blockRaycastRectTf.gameObject.AddComponent<Image>();
        _blockRaycastImage.color = new Color(0,0,0,0);
        _blockRaycastImage.enabled = false;
        
        blockRaycastRectTf.anchorMin = Vector2.zero;
        blockRaycastRectTf.anchorMax = Vector2.one;
        blockRaycastRectTf.offsetMin = Vector2.zero;
        blockRaycastRectTf.offsetMax = Vector2.zero;
        blockRaycastRectTf.localScale = Vector3.one;
        
        
        // button instantiation
        _toggleFocusButton = Instantiate(_toggleScorecardButtonPrefab, transform);
        _toggleFocusButton.onClick.AddListener(ToggleMoveScorecard);
        _arrowImage = _toggleFocusButton.transform.GetChild(0).GetComponent<Image>();
        
        
        _holderRectTransform = GetComponent<RectTransform>();
        
        SetupPlacement();
    }
    
    public void InitializeInput(Player localPlayer) {
        _scorecardUI.InitializeInputUI(localPlayer, GameHandler.Instance.Players.IndexOf(localPlayer));
    }
    
    
    private void SetupPlacement() {
        // add a button to the scorecard that will toggle the scorecard to move up and down
        
        // _scorecardUITransform.anchorMax = Vector2.right;
        // _scorecardUITransform.anchorMin = Vector2.right;

        LayoutRebuilder.ForceRebuildLayoutImmediate(_holderRectTransform);
        
        _holderRect = _holderRectTransform.rect;
        
        float range = Screen.width - _holderRect.width;

        Vector2 targetPos = new Vector2(_holderRectTransform.anchoredPosition.x, _holderRect.height / 2);
        // Vector2 targetPos = new Vector2(-(_scorecardRect.width/2 + (range * _percentage/100f)), _scorecardRect.height/2);
        // Vector2 targetPos = new Vector2(-_scorecardRect.width/2, _scorecardRect.height/2);
        Debug.Log(targetPos);

        _holderRectTransform.anchoredPosition = targetPos;
        _defaultPos = targetPos;
        
        _isScorecardFocused = true;
    }
    
    
    public void ToggleMoveScorecard() {
        if (_isScorecardFocused) {
            MoveScorecardOut();
        }
        else {
            MoveScorecardIn();
        }
    }
    
    public void MoveScorecardOut() {
        if (_holderRectTransform == null)
            return;
        
        if (!_isScorecardFocused)
            return;
        
        float movePercentage = 80/100f;
        Vector2 holderPos = _holderRectTransform.anchoredPosition;
        Vector2 targetPos = new Vector2(holderPos.x, _defaultPos.y - (_holderRect.height * movePercentage) );

        // todo: move down using percentage
        MoveToPosition(_holderRectTransform, targetPos);
        RotateArrowImage(0);
        
        _isScorecardFocused = false;
        _blockRaycastImage.enabled = true;
    }

    public void MoveScorecardIn() {
        if (_holderRectTransform == null)
            return;
        
        if (_isScorecardFocused)
            return;
        
        MoveToPosition(_holderRectTransform, _defaultPos);
        RotateArrowImage(180);
        
        _isScorecardFocused = true;
        _blockRaycastImage.enabled = false;
        // UnhighlightScorecard();
    }
    
    
    public void MoveToPosition(RectTransform rectTransform, Vector2 targetPos) {
        if (_moveScorecardCoroutine != null) {
            StopCoroutine(_moveScorecardCoroutine);
        }
        _moveScorecardCoroutine = StartCoroutine(LerpToPosition(rectTransform, targetPos));
    }
    
    private IEnumerator LerpToPosition(RectTransform rectTransform, Vector2 targetPos) {
        
        while (Vector2.Distance(rectTransform.anchoredPosition, targetPos) > 1f) {
            
            rectTransform.anchoredPosition = Vector2.Lerp(rectTransform.anchoredPosition, targetPos, 0.025f * _moveSpeed);
            rectTransform.ForceUpdateRectTransforms();
            // Debug.Log(rectTransform.anchoredPosition);
            
            yield return null;
        }

        rectTransform.anchoredPosition = targetPos;
    }
    
    private void RotateArrowImage(float zRotation) {
        if (_arrowRotateCoroutine != null) {
            StopCoroutine(_arrowRotateCoroutine);
        }
        _arrowRotateCoroutine = StartCoroutine(LerpRotateArrow(zRotation));
    }
    
    private IEnumerator LerpRotateArrow(float zRotation) {
        float speed = 1f;
        
        float tValue = 0;
        while (tValue < 1) {
            _arrowImage.rectTransform.rotation = Quaternion.Lerp(_arrowImage.rectTransform.rotation, Quaternion.Euler(0,0,zRotation), tValue);
            
            tValue += Time.deltaTime * speed;
            yield return null;
        }
    }
    
    public void ShowToggleScorecardButton() {
        _toggleFocusButton.gameObject.SetActive(true);
    }
    
    public void HideToggleScorecardButton() {
        _toggleFocusButton.gameObject.SetActive(false);
    }
}

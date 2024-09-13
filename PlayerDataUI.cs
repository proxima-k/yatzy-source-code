using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerDataUI : MonoBehaviour {
    [SerializeField] private GameObject _UIContainer;
    [SerializeField] private Image _playerColorImage;
    [SerializeField] private TextMeshProUGUI _playerNameText;
    
    
    // only show if it's online multiplayer
    private void Start() {
        if (SceneLoader.CurrentGameMode == SceneLoader.GameMode.OnlineMultiplayer) {
            Show();
            UpdateText();
        }
        else {
            Hide();
        }
    }
    
    private void Show() {
        _UIContainer.SetActive(true);
    }
    
    private void Hide() {
        _UIContainer.SetActive(false);
    }

    private void UpdateText() {
        
        PlayerData localPlayer = PlayerDataHandler.Instance.GetLocalPlayerData();
        
        _playerColorImage.color = localPlayer.PlayerColor;
        _playerNameText.text = $"{localPlayer.PlayerName} (you)";
        
        LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
    }
}

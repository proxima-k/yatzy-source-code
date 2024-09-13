using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

// responsible for initializing the scorecard UI with required references
public class ScorecardUI : MonoBehaviour {

    public List<PlayerScoresColumnUI> PlayerScoresColumnUIList => _playerScoresColumnUIList;
    [SerializeField] List<PlayerScoresColumnUI> _playerScoresColumnUIList = new List<PlayerScoresColumnUI>();
    
    public void Initialize(List<PlayerScoresColumnUI> playerScorecardUIList) {
        _playerScoresColumnUIList = playerScorecardUIList;
    }

    public void InitializePlayers(List<Player> players) {
        for (int i = 0; i < players.Count; i++) {
            _playerScoresColumnUIList[i].AssignPlayer(players[i]);
        }
        
        GameHandler.Instance.OnPlayerDisconnected += GameHandler_OnPlayerDisconnected;
    }
    
    public void InitializeInputUI(Player player, int playerIndex) {
        PlayerScoreInputUI inputUI = _playerScoresColumnUIList[playerIndex].AddComponent<PlayerScoreInputUI>();
        inputUI.InitializePlayer(player, playerIndex);
    }
    
    private void GameHandler_OnPlayerDisconnected(object sender, GameHandler.OnPlayerDisconnectedEventArgs e) {
        // potential indexing error
        if (GameHandler.Instance.IsGameEnded)
            return;
        
        Destroy(_playerScoresColumnUIList[e.PlayerIndex].gameObject);
        _playerScoresColumnUIList.RemoveAt(e.PlayerIndex);
    }

    public void ShowPlayerTotalScores() {
        StartCoroutine(ShowPlayerTotalScoresRoutine());
    }
    
    private IEnumerator ShowPlayerTotalScoresRoutine() {
        
        for (int i = 0; i < _playerScoresColumnUIList.Count; i++) {
            _playerScoresColumnUIList[i].ShowFinalScore();
            yield return new WaitForSeconds(1.5f);
        }
    }
}


// UI architecture:
// Scorecard UI
    // List of PlayerScoresColumnUI
        // PlayerScoresColumnUI
            // List of ScoreUI
                // ScoreUI
            // List of RuleSectionScoreUI
                // RuleSectionScoreUI
                    // ScoreUI
                    // ScoreUI
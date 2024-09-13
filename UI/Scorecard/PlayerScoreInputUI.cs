using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

// todo: move this to be together with PlayerScorecardUI so that there can be multiple within the same canvas
// responsible for getting options and displaying them in a score column
// handles a list of RuleScoreOptionUI
public class PlayerScoreInputUI : MonoBehaviour {
    
    private PlayerScorecard _playerScorecard;
    private List<RuleScoreOptionUI> _ruleScoreInputUIList = new List<RuleScoreOptionUI>();
    
    public void InitializePlayer(Player player, int playerIndex) {
        Button scoreButtonPrefab = GameHandler.Instance.GameSettings.ScoreButtonPrefab;
        
        PlayerScoresColumnUI playerScoresColumnUI = GetComponentInChildren<PlayerScoresColumnUI>();
        List<ScoreUI> ruleScoreUIList = playerScoresColumnUI.RuleScoreUIList;
        
        _playerScorecard = player.PlayerScorecard;
        player.PlayerScorecard.OnGetOptions += PlayerScorecard_OnGetOptions;
        player.PlayerScorecard.OnClearOptions += PlayerScorecard_OnClearOptions;

        for (var i = 0; i < ruleScoreUIList.Count; i++) {
            var ruleScoreUI = ruleScoreUIList[i];
            // for each input button UI, add a RuleScoreInputUI component   
            RuleScoreOptionUI optionUI = ruleScoreUI.transform.AddComponent<RuleScoreOptionUI>();
            
            // needs to add a button component to the ruleScoreUI
            Button button = optionUI.gameObject.AddComponent<Button>();
            CopyButtonSettings(scoreButtonPrefab, button);
            button.targetGraphic = ruleScoreUI.ScoreImage;

            optionUI.Initialize(i, ruleScoreUI, button);
            
            optionUI.OnScoreButtonClicked += RuleScoreInputUI_OnScoreButtonClicked;
            _ruleScoreInputUIList.Add(optionUI);

            optionUI.gameObject.AddComponent<ButtonSound>();
        }
    }
    
    private void PlayerScorecard_OnGetOptions(object sender, PlayerScorecard.OnGetOptionsEventArgs e) {
        
        for (int i = 0; i < e.RuleScoresOptions.Count; i++) {
            if (e.RuleScoresOptions[i].Count <= 0) {
                continue;
            }
            
            _ruleScoreInputUIList[i].ShowOptions(e.RuleScoresOptions[i]);
        }
    }
    
    private void PlayerScorecard_OnClearOptions(object sender, System.EventArgs e) {
        ClearOptions();
    }
    
    private void RuleScoreInputUI_OnScoreButtonClicked(object sender, RuleScoreOptionUI.OnScoreButtonClickedEventArgs e) {
        
        // handles the input UI
        ClearOptions();
        
        // official score submission
        _playerScorecard.SubmitRuleScoreServerRpc(e.RuleIndex, e.OptionIndex);
    }

    private void ClearOptions() {
        for (var i = 0; i < _playerScorecard.RuleScores.Count; i++) {
            RuleScore ruleScore = _playerScorecard.RuleScores[i];
            
            // _ruleScoreInputUIList[i].DisableButton();
            
            if (!ruleScore.Completed) {
                _ruleScoreInputUIList[i].ClearUI();
            }
        }
    }
    
    private void CopyButtonSettings(Button source, Button target) {
        target.colors = source.colors;
        target.transition = source.transition;
        target.navigation = source.navigation;
        // target.targetGraphic = source.targetGraphic;
        // target.onClick = source.onClick;
    }
}

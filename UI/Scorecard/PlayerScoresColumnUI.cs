using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// responsible for updating scores for a player column in a scorecard
public class PlayerScoresColumnUI : MonoBehaviour {
    public List<ScoreUI> RuleScoreUIList => _ruleScoreUIList;
    public List<RuleSectionScoreUI> RuleSectionScoreUIList => _ruleSectionScoreUIList;
    public ScoreUI TotalScoreUI => _totalScoreUI;
    
    [SerializeField] private Image _playerColorImage;
    [SerializeField] private List<ScoreUI> _ruleScoreUIList;
    [SerializeField] private List<RuleSectionScoreUI> _ruleSectionScoreUIList;
    [SerializeField] private ScoreUI _totalScoreUI;
    
    public Player Player => _player;
    [SerializeField] private Player _player;
    
    private Coroutine _flashColorCoroutine;
    
    public void AssignPlayer(Player player) {
        // Debug.Log(_ruleSectionScoreUIList.Count);

        _playerColorImage.color = player.PlayerColor;
        _player = player;
        
        // there's a chance that this might be copied to the duplicates
        // seems like events won't be duplicated
        player.PlayerScorecard.OnScoreSubmitted += Scorecard_OnScoreSubmitted;
        // player.OnStartTurn += Player_OnStartTurn;
        // player.OnEndTurn += Player_OnEndTurn;
        
        int sectionUIIndex = 0;
        foreach (var ruleSectionScore in player.PlayerScorecard.RuleSectionScoreList) {
            if (!ruleSectionScore.RuleSection.HasBonus)
                continue;
            _ruleSectionScoreUIList[sectionUIIndex].Initialize(ruleSectionScore);
            sectionUIIndex++;
        }
    }

    public void Initialize(Image playerColorImage, List<ScoreUI> ruleScoreUIList, List<RuleSectionScoreUI> ruleSectionUIList, ScoreUI totalScoreUI) {
        _playerColorImage = playerColorImage;
        _ruleScoreUIList = ruleScoreUIList;
        _ruleSectionScoreUIList = ruleSectionUIList;
        _totalScoreUI = totalScoreUI;
    }

    private void Scorecard_OnScoreSubmitted(object sender, PlayerScorecard.OnScoreSubmittedEventArgs e) {
        ScoreUI _scoreUI = _ruleScoreUIList[e.RuleIndex];
        _scoreUI.UpdateText(e.Score.ToString());
        _scoreUI.FlashScore(e.Score, _player.PlayerColor, 3f);
    }

    public void ShowFinalScore() {
        Debug.Log("Test show final score");
        // play animation
        _totalScoreUI.UpdateText(_player.PlayerScorecard.GetTotalScore().ToString());
        _totalScoreUI.Highlight(_player.PlayerColor);
    }
}

[Serializable]
public class RuleSectionScoreUI {
    
    public ScoreUI BonusScoreUI => _bonusScoreText;
    public ScoreUI SumScoreUI => _sumScoreText;
    
    [SerializeField] private ScoreUI _bonusScoreText;
    [SerializeField] private ScoreUI _sumScoreText;
    private RuleSectionScore _ruleSectionScore;

    public RuleSectionScoreUI(ScoreUI sumScoreText, ScoreUI bonusScoreText) {
        _sumScoreText = sumScoreText;
        _bonusScoreText = bonusScoreText;
    }
    
    public void Initialize(RuleSectionScore ruleSectionScore) {
        _ruleSectionScore = ruleSectionScore;
        
        ruleSectionScore.OnSectionCompleted += RuleSectionScore_OnSectionCompleted;
    }

    private void RuleSectionScore_OnSectionCompleted(object sender, RuleSectionScore.OnSectionCompletedEventArgs e) {
        _bonusScoreText.UpdateText(e.SectionBonus.ToString());
        _sumScoreText.UpdateText(e.SectionSum.ToString());
    }
    
    ~RuleSectionScoreUI() {
        // unsubscribe from events
        _ruleSectionScore.OnSectionCompleted -= RuleSectionScore_OnSectionCompleted;
    }
}
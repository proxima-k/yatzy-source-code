using System;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public class RuleSectionScore {
    
    public event EventHandler<OnSectionCompletedEventArgs> OnSectionCompleted;
    public class OnSectionCompletedEventArgs : EventArgs {
        public int SectionSum;
        public int SectionBonus;
        public OnSectionCompletedEventArgs(int sectionSum, int sectionBonus) {
            SectionSum = sectionSum;
            SectionBonus = sectionBonus;
        }
    }
    
    public RuleSection RuleSection => _ruleSection;
    public int SectionScore => _sectionScore;
    public int SectionBonus => _sectionBonus;
    public List<RuleScore> RuleScores => _ruleScores;
    
    private RuleSection _ruleSection;
    private int _sectionRulesCompleted = 0;
    private int _sectionBonus = 0;
    private int _sectionScore = 0;
    
    // todo: keep track of ruleScores?
    private List<RuleScore> _ruleScores = new List<RuleScore>();
    
    public RuleSectionScore(RuleSection ruleSection, List<RuleScore> ruleScores) {
        _ruleSection = ruleSection;
        
        _ruleScores = ruleScores;
        foreach (var ruleScore in ruleScores) {
            ruleScore.OnScoreSubmitted += RuleScore_OnScoreSubmitted;
        }
    }

    private void RuleScore_OnScoreSubmitted(object sender, RuleScore.OnScoreSubmittedEventArgs e) {
        _sectionScore += e.Score;
        
        _sectionRulesCompleted++;
        if (_sectionRulesCompleted < _ruleSection.CombinationRules.Count)
            return;
        
        if (_sectionScore >= _ruleSection.BonusThreshold) 
            _sectionBonus = _ruleSection.BonusPoints;

        Debug.Log($"{_ruleSection.Name} completed.");
        OnSectionCompleted?.Invoke(this, new OnSectionCompletedEventArgs(_sectionScore, _sectionBonus));
    }

    ~RuleSectionScore() {
        foreach (var ruleScore in _ruleScores) {
            ruleScore.OnScoreSubmitted -= RuleScore_OnScoreSubmitted;
        }
    }
}
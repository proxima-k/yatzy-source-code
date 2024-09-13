using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;


public class PlayerScorecard : NetworkBehaviour {
    
    public Player Player => _player;
    private Player _player;
    
    // events
    public event EventHandler<OnScoreSubmittedEventArgs> OnScoreSubmitted;
    public class OnScoreSubmittedEventArgs : EventArgs {
        public int RuleIndex;
        public int Score;
        public OnScoreSubmittedEventArgs(int ruleIndex, int score) {
            RuleIndex = ruleIndex;
            Score = score;
        }
    }
    public event EventHandler<OnGetOptionsEventArgs> OnGetOptions;
    public class OnGetOptionsEventArgs : EventArgs {
        public List<List<int>> RuleScoresOptions;
        public OnGetOptionsEventArgs(List<List<int>> ruleScoresOptions) {
            RuleScoresOptions = ruleScoresOptions;
        }
    }
    public event EventHandler OnClearOptions;
    


    public List<RuleScore> RuleScores => _ruleScores;
    public List<RuleSectionScore> RuleSectionScoreList => ruleSectionScoreList;
    
    [SerializeField] private List<RuleScore> _ruleScores = new List<RuleScore>();
    [SerializeField] private List<RuleSectionScore> ruleSectionScoreList = new List<RuleSectionScore>();
    private int _rulesCompleted = 0;
    private List<List<int>> _ruleScoresOptions = new List<List<int>>();
    
    void Awake() {
        _player = GetComponent<Player>();
        
        // create rule scores and rule section scores
        foreach (var ruleSection in GameHandler.Instance.GameSettings.RuleSections) {
            
            List<RuleScore> ruleScores = new List<RuleScore>();
            foreach (var combinationRule in ruleSection.CombinationRules) {
                ruleScores.Add(new RuleScore(combinationRule));
            }
            _ruleScores.AddRange(ruleScores);
            
            ruleSectionScoreList.Add(new RuleSectionScore(ruleSection, ruleScores));
        }
    }
    
    // needs an input from anywhere
    public void GetOptions() {
        StoreOptions();
        
        OnGetOptions?.Invoke(this, new OnGetOptionsEventArgs(_ruleScoresOptions));
    }
    
    public void ClearOptions() {
        _ruleScoresOptions.Clear();
     
        OnClearOptions?.Invoke(this, EventArgs.Empty);
    }
    
    // can be called anywhere, also from the UI
    // one for server, one for client
    [ServerRpc(RequireOwnership = false)]
    public void SubmitRuleScoreServerRpc(int ruleIndex, int optionIndex) {
        
        // store options on the server
        StoreOptions();
        
        int score = _ruleScoresOptions[ruleIndex][optionIndex];
        _ruleScores[ruleIndex].LockScore(score);
        _rulesCompleted++;
        
        Debug.Log("Server received score submission.");
        Debug.Log($"{_player.Name} Submitted score {score} for rule {_ruleScores[ruleIndex].CombinationRule.RuleName}.");
        
        UpdateScorecardClientRpc(ruleIndex, score);
        
        _player.EndTurnClientRpc();
    }

    [ClientRpc]
    public void UpdateScorecardClientRpc(int ruleIndex, int score) {
        _ruleScores[ruleIndex].LockScore(score);
        OnScoreSubmitted?.Invoke(this, new OnScoreSubmittedEventArgs(ruleIndex, score));
        
        Debug.Log("Test");
        // play animation here
        // when done, notify server to end turn
    }

    private void StoreOptions() {
        List<int> dieValues = DiceRoller.Instance.DieValues;
        
        _ruleScoresOptions.Clear();

        foreach (var ruleScore in _ruleScores) {
            List<int> options = new List<int>();
            
            if (ruleScore.Completed) {
                _ruleScoresOptions.Add(options);
                continue;
            }
            
            if (ruleScore.CombinationRule.TryMatch(dieValues, out List<int> scores)) {
                foreach (int score in scores) {
                    options.Add(score);
                }
            }
            else {
                options.Add(0);
            }
            
            _ruleScoresOptions.Add(options);
        }
    }
    
    
    public bool AllRulesCompleted() {
        return _rulesCompleted == _ruleScores.Count;
    }
    
    public int GetTotalScore() {
        int totalScore = 0;
        
        foreach (var ruleSectionScore in ruleSectionScoreList) {
            totalScore += ruleSectionScore.SectionScore;
            totalScore += ruleSectionScore.SectionBonus;
        }
        
        return totalScore;
    }
}

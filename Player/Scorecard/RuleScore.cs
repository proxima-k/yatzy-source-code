using System;
using UnityEngine;

// this is part of the player scorecard and is used to keep track of the score for each rule
[Serializable]
public class RuleScore {
    public CombinationRule CombinationRule;
    
    public int Score => _score;
    private int _score;
    
    public bool Completed => _completed;
    private bool _completed = false;

    public RuleScore(CombinationRule combinationRule) {
        this.CombinationRule = combinationRule;
        _score = 0;
    }
    
    public void LockScore(int score) {
        if (_completed) {
            Debug.LogWarning("Rule score already completed.");
            return;
        }
        
        _score = score;
        _completed = true;
        
        OnScoreSubmitted?.Invoke(this, new OnScoreSubmittedEventArgs(score));
    }
    
    public event EventHandler<OnScoreSubmittedEventArgs> OnScoreSubmitted;
    public class OnScoreSubmittedEventArgs : EventArgs {
        public int Score;
        public OnScoreSubmittedEventArgs(int score) {
            Score = score;
        }
    }
}
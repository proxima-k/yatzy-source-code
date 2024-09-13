using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "GameSettings", menuName = "ScriptableObjects/GameSettings", order = 0)]
public class GameSettings : ScriptableObject {
    public int NumberOfPlayers = 2;
    public List<Color> PlayerPresetColors = new List<Color>();
    
    public List<RuleSection> RuleSections => _ruleSections;
    [SerializeField] private List<RuleSection> _ruleSections = new List<RuleSection>();
    
    public List<CombinationRule> AllCombinationRules { 
        get {
            List<CombinationRule> combinationRules = new List<CombinationRule>();
            foreach (var ruleSection in RuleSections) {
                combinationRules.AddRange(ruleSection.CombinationRules);
            }
            return combinationRules;
        } 
    }
    
    public Button ScoreButtonPrefab => _scoreButtonPrefab;
    [SerializeField] private Button _scoreButtonPrefab;
}

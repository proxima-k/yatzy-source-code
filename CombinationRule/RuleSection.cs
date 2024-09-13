using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class RuleSection {
    public string Name = "section";
    
    public List<CombinationRule> CombinationRules => _combinationRules;
    public bool HasBonus => _hasBonus;
    public int BonusThreshold => _bonusThreshold;
    public int BonusPoints => _bonusPoints;
    
    [SerializeField] private bool _hasBonus = false;
    [SerializeField] private int _bonusThreshold = 50;
    [SerializeField] private int _bonusPoints = 50;
    [SerializeField] private List<CombinationRule> _combinationRules;
}

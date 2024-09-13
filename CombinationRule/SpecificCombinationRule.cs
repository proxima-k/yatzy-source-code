using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "CombinationRule", menuName = "ScriptableObjects/CombinationRule/SpecificCombination", order = 1)]
public class SpecificCombinationRule : CombinationRule {

    public bool SumOfAllValues;
    
    public bool ValueMatters;
    
    // Variables
    public List<Type> RuleTypes => _ruleTypes;
    [SerializeField] private List<Type> _ruleTypes;

    public override bool TryMatch(List<int> dieValues, out List<int> scores) {
        
        // if value matters, then just check if the die values match the rule values
        scores = new List<int>();
        
        if (ValueMatters) {
            List<int> ruleValues = GetRuleValues(dieValues.Distinct().ToList());
            
            if (TryMatchValues(ruleValues, dieValues, out int score)) {
                scores.Add(score);
                return true;
            }
        }
        else {
            if (TryMatchWithPermutation(dieValues, out scores)) {
                return true;
            }
        }
        
        return false;
    }

    // Gets the unique types in the given die list
    public List<Type> GetDieTypes(List<int> dieValues) {
        List<Type> dieTypes = new List<Type>();
        dieTypes = (
            from dieValue in dieValues
            group dieValue by dieValue
            into dieType
            select new Type {
                Value = dieType.Key,
                Quantity = dieType.Count()
            }
        ).ToList();

        return dieTypes;
    }

    // Gets a list of die values based on the given die list
    public List<int> GetRuleValues(List<int> uniqueDieValues) {
        List<int> ruleValues = new List<int>();

        for (int i=0; i < _ruleTypes.Count; i++) {
            for (int j=0; j < _ruleTypes[i].Quantity; j++) {
                if (ValueMatters)
                    ruleValues.Add(_ruleTypes[i].Value);
                else
                    ruleValues.Add(uniqueDieValues[i]);
            }
        }

        return ruleValues;
    }
    
    private bool TryMatchWithPermutation(List<int> dieValues, out List<int> scores) {

        List<Type> dieTypes = GetDieTypes(dieValues);
        List<int> uniqueScores = new List<int>();
        scores = uniqueScores;
        
        if (_ruleTypes.Count > dieTypes.Count) {
            Debug.LogWarning("Rule requires more types from die!");
            
            // score = 0;
            return false;
        }
        
        int matchScore = 0;
        
        void SwapItem(int a, int b) {
            (dieTypes[a], dieTypes[b]) = (dieTypes[b], dieTypes[a]);
        }
        
        // Recursive function to create permutations of the unique die values
        void Process(int itemCount) {

            if (itemCount == 1) {
                // reaches here when the the unique die values have been permuted
                
                // where matching starts
                if (TryMatchTypes(_ruleTypes, dieTypes, out matchScore)) {
                    if (!uniqueScores.Contains(matchScore))
                        uniqueScores.Add(matchScore);
                }
            }
            else {
                // items is 2 or more
                for (int i = 0; i < itemCount; i++) {
                    Process(itemCount - 1);
                    
                    if (itemCount % 2 == 0) {
                        SwapItem(i, itemCount - 1);
                    }
                    else {
                        SwapItem(0, itemCount - 1);
                    }
                }
            }

        }

        Process(dieTypes.Count);
        return scores.Count > 0;
    }
    
    private bool TryMatchValues(List<int> ruleValues, List<int> dieValues, out int score) {
        List<int> combinationDuplicate = new List<int>();
        combinationDuplicate.AddRange(ruleValues);
        score = 0;
        
        foreach (var value in dieValues) {
            
            if (combinationDuplicate.Remove(value)) {
                score += value;
                continue;
            }
            
            // if fail to remove and still want to add values that doesn't match
            if (SumOfAllValues) {
                score += value;
            }
            
        }
        
        if (combinationDuplicate.Count == 0) {
            if (Points > 0) {
                score = Points;
            }
            // Debug.Log($"Rule values left: {combinationDuplicate.Count}");
            return true;
        }
        return false;
    }
    
    private bool TryMatchTypes(List<Type> ruleTypes, List<Type> dieTypes, out int score) {
        
        score = 0;

        for (int i = 0; i < ruleTypes.Count; i++) {
            
            if (dieTypes[i].Quantity >= ruleTypes[i].Quantity) {
                score += ruleTypes[i].Quantity * dieTypes[i].Value;
                continue;
            }

            score = 0;
            return false;
        }

        if (Points > 0) {
            score = Points;
        } else if (SumOfAllValues) {
            score = 0;
            foreach (var dieType in dieTypes) {
                score += dieType.TotalValue;
            }
        }
        
        return true;
    }
}

[Serializable]
public class Type {

    public int Value;
    public int Quantity = 1;
    public int TotalValue => Value * Quantity;

    public override bool Equals(object obj) {

        Type type = obj as Type;
        if (type == null) {
            return false;
        }

        return (Value == type.Value && Quantity == type.Quantity);
    }
}
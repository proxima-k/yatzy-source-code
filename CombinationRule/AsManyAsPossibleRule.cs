using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CombinationRule", menuName = "ScriptableObjects/CombinationRule/AsManyAsPossibleRule", order = 0)]
public class AsManyAsPossibleRule : CombinationRule {

    public int ValueToMatch;
    
    public override bool TryMatch(List<int> dieValues, out List<int> scores) {
        scores = new List<int>();

        int matchCount = 0;
        foreach (int diceValue in dieValues) {
            if (diceValue == ValueToMatch) {
                matchCount++;
            }
        }

        scores.Add(matchCount * ValueToMatch);
        
        if (matchCount > 0) {
            return true;
        }
        return false;
    }
}

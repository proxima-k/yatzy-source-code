using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CombinationRule : ScriptableObject {
    public string RuleName; 
    
    public int Points = 0;

    public abstract bool TryMatch(List<int> dieValues, out List<int> scores);
}

using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DiceValueDebugUI : MonoBehaviour {
    
    [SerializeField] private TextMeshProUGUI _diceValueText;
    
    void Start() {
        DiceRoller.Instance.OnAllDieRolled += DiceRoller_OnAllDieRolled;
    }

    private void DiceRoller_OnAllDieRolled(object sender, DiceRoller.OnAllDieRolledEventArgs e) {
        string debugText = "";
        foreach (var value in e.DieValues) {
            debugText += value + " - ";
        }
        
        if (debugText.Length > 0) {
            debugText = debugText.Remove(debugText.Length - 3);
        }
        
        _diceValueText.text = debugText;
    }
}

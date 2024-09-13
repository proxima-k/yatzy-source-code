using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Dice : MonoBehaviour {
    
    
    [SerializeField] private List<DiceFace<int>> _diceFaces;
    
    public DiceLocomotion DiceLocomotion => _diceLocomotion;
    [SerializeField] private DiceLocomotion _diceLocomotion;

    private void Awake() {
        _diceLocomotion = GetComponent<DiceLocomotion>();
    }

    public bool TryGetValue(out int value) {
        foreach (var diceFace in _diceFaces) {
            Vector3 direction = (diceFace.FaceTf.position - transform.position).normalized;
            float dotProduct = Vector3.Dot(Vector3.up, direction);
            
            if (dotProduct > 0.95f) {
                value = diceFace.Value;
                return true;
            }
        }

        value = -1;
        return false;
    }
    
    public DiceFace<int> GetDiceFaceByValue (int value) {
        foreach (var diceFace in _diceFaces) {
            if (diceFace.Value == value) {
                return diceFace;
            }
        }

        return null;
    }
}

[Serializable]
public class DiceFace<T> {
    public T Value => _value;
    public Transform FaceTf => _faceTf;

    [SerializeField] private T _value;
    [SerializeField] private Transform _faceTf;
}
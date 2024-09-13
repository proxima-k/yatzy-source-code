using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Dice))]
public class DiceLocomotion : MonoBehaviour {
    
    public event EventHandler<OnStoppedRollingEventArgs> OnStoppedRolling;
    public class OnStoppedRollingEventArgs : EventArgs {
        public Dice Dice;
        public DiceLocomotion DiceLocomotion;
        public OnStoppedRollingEventArgs(Dice dice, DiceLocomotion diceLocomotion) { 
            Dice = dice;
            DiceLocomotion = diceLocomotion;
        }
    }
    
    public Dice Dice => _dice;
    [SerializeField] private Dice _dice;
    
    private bool _delayEnded = false;
    private Coroutine _delayResultCoroutine;
    private Rigidbody _rigidbody;
    
    private Coroutine _moveCoroutine;
    private Coroutine _rotateCoroutine;
    
    private void Awake() {
        _dice = GetComponent<Dice>();
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void Update() {
        if (!_delayEnded)
            return;
        
        if (!_rigidbody.isKinematic && _rigidbody.velocity.sqrMagnitude == 0) {

            // if dice has stopped moving but still can't get value then rethrow the dice
            DisableMovement();
            OnStoppedRolling?.Invoke(this, new OnStoppedRollingEventArgs(_dice, this));
        }
    }
    
    
    public void TriggerRoll() {
        EnableMovement();
        _delayEnded = false;
        
        if (_delayResultCoroutine != null)
            StopCoroutine(_delayResultCoroutine);
        _delayResultCoroutine = StartCoroutine(DelayResultRoutine());
    }
    
    private IEnumerator DelayResultRoutine() {
        yield return new WaitForSeconds(0.5f);
        _delayEnded = true;
    }

    public void EnableMovement() {
        _rigidbody.isKinematic = false;
    }
    
    public void DisableMovement() {
        _rigidbody.velocity = Vector3.zero;
        _rigidbody.isKinematic = true;
    }
    
    
    // rotate the dice to make a dice face face a certain direction 
    public void RotateDiceValueToFace(Vector3 faceDirection, int value, bool withLerp = true) {
        DiceFace<int> diceFace = _dice.GetDiceFaceByValue(value);
        if (diceFace == null) {
            Debug.LogError($"Dice face with value {value} not found!");
            return;
        }
        
        Vector3 diceFaceDirection = (diceFace.FaceTf.position - transform.position).normalized;
        Quaternion targetRotation = Quaternion.FromToRotation(diceFaceDirection, faceDirection) * transform.rotation;

        // Debug.Log($"Current rotation: {transform.rotation.eulerAngles}");
        // Debug.Log($"Target rotation: {targetRotation.eulerAngles}");
        
        if (!withLerp) {
            transform.rotation = targetRotation;
            return;
        }

        if (_rotateCoroutine != null) {
            StopCoroutine(_rotateCoroutine);
        }
        _rotateCoroutine = StartCoroutine(LerpRotateTowards(targetRotation));
    }
    
    private IEnumerator LerpRotateTowards(Quaternion targetRotation) {
        float t = 0.075f;
        while (Quaternion.Angle(transform.rotation, targetRotation) > 0.1f) {
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, t);
            
            yield return null;
        }
        
        transform.rotation = targetRotation;
        
        yield return null;
    }
    
    private IEnumerator LerpMoveTowards(Vector3 targetPosition) {
        float t = 0.025f;
        while (Vector3.SqrMagnitude(transform.position - targetPosition) > 0.01f) {
            transform.position = Vector3.Lerp(transform.position, targetPosition, t);
            yield return null;
        }

        yield return null;
    }
    
}

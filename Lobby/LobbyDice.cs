using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyDice : MonoBehaviour {

    [SerializeField] private DiceNameTagUI _diceNameTagUI;
    [SerializeField] private LobbyDiceReady _lobbyDiceReady;
    [SerializeField] private DiceLocomotion _diceLocomotion;
    [Range(1, 6)] 
    [SerializeField] private int _valueToFace;
    
    public int ClientID => _clientId;
    private int _clientId;
    
    private Coroutine _moveCoroutine;
    
    // stores player data?
    
    public void SetClientID(int clientId) {
        _clientId = clientId;
    }
    
    public void SetName(string name) {
        _diceNameTagUI.SetName(name);
    }
    
    public void SetColor(Color color) {
        _diceNameTagUI.SetColor(color);
    }
    
    public void SetReady(bool isReady) {
        _lobbyDiceReady.SetReady(isReady);
    }
    
    public void MoveToPosition(Vector3 targetPosition) {
        if (_moveCoroutine != null)
            StopCoroutine(_moveCoroutine);
        _moveCoroutine = StartCoroutine(LerpMoveToPosition(targetPosition));
    }
    
    private IEnumerator LerpMoveToPosition(Vector3 targetPosition) {
        float t = 0.05f;
        while (Vector3.SqrMagnitude(transform.position - targetPosition) > 0.01f) {
            transform.position = Vector3.Lerp(transform.position, targetPosition, t);
            yield return null;
        }

        yield return null;
    }
    
    public void SetUpwardFacingValue(int value) {
        _diceLocomotion.RotateDiceValueToFace(Vector3.up, value);
    }
}

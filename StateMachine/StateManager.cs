using UnityEngine;

public abstract class StateManager<TBaseState> : MonoBehaviour where TBaseState: BaseState {
    
    private bool _isTransitioning = false;
    private TBaseState _currentState;

    
    private void Update() {
        if (_currentState == null) 
            return;
        
        if (_isTransitioning) 
            return;
        
        _currentState.UpdateState();
    }
    
    public void ChangeState(TBaseState newState) {
        // Debug.LogWarning($"Changing state from {_currentState} to {newState}");
        _isTransitioning = true;
        if (_currentState != null) {
            _currentState.ExitState();
        }
        _currentState = newState;
        _currentState.EnterState();
        _isTransitioning = false;
    }
}

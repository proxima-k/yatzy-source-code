using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseState {

    // act as OnEnable
    public abstract void EnterState();

    public abstract void UpdateState();

    // act as OnDisable
    public abstract void ExitState();
}
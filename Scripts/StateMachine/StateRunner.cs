using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class StateRunner<T> : MonoBehaviour where T : MonoBehaviour
{
    private State<T> _activeState;

    public void SetState(State<T> newState)
    {
        if (_activeState != null)
            _activeState.OnExit();

        _activeState = newState;
        _activeState.OnEnter(GetComponent<T>());
    }

    public virtual void ClearState () {
        if (_activeState != null)
            _activeState.OnExit();

        _activeState = null;
    }

    protected virtual void Update() {
        if (_activeState != null)
            _activeState.OnUpdate();
    }

    private void FixedUpdate () {
        if (_activeState != null)
            _activeState.OnFixedUpdate();
    }
}

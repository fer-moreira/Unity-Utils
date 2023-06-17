using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class StateRunner<T> : MonoBehaviour where T : MonoBehaviour
{
    private State<T> _activeState;

    public void SetState(State<T> newState)
    {
        if (_activeState != null)
            _activeState.Exit();

        _activeState = newState;
        _activeState.Initialize(GetComponent<T>());
    }

    private void Update () => _activeState.Update();
    private void FixedUpdate () => _activeState.FixedUpdate();
}

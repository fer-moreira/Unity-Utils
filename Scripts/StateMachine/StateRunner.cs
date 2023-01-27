using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Gigachad.StateMachine
{
    public abstract class StateRunner<T> : MonoBehaviour where T : MonoBehaviour
    {
        [SerializeField]
        private List<State<T>> _states;
        private State<T> _activeState;

        protected virtual void Start () {
            SetState(_states[0].GetType());
        }

        public void SetState(Type newStateType)
        {
            if (_activeState != null)
                _activeState.Exit();

            _activeState = _states.First(s => s.GetType() == newStateType);
            _activeState.Init(GetComponent<T>());
        }

        private void Update()
        {
            _activeState.UpdateInput();
            _activeState.Update();
        }

        private void FixedUpdate()
        {
            _activeState.FixedUpdate();
        }
    }
}

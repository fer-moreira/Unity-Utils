using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(CapsuleCollider))]
public class PlayerRunner : StateRunner<PlayerRunner>
{
    public State<PlayerRunner> m_WalkState;
    public State<PlayerRunner> m_RunState;

    public Rigidbody        CachedRigidbody     { get; private set; }
    public Transform        CacahedTransform    { get; private set; }
    public CapsuleCollider  CachedCollider      { get; private set; }

    protected void Start()
    {
        CachedRigidbody = GetComponent<Rigidbody>();
        CachedCollider = GetComponent<CapsuleCollider>();
        CacahedTransform = this.transform;

        base.SetState(m_WalkState);
    }
}

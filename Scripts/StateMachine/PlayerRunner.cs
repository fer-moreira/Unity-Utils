using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gigachad.StateMachine;

[RequireComponent(typeof(Rigidbody)), 
RequireComponent(typeof(CapsuleCollider2D))]
public class PlayerRunner : StateRunner<PlayerRunner>
{
    [SerializeField] private float m_GroundDistance=1;

    public Rigidbody2D CachedRigidbody {get; private set;}
    public Collider2D CachedCollider {get; private set;}
    public Transform CacahedTransform {get; private set;}

    protected override void Start() {
        CachedRigidbody = GetComponent<Rigidbody2D>();
        CachedCollider = GetComponent<Collider2D>();
        CacahedTransform = this.transform;
        
        base.Start();
    }

    public bool Grounded => Physics2D.Raycast(CacahedTransform.position, Vector2.down * m_GroundDistance);

    private void OnDrawGizmos() {
        Gizmos.color = Color.red;

        if (!CacahedTransform)
            Gizmos.DrawRay(transform.position, Vector3.down * m_GroundDistance);
        else
            Gizmos.DrawRay(CacahedTransform.position, Vector3.down * m_GroundDistance);
    }
}

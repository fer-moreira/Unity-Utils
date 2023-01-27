using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gigachad.StateMachine;
using Gigachad;

[CreateAssetMenu(fileName = "STATE_PLAYER_MOTION", menuName = "States/Player/Motion")]
public class PlayerMotionState : State<PlayerRunner>
{
    [SerializeField] private float m_MovmentSpeed=4;

    private float _horizontal;
    private Rigidbody2D _rigidbody;
    private Vector2 _velocity;

    public override void Init(PlayerRunner parent)
    {
        base.Init(parent);
        _rigidbody = _runner.CachedRigidbody;
    }

    public override void UpdateInput()
    {
        _horizontal = InputReader.GetAxis("Horizontal");
    }

    public override void Exit()
    {

    }

    public override void FixedUpdate()
    {
        _velocity = Vector2.right * _horizontal;
        _velocity *= (m_MovmentSpeed * 100) * Time.fixedDeltaTime;

        _velocity.y = this._rigidbody.velocity.y;

        this._rigidbody.AddForce(_velocity, ForceMode2D.Force);
    }

    public override void Update()
    {
        if (InputReader.GetButtonDown("Jump"))
        {
            Debug.Log("TESTE");
        }
    }
}

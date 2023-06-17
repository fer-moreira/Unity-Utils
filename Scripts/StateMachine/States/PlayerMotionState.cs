using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "STATE_PLAYER_MOTION", menuName = "States/Player/Motion")]
public class PlayerMotionState : State<PlayerRunner>
{
    public override void Initialize(RobotStateRunner parent)
    {
        base.Initialize(parent);
    }

    public override void Exit()
    {
    }

    public override void FixedUpdate()
    {
    }

    public override void Update()
    {
    }
}

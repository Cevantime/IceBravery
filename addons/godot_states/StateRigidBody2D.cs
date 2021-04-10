using Godot;
using System;

public class StateRigidBody2D : RigidBody2D
{
    [Signal]
    delegate void _forces_integrated();

    public override void _IntegrateForces(Physics2DDirectBodyState state)
    {
        EmitSignal("_forces_integrated", new object[] { state });
    }
}

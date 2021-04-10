using Godot;
using System;

[Tool]
public class JellyAtomJoints : RigidBody2D
{
    [Signal]
    delegate void _forces_integrated(Physics2DDirectBodyState state);

    public override void _IntegrateForces(Physics2DDirectBodyState state)
    {
        EmitSignal("_forces_integrated", new object[] { state });
    }

    public void SetRadius(float radius)
    {
        CollisionShape2D shape = GetNode<CollisionShape2D>("CollisionShape2D");
        ((CircleShape2D)shape.Shape).Radius = radius;
    }

    public void SetDraggable(bool draggable)
    {
        GetNode<Draggable>("Draggable").Disabled = !draggable;
    }
}

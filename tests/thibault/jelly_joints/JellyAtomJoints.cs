using Godot;
using System;

[Tool]
public class JellyAtomJoints : RigidBody2D, Meltable
{

    private float originalRadius = -1;

    private float radius;

    public float Radius
    {
        get
        {
            return radius;
        }
        set
        {
            radius = value;
            if (originalRadius == -1)
            {
                originalRadius = radius;
            }
            CollisionShape2D shape = GetNode<CollisionShape2D>("CollisionShape2D");
            ((CircleShape2D)shape.Shape).Radius = radius;
        }
    }


    [Signal]
    delegate void _forces_integrated(Physics2DDirectBodyState state);

    public override void _Ready()
    {
    }
    public override void _IntegrateForces(Physics2DDirectBodyState state)
    {
        EmitSignal("_forces_integrated", new object[] { state });
    }

    public void SetDraggable(bool draggable)
    {
        GetNode<Draggable>("Draggable").Disabled = !draggable;
    }

    public void Melt(float factor)
    {
        Radius -= factor * originalRadius;
    }

    public void Reset()
    {
        Radius = originalRadius;
    }
}

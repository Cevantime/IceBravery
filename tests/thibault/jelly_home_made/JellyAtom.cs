using Godot;
using System;
using Godot.Collections;

[Tool]
public class JellyAtom : RigidBody2D
{
    public Array<Neighbour> neighbours = new Array<Neighbour>();
    public Jelly jelly;

    [Export]
    public bool draggable = false;

    private bool dragged = false;

    public override void _IntegrateForces(Physics2DDirectBodyState state)
    {
        if (dragged)
        {
            Vector2 pos = state.Transform.origin;
            Vector2 mousePos = GetGlobalMousePosition();
            ApplyCentralImpulse((mousePos - pos) * state.Step * 100);
        }

        if (jelly == null)
        {
            return;
        }

        float damping = jelly.damping;
        float stiffness = jelly.stiffness;
        Vector2 totalImpulse = Vector2.Zero;

        foreach (Neighbour n in neighbours)
        {
            JellyAtom neighbourAtom = n.jellyAtom;
            Vector2 posNeighbour = neighbourAtom.GlobalPosition;
            Vector2 selfPos = state.Transform.origin;
            float restLength = n.restLength;
            Vector2 diff = posNeighbour - selfPos;
            float distanceToNeighbour = diff.Length();
            Vector2 diffNormalized = diff / distanceToNeighbour;
            Vector2 selfVelocity = state.LinearVelocity;
            Vector2 neighbourVelocity = neighbourAtom.LinearVelocity;
            float force = (distanceToNeighbour - restLength) * stiffness
                + (diffNormalized).Dot(neighbourVelocity - selfVelocity) * damping;

            totalImpulse += force * diff.Normalized() * state.Step;
        }

        ApplyCentralImpulse(totalImpulse);

    }

    public void AddNeighbour(Neighbour n)
    {
        neighbours.Add(n);
    }

    public override void _InputEvent(Godot.Object viewport, InputEvent @event, int shapeIdx)
    {
        if (@event.IsActionPressed("touch") && draggable)
        {
            dragged = true;
        }
    }

    public override void _Input(InputEvent @event)
    {
        if (@event.IsActionReleased("touch") && dragged)
        {
            dragged = false;
        }
    }
}

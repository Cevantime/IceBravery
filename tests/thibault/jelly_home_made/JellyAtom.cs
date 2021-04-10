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

    [Signal]
    delegate void _forces_integrated(Physics2DDirectBodyState state);

    public override void _IntegrateForces(Physics2DDirectBodyState state)
    {
        EmitSignal("_forces_integrated", new object[] { state });

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

}

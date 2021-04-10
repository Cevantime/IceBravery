using Godot;
using System;
using Godot.Collections;

public class JellyAtom : RigidBody2D
{
    public Array<Neighbour> neighbours = new Array<Neighbour>();
    public Jelly jelly;

    public override void _IntegrateForces(Physics2DDirectBodyState state)
    {
        float damping = jelly.damping;
        float stiffness = jelly.stiffness;

        foreach (Neighbour n in neighbours)
        {
            JellyAtom neighbourAtom = n.jellyAtom;
            Vector2 posNeighbour = neighbourAtom.GlobalPosition;
            Vector2 selfPos = state.Transform.origin;
            float restLength = n.restLength;
            Vector2 diff = posNeighbour - selfPos;
            float distanceToNeighbour = diff.Length();
            Vector2 selfVelocity = state.LinearVelocity;
            Vector2 neighbourVelocity = neighbourAtom.LinearVelocity;
            float force = (distanceToNeighbour - restLength) * stiffness
                + (diff / distanceToNeighbour).Dot(neighbourVelocity - selfVelocity) * damping;

            state.ApplyCentralImpulse(force * diff.Normalized() * state.Step);
        }
    }

    public void AddNeighbour(Neighbour n)
    {
        neighbours.Add(n);
    }
}

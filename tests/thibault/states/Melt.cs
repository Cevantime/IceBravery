using Godot;
using System;
using GodotStates;

public class Melt : StateMachine
{
    [Export]
    public float meltingSpeed = 0.03f;

    private JellyJoints jellyJoints;
    public override bool _Supports(Node node)
    {
        return node is JellyJoints;
    }

    public override void _Ready()
    {
        base._Ready();
        jellyJoints = (JellyJoints)Referer;
    }

    public override void _Process(float delta)
    {
        int nbCollinding = 0;
        foreach (Node n in jellyJoints.rayCasts.GetChildren())
        {
            RayCast2D rayCast = (RayCast2D)n;
            if (rayCast.IsColliding())
            {
                nbCollinding++;
            }
        }
        float meltingFactor = nbCollinding * meltingSpeed * delta;
        foreach (JellyAtomJoints jellyAtom in jellyJoints.mapAtoms.Values)
        {
            jellyAtom.Melt(meltingFactor);
        }

        foreach (JellyJoint jellyJoint in jellyJoints.joints)
        {
            jellyJoint.Melt(meltingFactor);
        }
        foreach (Node n in jellyJoints.rayCasts.GetChildren())
        {
            JellyRayCast jellyRayCast = (JellyRayCast)n;
            jellyRayCast.Melt(meltingFactor);
        }
    }

    public override void _EnterState(StateMachine previous, params object[] list)
    {

    }
    public override void _ExitState(StateMachine next)
    {

    }

    public override void _IntegrateForces(Godot.Object state)
    {

    }
}

using Godot;
using System;
using GodotStates;

public class MeltingTest : Node2D
{

    public override void _Ready()
    {
        base._Ready();
    }

    public override void _Process(float delta)
    {
        DampedSpringJoint2D j = GetNode<DampedSpringJoint2D>("DampedSpringJoint2D");
        if (j.Length > 1)
        {
            j.RestLength -= delta;
            j.Length -= delta;
        }
    }
}

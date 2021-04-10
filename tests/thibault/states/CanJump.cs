using Godot;
using System;
using GodotStates;

public class CanJump : StateMachine
{
    JellyJoints jelly;
    public override bool _Supports(Node node)
    {
        return node is JellyJoints;
    }

    public override void _Ready()
    {
        base._Ready();
        jelly = (JellyJoints)Referer;
    }

    public override void _Process(float delta)
    {
        if (Input.IsActionPressed("ui_accept"))
        {
            Vector2 dirJump = Vector2.Zero;
            foreach (Node n in jelly.GetNode("RayCasts").GetChildren())
            {
                RayCast2D rayCast = (RayCast2D)n;
                if (rayCast.IsColliding())
                {
                    dirJump -= rayCast.CastTo;
                }
            }
            dirJump = dirJump.Normalized();
            jelly.center.ApplyCentralImpulse(dirJump * 120);
        }
    }
}

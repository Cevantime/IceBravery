using Godot;
using System;
using GodotStates;

[Tool]
public class Draggable : StateMachine
{
    private bool dragged = false;
    private RigidBody2D body;

    public override void _Ready()
    {
        base._Ready();
        body = (RigidBody2D)Referer;
        body.Connect("input_event", this, nameof(_InputEvent));
        if (!body.InputPickable)
        {
            body.InputPickable = !Disabled;
        }
        if (body.CanSleep)
        {
            body.CanSleep = Disabled;
        }
    }


    public void _InputEvent(Godot.Object viewport, InputEvent @event, int shapeIdx)
    {
        if (@event.IsActionPressed("touch"))
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

    public override bool _Supports(Node node)
    {
        return node is RigidBody2D;
    }

    public override void _IntegrateForces(Godot.Object st)
    {
        Physics2DDirectBodyState state = (Physics2DDirectBodyState)st;

        if (dragged)
        {
            Vector2 pos = state.Transform.origin;
            Vector2 mousePos = body.GetGlobalMousePosition();
            state.ApplyCentralImpulse((mousePos - pos) * state.Step * 100);
        }
    }

}

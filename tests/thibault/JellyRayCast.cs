using Godot;
using System;

public class JellyRayCast : RayCast2D, Meltable
{
    private Vector2 originalCastTo;

    public override void _Ready()
    {
        originalCastTo = CastTo;
    }

    public void Melt(float factor)
    {
        CastTo -= factor * originalCastTo;
    }

    public void Reset()
    {
        CastTo = originalCastTo;
    }
}

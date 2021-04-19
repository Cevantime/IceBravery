using Godot;
using System;

public class JellyJoint : DampedSpringJoint2D, Meltable
{
    private float originalLength;

    public override void _Ready()
    {
        originalLength = RestLength;
    }
    public void Melt(float factor)
    {
        RestLength -= factor * originalLength;
        Length = RestLength;
    }

    public void Reset()
    {
        Length = RestLength = originalLength;
    }
}

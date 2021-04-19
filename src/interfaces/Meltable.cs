using Godot;
using System;

public interface Meltable
{
    void Melt(float factor);
    void Reset();
}

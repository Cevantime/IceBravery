using Godot;
using System;

public class Neighbour : Godot.Object
{
    public JellyAtom jellyAtom;
    public float restLength;

    public Neighbour()
    {

    }
    public Neighbour(JellyAtom jellyAtom, float restLength)
    {
        this.jellyAtom = jellyAtom;
        this.restLength = restLength;
    }
}

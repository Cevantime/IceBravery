using Godot;
using System;
using Godot.Collections;

[Tool]
public class Jelly : Polygon2D
{
    private Rect2 rect = new Rect2(-10, -10, 20, 20);
    private int atomW = 7;
    private int atomH = 7;

    [Export]
    public float stiffness = 100.0f;

    [Export]
    public float damping = 10.0f;

    [Export]
    public Rect2 Rect
    {
        get
        {
            return rect;
        }
        set
        {
            rect = value;
            UpdateRect();
        }
    }

    [Export]
    public int AtomW
    {
        get
        {
            return atomW;
        }
        set
        {
            atomW = value;
            UpdateRect();
        }
    }

    [Export]
    public int AtomH
    {
        get
        {
            return atomH;
        }
        set
        {
            atomH = value;
            UpdateRect();
        }
    }

    private Array<JellyAtom> edgeBodies = new Array<JellyAtom>();

    private Dictionary<Vector2, JellyAtom> mapAtoms = new Dictionary<Vector2, JellyAtom>();

    private PackedScene jellyAtomPacked;
    public override void _Ready()
    {
        jellyAtomPacked = GD.Load<PackedScene>("res://tests/thibault/jelly/JellyAtom.tscn");
        UpdateRect();
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        Vector2[] p = new Vector2[edgeBodies.Count];
        int i = 0;
        foreach (RigidBody2D b in edgeBodies)
        {
            p[i++] = b.Position;
        }
        Polygon = p;
    }

    private void UpdateRect()
    {
        edgeBodies = new Array<JellyAtom>();
        if (jellyAtomPacked == null)
        {
            return;
        }
        Vector2 atomSeparation = new Vector2(rect.Size.x / (atomW - 1), rect.Size.y / (atomH - 1));
        Vector2 origin = rect.Position;
        foreach (Node n in GetChildren())
        {
            n.QueueFree();
        }
        mapAtoms = new Dictionary<Vector2, JellyAtom>();
        for (int j = atomH - 1; j >= 0; j--)
        {
            for (int i = atomW - 1; i >= 0; i--)
            {
                JellyAtom jellyAtom = (JellyAtom)jellyAtomPacked.Instance();
                Vector2 gridPos = new Vector2(i, j);
                jellyAtom.Position = origin + atomSeparation * gridPos;
                jellyAtom.jelly = this;
                AddChild(jellyAtom);
                mapAtoms.Add(gridPos, jellyAtom);
                Vector2[] neighbours = new Vector2[] {
                    new Vector2(i + 1, j),
                    new Vector2(i, j + 1),
                    new Vector2(i + 1, j + 1),
                    new Vector2(i - 1, j + 1)
                };
                foreach (Vector2 neighbour in neighbours)
                {
                    JellyAtom neighbourBody;

                    if (!mapAtoms.TryGetValue(neighbour, out neighbourBody))
                    {
                        continue;
                    }

                    float dist = (neighbourBody.Position - jellyAtom.Position).Length();
                    neighbourBody.AddNeighbour(new Neighbour(jellyAtom, dist));
                    jellyAtom.AddNeighbour(new Neighbour(neighbourBody, dist));
                }
            }
        }

        // for (int i = 0; i < atomW; i++)
        // {
        //     edgeBodies.Add(mapAtoms[new Vector2(i, 0)]);
        // }

        // for (int j = 1; j < atomH; j++)
        // {
        //     edgeBodies.Add(mapAtoms[new Vector2(atomW - 1, j)]);
        // }

        // for (int i = atomW - 2; i >= 0; i--)
        // {
        //     edgeBodies.Add(mapAtoms[new Vector2(i, atomH - 1)]);
        // }

        // for (int j = atomH - 2; j >= 1; j--)
        // {
        //     edgeBodies.Add(mapAtoms[new Vector2(0, j)]);
        // }
    }
}

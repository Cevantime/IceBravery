using Godot;
using System;
using Godot.Collections;

[Tool]
public class JellyJoints : Polygon2D
{
    private Rect2 rect = new Rect2(-10, -10, 20, 20);
    private int atomW = 7;
    private int atomH = 7;

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

    private Array<RigidBody2D> edgeBodies = new Array<RigidBody2D>();

    private Dictionary<Vector2, RigidBody2D> mapAtoms = new Dictionary<Vector2, RigidBody2D>();

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
        edgeBodies = new Array<RigidBody2D>();
        if (jellyAtomPacked == null)
        {
            return;
        }
        Vector2 atomSeparation = new Vector2(atomW > 1 ? rect.Size.x / (atomW - 1) : 0, atomH > 1 ? rect.Size.y / (atomH - 1) : 0);
        Vector2 origin = rect.Position;
        foreach (Node n in GetChildren())
        {
            n.QueueFree();
        }
        mapAtoms = new Dictionary<Vector2, RigidBody2D>();
        for (int j = atomH - 1; j >= 0; j--)
        {
            for (int i = atomW - 1; i >= 0; i--)
            {
                RigidBody2D jellyAtom = (RigidBody2D)jellyAtomPacked.Instance();
                Vector2 gridPos = new Vector2(i, j);
                jellyAtom.Position = origin + atomSeparation * gridPos;
                AddChild(jellyAtom);
                mapAtoms.Add(gridPos, jellyAtom);
                Vector2[] neighbours = new Vector2[] {
                    new Vector2(i + 1, j),
                    new Vector2(i, j + 1),
                    new Vector2(i + 1, j + 1),
                    new Vector2(i - 1, j + 1)
                };
                if (i == 3 && j == 3)
                {
                    foreach (Node n in GetTree().GetNodesInGroup("CameraOffset"))
                    {
                        Node2D cameraOffset = (Node2D)n;
                        RemoteTransform2D rt = new RemoteTransform2D();
                        rt.UpdateRotation = false;
                        rt.UpdateRotation = false;
                        rt.RemotePath = cameraOffset.GetPath();
                        jellyAtom.AddChild(rt);
                    }
                }
                foreach (Vector2 neighbour in neighbours)
                {
                    RigidBody2D neighbourBody;

                    if (!mapAtoms.TryGetValue(neighbour, out neighbourBody))
                    {
                        continue;
                    }

                    DampedSpringJoint2D joint = new DampedSpringJoint2D();
                    Vector2 separation = new Vector2(neighbourBody.Position - jellyAtom.Position);
                    joint.Position = jellyAtom.Position;
                    joint.RestLength = separation.Length();
                    joint.Rotation = separation.Angle() - Mathf.Pi / 2;
                    joint.Length = separation.Length();
                    joint.NodeA = jellyAtom.GetPath();
                    joint.NodeB = neighbourBody.GetPath();
                    joint.DisableCollision = false;
                    joint.Stiffness = 85;
                    joint.Damping = 1f;
                    RemoteTransform2D rt = new RemoteTransform2D();
                    rt.UpdateRotation = false;
                    rt.UpdateScale = false;
                    // joint.Scale = new Vector2(0.1f, 0.1f);
                    AddChild(joint);
                    // rt.RemotePath = joint.GetPath();
                    // jellyAtom.AddChild(rt);
                }
            }
        }

        for (int i = 0; i < atomW; i++)
        {
            edgeBodies.Add(mapAtoms[new Vector2(i, 0)]);
        }

        for (int j = 1; j < atomH; j++)
        {
            edgeBodies.Add(mapAtoms[new Vector2(atomW - 1, j)]);
        }

        for (int i = atomW - 2; i >= 0; i--)
        {
            edgeBodies.Add(mapAtoms[new Vector2(i, atomH - 1)]);
        }

        for (int j = atomH - 2; j >= 1; j--)
        {
            edgeBodies.Add(mapAtoms[new Vector2(0, j)]);
        }
    }
}

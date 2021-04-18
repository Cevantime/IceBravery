using Godot;
using System;
using Godot.Collections;

[Tool]
public class JellyJoints : Node2D
{
    private Rect2 rect = new Rect2(-10, -10, 20, 20);
    private int atomW = 7;
    private int atomH = 7;

    [Export]
    public float stiffness = 100.0f;

    [Export]
    public float damping = 1.0f;

    [Export]
    public float gravityScale = 1.0f;

    [Export]
    public bool draggable = false;

    public float atomRadius = 1.5f;

    public JellyAtomJoints center;
    public JellyAtomJoints east;

    public Polygon2D polygon;
    public Node2D rayCasts;

    [Export(PropertyHint.Range, "3,15")]
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

    [Export(PropertyHint.Range, "3,15")]
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

    [Export]
    public float AtomRadius
    {
        get
        {
            return atomRadius;
        }

        set
        {
            atomRadius = value;
            UpdateRect();
        }
    }

    private Array<RigidBody2D> edgeBodies = new Array<RigidBody2D>();

    private Dictionary<Vector2, RigidBody2D> mapAtoms = new Dictionary<Vector2, RigidBody2D>();

    private PackedScene jellyAtomPacked;

    [Signal]
    delegate void IsDecomposed(JellyJoints self);
    public override void _Ready()
    {
        polygon = GetNode<Polygon2D>("Polygon2D");
        jellyAtomPacked = GD.Load<PackedScene>("res://tests/thibault/jelly_joints/JellyAtomJoints.tscn");
        UpdateRect();
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        polygon.GlobalPosition = center.GlobalPosition;
        Vector2[] p = new Vector2[edgeBodies.Count];
        int i = 0;
        foreach (RigidBody2D b in edgeBodies)
        {
            p[i++] = polygon.ToLocal(b.GlobalPosition);
        }
        polygon.Polygon = p;
        int[] ps = Geometry.TriangulatePolygon(polygon.Polygon);
        if (ps.Length <= 0)
        {
            EmitSignal("IsDecomposed", this);
            GD.Print("decomposed");
        }
        rayCasts.GlobalRotation = center.GlobalPosition.AngleToPoint(east.GlobalPosition);
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
        foreach (Node n in mapAtoms.Values)
        {
            n.QueueFree();
        }
        mapAtoms = new Dictionary<Vector2, RigidBody2D>();
        for (int j = atomH - 1; j >= 0; j--)
        {
            for (int i = atomW - 1; i >= 0; i--)
            {
                JellyAtomJoints jellyAtom = (JellyAtomJoints)jellyAtomPacked.Instance();
                Vector2 gridPos = new Vector2(i, j);
                jellyAtom.Position = origin + atomSeparation * gridPos;
                jellyAtom.GravityScale = gravityScale;
                jellyAtom.SetRadius(atomRadius);
                jellyAtom.SetDraggable(draggable);
                AddChild(jellyAtom);
                mapAtoms.Add(gridPos, jellyAtom);
                int mX = (int)(atomW / 2);
                int mY = (int)(atomH / 2);
                if (i == mX && j == mY)
                {
                    center = jellyAtom;
                    rayCasts = (Node2D)GD.Load<PackedScene>("res://tests/thibault/RayCasts.tscn").Instance();
                    AddChild(rayCasts);
                    RemoteTransform2D rt = new RemoteTransform2D();
                    rt.UpdateRotation = false;
                    rt.UpdateScale = false;
                    rt.RemotePath = rayCasts.GetPath();
                    jellyAtom.AddChild(rt);
                    foreach (Node n in GetTree().GetNodesInGroup("CameraOffset"))
                    {
                        Node2D cameraOffset = (Node2D)n;
                        rt = new RemoteTransform2D();
                        rt.UpdateRotation = false;
                        rt.UpdateScale = false;
                        rt.RemotePath = cameraOffset.GetPath();
                        jellyAtom.AddChild(rt);
                    }
                }
                else if (j == mY && i == atomW - 1)
                {
                    east = jellyAtom;
                }
                Vector2[] neighbours = new Vector2[] {
                    new Vector2(i + 1, j),
                    new Vector2(i, j + 1),
                    new Vector2(i + 1, j + 1),
                    new Vector2(i - 1, j + 1)
                };
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
                    joint.Stiffness = stiffness;
                    joint.Damping = damping;
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

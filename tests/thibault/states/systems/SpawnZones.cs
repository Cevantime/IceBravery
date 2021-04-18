using Godot;
using System;
using GodotStates;
using Godot.Collections;

public class SpawnZones : StateMachine
{
    private LavaSurfaceBurn lavaSurface;
    private PackedScene lavaPacked;

    private Array<LavaSurfaceBurn> surfaces;
    public override bool _Supports(Node node)
    {
        return node is Node2D;
    }

    public override void _Ready()
    {
        lavaPacked = GD.Load<PackedScene>("res://tests/thibault/LavaSurfaceBurn.tscn");
        lavaSurface = (LavaSurfaceBurn)lavaPacked.Instance();
        LavaSurfaceBurn lava1 = (LavaSurfaceBurn)GetTree().GetNodesInGroup("LavaBurn")[0];
        lava1.CallDeferred("queue_free");
        lavaSurface.Points = lava1.Points;
        lavaSurface.isInterrupted = lava1.isInterrupted;
        GetTree().CurrentScene.CallDeferred("add_child", lavaSurface);
        lava1.QueueFree();
        surfaces = new Array<LavaSurfaceBurn>();
    }
    public override void _Process(float delta)
    {
        float spawnZoneExtends = 640;
        Vector2 globalPosition = ((Node2D)Referer).GlobalPosition;
        Rect2 spawnZone = new Rect2(globalPosition, spawnZoneExtends, spawnZoneExtends);

        foreach (Node n in GetTree().GetNodesInGroup("EndPositions"))
        {
            Position2D pos = (Position2D)n;
            LavaBlock lb = (LavaBlock)pos.GetParent();
            if (spawnZone.HasPoint(pos.GlobalPosition) && !lb.loadedNext)
            {
                PackedScene nextToSpawn = GD.Load<PackedScene>("tests/thibault/lava_blocks/" + lb.nexts[(int)(GD.Randi() % lb.nexts.Count)] + ".tscn");
                LavaBlock nextBlock = (LavaBlock)nextToSpawn.Instance();
                nextBlock.camera = lb.camera;
                nextBlock.GlobalPosition = pos.GlobalPosition;
                Referer.GetParent().AddChild(nextBlock);
                lb.loadedNext = true;
                Array<LavaSurfaceBurn> nextLavas = new Array<LavaSurfaceBurn>();
                foreach (Node n2 in GetTree().GetNodesInGroup("LavaBurn"))
                {
                    LavaSurfaceBurn l = (LavaSurfaceBurn)n2;
                    if (l.GetParent() == nextBlock)
                    {
                        nextLavas.Add(l);
                    }
                }

                for (int i = 0; i < nextLavas.Count; i++)
                {
                    if (i != 0 || lavaSurface.isInterrupted)
                    {
                        surfaces.Add(lavaSurface);
                        lavaSurface = (LavaSurfaceBurn)lavaPacked.Instance();
                        GetTree().CurrentScene.AddChild(lavaSurface);
                    }

                    LavaSurfaceBurn nextLava = nextLavas[i];
                    Vector2 offset = nextLava.GlobalPosition;
                    foreach (Vector2 p in nextLava.Points)
                    {
                        lavaSurface.AddPoint(p + offset);
                    }

                    lavaSurface.isInterrupted = nextLava.isInterrupted;
                    nextLava.QueueFree();
                }

            }
            else if ((pos.GlobalPosition.x - globalPosition.x) < -spawnZoneExtends)
            {
                lb.QueueFree();
                while (surfaces.Count > 0)
                {
                    LavaSurfaceBurn ls = surfaces[0];
                    float maxX = 0;
                    for (int i = 0; i < ls.Points.Length; i++)
                    {
                        maxX = Mathf.Max(maxX, ls.GetPointPosition(i).x);
                    }
                    if (maxX - globalPosition.x < -spawnZoneExtends)
                    {
                        surfaces.RemoveAt(0);
                        ls.QueueFree();
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }
    }
}

using Godot;
using System;
using GodotStates;

public class SpawnZones : StateMachine
{
    public override bool _Supports(Node node)
    {
        return node is Node2D;
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
            }
            else if ((pos.GlobalPosition.x - globalPosition.x) < -spawnZoneExtends)
            {
                lb.QueueFree();
            }
        }
    }
}

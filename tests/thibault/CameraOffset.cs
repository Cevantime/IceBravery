using Godot;
using System;

public class CameraOffset : Node2D
{

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        float spawnZoneExtends = 640;
        Rect2 spawnZone = new Rect2(GlobalPosition, spawnZoneExtends, spawnZoneExtends);

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
                GetParent().AddChild(nextBlock);
                lb.loadedNext = true;
            }
            else if ((pos.GlobalPosition.x - GlobalPosition.x) < -spawnZoneExtends)
            {
                lb.QueueFree();
            }
        }
    }
}

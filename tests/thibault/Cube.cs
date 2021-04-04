using Godot;
using System;

public class Cube : RigidBody2D
{
    private Vector2 screenSize;
    private ShaderMaterial innerMaterial;
    private Camera2D camera;

    [Export]
    public NodePath cameraPath;

    public override void _Ready()
    {
        GD.Randomize();
        float screenWidth = (int)ProjectSettings.GetSetting("display/window/size/width");
        float screenHeight = (int)ProjectSettings.GetSetting("display/window/size/height");

        screenSize = new Vector2(screenWidth, screenHeight);

        Vector2 resolutionFactor = OS.WindowSize / screenSize;
        innerMaterial = (ShaderMaterial)GetNode<Sprite>("IceCubeInner").Material;
        innerMaterial.SetShaderParam("resolution_factor", resolutionFactor);
        if (cameraPath != null)
        {
            camera = GetNode<Camera2D>(cameraPath);
        }

    }
    public override void _Process(float delta)
    {
        Vector2 centerPosition = GlobalPosition;
        if (camera != null)
        {
            centerPosition = screenSize / 2;
            innerMaterial.SetShaderParam("center_position", centerPosition - (camera.GlobalPosition - GlobalPosition));
        }

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

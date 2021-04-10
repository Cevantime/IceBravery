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
    }
}

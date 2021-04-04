using Godot;
using System;

[Tool]
public class LavaBlock : Polygon2D
{
    [Export]
    public NodePath cameraPath;

    [Export]
    public Godot.Collections.Array<string> nexts;
    private Vector2 screenSize;
    private float screenRatio;
    Vector2 resolutionFactor;

    private ShaderMaterial innerMaterial;
    public Camera2D camera;

    public bool loadedNext = false;


    public override void _Ready()
    {
        _Prepare();
    }

    public override void _EnterTree()
    {
        if (Engine.EditorHint)
        {
            _Prepare();
        }
    }

    public void _Prepare()
    {
        float screenWidth = (int)ProjectSettings.GetSetting("display/window/size/width");
        float screenHeight = (int)ProjectSettings.GetSetting("display/window/size/height");

        screenSize = new Vector2(screenWidth, screenHeight);

        screenRatio = screenSize.x / screenSize.y;
        Material = (Material)Material.Duplicate();
        innerMaterial = (ShaderMaterial)Material;
        resolutionFactor = OS.WindowSize / screenSize;
        innerMaterial.SetShaderParam("resolution_factor", resolutionFactor);
        if (cameraPath != "")
        {
            camera = GetNode<Camera2D>(cameraPath);
        }
        CollisionPolygon2D collisionPolygon2D = GetNode<CollisionPolygon2D>("StaticBody2D/CollisionPolygon2D");
        collisionPolygon2D.Polygon = Polygon;
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        if (camera != null)
        {
            innerMaterial.SetShaderParam("camera_offset", camera.GlobalPosition * resolutionFactor);
        }
    }
}

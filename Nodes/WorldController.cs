using Godot;
using System;

public class WorldController : Spatial
{
    public static WorldController Instance {get;private set;}

    private LevelController currentLevel;

    public override void _Ready()
    {
        WorldController.Instance = this;
        LoadLevel("res://Nodes/Level/Level_Test.tscn");
    }

    public void LoadLevel(NodePath path)
    {
        if(currentLevel != null)
        {
            this.RemoveChild(currentLevel);
            currentLevel.QueueFree();
        }

        PackedScene scene = (PackedScene)ResourceLoader.Load(path);
        currentLevel = scene.Instance<LevelController>();
        this.AddChild(currentLevel);
    }

    public void LoadMainMenu()
    {
        this.GetTree().ChangeScene("res://Nodes/Menu/MainMenu.tscn");
    }

    public void LoadDeathMenu()
    {
        this.GetTree().ChangeScene("res://Nodes/Menu/MainMenu.tscn");
    }

}

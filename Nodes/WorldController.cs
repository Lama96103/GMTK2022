using Godot;
using System;

public class WorldController : Spatial
{
    public static WorldController Instance {get;private set;}

    public static string CurrentLevelPath = "res://Nodes/Level/Level_003.tscn";

    private LevelController currentLevel;

    public override void _Ready()
    {
        WorldController.Instance = this;

        LoadLevel(CurrentLevelPath);
    }

    public void LoadLevel(NodePath path)
    {
        if(currentLevel != null)
        {
            this.RemoveChild(currentLevel);
            currentLevel.QueueFree();
        }
        CurrentLevelPath = path;

        PackedScene scene = (PackedScene)ResourceLoader.Load(path);
        currentLevel = scene.Instance<LevelController>();
        this.AddChild(currentLevel);
    }


    public void LoadNextLevel()
    {
        LoadLevel(currentLevel.NextLevel);
    }

    public void LoadMainMenu()
    {
        this.GetTree().ChangeScene("res://Nodes/Menu/MainMenu.tscn");
    }

    public void LoadDeathMenu()
    {
        this.GetTree().ChangeScene("res://Nodes/Menu/DeathMenu.tscn");
    }

}

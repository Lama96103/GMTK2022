using Godot;
using System;

public class WorldController : Spatial
{
    public static WorldController Instance {get;private set;}

    public static string CurrentLevelPath = "res://Nodes/Level/Level_001.tscn";

    private LevelController currentLevel;

    private Control finishedLevelScreen;

    public override void _Ready()
    {
        WorldController.Instance = this;
        finishedLevelScreen = GetNode<Control>("Game_UI/Status");
        finishedLevelScreen.Visible = false;
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

    public void OnFinishedLevel()
    {
        finishedLevelScreen.Visible = true;
    }


    private void LoadNextLevel()
    {
        finishedLevelScreen.Visible = false;
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

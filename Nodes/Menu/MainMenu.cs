using Godot;
using System;

public class MainMenu : Control
{

    public void ButtonStartGamePressed()
    {
        WorldController.CurrentLevelPath = "res://Nodes/Level/Level_001.tscn";
        this.GetTree().ChangeScene("res://Nodes/World.tscn");
    }
}

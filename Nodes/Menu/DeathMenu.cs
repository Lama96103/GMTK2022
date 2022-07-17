using Godot;
using System;

public class DeathMenu : Control
{
    public void ButtonRetryPressed()
    {
        this.GetTree().ChangeScene("res://Nodes/World.tscn");
    }

    public void ButtonMainMenuPressed()
    {
        this.GetTree().ChangeScene("res://Nodes/Menu/MainMenu.tscn");
    }
}

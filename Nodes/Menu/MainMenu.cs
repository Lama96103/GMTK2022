using Godot;
using System;

public class MainMenu : Control
{

    public void ButtonStartGamePressed()
    {
        this.GetTree().ChangeScene("res://Nodes/World.tscn");
    }
}

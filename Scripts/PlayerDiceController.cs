using Godot;
using System;

public class PlayerDiceController : DiceController
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        base._Process(delta);
         if(Input.IsActionJustPressed("key_forward"))
        {
            RollDice(Vector3.Forward);
        }
        if(Input.IsActionJustPressed("key_backward"))
        {
            RollDice(Vector3.Back);
        }
        if(Input.IsActionJustPressed("key_right"))
        {
            RollDice(Vector3.Right);
        }
        if(Input.IsActionJustPressed("key_left"))
        {
            RollDice(Vector3.Left);
        }
    }
}

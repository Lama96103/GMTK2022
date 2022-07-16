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

    public override bool ExecuteTurn()
    {
        if(Input.IsActionJustPressed("key_forward"))
        {
            RollDice(Vector3.Forward);
            return true;
        }
        if(Input.IsActionJustPressed("key_backward"))
        {
            RollDice(Vector3.Back);
            return true;
        }
        if(Input.IsActionJustPressed("key_right"))
        {
            RollDice(Vector3.Right);
            return true;
        }
        if(Input.IsActionJustPressed("key_left"))
        {
            RollDice(Vector3.Left);
            return true;
        }
        
        return false;
    }
}

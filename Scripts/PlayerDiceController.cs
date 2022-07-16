using Godot;
using System;

[Tool]
public class PlayerDiceController : DiceController
{

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

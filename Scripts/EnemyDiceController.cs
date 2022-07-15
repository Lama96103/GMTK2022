using Godot;
using System;
using Godot.Collections;
using System.Linq;

public class EnemyDiceController : DiceController
{

    [Export] private Array<Vector3> path = new Array<Vector3>();
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        
    }

    public void Move()
    {
        if(path.Count() > 0)
        {
            RollDice(path[0]);
            path.RemoveAt(0);
        }
    }
//  // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        base._Process(delta);
        if(Input.IsActionJustPressed("key_endRound"))
        {
            Move();
        }
    }
}

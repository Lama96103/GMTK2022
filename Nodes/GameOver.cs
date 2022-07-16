using Godot;
using System;

public class GameOver : Spatial
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
        foreach(DiceController dice in this.GetTree().GetNodesInGroup("EnemyDice"))
        {
            if(dice.GlobalTransform.Equals(new Vector3(0, 0, 0.5f)))
            {
                GD.Print("Game Over");
            }
        }
    }
}

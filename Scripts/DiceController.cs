using Godot;
using System;

public abstract class DiceController : Spatial
{
    public LevelController CurrentLevel;

    private Spatial dice;

    public override void _Ready()
    {
       dice = GetChild<Spatial>(0);
    }


    public void RollDice(Vector3 direction)
    {
        dice.Call("roll", direction);
    }


    public abstract bool ExecuteTurn();
}

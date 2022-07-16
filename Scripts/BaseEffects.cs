using Godot;
using System;
using System.Collections.Generic;

public interface IGridEffect
{
    int Duration { get;}
    Vector3[] Locations {get;}
    void ApplyEffect(DiceController dice);
    
}

public class FireEffect : IGridEffect
{
    public int Duration => 2;

    public Vector3[] Locations => new Vector3[]{Vector3.Right, Vector3.Left, Vector3.Forward, Vector3.Back};

    public void ApplyEffect(DiceController dice)
    {
        GD.Print("Dice is destroyed ", dice);
    }
}
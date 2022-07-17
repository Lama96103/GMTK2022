using Godot;
using System;

[Tool]
public class PlayerDiceController : DiceController
{

    public override bool ExecuteTurn()
    {
        if(Input.IsActionJustPressed("key_forward"))
        {
            return RollDice(Vector3.Forward);
        }
        if(Input.IsActionJustPressed("key_backward"))
        {
            return RollDice(Vector3.Back);
        }
        if(Input.IsActionJustPressed("key_right"))
        {
            return RollDice(Vector3.Right);
        }
        if(Input.IsActionJustPressed("key_left"))
        {
            return RollDice(Vector3.Left);
        }
        
        return false;
    }

     public void SetDead()
    {
        PackedScene particleScene = (PackedScene)ResourceLoader.Load("res://Particles/BurningEffect.tscn");
        Spatial particle = particleScene.Instance<Spatial>();
        particle.Translate(dice.Translation);
        this.AddChild(particle);
    }
}

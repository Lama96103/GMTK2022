using Godot;
using System;
using System.Collections.Generic;

public interface IGridEffect
{
    int Duration { get;}
    string ParticleEffectPath {get;}
    string ParticleSoundPath {get;}
    string DiceMaterialPath {get;}
    Vector3[] Locations {get;}
    void ApplyEffect(DiceController dice);
    
}

public class FireEffect : IGridEffect
{
    public int Duration => 1;
    public Vector3[] Locations => new Vector3[]{Vector3.Right, Vector3.Left, Vector3.Forward, Vector3.Back};
    public string ParticleEffectPath => "res://Particles/StylizedFire/FireEffect.tscn";

    public string ParticleSoundPath => "res://Sounds and Music/Sounds/mixkit-fire-swoosh-burning-2s.wav";

    public string DiceMaterialPath => "res://Materials/Dice_FireEffect.tres";

    public void ApplyEffect(DiceController dice)
    {
        if(dice.IsInGroup("EnemyDice"))
        {
            dice.QueueFree();
            dice.CurrentLevel.RemoveDice(dice);
        }
        else
        {
            WorldController.Instance.LoadDeathMenu();
        }
    }
}

public class IceEffect : IGridEffect
{
    private Vector3 effectDirection;

    public int Duration => 3;

    public string ParticleEffectPath => "res://Particles/IceEffect.tscn";

    public string ParticleSoundPath => "res://Sounds and Music/Sounds/Freeze.wav";
    public string DiceMaterialPath => "res://Materials/Dice_IceEffect.tres";

    public Vector3[] Locations => new Vector3[]{effectDirection, effectDirection * 2, effectDirection * 3};

    public IceEffect(Vector3 effectDirection)
    {
        this.effectDirection = effectDirection;
    }

    public void ApplyEffect(DiceController dice)
    {
        if(dice.IsInGroup("EnemyDice"))
        {
            EnemyDiceController enemyDice = (EnemyDiceController) dice;
            enemyDice.setFrozen();
        }
    }
}
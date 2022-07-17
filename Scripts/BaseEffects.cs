using Godot;
using System;
using System.Collections.Generic;

public interface IGridEffect
{
    int Duration { get;}
    string ParticleEffectPath {get;}
    string ParticleSoundPath {get;}
    string DiceMaterialPath {get;}
    bool EffectsPlayer {get;}
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

    public bool EffectsPlayer => true;

    public void ApplyEffect(DiceController dice)
    {
        if(dice.IsInGroup("EnemyDice"))
        {
            ((EnemyDiceController)dice).SetDead();
            dice.CurrentLevel.RemoveDice(dice);
        }
        else
        {
            ((PlayerDiceController)dice).SetDead();
            WorldController.Instance.LoadDeathMenu();
        }
    }
}

public class IceEffect : IGridEffect
{
    private Vector3 effectDirection;

    public int Duration => 3;

    public string ParticleEffectPath => "res://Particles/IceEffect.tscn";

    public string ParticleSoundPath => "res://Sounds and Music/Sounds/Freeze_louder.wav";
    public string DiceMaterialPath => "res://Materials/Dice_IceEffect.tres";
    
    public Vector3[] Locations => new Vector3[]{effectDirection, effectDirection * 2, effectDirection * 3};

    public bool EffectsPlayer => false;

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

public class MineEffect : IGridEffect
{
    public int Duration => 1000;

    public string ParticleEffectPath => "res://Nodes/Landmine.tscn";

    public string ParticleSoundPath => "res://Sounds and Music/Sounds/mixkit-putdown-mine.wav";

    public string DiceMaterialPath => "res://Materials/Dice_MineEffect.tres";

    public bool EffectsPlayer => true;

    public Vector3[] Locations => new Vector3[]{Vector3.Zero};

    public void ApplyEffect(DiceController dice)
    {
        AudioStreamSample audioStream = ResourceLoader.Load<AudioStreamSample>("res://Sounds and Music/Sounds/mixkit-explosion-hit.wav");
        dice.soundEffectSteamPlayer.Stream = audioStream;
        dice.soundEffectSteamPlayer.Play();
        if(dice.IsInGroup("EnemyDice"))
        {
            ((EnemyDiceController)dice).SetDead();
            dice.CurrentLevel.RemoveDice(dice);
        }
        else
        {
            ((PlayerDiceController)dice).SetDead();
            WorldController.Instance.LoadDeathMenu();
        }
    }
}
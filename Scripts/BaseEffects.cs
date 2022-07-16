using Godot;
using System;
using System.Collections.Generic;

public interface IGridEffect
{
    int Duration { get;}
    string ParticleEffectPath {get;}
    string ParticleSoundPath {get;}
    Vector3[] Locations {get;}
    void ApplyEffect(DiceController dice);
    
}

public class FireEffect : IGridEffect
{
    public int Duration => 1;
    public Vector3[] Locations => new Vector3[]{Vector3.Right, Vector3.Left, Vector3.Forward, Vector3.Back};
    public string ParticleEffectPath => "res://Particles/StylizedFire/FireEffect.tscn";

    public string ParticleSoundPath => "res://Sounds and Music/Sounds/mixkit-fire-swoosh-burning-2s.wav";

    public void ApplyEffect(DiceController dice)
    {
        GD.Print("Dice is destroyed ", dice);
    }
}
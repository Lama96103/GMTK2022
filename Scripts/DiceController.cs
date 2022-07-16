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
        PlaySound();
    }

    private void PlaySound()
    {
        String path = "res://Sounds and Music/Sounds/";
        int lastIndex = CurrentLevel.GetParent().GetChildCount();
        AudioStreamPlayer audioPlayer = CurrentLevel.GetParent().GetChild<AudioStreamPlayer>(lastIndex - 1);
        if(ActiveEffect() == null)
        {
            String[] diceSounds = new String[4];
            for(int i = 0; i < diceSounds.Length; i++)
            {
                diceSounds[i] = "Dice 0" + (i+1) + ".wav";
            }
            Random random = new Random();
            int selection = random.Next(0, diceSounds.Length);
            AudioStreamSample audioStream = ResourceLoader.Load<AudioStreamSample>(path + diceSounds[selection]);
            audioPlayer.Stream = audioStream;
            audioPlayer.Play();
        }
        if(ActiveEffect() == "FireEffect")
        {           
            AudioStreamSample audioStream = ResourceLoader.Load<AudioStreamSample>(path + "mixkit-fire-swoosh-burning-2s.wav");
            audioPlayer.Stream = audioStream;
            audioPlayer.Play();
        }
    }

    public String ActiveEffect()
    {
        return null;
    }

    public abstract bool ExecuteTurn();
}

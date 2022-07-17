using Godot;
using System;

public class MainMenu : Control
{

    public override void _Ready()
    {
        int sfx_index= AudioServer.GetBusIndex("Master");
        Slider masterSlider = GetNode<Slider>("Settings/Music/Master Control/HSlider");
        masterSlider.Value = AudioServer.GetBusVolumeDb(sfx_index);

        sfx_index= AudioServer.GetBusIndex("Music");
        Slider musicSlider = GetNode<Slider>("Settings/Music/Music Control/HSlider");
        musicSlider.Value = AudioServer.GetBusVolumeDb(sfx_index);

        sfx_index= AudioServer.GetBusIndex("SoundEffects");
        Slider effectSlider = GetNode<Slider>("Settings/Music/Effect Controll/HSlider");
        effectSlider.Value = AudioServer.GetBusVolumeDb(sfx_index);
    }

    public void ButtonStartGamePressed()
    {
        WorldController.CurrentLevelPath = "res://Nodes/Level/Level_001.tscn";
        this.GetTree().ChangeScene("res://Nodes/World.tscn");
    }

    public void ToggleSettings()
    {
        Control mainMenu = this.GetNode<Control>("MainMenu");
        Control setting = this.GetNode<Control>("Settings");

        mainMenu.Visible = !mainMenu.Visible;
        setting.Visible = !setting.Visible;
    }

    public void ExitGame()
    {
        GetTree().Quit();
    }

    public void OnMasterVolumeChange(float value)
    {
        int sfx_index= AudioServer.GetBusIndex("Master");
        AudioServer.SetBusVolumeDb(sfx_index, value);
    }

    public void OnMusicVolumeChange(float value)
    {
        int sfx_index= AudioServer.GetBusIndex("Music");
        AudioServer.SetBusVolumeDb(sfx_index, value);
    }

    public void OnEffectVolumeChange(float value)
    {
        int sfx_index= AudioServer.GetBusIndex("SoundEffects");
        AudioServer.SetBusVolumeDb(sfx_index, value);
    }
}

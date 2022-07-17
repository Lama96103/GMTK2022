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

        FillLevelScreen();
        
    }

    private void FillLevelScreen()
    {
        Control level = this.GetNode<Control>("Level/Levels");

        Directory dir = new Directory();
        dir.Open("res://Nodes/Level/");
        Error error = dir.ListDirBegin();

        string fileName = dir.GetNext();
        while(!string.IsNullOrEmpty(fileName))
        {
            if(fileName != "." && fileName != ".." )
            {
                string fullPath = "res://Nodes/Level/" + fileName;
                GD.Print(fileName);
                string name = fileName.Substring(0, fileName.Length - 5).Remove(0, "Level_".Length);

                if(int.TryParse(name, out int result))
                {
                    Label l = new Label();
                    l.Text = "Level " + result;
                    l.Align = Label.AlignEnum.Center;
                    l.SizeFlagsHorizontal = (int)(SizeFlags.Expand | SizeFlags.Fill); 

                    Button b = new Button();
                    b.Text = "Start";
                    b.SizeFlagsHorizontal = (int)(SizeFlags.Expand | SizeFlags.Fill); 
                    b.Connect("pressed", this, "LoadSpecificLevel", new Godot.Collections.Array(){fullPath});

                    level.AddChild(l);
                    level.AddChild(b);
                }

                
                
            }
            fileName = dir.GetNext();
        }
    }

    private void LoadSpecificLevel(string path)
    {
        WorldController.CurrentLevelPath = path;
        this.GetTree().ChangeScene("res://Nodes/World.tscn");
    }

    public void ButtonStartGamePressed()
    {
        WorldController.CurrentLevelPath = "res://Nodes/Level/Level_001.tscn";
        this.GetTree().ChangeScene("res://Nodes/World.tscn");
    }

    public void ToggleSettings(string state)
    {
        Control mainMenu = this.GetNode<Control>("MainMenu");
        Control setting = this.GetNode<Control>("Settings");
        Control level = this.GetNode<Control>("Level");

        mainMenu.Visible = state == "Main";
        setting.Visible = state == "Settings";
        level.Visible = state == "Levels";
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

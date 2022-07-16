using Godot;
using System;

[Tool]
public abstract class DiceController : Spatial
{
    [Export] Godot.Collections.Array<Vector3> EffectLocation = new Godot.Collections.Array<Vector3> ();
    [Export] Godot.Collections.Array<string> EffectType = new Godot.Collections.Array<string>();
    public LevelController CurrentLevel;

    private Godot.Collections.Dictionary<Vector3, RayCast> RayCasts = new Godot.Collections.Dictionary<Vector3, RayCast>();

    private Spatial dice;
    private RayCast rayCast;

    protected bool IsRolling = false;

    public override void _Ready()
    {
       dice = GetChild<Spatial>(0);
       dice.Connect("DiceRolled", this, "OnDiceRolled");
       SpawnDiceSides();
    }

    private void SpawnDiceSides()
    {
        Node mesh = dice.GetChild(0).GetChild(0);

        RayCasts.Clear();

        int childCount = mesh.GetChildCount();
        for(int i = 0; i < childCount; i++)
            mesh.RemoveChild(mesh.GetChild(0));

        foreach(Vector3 pos in EffectLocation)
        {
            MeshInstance meshInstance = new MeshInstance();
            PlaneMesh planeMesh = new PlaneMesh();
            meshInstance.Mesh = planeMesh;
            planeMesh.Size = new Vector2(0.8f, 0.8f);
            meshInstance.Translate(pos * 0.51f);

          

            Vector3 rotation = Vector3.Up.Cross(pos);
            if(rotation.Length() > 0)
            {
                meshInstance.RotateObjectLocal(rotation, Mathf.Pi / 2);
            }
            else
            {
                if(pos.y == -1) 
                {
                    meshInstance.RotateX(Mathf.Pi);
                }
            }
            SpatialMaterial material = new SpatialMaterial();
            material.AlbedoColor = new Color(1,0,0);
            meshInstance.MaterialOverride = material;

            rayCast = new RayCast();
            rayCast.CollideWithAreas = true;
            rayCast.Enabled =true;
            rayCast.CastTo = pos * 10;
            mesh.AddChild(rayCast);
            RayCasts.Add(pos, rayCast);

            mesh.AddChild(meshInstance);

            
        }

        
    }


    public void RollDice(Vector3 direction)
    {
        IsRolling = true;
        dice.Call("roll", direction);
        PlaySound();
    }

    private void OnDiceRolled()
    {
        IsRolling = false;

        foreach(var item in RayCasts)
        {
            if(item.Value.IsColliding())
            {
                GD.Print("Fired " , this.Name);
                int index = EffectLocation.IndexOf(item.Key);
                CurrentLevel.AddEffect(new FireEffect(), dice.GlobalTransform.origin);
            }
        }

        CurrentLevel.UpdateDiceLocation(this, this.GlobalTransform.origin);
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

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

    public bool IsRolling = false;

    public Vector3 currentDirection;

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

        int index = 0;;
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
            
            string effectType = EffectType[index];
            IGridEffect effect = GetEffect(effectType, Vector3.Zero);
            
            SpatialMaterial material = new SpatialMaterial();
            material.AlbedoColor = new Color(1,0,0);
            meshInstance.MaterialOverride = ResourceLoader.Load<Material>(effect.DiceMaterialPath);

            rayCast = new RayCast();
            rayCast.CollideWithAreas = true;
            rayCast.Enabled =true;
            rayCast.CastTo = pos * 10;
            mesh.AddChild(rayCast);
            RayCasts.Add(pos, rayCast);

            mesh.AddChild(meshInstance);

            index++;
        }

        
    }


    public bool RollDice(Vector3 direction)
    {
        if(CurrentLevel.CanMoveTo(dice.GlobalTransform.origin + direction))
        {
            IsRolling = true;
            currentDirection = direction;
            dice.Call("roll", direction);
            PlaySound(null);
            return true;
        }
        return false;
    }

    private void OnDiceRolled()
    {
        CurrentLevel.UpdateDiceLocation(this, dice.GlobalTransform.origin);

        foreach(var item in RayCasts)
        {
            if(item.Value.IsColliding())
            {
                GD.Print("Fired " , this.Name);
                int index = EffectLocation.IndexOf(item.Key);
                IGridEffect effect = GetEffect(EffectType[index], currentDirection);
                CurrentLevel.AddEffect(effect, dice.GlobalTransform.origin, this);
                PlaySound(effect);
            }
        }

        AfterDiceRolled();
        IsRolling = false;
    }

    protected virtual void AfterDiceRolled(){}

    private IGridEffect GetEffect(string effectName, Vector3 dir )
    {
        if(effectName.Contains("Ice")) return new IceEffect(dir);
        else return new FireEffect();
    }

    private void PlaySound(IGridEffect effect)
    {
        String path = "res://Sounds and Music/Sounds/";
        AudioStreamPlayer audioPlayer = CurrentLevel.GetParent().GetNode<AudioStreamPlayer>("AudioStreamPlayerSounds");
        if(effect == null)
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
        }else
        {
            AudioStreamSample audioStream = ResourceLoader.Load<AudioStreamSample>(effect.ParticleSoundPath);
            audioPlayer.Stream = audioStream;
            audioPlayer.Play();
        }
    }


    public abstract bool ExecuteTurn();
}

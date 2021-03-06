using Godot;
using System;

[Tool]
public abstract class DiceController : Spatial
{
    [Export] Godot.Collections.Array<Vector3> EffectLocation = new Godot.Collections.Array<Vector3> ();
    [Export] Godot.Collections.Array<string> EffectType = new Godot.Collections.Array<string>();
    [Export] Color diceColor = new Color();
    public LevelController CurrentLevel;

    private Godot.Collections.Dictionary<Vector3, RayCast> RayCasts = new Godot.Collections.Dictionary<Vector3, RayCast>();

    protected Spatial dice;
    private RayCast rayCast;

    public bool IsRolling = false;

    public Vector3 currentDirection;
    public AudioStreamPlayer3D soundEffectSteamPlayer;

    public override void _Ready()
    {
       dice = GetChild<Spatial>(0);
       dice.Connect("DiceRolled", this, "OnDiceRolled");
       MeshInstance mi = (MeshInstance) dice.GetChild(0).GetChild(0);
       SpatialMaterial spatialMat = new SpatialMaterial();
       spatialMat.AlbedoColor = diceColor;
       mi.MaterialOverride = spatialMat;

       soundEffectSteamPlayer = new AudioStreamPlayer3D();
       this.AddChild(soundEffectSteamPlayer);
       soundEffectSteamPlayer.UnitSize = 10;
       soundEffectSteamPlayer.Bus = "SoundEffects";
       
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
        if(direction == Vector3.Zero) return true;
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
        bool gotEffect = CurrentLevel.UpdateDiceLocation(this, dice.GlobalTransform.origin);

        if(!gotEffect)
        {
            foreach(var item in RayCasts)
            {
                if(item.Value.IsColliding())
                {
                    int index = EffectLocation.IndexOf(item.Key);
                    IGridEffect effect = GetEffect(EffectType[index], currentDirection);
                    CurrentLevel.AddEffect(effect, dice.GlobalTransform.origin, this);
                    PlaySound(effect);
                }
            }
        }
        AfterDiceRolled();
        IsRolling = false;
    }

    protected virtual void AfterDiceRolled(){}

    private IGridEffect GetEffect(string effectName, Vector3 dir )
    {
        if(effectName.Contains("Ice")) return new IceEffect(dir);
        if(effectName.Contains("Landmine")) return new MineEffect();
        else return new FireEffect();
    }

    private void PlaySound(IGridEffect effect)
    {
        String path = "res://Sounds and Music/Sounds/";
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
            soundEffectSteamPlayer.Stream = audioStream;
            soundEffectSteamPlayer.Play();
        }else
        {
            AudioStreamSample audioStream = ResourceLoader.Load<AudioStreamSample>(effect.ParticleSoundPath);
            soundEffectSteamPlayer.Stream = audioStream;
            soundEffectSteamPlayer.Play();
        }
    }


    public abstract bool ExecuteTurn();
}

using Godot;
using System;
using Godot.Collections;
using System.Collections.Generic;

[Tool]
public class LevelController : Spatial
{
    [Export] private float SizeX = 10.5f;
    [Export] private float SizeZ = 10.5f;

    private System.Collections.Generic.Dictionary<Vector3, GridState> gridStatus = new System.Collections.Generic.Dictionary<Vector3, GridState>();

    [Export] private Mesh GridMesh;

    private int currentDice = 0;
    private Array<DiceController> executionOrder;

    public override void _Ready()
    {
        DrawGrid();
    }

    public override void _Process(float delta)
    {
        if(Engine.EditorHint) return;
        if(executionOrder == null)
        {
            FindDice();
        }
        
        if(executionOrder[currentDice].ExecuteTurn())
        {
            currentDice = (currentDice + 1) % executionOrder.Count;
        } 
    }

    private void FindDice()
    {
        currentDice = 0;
        executionOrder = new Array<DiceController>();
        DiceController playerDice = (DiceController) this.GetTree().GetNodesInGroup("Player")[0];
        executionOrder.Add(playerDice);
        foreach(DiceController dice in this.GetTree().GetNodesInGroup("EnemyDice"))
        {
            executionOrder.Add(dice);
        } 

        foreach(DiceController dice in executionOrder)
        {
            dice.CurrentLevel = this;
        }
    }

    public void AddEffect(IGridEffect effect, Vector3 position)
    {
        PackedScene particleEffect = ResourceLoader.Load<PackedScene>(effect.ParticleEffectPath);

        foreach(Vector3 loc in effect.Locations)
        {
            Vector3 affectedPos = position + loc;
            if(!gridStatus.ContainsKey(affectedPos)) gridStatus.Add(affectedPos, new GridState());


            // ToDo: Catch if there is already an effect present

            gridStatus[affectedPos].Effect = effect;
            gridStatus[affectedPos].EffectDuration = effect.Duration;

            if(gridStatus[affectedPos].Dice != null)
                gridStatus[affectedPos].Effect.ApplyEffect(gridStatus[affectedPos].Dice);

            Spatial particleEffectObject = particleEffect.Instance<Spatial>();
            this.AddChild(particleEffectObject);
            particleEffectObject.TranslateObjectLocal(affectedPos);
            gridStatus[affectedPos].ParticleEffects.Add(particleEffectObject);
        }
    }

    public void UpdateDiceLocation(DiceController dice, Vector3 position)
    {
        bool foundLocation = false;
        foreach(var item in gridStatus)
        {
            if(item.Value.Dice == dice) item.Value.Dice = null;

            if(item.Key == position)
            {
                foundLocation = true;
                item.Value.Dice = dice;

                if(item.Value.Effect != null)
                {
                    item.Value.Effect.ApplyEffect(dice);
                }
            }
        }

        if(!foundLocation)
        {
            gridStatus.Add(position, new GridState());
            gridStatus[position].Dice = dice;
        }
    }

    public void OnRoundStart()
    {
        foreach(var item in gridStatus)
        {
            if(item.Value.Effect != null)
            {
                item.Value.EffectDuration--;

                if(item.Value.EffectDuration <= 0)
                {
                    foreach(Spatial particle in item.Value.ParticleEffects)
                    {
                        this.RemoveChild(particle);
                    }
                    item.Value.Effect = null;
                    item.Value.ParticleEffects.Clear();
                }
            }
        }
    }
    private void DrawGrid()
    {
        MultiMeshInstance multiMeshInstance = GetChild<MultiMeshInstance>(0);

        MultiMesh mesh = new MultiMesh();
        mesh.Mesh = GridMesh;
        multiMeshInstance.Multimesh = mesh;

        mesh.TransformFormat = MultiMesh.TransformFormatEnum.Transform3d;
        mesh.ColorFormat = MultiMesh.ColorFormatEnum.Color8bit;
        mesh.CustomDataFormat = MultiMesh.CustomDataFormatEnum.None;

        mesh.InstanceCount =  1000;
        float y = 0;
        int index = 0;
        for(float x = -SizeX; x <= SizeX; x++)
        {
            for(float z = -SizeZ; z <= SizeZ; z++)
            {
                //float colorVal = random.RandfRange(0.3f, 0.8f);
                Color color = new Color(1, 1, 1);


                Transform transform = Transform.Identity;
                //transform = transform.Rotated(new Vector3(0,1,0), random.Randf());
                transform.origin = new Vector3(x, y, z);

                mesh.SetInstanceTransform(index, transform);
                mesh.SetInstanceColor(index, color);
                index++;

                transform = Transform.Identity;
                transform = transform.Rotated(new Vector3(0,1,0), Mathf.Deg2Rad(90));
                transform.origin = new Vector3(x, y, z);

                mesh.SetInstanceTransform(index, transform);
                mesh.SetInstanceColor(index, color);
                index++;
            }
        }
        mesh.VisibleInstanceCount = index;
    }
}

public class GridState
{
    public DiceController Dice = null;

    public IGridEffect Effect = null;
    public int EffectDuration;

    public List<Spatial> ParticleEffects = new List<Spatial>();
}


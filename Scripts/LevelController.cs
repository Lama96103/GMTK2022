using Godot;
using System;
using Godot.Collections;
using System.Collections.Generic;

[Tool]
public class LevelController : Spatial
{
    [Export] private float SizeX = 15.5f;
    [Export] private float SizeZ = 15.5f;
    [Export] public string NextLevel = "";

    private System.Collections.Generic.Dictionary<Vector3, GridState> gridStatus = new System.Collections.Generic.Dictionary<Vector3, GridState>();

    [Export] private Mesh GridMesh;

    private int currentDice = 0;
    private Array<DiceController> executionOrder;
    private Array<DiceController> alreadyExecutedDice = new Array<DiceController>();

    private bool startedRolling = false;
    private float enemyCountDown = -1;

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

        if(currentDice != 0 && enemyCountDown > 0)
        {
            enemyCountDown -= delta;
            return;
        }
        
        if(!startedRolling)
        {
            if(executionOrder[currentDice].ExecuteTurn())
            {
                alreadyExecutedDice.Add(executionOrder[currentDice]);
                startedRolling = true;
            }
        }
        else
        {
            if(!executionOrder[currentDice].IsRolling)
            {
                ProgressRound();
            }
        }
    }

    private void ProgressRound()
    {
        currentDice = (currentDice + 1) % executionOrder.Count;
        if(currentDice == 0)
            alreadyExecutedDice.Clear();


        startedRolling = false;
        enemyCountDown = 0.3f;
        if(currentDice == 0 || currentDice == 1)
        {
            CalulcateEffectsDuration();
        }

        if(alreadyExecutedDice.Contains(executionOrder[currentDice])) ProgressRound();
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

    public void AddEffect(IGridEffect effect, Vector3 position, DiceController dice)
    {
        PackedScene particleEffect = ResourceLoader.Load<PackedScene>(effect.ParticleEffectPath);

        foreach(Vector3 loc in effect.Locations)
        {
            Vector3 affectedPos = position + loc;
            if(!gridStatus.ContainsKey(affectedPos)) gridStatus.Add(affectedPos, new GridState());


            if(gridStatus[affectedPos].Effect != null)
            {   
                if(gridStatus[affectedPos].Effect.GetType() == typeof(FireEffect))
                    continue;
                else
                {
                    foreach(Spatial particle in gridStatus[affectedPos].ParticleEffects)
                    {
                        this.RemoveChild(particle);
                    }
                    gridStatus[affectedPos].Effect = null;
                    gridStatus[affectedPos].ParticleEffects.Clear();
                }
            }

            if(gridStatus[affectedPos].Effect != null)
            {
                 
            }


            gridStatus[affectedPos].Effect = effect;
            gridStatus[affectedPos].EffectDuration = effect.Duration;
            gridStatus[affectedPos].IsPlayerEffect = dice == executionOrder[0];

            if(gridStatus[affectedPos].Dice != null)
                gridStatus[affectedPos].Effect.ApplyEffect(gridStatus[affectedPos].Dice);

            Spatial particleEffectObject = particleEffect.Instance<Spatial>();
            this.AddChild(particleEffectObject);
            particleEffectObject.TranslateObjectLocal(affectedPos);
            gridStatus[affectedPos].ParticleEffects.Add(particleEffectObject);
        }
    }

    public void RemoveDice(DiceController dice)
    {
        executionOrder.Remove(dice);
        foreach(var item in gridStatus)
        {
            if(item.Value.Dice == dice) item.Value.Dice = null;
        }


        if(currentDice >= executionOrder.Count || executionOrder[currentDice] == null)
        {
            ProgressRound();
        }

        if(executionOrder.Count == 1)
        {
            WorldController.Instance.LoadNextLevel();
        }
    }

    public void UpdateDiceLocation(DiceController dice, Vector3 position)
    {
        bool foundLocation = false;
        foreach(var item in gridStatus)
        {
            if(item.Value.Dice == dice)
            {
                item.Value.Dice = null;
            } 

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

    public bool CanMoveTo(Vector3 position)
    {
        if(gridStatus.TryGetValue(position, out GridState value))
        {
            return value.Dice == null;
        }
        return true;
    }
    public void CalulcateEffectsDuration()
    {
        foreach(var item in gridStatus)
        {
            if(item.Value.Effect != null)
            {
                if(item.Value.IsPlayerEffect && currentDice != 0 ) continue;
                if(!item.Value.IsPlayerEffect && currentDice != 1 ) continue;

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

        mesh.InstanceCount =  5000;
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
    public bool IsPlayerEffect = true;

    public List<Spatial> ParticleEffects = new List<Spatial>();
}


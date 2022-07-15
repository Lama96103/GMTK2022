using Godot;
using System;
using System.Collections.Generic;

[Tool]
public class LevelController : Spatial
{
    [Export] private float SizeX = 10.5f;
    [Export] private float SizeZ = 10.5f;

    private Dictionary<Vector3, GridState> gridStatus = new Dictionary<Vector3, GridState>();

    public override void _Ready()
    {
        
    }

    public void AddEffect(IGridEffect effect, Vector3 position)
    {
        GD.Print("Adding effect add");
        foreach(Vector3 loc in effect.Locations)
        {
            Vector3 affectedPos = position + loc;
            if(!gridStatus.ContainsKey(affectedPos)) gridStatus.Add(affectedPos, new GridState());

            gridStatus[affectedPos].Effect = effect;
            gridStatus[affectedPos].EffectDuration = effect.Duration;
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

        GD.Print("Dice is at location ", position);
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        DrawGrid();
    }

    private void DrawGrid()
    {
        DebugControl debugControl = null;
        if(Engine.EditorHint)
        {
            debugControl = GetNodeOrNull<DebugControl>("DebugDraw/Control");
            if(debugControl == null)
            {
                PackedScene debugScene = ResourceLoader.Load("res://addons/DebugDraw.tscn") as PackedScene;
                Node debug = debugScene.Instance();
                AddChild(debug);
                GD.Print("Adding DebugControl");
                if(debugControl == null) return;        
            }
        }
        else
        {
            debugControl = DebugControl.Instance;
        }


        float y = -0.49f;
        for(float x = -SizeX; x <= SizeX; x++)
        {
            debugControl.DebugLine(new Vector3(x, y, -SizeZ), new Vector3(x, y, SizeZ), new Color(1,1,1, 0.5f));
        }

        for(float z = -SizeZ; z <= SizeZ; z++)
        {
            debugControl.DebugLine(new Vector3(-SizeX, y, z), new Vector3(SizeX, y, z), new Color(1,1,1, 0.5f));
        }
    }
}

public class GridState
{
    public DiceController Dice = null;

    public IGridEffect Effect = null;
    public int EffectDuration;
}


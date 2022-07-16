using Godot;
using System;
using Godot.Collections;
using System.Linq;

[Tool]
public class EnemyDiceController : DiceController
{

    [Export] private Array<Vector3> path = new Array<Vector3>();

    public override void _Ready()
    {
        base._Ready();
        ShowNextSteps();
    }

    public void Move()
    {
        if(path.Count() > 0)
        {
            RollDice(path[0]);
            path.RemoveAt(0);
            
        }
    }

    public void setFrozen()
    {
        path.Insert(0, new Vector3(0, 0, 0));
        path.Insert(0, new Vector3(0, 0, 0));
        path.Insert(0, new Vector3(0, 0, 0));
    }

    private void ShowNextSteps()
    {
        int pathMarkerCount = 0;
        foreach(Node node in this.GetChildren())
        {
            if(node.GetType().Equals(typeof(MeshInstance)))
            {
                MeshInstance stepMarker = (MeshInstance) node;
                stepMarker.Translation = this.GetChild<Spatial>(0).Translation;
                for(int i = 0; i <= pathMarkerCount; i++)
                {
                    if(i >= path.Count) break;
                    stepMarker.Translation += path[i];
                }
                pathMarkerCount++;
            }
        }
    }

    protected override void AfterDiceRolled()
    {
        ShowNextSteps();
    }

    public override bool ExecuteTurn()
    {
        if(IsRolling) return false;
        Move();
        return true;
    }
}

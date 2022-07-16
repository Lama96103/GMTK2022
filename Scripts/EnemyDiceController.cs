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
            ShowNextSteps();
        }
    }

    private void ShowNextSteps()
    {
        int pathMarkerCount = 0;
        foreach(Node node in this.GetChildren())
        {
            if(node.GetType().Equals(typeof(MeshInstance)))
            {
                MeshInstance stepMarker = (MeshInstance) node;
                for(int i = 0; i <= pathMarkerCount; i++)
                {
                    stepMarker.Translation = this.GetChild<Spatial>(0).Translation + path[i];
                }
            }
        }
    }

    public override bool ExecuteTurn()
    {
        if(IsRolling) return false;
        Move();
        return true;
    }
}

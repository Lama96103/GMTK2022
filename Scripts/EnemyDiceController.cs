using Godot;
using System;
using Godot.Collections;
using System.Linq;

[Tool]
public class EnemyDiceController : DiceController
{

    [Export] private Array<Vector3> path = new Array<Vector3>();

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
        int pathMarkerCount = this.GetTree().GetNodesInGroup("PathMarker").Count;
        int selectedPathMarker = 0;
        foreach(Vector3 step in path)
        {
            if(selectedPathMarker < pathMarkerCount)
            {
                MeshInstance stepMarker = (MeshInstance) this.GetTree().GetNodesInGroup("PathMarker")[selectedPathMarker];
                if(stepMarker.GetParent().Equals(this))
                {
                    stepMarker.Translation = this.GetChild<Spatial>(0).Translation + step;
                    selectedPathMarker++;
                }
            }else
            {
                break;
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

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
        if(path.Count > 0) path.RemoveAt(path.Count - 1);
        int selectedPathMarker = 0;
        foreach(Vector3 step in path)
        {
            if(selectedPathMarker < pathMarkerCount)
            {
                MeshInstance stepMarker = (MeshInstance) this.GetTree().GetNodesInGroup("PathMarker")[selectedPathMarker];
                stepMarker.Translate(step);
                GD.Print("newCords", step);
                selectedPathMarker++;
            }
        }
    }

    public override bool ExecuteTurn()
    {
        Move();
        return true;
    }
}

using Godot;
using System;
using Godot.Collections;
using System.Linq;

[Tool]
public class EnemyDiceController : DiceController
{

    [Export] private Array<Vector3> path = new Array<Vector3>();
    [Export] private bool Patrol = false;

    private Array<Vector3> currentPath = new Array<Vector3>();
    private bool lastPathDirection = false;



    public override void _Ready()
    {
        base._Ready();
        FillPath(false);
        ShowNextSteps();
    }

    private void FillPath(bool reverse)
    {
        currentPath.Clear();
        if(reverse) foreach(Vector3 pos in path) currentPath.Insert(0, pos * -1);
        else foreach(Vector3 pos in path) currentPath.Add(pos);
    }

    public void Move()
    {
        if(currentPath.Count > 0)
        {
            RollDice(currentPath[0]);
            currentPath.RemoveAt(0);

            if(Patrol && currentPath.Count == 0)
            {
                lastPathDirection = !lastPathDirection;
                FillPath(lastPathDirection);
            }
        }
    }

    public void setFrozen()
    {
        currentPath.Insert(0, new Vector3(0, 0, 0));
        currentPath.Insert(0, new Vector3(0, 0, 0));
        currentPath.Insert(0, new Vector3(0, 0, 0));
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
                    if(i >= currentPath.Count) break;
                    stepMarker.Translation += currentPath[i];
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

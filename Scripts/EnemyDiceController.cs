using Godot;
using System;
using Godot.Collections;
using System.Linq;

public class EnemyDiceController : DiceController
{

    [Export] private Array<Vector3> path = new Array<Vector3>();

    public void Move()
    {
        if(path.Count() > 0)
        {
            RollDice(path[0]);
            path.RemoveAt(0);
        }
    }

    public override bool ExecuteTurn()
    {
        Move();
        return true;
    }
}

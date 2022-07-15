using Godot;
using System;
using Godot.Collections;

public class TurnController : Node
{
    private int currentDice = 0;
    private Array<DiceController> executionOrder;

    private LevelController levelController;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
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
        executionOrder.Add(GetChild<DiceController>(0));
        foreach(DiceController dice in this.GetTree().GetNodesInGroup("EnemyDice"))
        {
            executionOrder.Add(dice);
        }

        levelController = GetChild<LevelController>(3);
        

        foreach(DiceController dice in executionOrder)
        {
            dice.CurrentLevel = levelController;
        }
    }

}

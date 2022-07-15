using Godot;
using System;

public abstract class DiceController : Spatial
{

    [Export] private float RollDuration = 1;

    private Vector3 startPosition;
    private Vector3 endPosition;

    private Vector3 startRotation;
    private Vector3 endRotation;
    private float progress;

    private bool isRolling = false;

    public override void _Ready()
    {
        //RollDice(Vector3.Right);
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        if(isRolling)
        {
            progress += (delta * 5);

            if(progress < 1) 
            {
                Vector3 newPosition = startPosition.LinearInterpolate(endPosition, progress);
                this.Translation = newPosition;

                Vector3 newRotation = startRotation.LinearInterpolate(endRotation, progress);
                this.GetChild<Spatial>(0).RotationDegrees = newRotation;
            }
            else
            {
                isRolling = false;
                this.Translation = endPosition;
                this.GetChild<Spatial>(0).RotationDegrees = endRotation;
            }

            
        }


        if(Input.IsActionJustPressed("key_forward"))
        {
            RollDice(Vector3.Forward);
        }
        if(Input.IsActionJustPressed("key_backward"))
        {
            RollDice(Vector3.Back);
        }
        if(Input.IsActionJustPressed("key_right"))
        {
            RollDice(Vector3.Right);
        }
        if(Input.IsActionJustPressed("key_left"))
        {
            RollDice(Vector3.Left);
        }

    }


    public void RollDice(Vector3 direction)
    {
        if(isRolling) return;
        startPosition = this.Translation;
        endPosition = this.Translation + direction;

        startRotation = this.GetChild<Spatial>(0).RotationDegrees;

        
        if(direction == Vector3.Right) endRotation = this.GetChild<Spatial>(0).RotationDegrees + (Vector3.Forward * 90);
        if(direction == Vector3.Left) endRotation = this.GetChild<Spatial>(0).RotationDegrees + (Vector3.Back * 90);
        if(direction == Vector3.Forward) endRotation = this.GetChild<Spatial>(0).RotationDegrees + (Vector3.Left * 90);
        if(direction == Vector3.Back) endRotation = this.GetChild<Spatial>(0).RotationDegrees + (Vector3.Right * 90);

        progress = 0;
        isRolling = true;
    }
}
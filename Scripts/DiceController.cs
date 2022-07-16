using Godot;
using System;

public abstract class DiceController : Spatial
{

    [Export] private float RollDuration = 1;

    private Vector3 startPosition;
    private Vector3 endPosition;

    private Spatial currentGimbal;
    private Vector3 startRotation;
    private Vector3 endRotation;
    private float progress;

    private bool isRolling = false;

    public LevelController CurrentLevel = null;



    public override void _Ready()
    {
        //RollDice(Vector3.Right);
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
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
                currentGimbal.Rotation = newRotation;
            }
            else
            {
                isRolling = false;
                this.Translation = endPosition;
                currentGimbal.Rotation = endRotation;

                CurrentLevel.UpdateDiceLocation(this, this.Translation);

                CurrentLevel.AddEffect(new FireEffect(), this.Translation);
            }
        }
    }

    public void RollDice(Vector3 direction)
    {
        if(isRolling) return;
        startPosition = this.Translation;
        endPosition = this.Translation + direction;

        


        currentGimbal = this.GetChild<Spatial>(0);
        startRotation = currentGimbal.Rotation;


        endRotation = startRotation + (Vector3.Up.Cross(direction.Rotated(startRotation.Normalized(), startRotation.Length())) * (Mathf.Pi / 2));

        /*
        if(direction == Vector3.Forward )
            endRotation = startRotation + (currentGimbal.GlobalTransform.basis.y.Cross(-currentGimbal.GlobalTransform.basis.z) * Mathf.Deg2Rad(90));
        if(direction == Vector3.Back )
            endRotation = startRotation + (currentGimbal.GlobalTransform.basis.y.Cross(currentGimbal.GlobalTransform.basis.z) * Mathf.Deg2Rad(90));
        if(direction == Vector3.Left )
            endRotation = startRotation + (currentGimbal.GlobalTransform.basis.y.Cross(-currentGimbal.GlobalTransform.basis.x) * Mathf.Deg2Rad(90));
        if(direction == Vector3.Right )
            endRotation = startRotation + (currentGimbal.GlobalTransform.basis.y.Cross(currentGimbal.GlobalTransform.basis.x) * Mathf.Deg2Rad(90));
        */
       
        

        progress = 0;
        isRolling = true;
    }   

    private Vector3 Rad2Deg(Vector3 inV)
    {
        return new Vector3(Mathf.Rad2Deg(inV.x), Mathf.Rad2Deg(inV.y), Mathf.Rad2Deg(inV.z));
    }


    public abstract bool ExecuteTurn();
}

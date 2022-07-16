using Godot;
using System;

public class CameraController : Spatial
{

    private float progress = 0;
    private bool isRotating = false;
    private Vector3 startRotation;
    private Vector3 endRotation;

    public override void _Process(float delta)
    {
        if(isRotating)
        {
            progress += (delta * 5);

            if(progress < 1) 
            {
                Vector3 newRotation = startRotation.LinearInterpolate(endRotation, progress);
                this.RotationDegrees = newRotation;
            }
            else
            {
                isRotating = false;
                this.RotationDegrees = endRotation;
            }
        }
        else
        {
            if(Input.IsActionJustPressed("camera_rotate_left"))
            {
                RotateCamera(-1);
            }
            if(Input.IsActionJustPressed("camera_rotate_right"))
            {
                RotateCamera(1);
            }
        }


    }

    public void RotateCamera(float dir)
    {
        if(isRotating) return;

        startRotation = this.RotationDegrees;

        endRotation = this.RotationDegrees + (Vector3.Up * 90 * dir);

        

        progress = 0;
        isRotating = true;
    }
}

using Godot;
using System;

public class CameraController : Spatial
{
    public override void _Process(float delta)
    {
        if(Input.IsActionJustPressed("camera_rotate_left"))
        {
            this.RotateY(Mathf.Deg2Rad(90));
        }
        if(Input.IsActionJustPressed("camera_rotate_right"))
        {
            this.RotateY(Mathf.Deg2Rad(-90));
        }
    }
}

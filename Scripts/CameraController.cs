using Godot;
using System;

public class CameraController : Spatial
{

    private float progress = 0;
    private bool isRotating = false;
    private Vector3 startRotation;
    private Vector3 endRotation;

    private float startNavigationRotation;
    private float endNavigationRotation;

    private TextureRect navigationCompass;
    private TextureRect navigationCompassHint;
    private int currentNavigationCompoassHintIndex = 0;
    [Export] private Texture[] navigationCompassHintTextures;

    public override void _Ready()
    {
        base._Ready();
        navigationCompass = GetParent().GetNode("Game_UI").GetNode("Compass").GetChild<TextureRect>(0);
        navigationCompassHint = GetParent().GetNode("Game_UI").GetNode("Compass").GetChild<TextureRect>(1);
    }

    public override void _Process(float delta)
    {
        if(isRotating)
        {
            progress += (delta * 5);

            if(progress < 1) 
            {
                Vector3 newRotation = startRotation.LinearInterpolate(endRotation, progress);
                this.RotationDegrees = newRotation;
                Mathf.Lerp(startNavigationRotation, endNavigationRotation, progress);
                navigationCompass.RectRotation = Mathf.Lerp(startNavigationRotation, endNavigationRotation, progress);
            }
            else
            {
                isRotating = false;
                this.RotationDegrees = endRotation;
                navigationCompass.RectRotation = endNavigationRotation;
                navigationCompassHint.Texture = navigationCompassHintTextures[currentNavigationCompoassHintIndex];
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

        startNavigationRotation = navigationCompass.RectRotation;
        endNavigationRotation = startNavigationRotation + 90 * dir;
        navigationCompassHint.Texture = null;

        currentNavigationCompoassHintIndex += (int)dir;

        if(currentNavigationCompoassHintIndex < 0) currentNavigationCompoassHintIndex = 3;
        if(currentNavigationCompoassHintIndex > 3) currentNavigationCompoassHintIndex = 0;

        

        progress = 0;
        isRotating = true;
    }
}

using Godot;
using System;

struct DebugLine : IDebugDrawable
{
	public Color color;
	public Vector2 start;
	public Vector2 end;

    public DebugLine(Color color, Vector2 start, Vector2 end)
    {
        this.color = color;
        this.start = start;
        this.end = end;
    }

    public void DrawElement(Control control)
    {
        control.DrawLine(start, end, color);
    }
}

struct DebugBox : IDebugDrawable
{
	public DebugLine[] Elements;

    public void DrawElement(Control control)
    {
        foreach(DebugLine line in Elements)
		{
			line.DrawElement(control);
		}
    }
}

interface IDebugDrawable
{
	void DrawElement(Control control);
}

class DebugTimeElement
{
	public IDebugDrawable Drawable;
	public float Lifetime;

	public DebugTimeElement(IDebugDrawable drawable, float seconds)
	{
		this.Drawable = drawable;
		this.Lifetime = seconds;
	}
}

[Tool]
public class DebugControl : Control
{
	public static DebugControl Instance {get; private set;}
	private Camera camera;
	private Label debugLabel;

	private float deltaTime = 0;
	private IDebugDrawable[] debugElements = new IDebugDrawable[512];
	private int currentDebugElement = 0;

	private System.Collections.Generic.List<DebugTimeElement> debugTimeElements = new System.Collections.Generic.List<DebugTimeElement>();



	public override void _Ready()
	{
		Instance = this;
		camera = GetCamera();
		debugLabel = GetParent().GetNode<Label>("Label");
	}
	
	public void DebugLine(Vector3 start, Vector3 end, Color color, bool elevate = false)
	{	
		if(camera == null) return;
		if(camera.IsPositionBehind(start) || camera.IsPositionBehind(end)) return;
		if(currentDebugElement < debugElements.Length)
		{
			if(elevate) 
			{
				start.y += 2;
				end.y += 2;
			}
			debugElements[currentDebugElement] = new DebugLine(color, camera.UnprojectPosition(start), camera.UnprojectPosition(end));

			currentDebugElement++;
		}
		else
		{
			GD.Print("Drawing to many DebugElements elements");	
		}
	}

	public void DebugBox(Vector3 pos, float size, Color color)
	{	
		DebugBox(pos, new Vector3(size, size, size), color);
	}

	public void DebugBox(Vector3 pos, Vector3 size, Color color)
	{	
		if(camera == null) return;
		if(camera.IsPositionBehind(pos)) return;
		if(camera.IsPositionBehind(pos + Vector3.Up * size)) return;
		if(camera.IsPositionBehind(pos + Vector3.Down * size)) return;
		if(camera.IsPositionBehind(pos + Vector3.Right * size)) return;
		if(camera.IsPositionBehind(pos + Vector3.Left * size)) return;
		if(camera.IsPositionBehind(pos + Vector3.Forward * size)) return;
		if(camera.IsPositionBehind(pos + Vector3.Back * size)) return;
		if(currentDebugElement < debugElements.Length)
		{
			size /= 2;

			DebugBox newBox = new DebugBox();
			newBox.Elements = new DebugLine[12];

			// Top Panel
			newBox.Elements[0] = new DebugLine(color, camera.UnprojectPosition(pos + (new Vector3(-1, 1, -1) * size)), camera.UnprojectPosition(pos + (new Vector3(1, 1, -1) * size)));
			newBox.Elements[1] = new DebugLine(color, camera.UnprojectPosition(pos + (new Vector3(1, 1, -1) * size)), camera.UnprojectPosition(pos + (new Vector3(1, 1, 1) * size)));
			newBox.Elements[2] = new DebugLine(color, camera.UnprojectPosition(pos + (new Vector3(1, 1, 1) * size)), camera.UnprojectPosition(pos + (new Vector3(-1, 1, 1) * size)));
			newBox.Elements[3] = new DebugLine(color, camera.UnprojectPosition(pos + (new Vector3(-1, 1, 1) * size)), camera.UnprojectPosition(pos + (new Vector3(-1, 1, -1) * size)));

			// Bottom Panel
			newBox.Elements[4] = new DebugLine(color, camera.UnprojectPosition(pos + (new Vector3(-1, -1, -1) * size)), camera.UnprojectPosition(pos + (new Vector3(1, -1, -1) * size)));
			newBox.Elements[5] = new DebugLine(color, camera.UnprojectPosition(pos + (new Vector3(1, -1, -1) * size)), camera.UnprojectPosition(pos + (new Vector3(1, -1, 1) * size)));
			newBox.Elements[6] = new DebugLine(color, camera.UnprojectPosition(pos + (new Vector3(1, -1, 1) * size)), camera.UnprojectPosition(pos + (new Vector3(-1, -1, 1) * size)));
			newBox.Elements[7] = new DebugLine(color, camera.UnprojectPosition(pos + (new Vector3(-1, -1, 1) * size)), camera.UnprojectPosition(pos + (new Vector3(-1, -1, -1) * size)));

			// Connections Top Bottom
			newBox.Elements[8] = new DebugLine(color, camera.UnprojectPosition(pos + (new Vector3(-1, -1, -1) * size)), camera.UnprojectPosition(pos + (new Vector3(-1, 1, -1) * size)));
			newBox.Elements[9] = new DebugLine(color, camera.UnprojectPosition(pos + (new Vector3(1, -1, -1) * size)), camera.UnprojectPosition(pos + (new Vector3(1, 1, -1) * size)));
			newBox.Elements[10] = new DebugLine(color, camera.UnprojectPosition(pos + (new Vector3(1, -1, 1) * size)), camera.UnprojectPosition(pos + (new Vector3(1, 1, 1) * size)));
			newBox.Elements[11] = new DebugLine(color, camera.UnprojectPosition(pos + (new Vector3(-1, -1, 1) * size)), camera.UnprojectPosition(pos + (new Vector3(-1, 1, 1) * size)));
			debugElements[currentDebugElement] = newBox;

			currentDebugElement++;
		}
		else
		{
			GD.Print("Drawing to many DebugElements elements");	
		}
	}

	public void DebugLineTime(Vector3 start, Vector3 end, Color color, float seconds)
	{
		if(camera.IsPositionBehind(start) || camera.IsPositionBehind(end)) return;
		DebugLine line = new DebugLine(color, camera.UnprojectPosition(start), camera.UnprojectPosition(end));
		debugTimeElements.Add(new DebugTimeElement(line, seconds));
	}

	public void DebugBoxTime(Vector3 pos, float size, Color color, float seconds)
	{
		if(camera.IsPositionBehind(pos)) return;
		

		size /= 2;
		DebugBox newBox = new DebugBox();
		newBox.Elements = new DebugLine[12];

		// Top Panel
		newBox.Elements[0] = new DebugLine(color, camera.UnprojectPosition(pos + (new Vector3(-1, 1, -1) * size)), camera.UnprojectPosition(pos + (new Vector3(1, 1, -1) * size)));
		newBox.Elements[1] = new DebugLine(color, camera.UnprojectPosition(pos + (new Vector3(1, 1, -1) * size)), camera.UnprojectPosition(pos + (new Vector3(1, 1, 1) * size)));
		newBox.Elements[2] = new DebugLine(color, camera.UnprojectPosition(pos + (new Vector3(1, 1, 1) * size)), camera.UnprojectPosition(pos + (new Vector3(-1, 1, 1) * size)));
		newBox.Elements[3] = new DebugLine(color, camera.UnprojectPosition(pos + (new Vector3(-1, 1, 1) * size)), camera.UnprojectPosition(pos + (new Vector3(-1, 1, -1) * size)));

		// Bottom Panel
		newBox.Elements[4] = new DebugLine(color, camera.UnprojectPosition(pos + (new Vector3(-1, -1, -1) * size)), camera.UnprojectPosition(pos + (new Vector3(1, -1, -1) * size)));
		newBox.Elements[5] = new DebugLine(color, camera.UnprojectPosition(pos + (new Vector3(1, -1, -1) * size)), camera.UnprojectPosition(pos + (new Vector3(1, -1, 1) * size)));
		newBox.Elements[6] = new DebugLine(color, camera.UnprojectPosition(pos + (new Vector3(1, -1, 1) * size)), camera.UnprojectPosition(pos + (new Vector3(-1, -1, 1) * size)));
		newBox.Elements[7] = new DebugLine(color, camera.UnprojectPosition(pos + (new Vector3(-1, -1, 1) * size)), camera.UnprojectPosition(pos + (new Vector3(-1, -1, -1) * size)));

		// Connections Top Bottom
		newBox.Elements[8] = new DebugLine(color, camera.UnprojectPosition(pos + (new Vector3(-1, -1, -1) * size)), camera.UnprojectPosition(pos + (new Vector3(-1, 1, -1) * size)));
		newBox.Elements[9] = new DebugLine(color, camera.UnprojectPosition(pos + (new Vector3(1, -1, -1) * size)), camera.UnprojectPosition(pos + (new Vector3(1, 1, -1) * size)));
		newBox.Elements[10] = new DebugLine(color, camera.UnprojectPosition(pos + (new Vector3(1, -1, 1) * size)), camera.UnprojectPosition(pos + (new Vector3(1, 1, 1) * size)));
		newBox.Elements[11] = new DebugLine(color, camera.UnprojectPosition(pos + (new Vector3(-1, -1, 1) * size)), camera.UnprojectPosition(pos + (new Vector3(-1, 1, 1) * size)));


		debugTimeElements.Add(new DebugTimeElement(newBox, seconds));
	}


	public override void _Draw()
	{
		for(int i = 0; i < currentDebugElement; i++)
		{
			debugElements[i].DrawElement(this);
		}
		
		currentDebugElement = 0;
		Godot.Collections.Array<int> toRemove = new Godot.Collections.Array<int>();
		int index = 0;
		foreach(DebugTimeElement timeElement in debugTimeElements)
		{
			timeElement.Drawable.DrawElement(this);
			timeElement.Lifetime -= deltaTime;

			if(timeElement.Lifetime <= 0) toRemove.Add(index);

			index++;
		}

		foreach(int timeElement in toRemove)
		{
			debugTimeElements.RemoveAt(timeElement);
		}
		
	}

	private float lastLabelUpdate = 0;
	public override void _Process(float delta)
	{
		deltaTime = delta;

		if(camera == null)
		{
			camera = GetCamera();
			if(camera == null) 
			{
				currentDebugElement = 0;
				return;
			}
		}
		
		
		this.Update();
		

		if(!Engine.EditorHint)
		{
			lastLabelUpdate += delta;
			if(lastLabelUpdate > 0.1f)
			{
				lastLabelUpdate = 0;
				debugLabel.Text = 
				"Debug Enabled\n" +
				$"FPS {Engine.GetFramesPerSecond()}\n" + 
				$"Delta {delta}\n";
			}

			
			if(Input.IsActionJustPressed("show_debug"))
			{
				debugLabel.Visible = !debugLabel.Visible;
			}
		}	
		
	}

	public void ResetCamera()
	{
		camera = null;
	}

	private Camera GetCamera()
	{
		if(!Engine.EditorHint)
			return GetViewport().GetCamera();
		else
		{
			Viewport viewport = FindEditorViewport(GetNode("/root/EditorNode"), 0).GetChild(0) as Viewport;
			this.GetParent<CanvasLayer>().CustomViewport = viewport;
			return viewport?.GetCamera();
		}	
	}

	private Node FindEditorViewport(Node node, int recursiveLevel)
	{
		if(node.GetClass() == "SpatialEditor") return node.GetChild(1).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0);

		recursiveLevel +=1;
		if(recursiveLevel > 15) return null;
		foreach(Node child in node.GetChildren())
		{
			Node result = FindEditorViewport(child, recursiveLevel);
			if(result != null) return result;
		}
		return null;
	}
}

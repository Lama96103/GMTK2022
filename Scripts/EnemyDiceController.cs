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

    private Array<MeshInstance> iceMeshes = new Array<MeshInstance>();

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
            if(!(currentPath[0].Equals(Vector3.Zero)))
            {
                foreach(MeshInstance meshInstance in iceMeshes)
                {
                    GetChild<Spatial>(0).GetChild(0).GetChild(0).RemoveChild(meshInstance);
                    meshInstance.Dispose();
                }
                iceMeshes.Clear();
            }
            RollDice(currentPath[0]);
            currentPath.RemoveAt(0);

            if(Patrol && currentPath.Count == 0)
            {
                lastPathDirection = !lastPathDirection;
                FillPath(lastPathDirection);
                GD.Print(currentPath);
            }
        }
    }

    public void setFrozen()
    {
        currentPath.Insert(0, Vector3.Zero);
        currentPath.Insert(0, Vector3.Zero);
        currentPath.Insert(0, Vector3.Zero);

        Spatial dice = GetChild<Spatial>(0);
        Node mesh = dice.GetChild(0).GetChild(0);

        int childCount = mesh.GetChildCount();

        int index = 0;;
        Godot.Collections.Array<Vector3> allSides = new Godot.Collections.Array<Vector3> ();
        allSides.Add(new Vector3(1,0,0));
        allSides.Add(new Vector3(-1,0,0));
        allSides.Add(new Vector3(0,0,1));
        allSides.Add(new Vector3(0,0,-1));
        allSides.Add(new Vector3(0,-1,0));
        allSides.Add(new Vector3(0,1,0));

        foreach(Vector3 pos in allSides)
        {
            PackedScene scene = (PackedScene)ResourceLoader.Load("res://Particles/IceEffect.tscn");
            MeshInstance meshInstance = scene.Instance<MeshInstance>();
            ((PlaneMesh) meshInstance.Mesh).Size = new Vector2(1.06f, 1.06f);
            meshInstance.Translate(pos * 0.53f);

            Vector3 rotation = Vector3.Up.Cross(pos);
            if(rotation.Length() > 0)
            {
                meshInstance.RotateObjectLocal(rotation, Mathf.Pi / 2);
            }
            else
            {
                if(pos.y == -1) 
                {
                    meshInstance.RotateX(Mathf.Pi);
                }
            }
            SpatialMaterial spatial = (SpatialMaterial) meshInstance.Mesh.SurfaceGetMaterial(0);
            spatial.AlbedoColor = new Color(1,1,1,0.6f);
            mesh.AddChild(meshInstance);

            iceMeshes.Add(meshInstance);

            index++;
        }
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

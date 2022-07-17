using Godot;
using System;

public class WorldController : Spatial
{
    public static WorldController Instance {get;private set;}

    public static string CurrentLevelPath = "res://Nodes/Level/Level_001.tscn";

    private LevelController currentLevel;

    private Control finishedLevelScreen;
    private Control enemyTurnIndication;
    private float startIndicatorSize = 0;
    private float targetIndicatorSize = 0;
    private float progressIndicatorTime = 0;
    private bool indicatorAnimationPlaying = false;

    public override void _Ready()
    {
        WorldController.Instance = this;
        finishedLevelScreen = GetNode<Control>("Game_UI/Status");
        enemyTurnIndication = GetNode<Control>("Game_UI/TurnIndicator");
        enemyTurnIndication.GetChild<Control>(0).RectScale = new Vector2(0, 0);
        finishedLevelScreen.Visible = false;
        LoadLevel(CurrentLevelPath);
    }

    public override void _PhysicsProcess(float delta)
    {
        if(indicatorAnimationPlaying)
        {
            progressIndicatorTime += (delta * 10);
            float curSize = Mathf.Lerp(startIndicatorSize, targetIndicatorSize, progressIndicatorTime);
            enemyTurnIndication.GetChild<Control>(0).RectScale = new Vector2(curSize, curSize);

            if(progressIndicatorTime >= 1)
            {
                indicatorAnimationPlaying = false;
                enemyTurnIndication.GetChild<Control>(0).RectScale = new Vector2(targetIndicatorSize, targetIndicatorSize);
            }

        }
    }

    public void LoadLevel(NodePath path)
    {
        if(currentLevel != null)
        {
            this.RemoveChild(currentLevel);
            currentLevel.QueueFree();
        }
        CurrentLevelPath = path;

        PackedScene scene = (PackedScene)ResourceLoader.Load(path);
        currentLevel = scene.Instance<LevelController>();
        this.AddChild(currentLevel);
    }

    public void OnFinishedLevel()
    {
        finishedLevelScreen.Visible = true;
    }

    public void SetCurrentRound(bool isEnemyRound)
    {
        indicatorAnimationPlaying = true;
        progressIndicatorTime = 0;
        startIndicatorSize = isEnemyRound ? 0 : 1;
        targetIndicatorSize = isEnemyRound ? 1 : 0;
        

    }


    private void LoadNextLevel()
    {
        finishedLevelScreen.Visible = false;
        LoadLevel(currentLevel.NextLevel);
    }

    public void LoadMainMenu()
    {
        this.GetTree().ChangeScene("res://Nodes/Menu/MainMenu.tscn");
    }

    public void LoadDeathMenu()
    {
        this.GetTree().ChangeScene("res://Nodes/Menu/DeathMenu.tscn");
    }

}

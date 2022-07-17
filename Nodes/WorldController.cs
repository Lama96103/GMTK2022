using Godot;
using System;

public class WorldController : Spatial
{
    public static WorldController Instance {get;private set;}

    public static string CurrentLevelPath = "res://Nodes/Level/Level_005.tscn";

    private LevelController currentLevel;

    private Control finishedLevelScreen;
    private Control failureLevelScreen;
    private Control enemyTurnIndication;
    private float startIndicatorSize = 0;
    private float targetIndicatorSize = 0;
    private float progressIndicatorTime = 0;
    private bool indicatorAnimationPlaying = false;

    public override void _Ready()
    {
        WorldController.Instance = this;
        finishedLevelScreen = GetNode<Control>("Game_UI/VictoryScreen");
        failureLevelScreen = GetNode<Control>("Game_UI/FailureScreen");
        finishedLevelScreen.Visible = false;
        failureLevelScreen.Visible = false;

        enemyTurnIndication = GetNode<Control>("Game_UI/TurnIndicator");
        enemyTurnIndication.GetChild<Control>(0).RectScale = new Vector2(0, 0);
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
        startIndicatorSize = isEnemyRound ? 0 : 1;
        targetIndicatorSize = isEnemyRound ? 1 : 0;
        if(targetIndicatorSize == enemyTurnIndication.GetChild<Control>(0).RectScale.x) return;
        indicatorAnimationPlaying = true;
        progressIndicatorTime = 0;
    }


    private void LoadNextLevel()
    {
        finishedLevelScreen.Visible = false;
        if(string.IsNullOrEmpty(currentLevel.NextLevel)) LoadMainMenu();
        else LoadLevel(currentLevel.NextLevel);
    }

    private void RetryLevel()
    {
        failureLevelScreen.Visible = false;
        LoadLevel(CurrentLevelPath);
    }

    public void LoadMainMenu()
    {
        this.GetTree().ChangeScene("res://Nodes/Menu/MainMenu.tscn");
    }

    public void LoadDeathMenu()
    {
        failureLevelScreen.Visible = true;
    }

}

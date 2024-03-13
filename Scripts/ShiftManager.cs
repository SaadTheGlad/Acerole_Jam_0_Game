using Godot;
using System;

public partial class ShiftManager : Node
{
    [Export] public float timeInMinutes = 0.1f;

    [Export] public AnimationPlayer winPlayer;
    [Export] public RichTextLabel scoreLabel;
    [Export] public DialogueHandler handler;

    SceneTreeTimer timer;

    int localCounter;

    public override void _EnterTree()
    {
        timer = GetTree().CreateTimer(timeInMinutes * 60f);

        SignalsManager.Instance.Admitted += IncrementCounter;

        timer.Timeout += ShowScreen;
    }

    public override void _ExitTree()
    {
        timer.Timeout -= ShowScreen;

        SignalsManager.Instance.Admitted -= IncrementCounter;
    }

    void IncrementCounter()
    {
        localCounter++;
    }

    void ShowScreen()
    {

        if (handler.GetChild(0) != null)
        {
            Node balloon = handler.GetChild(0);
            balloon.QueueFree();

        }

        AudioManager.Instance.Play("ring");
        scoreLabel.Text = "\r\n[center][font_size={40}]A total of " + localCounter + " people have been admitted into the bunker.";
        winPlayer.Play("Open");
    }

    void NextDay()
    {
        DialogueData.currentDay++;
        GetTree().ReloadCurrentScene();
    }

}

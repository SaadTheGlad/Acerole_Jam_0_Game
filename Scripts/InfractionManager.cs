using Godot;
using System;

public partial class InfractionManager : Node
{
    public int numOfInfractions;
    [Export] public Label infractionLabel;
    [Export] public AnimationPlayer player;
    [Export] public DialogueHandler handler;

    bool canPress = true;

    public override void _EnterTree()
    {
        SignalsManager.Instance.IncreaseInfraction += IncreaseInfraction;
        SignalsManager.Instance.NPCHasPassed += CanPress;
    }

    public override void _ExitTree()
    {
        SignalsManager.Instance.IncreaseInfraction -= IncreaseInfraction;
        SignalsManager.Instance.NPCHasPassed -= CanPress;


    }

    async void CanPress()
    {
        await ToSignal(GetTree().CreateTimer(2f), SceneTreeTimer.SignalName.Timeout);
        canPress = true;

    }



    async public void IncreaseInfraction()
    {
        if(canPress)
        {
            canPress = false;
            numOfInfractions++;
            AudioManager.Instance.Play("incorrect");
        }

        if (numOfInfractions == 3)
        {

            if (handler.GetChild(0) != null)
            {
                Node balloon = handler.GetChild(0);
                balloon.QueueFree();

            }

            await ToSignal(GetTree().CreateTimer(1f), SceneTreeTimer.SignalName.Timeout);
            player.Play("Open");

        }
        infractionLabel.Text = "Total\ninfractions:\n" + numOfInfractions + "/3";

    }

    public void Restart()
    {
        GetTree().ReloadCurrentScene();
    }
}

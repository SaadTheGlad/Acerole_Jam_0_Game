using Godot;
using System;

public partial class InfractionManager : Node
{
    public int numOfInfractions;
    [Export] public Label infractionLabel;
    [Export] public AnimationPlayer player;
    [Export] public DialogueHandler handler;



    public override void _EnterTree()
    {
        SignalsManager.Instance.IncreaseInfraction += IncreaseInfraction;
    }

    public override void _ExitTree()
    {
        SignalsManager.Instance.IncreaseInfraction -= IncreaseInfraction;

    }
    async public void IncreaseInfraction()
    {
        numOfInfractions++;
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

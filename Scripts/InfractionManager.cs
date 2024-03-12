using Godot;
using System;

public partial class InfractionManager : Node
{
    public int numOfInfractions;
    [Export] public Label infractionLabel;

    public override void _EnterTree()
    {
        SignalsManager.Instance.IncreaseInfraction += IncreaseInfraction;
    }

    public override void _ExitTree()
    {
        SignalsManager.Instance.IncreaseInfraction -= IncreaseInfraction;

    }
    public void IncreaseInfraction()
    {
        numOfInfractions++;
        if (numOfInfractions == 3)
        {
            GD.Print("Lost");
            // do fired thing
        }
        infractionLabel.Text = "Total\ninfractions:\n" + numOfInfractions + "/3";

    }
}

using Godot;
using System;

[GlobalClass]
public partial class DialogueData : Node
{
    public static DialogueData Instance { get; private set; }
    public override void _EnterTree()
    {

        if (IsInstanceValid(this))
        {
            GD.Print("More than one ", Instance.Name);

        }
        else
        {
            Instance = this;
        }

    }

    public static string organName;
    public static string colourName;

    public override void _Ready()
    {
        ResetVariables();
        organName = "dick and balls";
    }

    void ResetVariables()
    {
        organName = null;
        colourName = null;
    }
}

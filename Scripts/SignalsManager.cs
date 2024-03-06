using Godot;
using System;

public partial class SignalsManager : Node
{
    public static SignalsManager Instance { get; private set; }

    public override void _EnterTree()
    {

        if (Instance != null)
        {
            //GD.Print("More than one ", Instance.Name);

        }
        else
        {
            Instance = this;
        }

    }

    [Signal] public delegate void DialogueStartedRunningEventHandler();
    [Signal] public delegate void DialogueEndedEventHandler();
}

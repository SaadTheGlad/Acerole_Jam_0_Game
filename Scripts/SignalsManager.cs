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
    //Main signal
    [Signal] public delegate void CreatedNPCEventHandler();
    [Signal] public delegate void EnableNPCEventHandler();

    [Signal] public delegate void DialogueStartedRunningEventHandler();
    [Signal] public delegate void DialogueEndedEventHandler();
    [Signal] public delegate void AdmittedEventHandler();
    [Signal] public delegate void DisposedOfEventHandler();
    [Signal] public delegate void NPCHasArrivedEventHandler();
    [Signal] public delegate void ResetScanEventHandler();
    [Signal] public delegate void CloseJudgingEventHandler();
    [Signal] public delegate void NPCHasPassedEventHandler();
    [Signal] public delegate void InterrogateEventHandler();
    [Signal] public delegate void DoneInterrogateEventHandler();
    [Signal] public delegate void ChangeSoundEventHandler(string sfxName);
    [Signal] public delegate void SendSoundEventHandler(string sfxName);

    //Dialogue signals
    [Signal] public delegate void MakeMissingEventHandler();
    [Signal] public delegate void MakeRotatedEventHandler();
    [Signal] public delegate void MakeDiscolouredEventHandler();
    [Signal] public delegate void MakeDuplicatedEventHandler();
    [Signal] public delegate void SetNameEventHandler(string name);
    [Signal] public delegate void EnableAbberationEventHandler();
    [Signal] public delegate void DisableAbberationEventHandler();
    [Signal] public delegate void SelectedCorrectEventHandler();
    [Signal] public delegate void SelectedIncorrectEventHandler();
    [Signal] public delegate void SendColourEventHandler(string name);
    [Signal] public delegate void SetNPCNameEventHandler(string name);
    [Signal] public delegate void SetSFXNameEventHandler(string name);
    [Signal] public delegate void HasSelectedEventHandler();
    [Signal] public delegate void PassThroughEventHandler();

    //Cutscene signals
    [Signal] public delegate void ImposterFinishedEventHandler();

}

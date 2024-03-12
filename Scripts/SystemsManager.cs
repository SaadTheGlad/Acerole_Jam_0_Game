using Godot;
using System;

public partial class SystemsManager : Node
{
    //important information
    [Export] private DialogueHolder npcHolder;
    [Export] private NPCController npcController;
    public string currentNPCSFXName;

    [ExportCategory("UI")]
    [Export] public Label personsAdmittedCounter;


    //Important Stats
    private float totalPeopleAdmitted;



    public override void _EnterTree()
    {
        SignalsManager.Instance.Admitted += IncrementCounter;
    }

    public override void _ExitTree()
    {
        SignalsManager.Instance.Admitted -= IncrementCounter;
    }

    void IncrementCounter()
    {
        totalPeopleAdmitted++;
        personsAdmittedCounter.Text = "Total people\nadmitted:" + "\n" + totalPeopleAdmitted.ToString();
    }

    public void Ring()
    {

        AudioManager.Instance.Play("ring");
        if(npcController.canRing == true)
        {
            currentNPCSFXName = npcHolder.sfxName;
            SignalsManager.Instance.EmitSignal(SignalsManager.SignalName.SendSound, currentNPCSFXName);
            SignalsManager.Instance.EmitSignal(SignalsManager.SignalName.CreatedNPC);
        }
        else
        {
        }

    }
}

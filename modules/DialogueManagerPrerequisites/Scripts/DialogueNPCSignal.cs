using Godot;
using System;

[GlobalClass]
public partial class DialogueNPCSignal : Node
{
    [Signal] public delegate void EncounteredNPCEventHandler(Node2D npc, DialogueHolder holder);

    [Export] NPCController npcController;
    [Export] Node2D player;

    public bool isTalking = false;

    public override void _EnterTree()
    {
        SignalsManager.Instance.NPCHasArrived += LookForNPCs;
        SignalsManager.Instance.Interrogate += TalkInterrogate;
    }

    public override void _ExitTree()
    {
        SignalsManager.Instance.NPCHasArrived -= LookForNPCs;
        SignalsManager.Instance.Interrogate -= TalkInterrogate;

    }

    void LookForNPCs()
    {
        //Looking for NPC
        if(npcController.currentGuy != null)
        {
            //Person detected, looking for dialogue handler
            foreach(Node n in npcController.currentGuy.GetChildren())
            {
                if(n is DialogueHolder holder)
                {
                    //Found handler and begins talking
                    isTalking = true;
                    EmitSignal(SignalName.EncounteredNPC, npcController.currentGuy, holder);
                    SignalsManager.Instance.EmitSignal(SignalsManager.SignalName.DialogueStartedRunning);
                    break;
                }
                else
                {
                    isTalking = false;
                    //GD.Print("Handler not found.");

                }
            }
        }
        else
        {
            isTalking = false;
            //Person not there

        }
    }

    void TalkInterrogate()
    {
        foreach(Node n in player.GetChildren())
        {
            if (n is DialogueHolder holder)
            {
                SignalsManager.Instance.EmitSignal(SignalsManager.SignalName.DialogueStartedRunning);
                EmitSignal(SignalName.EncounteredNPC, npcController.currentGuy, holder);
            }
        }
    }
}

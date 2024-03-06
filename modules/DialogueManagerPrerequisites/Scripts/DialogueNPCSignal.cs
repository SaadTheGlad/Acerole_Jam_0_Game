using Godot;
using System;

[GlobalClass]
public partial class DialogueNPCSignal : Node
{
    [Signal] public delegate void EncounteredNPCEventHandler(Node2D npc, DialogueHolder holder);

    [Export] Node2D player;

    [Export] public NPCController npcController;

    public bool isTalking = false;

    public override void _EnterTree()
    {
        npcController.GuyCame += LookForNPCs;
        SignalsManager.Instance.Interrogate += TalkInterrogate;
    }

    public override void _ExitTree()
    {
        npcController.GuyCame -= LookForNPCs;
        SignalsManager.Instance.Interrogate -= TalkInterrogate;

    }

    void LookForNPCs()
    {
        //GD.Print("Looking...");
        if(npcController.currentGuy != null)
        {
            //GD.Print("Guy detected, searching for handler...");
            foreach(Node n in npcController.currentGuy.GetChildren())
            {
                if(n is DialogueHolder holder)
                {
                    //GD.Print("Found handler and talking.");
                    isTalking = true;
                    EmitSignal(SignalName.EncounteredNPC, npcController.currentGuy, holder);
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
            //GD.Print("Guy not detected.");

        }
    }

    void TalkInterrogate(DialogueJudges judge)
    {
        foreach(Node n in player.GetChildren())
        {
            if (n is DialogueHolder holder)
            {
                holder.dialogue = judge.dialogue;
                EmitSignal(SignalName.EncounteredNPC, npcController.currentGuy, holder);
            }
        }
    }
}

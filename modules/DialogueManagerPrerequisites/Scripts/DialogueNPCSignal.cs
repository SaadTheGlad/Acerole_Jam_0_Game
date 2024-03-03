using Godot;
using System;

[GlobalClass]
public partial class DialogueNPCSignal : Node
{
    [Signal] public delegate void EncounteredNPCEventHandler(Node2D npc, DialogueHolder holder);

    [Export] public NPCController npcController;

    public bool isTalking = false;

    public override void _Process(double delta)
	{
        //Make this only work when you click on the person, no when they come in
        //if (Input.IsActionJustPressed("action"))
        //{
        //    LookForNPCs();
        //}
    }

    void LookForNPCs()
    {
        GD.Print("Looking...");
        if(npcController.currentGuy != null)
        {
            GD.Print("Guy detected, searching for handler...");
            foreach(Node n in npcController.currentGuy.GetChildren())
            {
                if(n is DialogueHolder holder)
                {
                    GD.Print("Found handler and talking.");
                    isTalking = true;
                    EmitSignal(SignalName.EncounteredNPC, npcController.currentGuy, holder);
                    break;
                }
                else
                {
                    isTalking = false;
                    GD.Print("Handler not found.");

                }
            }
        }
        else
        {
            isTalking = false;
            GD.Print("Guy not detected.");

        }
    }
}

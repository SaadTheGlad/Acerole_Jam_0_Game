using Godot;
using System;
using System.Security;

[GlobalClass]
public partial class NPCController : Node
{
    [Export] public Node2D NPC;
    [Export] private float movementSpeed = 10f;
    [Export] private Curve curve;
    [Export] public DialogueNPCSignal npcSignal;
    [Export] public JudgingManager judge;
    public Node2D currentGuy;

    private const float CENTEROFWINDOWX = 518f;
    bool hasReached;
    public bool canRing = false;
    public Vector2 startingPos;

    public override void _EnterTree()
    {
        SignalsManager.Instance.CreatedNPC += Ring;
        //SignalsManager.Instance.EnableNPC +=
        //;
    }

    public override void _ExitTree()
    {
        SignalsManager.Instance.CreatedNPC -= Ring;
        //SignalsManager.Instance.EnableNPC -= EnableScan;

    }

    public override void _Ready()
    {
        startingPos = NPC.GlobalPosition;
    }

    //public void EnableScan() => canRing = true;

    public void Ring()
    {
        if(canRing)
        {
            CheckGuy();
            canRing = false;
        }


    }


    public void Admit()
    {
        if(hasReached && !npcSignal.isTalking)
        {
            
            SignalsManager.Instance.EmitSignal(SignalsManager.SignalName.ResetScan);
            SignalsManager.Instance.EmitSignal(SignalsManager.SignalName.Admitted);
            AdmitGuy();
            hasReached = false;
        }
    }

    async private void CheckGuy()
    {
        float current = 0f;
        float target = 1f;

        Vector2 startPos = NPC.GlobalPosition;
        Vector2 centerPos = new Vector2(CENTEROFWINDOWX, NPC.GlobalPosition.Y);

        while (true)
        {

            current = Mathf.MoveToward(current, target, movementSpeed * (float)GetPhysicsProcessDeltaTime());

            NPC.GlobalPosition = startPos.Lerp(centerPos, curve.Sample(current));

            if (NPC.GlobalPosition.IsEqualApprox(centerPos))
            {
                hasReached = true;
                currentGuy = NPC;
                SignalsManager.Instance.EmitSignal(SignalsManager.SignalName.NPCHasArrived);
                SignalsManager.Instance.EmitSignal(SignalsManager.SignalName.CameIn);
                break;
            }

            await ToSignal(GetTree(), SceneTree.SignalName.PhysicsFrame);

        }
    }
    async private void AdmitGuy()
    {

        float current = 0f;
        float target = 1f;

        Vector2 startPos = NPC.GlobalPosition;
        Vector2 offScreenPos = new Vector2(CENTEROFWINDOWX + 1000f, NPC.GlobalPosition.Y);

        while (true)
        {

            current = Mathf.MoveToward(current, target, movementSpeed * (float)GetPhysicsProcessDeltaTime());

            NPC.GlobalPosition = startPos.Lerp(offScreenPos, curve.Sample(current));

            if (NPC.GlobalPosition.IsEqualApprox(offScreenPos))
            {
                currentGuy = null;
                NPC.GlobalPosition = startingPos;
                SignalsManager.Instance.EmitSignal(SignalsManager.SignalName.NPCHasPassed);
                judge.canClick = true;
                canRing = true;
                break;
            }
            GD.Print("is moving");

            await ToSignal(GetTree(), SceneTree.SignalName.PhysicsFrame);

        }
    }

}

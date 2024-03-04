using Godot;
using System;
using System.Security;

[GlobalClass]
public partial class NPCController : Node
{
    [Export] private Node2D guy;
    [Export] private float movementSpeed = 10f;
    [Export] private Curve curve;

    [Export] public DialogueNPCSignal npcSignal;
    public Node2D currentGuy;

    private const float CENTEROFWINDOWX = 518f;

    bool hasReached;

    bool canRing = true;

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
            AdmitGuy();
            hasReached = false;
        }
    }

    async private void CheckGuy()
    {

        float current = 0f;
        float target = 1f;

        Vector2 startPos = guy.GlobalPosition;
        Vector2 centerPos = new Vector2(CENTEROFWINDOWX, guy.GlobalPosition.Y);

        while (true)
        {

            current = Mathf.MoveToward(current, target, movementSpeed * (float)GetPhysicsProcessDeltaTime());

            guy.GlobalPosition = startPos.Lerp(centerPos, curve.Sample(current));

            if (guy.GlobalPosition.IsEqualApprox(centerPos))
            {
                hasReached = true;
                currentGuy = guy;
                break;
            }

            await ToSignal(GetTree(), SceneTree.SignalName.PhysicsFrame);

        }
    }
    async private void AdmitGuy()
    {
        currentGuy = null;

        float current = 0f;
        float target = 1f;

        Vector2 startPos = guy.GlobalPosition;
        Vector2 offScreenPos = new Vector2(CENTEROFWINDOWX + 1000f, guy.GlobalPosition.Y);

        while (true)
        {

            current = Mathf.MoveToward(current, target, movementSpeed * (float)GetPhysicsProcessDeltaTime());

            guy.GlobalPosition = startPos.Lerp(offScreenPos, curve.Sample(current));

            if (guy.GlobalPosition.IsEqualApprox(offScreenPos))
            {
                currentGuy = null;
                canRing = true;
                break;
            }

            await ToSignal(GetTree(), SceneTree.SignalName.PhysicsFrame);

        }
    }

}

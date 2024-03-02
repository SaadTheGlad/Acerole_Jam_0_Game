using Godot;
using System;
using System.Security;

public partial class NPCController : Node
{
    [Export] private Node2D guy;
    [Export] private float movementSpeed = 10f;
    [Export] private Curve curve;

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
        if(hasReached)
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
                break;
            }

            await ToSignal(GetTree(), SceneTree.SignalName.PhysicsFrame);

        }
    }
    async private void AdmitGuy()
    {

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
                canRing = true;
                break;
            }

            await ToSignal(GetTree(), SceneTree.SignalName.PhysicsFrame);

        }
    }

}

using Godot;
using System;

public partial class JudgingManager : Node
{
    [Export] public AnimationPlayer judgingPlayer;
    [Export] public NPCController controller;
    [Export] public float fallingSpeed;
    [Export] public Curve curve;

    async private void FallGuy()
    {
        judgingPlayer.Play("Close");
        Node2D NPC = controller.NPC;

        float current = 0f;
        float target = 1f;

        Vector2 startPos = NPC.GlobalPosition;
        Vector2 targetPos = new Vector2(NPC.GlobalPosition.X, 1500f);

        while (true)
        {

            current = Mathf.MoveToward(current, target, fallingSpeed * (float)GetPhysicsProcessDeltaTime());
            NPC.GlobalPosition = startPos.Lerp(targetPos, curve.Sample(current));

            if (NPC.GlobalPosition.Y >= 1400f)
            {
                controller.currentGuy = null;
                NPC.GlobalPosition = controller.startingPos;
                SignalsManager.Instance.EmitSignal(SignalsManager.SignalName.NPCHasPassed);
                controller.canRing = true;
                break;
            }

            await ToSignal(GetTree(), SceneTree.SignalName.PhysicsFrame);

        }
    }
}

using Godot;
using System;

public partial class JudgingManager : Node
{
    [Export] public AnimationPlayer judgingPlayer;
    [Export] public NPCController controller;
    [Export] public float fallingSpeed;
    [Export] public Curve curve;

    [Export] XRayManager xRayManager;

    Sprite2D anomalyBone;
    Sprite2D anomalyOrgan;
    Sprite2D selectedBone;
    Sprite2D selectedOrgan;


    public override void _EnterTree()
    {
        SignalsManager.Instance.Admitted += CloseJudge;
    }

    public override void _ExitTree()
    {
        SignalsManager.Instance.Admitted -= CloseJudge;
    }

    async private void FallGuy()
    {
        SignalsManager.Instance.EmitSignal(SignalsManager.SignalName.ResetScan);

        CloseJudge();
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

    void CloseJudge()
    {
        judgingPlayer.Play("Close");
    }

    public void Interrogate()
    {

        if(anomalyBone != null)
        {
            //anomalous bone

            if (selectedBone.Name == anomalyBone.Name)
            {
                //do interrogation depending on the type of anomaly
                selectedBone = null;

            }
            else
            {
                controller.Admit();
                GetTree().ReloadCurrentScene();
            }
        }
        else if(anomalyOrgan != null)
        {
            //anomalous organ
            if (selectedOrgan.Name == anomalyOrgan.Name)
            {
                //do interrogation depending on the type of anomaly
                selectedOrgan = null;

            }
            else
            {
                controller.Admit();
                GetTree().ReloadCurrentScene();
            }

        }
        else
        {
            controller.Admit();

        }

        SignalsManager.Instance.EmitSignal(SignalsManager.SignalName.ResetScan);

    }

    public void SelectAnomaly()
    {


        selectedBone = xRayManager.currentSelectedBone;
        selectedOrgan = xRayManager.currentSelectedOrgan;

        if(xRayManager.anomalyBone != null)
        {
            anomalyBone = xRayManager.anomalyBone;
        }

        if(xRayManager.anomalyOrgan != null)
        {
            anomalyOrgan = xRayManager.anomalyOrgan;
        }

    }
}

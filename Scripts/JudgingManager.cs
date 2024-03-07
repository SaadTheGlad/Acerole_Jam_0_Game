using Godot;
using Godot.Collections;
using System;

public partial class JudgingManager : Node
{
    [ExportCategory("Dialogues")]
    [Export] public DialogueJudges[] skeletonDialogueJudges;
    [Export] public DialogueJudges[] organsDialogueJudges;
    [ExportCategory("Other")]
    [Export] public AnimationPlayer judgingPlayer;
    [Export] public NPCController controller;
    [Export] public float fallingSpeed;
    [Export] public Curve curve;
    [Export] XRayManager xRayManager;

    Sprite2D anomalyBone;
    Sprite2D anomalyOrgan;
    Sprite2D selectedBone;
    Sprite2D selectedOrgan;

    Dictionary<String, DialogueJudges> nameBoneTypeDic = new Dictionary<String, DialogueJudges>();
    Dictionary<String, DialogueJudges> nameOrganTypeDic = new Dictionary<String, DialogueJudges>();

    public override void _EnterTree()
    {
        SignalsManager.Instance.Admitted += CloseJudge;
    }

    public override void _ExitTree()
    {
        SignalsManager.Instance.Admitted -= CloseJudge;
    }

    public override void _Ready()
    {
        //PopulateDic(nameBoneTypeDic, skeletonDialogueJudges);
        PopulateDic(nameOrganTypeDic, organsDialogueJudges);
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

    public void PopulateDic(Dictionary<String, DialogueJudges> dictionary, DialogueJudges[] judges)
    {
        foreach(DialogueJudges n in judges)
        {
            dictionary.Add(n.dialogueName, n);
        }
    }

    public void Interrogate()
    {

        if(anomalyBone != null)
        {
            //anomalous bone

            if (selectedBone.Name == anomalyBone.Name)
            {
                string boneName = anomalyBone.Name;
                if(boneName.Contains("Rib"))
                {
                    SignalsManager.Instance.EmitSignal(SignalsManager.SignalName.Interrogate, nameBoneTypeDic["rib"]);
                }
                else if(boneName.Contains("Backbone"))
                {
                    SignalsManager.Instance.EmitSignal(SignalsManager.SignalName.Interrogate, nameBoneTypeDic["backbone"]);

                }
                else if(boneName.Contains("Knee"))
                {
                    SignalsManager.Instance.EmitSignal(SignalsManager.SignalName.Interrogate, nameBoneTypeDic["knee"]);

                }
                else if(boneName.Contains("Toe"))
                {
                    SignalsManager.Instance.EmitSignal(SignalsManager.SignalName.Interrogate, nameBoneTypeDic["toe"]);  
                }



                selectedBone = null;

            }
            else
            {
                controller.Admit();
                //Dies
            }
        }
        else if(anomalyOrgan != null)
        {
            //anomalous organ
            if (selectedOrgan.Name == anomalyOrgan.Name)
            {
                string organName = anomalyOrgan.Name;


                if (organName.Contains("Kidney"))
                {
                    SignalsManager.Instance.EmitSignal(SignalsManager.SignalName.Interrogate, nameOrganTypeDic["kidney"]);
                }
                else if (organName.Contains("Pancreas"))
                {
                    SignalsManager.Instance.EmitSignal(SignalsManager.SignalName.Interrogate, nameOrganTypeDic["pancreas"]);

                }
                else if (organName.Contains("Testicles"))
                {
                    SignalsManager.Instance.EmitSignal(SignalsManager.SignalName.Interrogate, nameOrganTypeDic["testicle"]);

                }
                else if (organName.Contains("Gallbladder"))
                {
                    SignalsManager.Instance.EmitSignal(SignalsManager.SignalName.Interrogate, nameOrganTypeDic["gallbladder"]);
                }
                else if (organName.Contains("Lungs"))
                {
                    SignalsManager.Instance.EmitSignal(SignalsManager.SignalName.Interrogate, nameOrganTypeDic["lung"]);
                }
                else if (organName.Contains("Penis"))
                {
                    SignalsManager.Instance.EmitSignal(SignalsManager.SignalName.Interrogate, nameOrganTypeDic["penis"]);
                }
                //Add one for stomach and bladder


                selectedOrgan = null;

            }
            else
            {
                controller.Admit();
                //Dies
            }

        }
        else
        {
            controller.Admit();
            SignalsManager.Instance.EmitSignal(SignalsManager.SignalName.ResetScan);


        }


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

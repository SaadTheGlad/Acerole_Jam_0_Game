using Godot;
using Godot.Collections;
using System;

public partial class JudgingManager : Node
{
    [ExportCategory("Other")]
    [Export] public AnimationPlayer judgingPlayer;
    [Export] public DialogueNPCSignal signal;
    [Export] public NPCController controller;
    [Export] public float fallingSpeed;
    [Export] public Curve curve;
    [Export] XRayManager xRayManager;

    bool canFall = true;
    public bool canClick = true;

    public override void _EnterTree()
    {
        SignalsManager.Instance.Admitted += CloseJudge;
        SignalsManager.Instance.DialogueEnded += EnableFall;
        SignalsManager.Instance.DialogueStartedRunning += DisableFall;
        SignalsManager.Instance.PassThrough += PassThrough;
    }

    public override void _ExitTree()
    {
        SignalsManager.Instance.Admitted -= CloseJudge;
        SignalsManager.Instance.DialogueEnded -= EnableFall;
        SignalsManager.Instance.DialogueStartedRunning -= DisableFall;
        SignalsManager.Instance.PassThrough -= PassThrough;



    }

    public void EnableFall() => canFall = true;
    public void DisableFall() => canFall = false;

    async private void FallGuy()
    {
        if (!DialogueData.Instance.isAnomaly)
        {
            SignalsManager.Instance.EmitSignal(SignalsManager.SignalName.IncreaseInfraction);
        }

        canClick = false;



        if (canFall)
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
                    SignalsManager.Instance.EmitSignal(SignalsManager.SignalName.EnableNPC);
                    SignalsManager.Instance.EmitSignal(SignalsManager.SignalName.DisposedOf);
                    ResetStuff();
                    signal.isTalking = false;
                    controller.canRing = true;
                    
                    canClick = true;


                    break;
                }

                await ToSignal(GetTree(), SceneTree.SignalName.PhysicsFrame);

            }
        }
    }
    void CloseJudge()
    {
        judgingPlayer.Play("Close");
    }

    public void Interrogate()
    {
        if(canClick)
        {
            SignalsManager.Instance.EmitSignal(SignalsManager.SignalName.CameIn);
            SignalsManager.Instance.EmitSignal(SignalsManager.SignalName.Interrogate);
        }
    }

    void PassThrough()
    {
        GD.Print("Was anomaly: " + DialogueData.Instance.isAnomaly);
        //kill
        signal.isTalking = false;


        SignalsManager.Instance.EmitSignal(SignalsManager.SignalName.CameIn);
        canClick = false;
        ResetStuff();
        controller.Admit();
        SignalsManager.Instance.EmitSignal(SignalsManager.SignalName.EnableNPC);
        SignalsManager.Instance.EmitSignal(SignalsManager.SignalName.ResetScan);
    }

    void ResetStuff()
    {
        SignalsManager.Instance.EmitSignal(SignalsManager.SignalName.DisableAbberation);
    }

    #region Old Code
    //public void PopulateDic(Dictionary<String, DialogueJudges> dictionary, DialogueJudges[] judges)
    //{
    //    foreach (DialogueJudges n in judges)
    //    {
    //        dictionary.Add(n.dialogueName, n);
    //    }
    //}

    //public void Interrogate()
    //{

    //    if(anomalyBone != null && selectedBone != null)
    //    {
    //        //anomalous bone

    //        if (selectedBone.Name == anomalyBone.Name)
    //        {
    //            string boneName = anomalyBone.Name;
    //            if(boneName.Contains("Rib"))
    //            {

    //                SignalsManager.Instance.EmitSignal(SignalsManager.SignalName.Interrogate, nameBoneTypeDic["rib"]);
    //            }
    //            else if(boneName.Contains("Backbone"))
    //            {

    //                SignalsManager.Instance.EmitSignal(SignalsManager.SignalName.Interrogate, nameBoneTypeDic["backbone"]);

    //            }
    //            else if(boneName.Contains("Knee"))
    //            {

    //                SignalsManager.Instance.EmitSignal(SignalsManager.SignalName.Interrogate, nameBoneTypeDic["knee"]);

    //            }
    //            else if(boneName.Contains("Toe"))
    //            {

    //                SignalsManager.Instance.EmitSignal(SignalsManager.SignalName.Interrogate, nameBoneTypeDic["toe"]);  
    //            }
    //            else
    //            {
    //                //I haven't added it yet so I guess just let em pass for now 
    //                PassThrough();

    //            }

    //            //ResetAnomalies();

    //        }
    //        else
    //        {
    //            PassThrough();
    //        }
    //    }
    //    else if(anomalyOrgan != null && selectedOrgan != null)
    //    {
    //        //anomalous organ
    //        if (selectedOrgan.Name == anomalyOrgan.Name)
    //        {
    //            string organName = anomalyOrgan.Name;


    //            if (organName.Contains("Kidney"))
    //            {
    //                SignalsManager.Instance.EmitSignal(SignalsManager.SignalName.Interrogate, nameOrganTypeDic["kidney"]);
    //            }
    //            else if (organName.Contains("Pancreas"))
    //            {

    //                SignalsManager.Instance.EmitSignal(SignalsManager.SignalName.Interrogate, nameOrganTypeDic["pancreas"]);

    //            }
    //            else if (organName.Contains("Testicles"))
    //            {

    //                SignalsManager.Instance.EmitSignal(SignalsManager.SignalName.Interrogate, nameOrganTypeDic["testicle"]);

    //            }
    //            else if (organName.Contains("Gallbladder"))
    //            {

    //                SignalsManager.Instance.EmitSignal(SignalsManager.SignalName.Interrogate, nameOrganTypeDic["gallbladder"]);
    //            }
    //            else if (organName.Contains("Lungs"))
    //            {

    //                SignalsManager.Instance.EmitSignal(SignalsManager.SignalName.Interrogate, nameOrganTypeDic["lung"]);
    //            }
    //            else if (organName.Contains("Penis"))
    //            {
    //                SignalsManager.Instance.EmitSignal(SignalsManager.SignalName.Interrogate, nameOrganTypeDic["penis"]);
    //            }
    //            else
    //            {
    //                //I haven't added it yet so I guess just let em pass for now 
    //                PassThrough();
    //            }


    //            //ResetAnomalies();

    //        }
    //        else
    //        {
    //            PassThrough();
    //            //Dies
    //        }

    //    }
    //    else
    //    {
    //        PassThrough();
    //    }

    //}

    //public void SelectAnomaly()
    //{
    //    selectedBone = xRayManager.currentSelectedBone;
    //    selectedOrgan = xRayManager.currentSelectedOrgan;

    //    if(xRayManager.anomalyBone != null)
    //    {
    //        anomalyBone = xRayManager.anomalyBone;
    //    }

    //    if(xRayManager.anomalyOrgan != null)
    //    {
    //        anomalyOrgan = xRayManager.anomalyOrgan;
    //    }

    //}
    #endregion
}

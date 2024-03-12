using Godot;
using System;

public partial class CutsceneManager : Node
{
    [Export] public Node2D NPC;
    [Export] public float movementSpeed;
    [Export] public Curve curve;

    [Export] public DialogueNPCSignal npcSignal;
    [Export] public DialogueHolder imposter, real;
    [Export] public NPCController controller;
    [Export] public Node2D fakeHandbook, realHandbook;
    [Export] public float fadingSpeed = 10f;

    bool calledPerson = false;
    bool playedImposter = false;
    bool playedReal = false;
    Vector2 originalPos;

    private const float CENTEROFWINDOWX = 518f;

    public override void _EnterTree()
    {
        SignalsManager.Instance.ImposterFinished += ImposterBye;
        SignalsManager.Instance.EnableFakeHandbook += EnableFakeHandbook;
    }

    public override void _ExitTree()
    {
        SignalsManager.Instance.ImposterFinished -= ImposterBye;
        SignalsManager.Instance.EnableFakeHandbook -= EnableFakeHandbook;

    }

    public override void _Ready()
    {
        SignalsManager.Instance.EmitSignal(SignalsManager.SignalName.SetNPCName, "Boss");
    }

    async public void PersonCome()
    { 
        if (!calledPerson)
        {
            calledPerson = true;
            float current = 0f;
            float target = 1f;

            originalPos = NPC.GlobalPosition;
            Vector2 startPos = NPC.GlobalPosition;
            Vector2 centerPos = new Vector2(CENTEROFWINDOWX, NPC.GlobalPosition.Y);

            while (true)
            {
                current = Mathf.MoveToward(current, target, movementSpeed * (float)GetPhysicsProcessDeltaTime());
                NPC.GlobalPosition = startPos.Lerp(centerPos, curve.Sample(current));

                if (NPC.GlobalPosition.IsEqualApprox(centerPos))
                {
                    //Start dialogue
                    if(!playedImposter)
                    {

                        npcSignal.TalkCustom((Node2D)imposter.GetParent(), imposter);
                        playedImposter = true;
                    }
                    else
                    {
                        npcSignal.TalkCustom((Node2D)imposter.GetParent(), real);
                        playedReal = true;
                        playedImposter = false;
                    }
                    break;
                }

                await ToSignal(GetTree(), SceneTree.SignalName.PhysicsFrame);
            }
        }
    }

    async public void ImposterBye()
    {

        float current = 0f;
        float target = 1f;

        Vector2 startPos = NPC.GlobalPosition;
        Vector2 destination = originalPos;

        while (true)
        {
            current = Mathf.MoveToward(current, target, movementSpeed * (float)GetPhysicsProcessDeltaTime());
            NPC.GlobalPosition = startPos.Lerp(destination, curve.Sample(current));

            if (NPC.GlobalPosition.IsEqualApprox(destination))
            {
                if(playedImposter)
                {
                    calledPerson = false;
                    //play real
                }

                if(playedReal)
                {
                    GD.Print("error");
                    calledPerson = true;
                    controller.canRing = true;
                    SignalsManager.Instance.EmitSignal(SignalsManager.SignalName.NPCHasPassed);
                    //make normal 
                }



                break;
            }

            await ToSignal(GetTree(), SceneTree.SignalName.PhysicsFrame);
        }
       
    }

    public void EnableFakeHandbook()
    {
        fakeHandbook.Modulate = new Color(fakeHandbook.Modulate.R, fakeHandbook.Modulate.G, fakeHandbook.Modulate.B, 0f);


        while (true)
        {

            fakeHandbook.Visible = true;

            float currentOpacity = 0f;
            float startingOpacity = 0f;
            float endingOpacity = 1f;

            float current = 0f;
            float target = 1f;

            current = Mathf.MoveToward(current, target, fadingSpeed * (float)GetProcessDeltaTime());
            currentOpacity = Mathf.Lerp(startingOpacity, endingOpacity, current);

            fakeHandbook.Modulate = new Color(fakeHandbook.Modulate.R, fakeHandbook.Modulate.G, fakeHandbook.Modulate.B, currentOpacity);

            if(fakeHandbook.Modulate.A == 1f)
            {
                break;
            }

        }
    }

}

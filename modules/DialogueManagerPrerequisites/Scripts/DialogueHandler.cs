using Godot;
using System;
using DialogueManagerRuntime;

[GlobalClass]
public partial class DialogueHandler : Node
{

    [Export] private DialogueNPCSignal encounteredNPCSignalHolder;

    [Export] private PackedScene normalBubble;
    [Export] private PackedScene noIconBubble;

    bool hasStartedDialogue;

    TextureRect iconTexture;
    DialogueHolder dialogueHolder;

    CanvasLayer currentBalloon;

    bool isInUse = false;
    string currentNPCSFXName;

    public override void _Ready()
    {
        encounteredNPCSignalHolder.EncounteredNPC += CheckNPC;
        SignalsManager.Instance.ChangeSound += ChangeSound;
        SignalsManager.Instance.SendSound += SetNPCSfxName;

        //Makes sure that you don't spawn a new balloon right after you close one by adding a little delay
        DialogueManager.DialogueEnded += (Resource dialogueResource) =>
        {
            WaitForBalloon();
        };

        DialogueManager.GotDialogue += (DialogueLine dialogueLine) =>
        {
            GotDialogue(dialogueLine);

        };
    }

    public override void _ExitTree()
    {
        encounteredNPCSignalHolder.EncounteredNPC -= CheckNPC;
        SignalsManager.Instance.ChangeSound -= ChangeSound;
        SignalsManager.Instance.SendSound -= SetNPCSfxName;


    }


    void GotDialogue(DialogueLine dialogueLine)
    {
        string mood = dialogueLine.GetTagValue("mood");
        if (iconTexture != null && dialogueHolder.emotions != null)
        {
            foreach (var e in dialogueHolder.emotions)
            {
                if (e.emotionName == mood)
                {
                    iconTexture.Texture = e.texture;
                    break;
                }
            }
        }
    }


    async void WaitForBalloon()
    {
        await ToSignal(GetTree().CreateTimer(0.1f), SceneTreeTimer.SignalName.Timeout);
        hasStartedDialogue = false;
        encounteredNPCSignalHolder.isTalking = false;
        SignalsManager.Instance.EmitSignal(SignalsManager.SignalName.DialogueEnded);

    }

    async void CheckNPC(Node2D npc, DialogueHolder holder)
    {
        dialogueHolder = holder;

        //Checks if dialogue has been started and if it has doesn't start a new balloon
        if (!hasStartedDialogue)
        {
            hasStartedDialogue = true;

            if (dialogueHolder.Name.IsEmpty)
            {
                //GD.PrintErr("No name entered");
                return;
            }

            Resource dialogueResource = dialogueHolder.dialogue;
            CanvasLayer canvas = null;

            if (dialogueHolder.emotions != null)
            {
                canvas = (CanvasLayer)normalBubble.Instantiate();
            }
            else
            {
                canvas = (CanvasLayer)noIconBubble.Instantiate();
            }


            var variant = new Godot.Collections.Array<Variant> { this};



            AddChild(canvas);
            var balloon = canvas;
            currentBalloon = balloon;
            var line = await DialogueManager.GetNextDialogueLine(dialogueResource, "start", variant);
            //string mood = line.GetTagValue("mood");
            balloon.Call("start", dialogueResource, "start", variant);




            //Sets the sound, font and the text speed 
            if (dialogueHolder.sfxName != null)
            {
                if(!isInUse)
                {
                    balloon.Set("soundName", dialogueHolder.sfxName);
                }
            }
            else
            {
                //GD.PrintErr("The NPC with name " + dialogueHolder.characterName + " does not have a sound sfx.");
            }

            if (dialogueHolder.font != null)
            {
                balloon.Call("change_fonts", dialogueHolder.font);
            }
            else
            {
                //GD.PrintErr("The NPC with name " + dialogueHolder.characterName + " does not have a font, default font set.");
            }

            balloon.Call("set_speed", dialogueHolder.baseSpeed);
            //Gets the icon to set it
            var balloonHolder = balloon.GetChild(0);

            if (dialogueHolder.emotions != null)
            {
                foreach (var n in balloonHolder.GetChildren())
                {
                    if (n is TextureRect textureRect)
                    {
                        iconTexture = textureRect;
                        break;
                    }
                }

                foreach (var e in dialogueHolder.emotions)
                {
                    //if (e.emotionName == mood)
                    //{
                    //    iconTexture.Texture = e.texture;
                    //    break;
                    //}
                }

                if (iconTexture.Texture == null)
                {
                    iconTexture.Texture = dialogueHolder.emotions[0].texture;
                }
            }





        }

    }

    void SetNPCSfxName(string name)
    {
        currentNPCSFXName = name;
    }

    void ChangeSound(string soundSFXName)
    {
        isInUse = true;
        currentBalloon.Set("soundName", soundSFXName);
    }

    void ResetSound()
    {
        isInUse = false;
        currentBalloon.Set("soundName", dialogueHolder.sfxName);
    }
}

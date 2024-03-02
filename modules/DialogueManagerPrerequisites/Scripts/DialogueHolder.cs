using Godot;
using System;

[GlobalClass]
public partial class DialogueHolder : Node
{
    [ExportGroup("Essentials")]
    [Export] public StringName characterName;
    [Export] public Resource dialogue;
    [ExportGroup("Auxilliary")]
    [Export] public ProfileEmotions[] emotions;
    [Export] public Font font;
    [Export] public StringName sfxName;
    [Export] public float baseSpeed = 0.08f;
    [Export] public bool metBefore = false;
}

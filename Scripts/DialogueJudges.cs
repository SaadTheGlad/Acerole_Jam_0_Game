using Godot;
using System;

[GlobalClass]
public partial class DialogueJudges : Resource
{
    [Export] public string dialogueName;
    [Export] public Resource dialogue;
}

using Godot;
using System;

[GlobalClass]
public partial class Eyes : Resource
{
    [Export] public Texture2D irisInk;
    [Export] public Texture2D irisColour;
    [Export] public Texture2D highlights;
    [Export] public Texture2D eyebrows;
    [Export] public Texture2D sclera;
}
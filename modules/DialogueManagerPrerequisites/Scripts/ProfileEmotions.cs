using Godot;
using System;

[GlobalClass]
public partial class ProfileEmotions : Resource
{
    [Export] public string emotionName;
    [Export] public Texture2D texture;
}

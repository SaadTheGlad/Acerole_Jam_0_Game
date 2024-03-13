using Godot;
using System;

public partial class FadeIn : Node2D
{
    public void Destroy()
    {
        QueueFree();
    }
}

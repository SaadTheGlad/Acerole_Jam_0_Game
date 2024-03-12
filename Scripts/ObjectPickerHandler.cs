using Godot;
using System;

public partial class ObjectPickerHandler : Node
{
    [Export] public Node2D groundLevel;
    float groundLevelY;

    public override void _Ready()
    {
        groundLevelY = groundLevel.GlobalPosition.Y;
    }


}

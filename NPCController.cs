using Godot;
using System;

public partial class NPCController : Node
{
    [Export] private Node2D guy;
    [Export] private float movementSpeed = 10f;

    bool ringed = false;

    private const float CENTEROFWINDOWX = 518f;

    public void Ring()
    {
        ringed = true;
    }

    public override void _PhysicsProcess(double delta)
    {
        if(ringed)
        {
            MoveGuy(delta);
        }
    }

    private void MoveGuy(double delta)
    {
        GD.Print("moving");
        guy.GlobalPosition = guy.GlobalPosition.Lerp(new Vector2(CENTEROFWINDOWX, guy.GlobalPosition.Y), (float)delta * movementSpeed);
    }
}

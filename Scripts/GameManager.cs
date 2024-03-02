using Godot;
using System;

[GlobalClass]
public partial class GameManager : Node
{
    public static GameManager Instance { get; private set; }

    public override void _EnterTree()
    {

        if (Instance != null)
        {
            GD.Print("More than one ", Instance.Name);

        }
        else
        {
            Instance = this;
        }
        
    }
    public override void _Process(double delta)
    {
        if (Input.IsActionJustPressed("restart"))
        {
            GetTree().ReloadCurrentScene();
        }
    }
}

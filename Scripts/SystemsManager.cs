using Godot;
using System;

public partial class SystemsManager : Node
{
    public void Ring()
    {
        SignalsManager.Instance.EmitSignal(SignalsManager.SignalName.CreatedNPC);
    }
}

using Godot;
using System;

public partial class SFXManager : Node
{
    public void PlayButton()
    {
        //playbutton
        AudioManager.Instance.Play("click");
    }
}

using Godot;
using System;

public partial class FPSCounter : Label
{

	public override void _Process(double delta)
	{
		float fps = (float)Engine.GetFramesPerSecond();
		Text = "FPS: " + fps.ToString();

	}
}

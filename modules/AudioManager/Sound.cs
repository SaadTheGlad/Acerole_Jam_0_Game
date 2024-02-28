using Godot;
using System;
using System.ComponentModel.DataAnnotations;

[GlobalClass]
public partial class Sound : Resource
{
	[Export] public string name;
	[Export] public AudioStream AudioStream { get; set; }
	[Export(PropertyHint.Range, "-60, 0,")] public float volume = 0f;
	[Export(PropertyHint.Range, "0, 1,")] public float pitch = 1f;
	[Export] public Buses bus;


	public AudioStreamPlayer player { get; set; }
	public bool ghostIsPlaying;
}

public enum Buses
{
	Master,
	Music,
	SFX,
	Reverb
}

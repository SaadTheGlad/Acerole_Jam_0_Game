using Godot;
using System;
using System.ComponentModel.DataAnnotations;

[GlobalClass]
public partial class LevelScene : Resource
{
	[Export] public string Name;
	[Export] public PackedScene Level;

}
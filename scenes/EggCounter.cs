using Godot;
using System;

public partial class EggCounter : Label
{
	public override void _Ready()
	{
		GGJ2026.scripts.Player.EggCountChanged += () =>
		{
			Text = $"Eggs: {GGJ2026.scripts.Player.EggCount}";
		};
	}
	
	public override void _Process(double delta)
	{
	}
}

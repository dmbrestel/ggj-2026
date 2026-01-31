using Godot;
using System;

public partial class EggCounter : Label
{
	public override void _Ready()
	{
		Player.EggCountChanged += () =>
		{
			Text = $"Eggs: {Player.EggCount}";
		};
	}
	
	public override void _Process(double delta)
	{
	}
}

using System;
using Godot;

public partial class Timer : Label
{
	private double _timeElapsed;
	
	public override void _Ready()
	{
		_timeElapsed = 0.0;
	}
	
	public override void _Process(double delta)
	{
		_timeElapsed += delta;
		
		Text = TimeSpan.FromSeconds(_timeElapsed).ToString(@"mm\:ss\:ff");
	}
}

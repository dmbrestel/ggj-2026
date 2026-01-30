using Godot;
using System;

public partial class Bullet : Area2D
{

	private int _speed = 750;

	public override void _Ready()
	{
		BodyEntered += OnBodyEntered;
	}
	
	public override void _PhysicsProcess(double delta)
	{
		Position += Transform.X * _speed * (float)delta;
	}

	private void OnBodyEntered(Node2D body)
	{
		QueueFree();
	}
}

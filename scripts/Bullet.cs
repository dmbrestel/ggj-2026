using Godot;
using System;

public partial class Bullet : Area2D
{

	[Export] public float Speed = 400f;
	[Export] public float DespawnAfter = 3.0f;

	public override void _Ready()
	{
		BodyEntered += OnBodyEntered;
		// despawn
		GetTree().CreateTimer(DespawnAfter).Timeout += () => QueueFree();
	}
	
	public override void _PhysicsProcess(double delta)
	{
		Position += Transform.X * Speed * (float)delta;
		
	}

	private void OnBodyEntered(Node2D body)
	{
		QueueFree();
	}
}

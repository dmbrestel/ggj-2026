using Godot;
using System;

public partial class Enemy : CharacterBody2D
{
	[Export]
	public Node2D PlayerNode; 

	private NavigationAgent2D _navAgent;
	private float _speed = 70.0f;

	public override void _Ready()
	{
		_navAgent = GetNode<NavigationAgent2D>("NavigationAgent2D");
		
		_navAgent.PathDesiredDistance = 4.0f;
		_navAgent.TargetDesiredDistance = 4.0f;
	}

	public override void _PhysicsProcess(double delta)
	{
		float distance = GlobalPosition.DistanceTo(PlayerNode.Position);

		if (distance < 1000 && distance > 200)
		{
			Vector2 direction = GlobalPosition.DirectionTo(PlayerNode.GlobalPosition);
			Velocity = direction * _speed;
			MoveAndSlide();
		}
	}
}

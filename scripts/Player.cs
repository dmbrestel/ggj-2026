using Godot;
using System;

public partial class Player : CharacterBody2D
{
	[Export] public float Speed = 200f;

	private Weapon _weapon;

	public override void _Ready()
	{
		_weapon = GetNode<Weapon>("Weapon");
		_weapon.Initialize(this);
	}

	public override void _Input(InputEvent @event)
	{
		if (@event.IsActionPressed("shoot"))
		{
			_weapon.TryShoot();
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		Vector2 direction = Input.GetVector("left", "right", "up", "down");

		if (direction != Vector2.Zero)
		{
			Velocity = direction.Normalized() * Speed;
		}
		else
		{
			Velocity = Velocity.MoveToward(Vector2.Zero, Speed);
		}

		MoveAndSlide();
	}
}

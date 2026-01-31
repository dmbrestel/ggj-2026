using Godot;
using System;

public partial class Player : CharacterBody2D
{
	[Export] public float Speed = 200f;
	[Export] public float OxygenLasts = 30.0f;

	// nodes
	private Weapon _weapon;
	private ProgressBar _oxygenBar;
	private ProgressBar _healthBar;
	private Label _ammoLabel;
	
	private float _oxygen = 1.0f;
	private float _maxOxygen = 1.0f;
	private float _oxygenDecreaseRate;

	private float _health = 1.0f;
	private float _maxHealth = 1.0f;

	private int _ticksPerSecond = Engine.PhysicsTicksPerSecond;
	

	public override void _Ready()
	{
		_weapon = GetNode<Weapon>("Weapon");
		_weapon.Initialize(this);
		
		_oxygenDecreaseRate = _maxOxygen / (OxygenLasts * _ticksPerSecond);
		_oxygenBar = GetNode<ProgressBar>("/root/Node2D/CanvasLayer/UserInterface/Oxygen");
		
		_healthBar = GetNode<ProgressBar>("/root/Node2D/CanvasLayer/UserInterface/Health");
		
		_ammoLabel = GetNode<Label>("/root/Node2D/CanvasLayer/UserInterface/Ammunition");
	}

	public override void _Input(InputEvent @event)
	{
		if (@event.IsActionPressed("shoot"))
		{
			_weapon.TryShoot();
		} else if (@event.IsActionPressed("reload"))
		{
			_weapon.Reload();
		}
		// update ammo display after weapon actions
		_ammoLabel.SetText($"{_weapon.AmmoInMag} / {_weapon.Ammunition}");

	}

	public override void _PhysicsProcess(double delta)
	{
		// movement
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
		
		// oxygen decrease
		_oxygen -= _oxygenDecreaseRate;
		_oxygenBar.SetValue(_oxygen);
		
		// health management
		_healthBar.SetValue(_health);
		
	}

	public void AddAmmo(int ammo)
	{
		_weapon.Ammunition += ammo;
		_ammoLabel.SetText($"{_weapon.AmmoInMag} / {_weapon.Ammunition}");
	}
}

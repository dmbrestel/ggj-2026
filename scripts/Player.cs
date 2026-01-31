using System;
using Godot;

namespace GGJ2026.scripts;

public partial class Player : CharacterBody2D
{
	[Export] public float Speed = 200f;
	[Export] public float OxygenLasts = 30.0f;
	[Export] public float HealthLasts = 45.0f;
	
	[Export] PackedScene EndScreenScene;

	// nodes
	private Weapon _weapon;
	private ProgressBar _oxygenBar;
	private ProgressBar _healthBar;
	private Label _ammoLabel;
	private AnimatedSprite2D _sprite;
	
	// movement animation
	private float _bobTime;
	private Vector2 _originalOffset;
	
	// ressources
	private float _oxygen = 1.0f;
	private float _maxOxygen = 1.0f;
	private float _oxygenDecreaseRate;

	private float _health = 1.0f;
	private float _maxHealth = 1.0f;
	private float _healthDecreaseRate;

	private int _ticksPerSecond = Engine.PhysicsTicksPerSecond;
	
	private CanvasLayer _canvasLayer;
	

	public override void _Ready()
	{
		_weapon = GetNode<Weapon>("Weapon");
		_weapon.Initialize(this);
		
		_oxygenDecreaseRate = _maxOxygen / (OxygenLasts * _ticksPerSecond);
		_oxygenBar = GetNode<ProgressBar>("/root/Node2D/CanvasLayer/UserInterface/Oxygen");
		
		_healthDecreaseRate = _maxHealth / (HealthLasts * _ticksPerSecond);
		_healthBar = GetNode<ProgressBar>("/root/Node2D/CanvasLayer/UserInterface/Health");
		
		_ammoLabel = GetNode<Label>("/root/Node2D/CanvasLayer/UserInterface/Ammunition");
		
		_sprite =  GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		_originalOffset = _sprite.Offset;
		
		_canvasLayer = GetNode<CanvasLayer>("/root/Node2D/CanvasLayer");
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
		
		// movement animation
		if (Velocity.Length() > 0.1f)
		{
			float angle = Velocity.Angle();
			_sprite.Frame = AngleToFrame(angle);
			
			// up and down
			_bobTime += (float)delta * 15f;
			_sprite.Offset = _originalOffset + new Vector2(0, Mathf.Sin(_bobTime) * 50f);
		}
		else
		{
			_bobTime = 0f;
			_sprite.Offset = _originalOffset;
		}
		
		// oxygen decrease
		_oxygen -= _oxygenDecreaseRate;
		
		if (_oxygen <= 0f)
		{
			_health -= _healthDecreaseRate;
			_oxygen = 0f;
		}
		
		_oxygenBar.SetValue(_oxygen);
		
		// health management
		_healthBar.SetValue(_health);
		
		if (_health <= 0f)
		{
			GetTree().Paused = true;
			var endScreen = EndScreenScene.Instantiate<GGJ2026.scenes.EndScreen>();
			_canvasLayer.CallDeferred("add_child", endScreen);
			endScreen.SetValues(GGJ2026.scenes.Timer.GameTime, EggCount);
		}
		
	}

	public void AddAmmo(int ammo)
	{
		_weapon.Ammunition += ammo;
		_ammoLabel.SetText($"{_weapon.AmmoInMag} / {_weapon.Ammunition}");
	}

	private int AngleToFrame(float angle)
	{
		float degrees = Mathf.RadToDeg(angle);
		if (degrees < 0) degrees += 360f;

		int[] frameMap = { 4, 3, 7, 2, 5, 0, 6, 1 }; // right, down-right, down, down-left, left, up-left, up, up-right

		int index = (int)Mathf.Round(degrees / 45f) % 8;
		return frameMap[index];
	}

	public static int EggCount = 0;
	
	public void AddEgg()
	{
		EggCount++;
		
		EggCountChanged?.Invoke();
	}
	
	public static event Action EggCountChanged;
}
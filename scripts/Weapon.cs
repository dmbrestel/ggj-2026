using Godot;
using System;

public partial class Weapon : Node2D
{

	[Export] public PackedScene Bullet;
	[Export] public float FireRate = 0.2f;

	private Node2D _owner;
	private Marker2D _muzzle;
	private bool _canFire = true;
	private bool _canReload = true;
	private const int MagSize = 6;
	public int AmmoInMag = 6;
	public int Ammunition = 12; 
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_muzzle = GetNode<Marker2D>("Muzzle");
	}

	public void Initialize(Node2D owner)
	{
		_owner = owner;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		LookAt(GetGlobalMousePosition());

		var sprite = GetNode<Sprite2D>("Sprite2D");
		bool flipped = GetGlobalMousePosition().X < GlobalPosition.X;
		sprite.FlipV = flipped;
		_muzzle.Position = new Vector2(_muzzle.Position.X, Mathf.Abs(_muzzle.Position.Y) * (flipped ? 1 : -1));
	}

	public void TryShoot()
	{
		if (!_canFire) return;
		if (AmmoInMag <= 0) return;

		var bullet = Bullet.Instantiate<Area2D>();
		GetTree().CurrentScene.AddChild(bullet);
		bullet.GlobalPosition = _muzzle.GlobalPosition;
		bullet.Rotation = Rotation;
		
		AmmoInMag--;
		GD.Print($"AmmoInMag{AmmoInMag}");

		StartCoolDown();
	}

	public void Reload()
	{
		if (!_canReload) return;
		if (Ammunition <= 0) return;

		var bulletsReloaded = Math.Min(MagSize - AmmoInMag, Ammunition);
		Ammunition -= bulletsReloaded;
		AmmoInMag += bulletsReloaded;
		GD.Print($"AmmoInMag{AmmoInMag}");
		GD.Print($"Ammunition{Ammunition}");
		
		// rotate sprite
		var sprite = GetNode<Sprite2D>("Sprite2D");
		var tween = CreateTween();
		tween.TweenProperty(sprite, "rotation", Mathf.Tau, 0.4f).From(0f);
		
		StartCoolDown();

	}

	private async void StartCoolDown()
	{
		_canFire = false;
		await ToSignal(GetTree().CreateTimer(FireRate), "timeout");
		_canFire = true;
	}
}

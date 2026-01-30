using Godot;
using System;

public partial class Weapon : Node2D
{

	[Export] public PackedScene Bullet;
	[Export] public float FireRate = 0.2f;

	private Node2D _owner;
	private Marker2D _muzzle;
	private bool _canFire = true;
	
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

		var bullet = Bullet.Instantiate<Area2D>();
		GetTree().CurrentScene.AddChild(bullet);
		bullet.GlobalPosition = _muzzle.GlobalPosition;
		bullet.Rotation = Rotation;

		StartCoolDown();
	}

	private async void StartCoolDown()
	{
		_canFire = false;
		await ToSignal(GetTree().CreateTimer(FireRate), "timeout");
		_canFire = true;
	}
}

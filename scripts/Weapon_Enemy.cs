using Godot;
using System;

public partial class Weapon_Enemy : Node2D
{

	[Export] public PackedScene Bullet;
	public float FireRate = 0.7f;
	public Node2D PlayerNode;

	private Node2D _owner;
	private Marker2D _muzzle;
	private bool _canFire = true;
	private bool _canReload = true;
	private const int MagSize = 6;
	public int AmmoInMag = 6;
	public int Ammunition = 12; 
	
	private float _timeSinceLastShot = 0.0f;
	
	
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_muzzle = GetNode<Marker2D>("Muzzle");
		
		var players = GetTree().GetNodesInGroup("Player");
		if (players.Count > 0)
		{
			PlayerNode = (Node2D)players[0];
		}
	}

	public void Initialize(Node2D owner)
	{
		_owner = owner;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		LookAt(PlayerNode.GlobalPosition);
		_timeSinceLastShot += (float)delta;

		var sprite = GetNode<Sprite2D>("Sprite2D");
		bool flipped = GetGlobalMousePosition().X < GlobalPosition.X;
		sprite.FlipV = flipped;
		_muzzle.Position = new Vector2(_muzzle.Position.X, Mathf.Abs(_muzzle.Position.Y) * (flipped ? 1 : -1));
	}

	public void Shoot()
	{
		// Only shoot if the timer is stopped
		if (_timeSinceLastShot < FireRate) return;
		
		var bullet = Bullet.Instantiate<Area2D>();
		GetTree().CurrentScene.AddChild(bullet);
		bullet.GlobalPosition = _muzzle.GlobalPosition;
		bullet.Rotation = Rotation;

		_timeSinceLastShot = 0;
	}
	
	
	
}

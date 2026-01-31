using Godot;
using System;

public partial class Enemy : CharacterBody2D
{
	private int _ticksPerSecond = Engine.PhysicsTicksPerSecond;
	
	public Node2D PlayerNode;
	
	// movement animation
	private float _bobTime;
	private Vector2 _originalOffset;

	private float _speed = 70.0f;
	private float _health = 1.0f;
	
	// nodes
	private Weapon_Enemy _weapon;
	private Mask _mask;
	private AnimatedSprite2D _sprite;
	
	private CanvasLayer _canvasLayer;

	public override void _Ready()
	{
		_weapon = GetNode<Weapon_Enemy>("Weapon");
		_weapon.Initialize(this);
		_mask = GetNode<Mask>("Mask");
		
		_sprite =  GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		_originalOffset = _sprite.Offset;
		
		_canvasLayer = GetNode<CanvasLayer>("/root/Node2D/CanvasLayer");
		
		var players = GetTree().GetNodesInGroup("Player");
		if (players.Count > 0)
		{
			PlayerNode = (Node2D)players[0];
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		float distance = GlobalPosition.DistanceTo(PlayerNode.Position);

		if (distance < 1000)
		{
			_weapon.Shoot();
			
			if (distance > 400)
			{
				Vector2 direction = GlobalPosition.DirectionTo(PlayerNode.GlobalPosition);
				Velocity = direction * _speed;
				MoveAndSlide();
				
				if (Velocity.Length() > 0.1f)
				{
					float angle = Velocity.Angle();
					int frame = AngleToFrame(angle);
					_sprite.Frame = frame;
					_mask?.SetDirection(frame);
				
					// up and down
					_bobTime += (float)delta * 15f;
					var offset = new Vector2(0, Mathf.Sin(_bobTime) * 50f);
					_sprite.Offset = _originalOffset + offset;
					_mask.MoveSprite(offset);
				}
				else
				{
					_bobTime = 0f;
					_sprite.Offset = _originalOffset;
					_mask.MoveSprite(Vector2.Zero);
				}
			}
			
		}
		
		if (_health <= 0f)
		{
			
		}
	}
	
	private int AngleToFrame(float angle)
	{
		float degrees = Mathf.RadToDeg(angle);
		if (degrees < 0) degrees += 360f;

		int[] frameMap = { 4, 3, 7, 2, 5, 0, 6, 1 }; // right, down-right, down, down-left, left, up-left, up, up-right

		int index = (int)Mathf.Round(degrees / 45f) % 8;
		return frameMap[index];
	}

	public void TakeDamage()
	{
		_health -= 1f;
		
		GetNode<CollisionShape2D>("CollisionShape2D").SetDeferred("disabled", true);
		QueueFree();
	}
}

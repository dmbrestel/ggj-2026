using Godot;

public partial class Mask : Node2D
{
	[Export] public float SpeedMultiplier = 1.0f;
	[Export] public float OxygenRateMultiplier = 1.0f;

	private AnimatedSprite2D _sprite;
	
	private Vector2 _originalOffset;

	public override void _Ready()
	{
		_sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
	}

	public void SetDirection(int bodyFrame)
	{
		// Body frames: 0=up-left, 1=up-right, 2=down-left, 3=down-right,
		//              4=right, 5=left, 6=back, 7=front
		// Mask frames: 0=right-side, 1=front-right, 2=back, 3=front
		(int maskFrame, bool behind) = bodyFrame switch
		{
			0 => (2, true),   // up-left → up
			1 => (2, true),  // up-right → up
			2 => (3, false),   // down-left → down
			3 => (3, false),  // down-right → down
			4 => (0, false),  // right → right-side
			5 => (1, false),   // left → left
			6 => (2, true),  // back → up
			7 => (3, false),  // front → down
			_ => (3, false)
		};

		_sprite.Frame = maskFrame;
		if (behind)
		{
			_sprite.SetZIndex(-1);
		}
		else
		{
			_sprite.SetZIndex(0);
		}
	}

	public void MoveSprite(Vector2 offset)
	{
		_sprite.Offset = _originalOffset + offset;
	}
}
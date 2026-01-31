using GGJ2026.scripts;
using Godot;

namespace GGJ2026.scenes;

public partial class Food : Area2D
{
	[Export] public float extraHealth = 0.25f;
	
	public override void _Ready()
	{
		BodyEntered += OnBodyEntered;
	}

	private void OnBodyEntered(Node2D body)
	{
		if (body is not Player player || player.IsHealthSomewhatFull) return;
		
		player.AddHealth(extraHealth);
		QueueFree();
	}
}
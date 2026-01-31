using GGJ2026.scripts;
using Godot;

namespace GGJ2026.scenes;

public partial class Oxygen : Area2D
{
	public override void _Ready()
	{
		BodyEntered += OnBodyEntered;
	}

	private void OnBodyEntered(Node2D body)
	{
		if (body is not Player player) return;
		
		player.AddOxygen();
		QueueFree();
	}
}
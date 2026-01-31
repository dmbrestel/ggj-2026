using Godot;
using GGJ2026.scripts;

public partial class Story : Area2D
{
	private bool isDeleting = false;
	private float deleteTimer = 0f;
	
	public override void _Ready()
	{
		BodyEntered += OnBodyEntered;
	}

	private void OnBodyEntered(Node2D body)
	{
		if (body is Player)
		{
			isDeleting = true;
		}
	}
	
	override public void _Process(double delta)
	{
		if (isDeleting)
		{
			deleteTimer += (float)delta;
			if (deleteTimer >= 5f)
			{
				QueueFree();
			}
		}
	}
}

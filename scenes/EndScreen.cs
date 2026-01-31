using Godot;

namespace GGJ2026.scenes;

public partial class EndScreen : Control
{
	[Export] public Label StuffLabel;
	
	public void SetValues(string time, int eggs)
	{
		var eggText = eggs > 0 ? $" and you collected {eggs} eggs!" : "!";
		StuffLabel.Text = $"You survived for {time}{eggText}";
		
		GetNode<Button>("Button").Pressed += () =>
		{
			GetTree().Paused = false;
			GetTree().ReloadCurrentScene();

			scripts.Player.EggCount = 0;
		};
	}
	
	public override void _Ready()
	{
	}
	
	public override void _Process(double delta)
	{
	}
}
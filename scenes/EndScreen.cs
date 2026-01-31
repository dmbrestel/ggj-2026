using Godot;

public partial class EndScreen : Control
{
	[Export] public Label StuffLabel;
	
	public void SetValues(string time, int eggs)
	{
		var eggText = eggs > 0 ? $" and you collected {eggs} eggs!" : "!";
		StuffLabel.Text = $"You survived for {time}{eggText}";
	}
	
	public override void _Ready()
	{
	}
	
	public override void _Process(double delta)
	{
	}
}

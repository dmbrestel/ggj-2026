using Godot;

namespace GGJ2026.scripts;

public partial class MapGenerator : Node
{
	private TileMapLayer TerrainLayer { get; set; }
	
	public override void _Ready()
	{
		TerrainLayer = GetNode<TileMapLayer>("TerrainLayer");
		
		for (var x = -10; x <= 10; x++)
		{
			for (var y = -10; y <= 10; y++)
			{
				TerrainLayer.SetCell(new Vector2I(x, y), 1, new Vector2I(1, 1));
			}
		}
	}
	
	public override void _Process(double delta)
	{
	}
}
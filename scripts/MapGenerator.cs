using GGJ2026.scripts.terrain;
using Godot;

namespace GGJ2026.scripts;

public partial class MapGenerator : Node
{
	[Export]
	public int Size { get; set; } = 20;
	
	public override void _Ready()
	{
		var terrainLayer = GetNode<TileMapLayer>("TerrainLayer");
		
		var rng = new RandomNumberGenerator();
		var map = new Map(rng, Size);
		
		map.Place((position, area) =>
		{
			var terrain = area switch
			{
				Area.Street => Terrain.Asphalt,
				Area.Grassland => Terrain.Grass,
				Area.Wasteland => Terrain.Sand,
				Area.Pond => Terrain.Water,
				Area.Forest => Terrain.Grass,
				Area.House => Terrain.House,
				_ => Terrain.Grass
			};
			
			for (var x = 0; x < Areas.Size; x++)
			{
				for (var y = 0; y < Areas.Size; y++)
				{
					var tilePosition = new Vector2I(position.X * Areas.Size + x, position.Y * Areas.Size + y);
					var randomOffset = (x + y) % 2;
					
					terrainLayer.SetCell(tilePosition, 0, new Vector2I(randomOffset, (int) terrain));
				}
			}
		});
	}
	
	public override void _Process(double delta)
	{
	}
}
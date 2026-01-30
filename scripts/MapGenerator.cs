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
		
		RandomNumberGenerator rng = new();
		Map map = new(rng, Size);
		
		map.Place((pos, area) =>
		{
			var terrain = area switch
			{
				Area.Street => Terrain.Asphalt,
				Area.Grassland => Terrain.Grass,
				Area.Pond => Terrain.Water,
				Area.Forest => Terrain.Grass,
				Area.House => Terrain.House,
				_ => Terrain.Grass
			};
			
			
		});
	}
	
	public override void _Process(double delta)
	{
	}
}
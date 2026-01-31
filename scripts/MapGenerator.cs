using GGJ2026.scripts.terrain;
using Godot;

namespace GGJ2026.scripts;

public partial class MapGenerator : Node
{
	[Export]
	public int Size { get; set; } = 20;
	
	private const int Variations = 2;
	
	public override void _Ready()
	{
		var terrainLayer = GetNode<TileMapLayer>("TerrainLayer");
		var objectLayer = GetNode<TileMapLayer>("ObjectLayer");
		
		var rng = new RandomNumberGenerator();
		var map = new Map(rng, Size);
		
		var totalSize = new Vector2I(map.Size * Areas.Size.X, map.Size * Areas.Size.Y);
		var offset = -(totalSize / 2);
		
		map.Place((position, area) =>
		{
			switch (area)
			{
				case Area.Street:
					PlaceStreet(position);
					return;
				
				case Area.Grassland:
					PlaceGrassland(position);
					return;
				
				case Area.Wasteland:
					PlaceWasteland(position);
					return;
				
				case Area.Pond:
					PlacePond(position);
					return;
				
				case Area.Forest:
					PlaceForest(position);
					return;
				
				case Area.House:
					PlaceHouse(position);
					return;
				
				case Area.Ocean:
					PlaceOcean(position);
					return;
			}
		});
		
		return;
		
		void PlaceStreet(Vector2I areaPosition)
		{
			for (var x = 0; x < Areas.Size.X; x++)
			{
				for (var y = 0; y < Areas.Size.Y; y++)
				{
					var tilePosition = new Vector2I(areaPosition.X * Areas.Size.X + x, areaPosition.Y * Areas.Size.Y + y) + offset;
					
					var isAtEdge = x == 0 || x == Areas.Size.X - 1 || y == 0 || y == Areas.Size.Y - 1;
					var isAtCorner = (x == 0 || x == Areas.Size.X - 1) && (y == 0 || y == Areas.Size.Y - 1);

					if (isAtEdge && !isAtCorner)
					{
						// Check if the area next to this edge is also a street
						var adjacentAreaPosition = x is 0
							? new Vector2I(areaPosition.X - 1, areaPosition.Y)
							: x == Areas.Size.X - 1
								? new Vector2I(areaPosition.X + 1, areaPosition.Y)
								: y is 0
									? new Vector2I(areaPosition.X, areaPosition.Y - 1)
									: new Vector2I(areaPosition.X, areaPosition.Y + 1);
						
						if (map.GetArea(adjacentAreaPosition.X, adjacentAreaPosition.Y) == Area.Street)
						{							
							SetCell(terrainLayer, tilePosition, Terrain.Asphalt, rng);
							
							continue;
						}
					}
					else if (!isAtEdge && !isAtCorner)
					{
						SetCell(terrainLayer, tilePosition, Terrain.Asphalt, rng);
						
						continue;
					}
					
					SetCell(terrainLayer, tilePosition, Terrain.Grass, rng);
				}
			}
		}
		
		void PlaceGrassland(Vector2I areaPosition)
		{
			for (var x = 0; x < Areas.Size.X; x++)
			{
				for (var y = 0; y < Areas.Size.Y; y++)
				{
					var tilePosition = new Vector2I(areaPosition.X * Areas.Size.X + x, areaPosition.Y * Areas.Size.Y + y) + offset;
					SetCell(terrainLayer, tilePosition, Terrain.Grass, rng);
				}
			}
		}
		
		void PlaceWasteland(Vector2I areaPosition)
		{
			for (var x = 0; x < Areas.Size.X; x++)
			{
				for (var y = 0; y < Areas.Size.Y; y++)
				{
					var tilePosition = new Vector2I(areaPosition.X * Areas.Size.X + x, areaPosition.Y * Areas.Size.Y + y) + offset;

					var distanceFromCenter = new Vector2I(
						Mathf.Abs(x - Areas.Size.X / 2),
						Mathf.Abs(y - Areas.Size.Y / 2)
					).Length();
					
					var maxDistance = new Vector2I(Areas.Size.X / 2, Areas.Size.Y / 2).Length();
					
					var sandChance = 1.0 - distanceFromCenter / maxDistance;

					SetCell(terrainLayer, tilePosition, rng.Randf() < sandChance ? Terrain.Sand : Terrain.Grass, rng);
				}
			}
		}
		
		void PlacePond(Vector2I areaPosition)
		{
			for (var x = 0; x < Areas.Size.X; x++)
			{
				for (var y = 0; y < Areas.Size.Y; y++)
				{
					var tilePosition = new Vector2I(areaPosition.X * Areas.Size.X + x, areaPosition.Y * Areas.Size.Y + y) + offset;
					
					var isAtEdge = x == 0 || x == Areas.Size.X - 1 || y == 0 || y == Areas.Size.Y - 1;
					SetCell(terrainLayer, tilePosition, isAtEdge ? Terrain.Grass : Terrain.Water, rng);
				}
			}
		}
		
		void PlaceForest(Vector2I areaPosition)
		{
			for (var x = 0; x < Areas.Size.X; x++)
			{
				for (var y = 0; y < Areas.Size.Y; y++)
				{
					var tilePosition = new Vector2I(areaPosition.X * Areas.Size.X + x, areaPosition.Y * Areas.Size.Y + y) + offset;
					SetCell(terrainLayer, tilePosition, Terrain.Grass, rng);
					
					// Place trees randomly
					if (rng.RandiRange(0, 100) < 30) // 30% chance to place a tree
					{
						SetCell(objectLayer, tilePosition, 1);
					}
				}
			}
		}
		
		void PlaceHouse(Vector2I areaPosition)
		{
			for (var x = 0; x < Areas.Size.X; x++)
			{
				for (var y = 0; y < Areas.Size.Y; y++)
				{
					var tilePosition = new Vector2I(areaPosition.X * Areas.Size.X + x, areaPosition.Y * Areas.Size.Y + y) + offset;
					
					var isAtEdge = x == 0 || x == Areas.Size.X - 1 || y == 0 || y == Areas.Size.Y - 1;
					SetCell(terrainLayer, tilePosition, isAtEdge ? Terrain.Grass : Terrain.House, rng);
				}
			}
		}
		
		void PlaceOcean(Vector2I areaPosition)
		{
			for (var x = 0; x < Areas.Size.X; x++)
			{
				for (var y = 0; y < Areas.Size.Y; y++)
				{
					var tilePosition = new Vector2I(areaPosition.X * Areas.Size.X + x, areaPosition.Y * Areas.Size.Y + y) + offset;
					SetCell(terrainLayer, tilePosition, Terrain.Water, rng);
				}
			}
		}
	}
	
	private void SetCell(TileMapLayer layer, Vector2I position, Terrain terrain, RandomNumberGenerator rng)
	{
		var randomOffset = rng.RandiRange(0, Variations - 1);
		layer.SetCell(position, 0, new Vector2I(randomOffset, (int) terrain));
	}

	private void SetCell(TileMapLayer layer, Vector2I position, int id)
	{
		layer.SetCell(position, id);
	}
	
	public override void _Process(double delta)
	{
	}
}
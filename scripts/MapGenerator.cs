using System.Collections.Generic;
using GGJ2026.scripts.terrain;
using Godot;

namespace GGJ2026.scripts;

public partial class MapGenerator : Node
{
	[Export]
	public int Size { get; set; } = 50;
	
	[Export]
	public PackedScene Ammunition { get; set; }
	
	[Export]
	public  PackedScene Egg { get; set; }
	
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

						var center = new Vector2I(Areas.Size.X / 2, Areas.Size.Y / 2);
						
						if ((x == center.X || y == center.Y) && map.GetArea(adjacentAreaPosition.X, adjacentAreaPosition.Y) == Area.House)
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
					
					List<int> treeIDs = [1, 14, 15, 16];
					
					// Place trees randomly
					if (rng.RandiRange(0, 100) < 30) // 30% chance to place a tree
					{
						SetCell(objectLayer, tilePosition, treeIDs[rng.RandiRange(0, treeIDs.Count - 1)]);
					}
				}
			}
		}
		
		void PlaceHouse(Vector2I areaPosition)
		{
			var directionToStreet = Vector2I.Zero;
			foreach (var (nx, ny) in new[] { new Vector2I(-1, 0), new Vector2I(1, 0), new Vector2I(0, -1), new Vector2I(0, 1) })
			{
				var neighborArea = map.GetArea(areaPosition.X + nx, areaPosition.Y + ny);
				if (neighborArea != Area.Street) continue;
				directionToStreet = new Vector2I(nx, ny);
				break;
			}
			
			var center = new Vector2I(Areas.Size.X / 2, Areas.Size.Y / 2);
			var doorPosition = center + directionToStreet * (Areas.Size.X / 2 - 1);
			var doorStreetPosition = center + directionToStreet * (Areas.Size.X / 2 - 0);
			
			for (var x = 0; x < Areas.Size.X; x++)
			{
				for (var y = 0; y < Areas.Size.Y; y++)
				{
					var tilePosition = new Vector2I(areaPosition.X * Areas.Size.X + x, areaPosition.Y * Areas.Size.Y + y) + offset;
					
					var isAtEdge = x == 0 || x == Areas.Size.X - 1 || y == 0 || y == Areas.Size.Y - 1;
					SetCell(terrainLayer, tilePosition, isAtEdge ? Terrain.Grass : Terrain.House, rng);
					
					if (x == doorStreetPosition.X && y == doorStreetPosition.Y)
					{
						SetCell(terrainLayer, tilePosition, Terrain.Asphalt, rng);
					}
					
					var isDoor = x == doorPosition.X && y == doorPosition.Y;
					
					var isAtWall = x == 1 || x == Areas.Size.X - 2 || y == 1 || y == Areas.Size.Y - 2;
					if (!isDoor && isAtWall && x > 0 && x < Areas.Size.X - 1 && y > 0 && y < Areas.Size.Y - 1)
					{
						var objectId = 2;
						
						var wx = x - 1;
						var wy = y - 1;
						
						var mx = Areas.Size.X - 3;
						var my = Areas.Size.Y - 3;
						
						if (wx == 0 && wy == 0) objectId = 11;
						else if (wx == 0 && wy == my) objectId = 10;
						else if (wx == mx && wy == 0) objectId = 12;
						else if (wx == mx && wy == my) objectId = 13;
						else if (wx == 0) objectId = 2;
						else if (wx == mx) objectId = 9;
						else if (wy == 0) objectId = 8;
						else if (wy == my) objectId = 3;
						
						SetCell(objectLayer, tilePosition - new Vector2I(0, 1), objectId);
					}
					
					var inside = x > 1 && x < Areas.Size.X - 2 && y > 1 && y < Areas.Size.Y - 2;
					if (!inside) continue;
					
					if (rng.RandiRange(0, 100) < 10)
					{
						PlaceObject(objectLayer, Ammunition, tilePosition);
					}
					else if (rng.RandiRange(0, 100) < 1)
					{
						PlaceObject(objectLayer, Egg, tilePosition);
					}
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
		layer.SetCell(position, id, new Vector2I(0, 0));
	}
	
	private void PlaceObject(TileMapLayer layer, PackedScene scene, Vector2I cell)
	{
		var instance = scene.Instantiate<Node2D>();
		GetParent().CallDeferred("add_child", instance);
		instance.Position = layer.MapToLocal(cell - new Vector2I(0, 1)) + layer.Position;
	}
	
	public override void _Process(double delta)
	{
	}
}
using System;
using Godot;

namespace GGJ2026.scripts.terrain;

public class Map
{
    private readonly Area[,] areas;
    
    public Map(RandomNumberGenerator rng, int size)
    {
        areas = new Area[size, size];
        
        for (var x = 0; x < size; x++)
        {
            for (var y = 0; y < size; y++)
            {
                areas[x, y] = Area.Grassland;
            }
        }
    }

    public void Place(Action<Vector2I, Area> placeArea)
    {
        for (var x = 0; x < areas.GetLength(0); x++)
        {
            for (var y = 0; y < areas.GetLength(1); y++)
            {
                placeArea(new Vector2I(x, y), areas[x, y]);
            }
        }
    }
}
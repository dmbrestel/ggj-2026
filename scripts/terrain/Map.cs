using System;
using System.Collections.Generic;
using Godot;

namespace GGJ2026.scripts.terrain;

public class Map
{
    private readonly Area[,] _areas;
    private readonly int _innerSize;
    private readonly int _totalSize;
    
    private const int Expansion = 5;
    
    public int Size => _totalSize;
    
    public Map(RandomNumberGenerator rng, int size)
    {
        _innerSize = size;
        _totalSize = size + Expansion * 2;
        _areas = new Area[_totalSize, _totalSize];
        
        InitializeDefaultAreas(rng);
        
        var streetRows = new HashSet<int>();
        var streetCols = new HashSet<int>();
        GenerateStreets(rng, streetRows, streetCols);
        
        PlaceHousesAtCrossings(rng, streetRows, streetCols);
        
        DistributePonds(rng);
    }
    
    public Area GetArea(int x, int y) => _areas[x, y];

    private void InitializeDefaultAreas(RandomNumberGenerator rng)
    {
        for (var x = 0; x < _totalSize; x++)
        {
            for (var y = 0; y < _totalSize; y++)
            {
                _areas[x, y] = Area.Ocean;
            }
        }
        
        for (var x = Expansion; x < _innerSize + Expansion; x++)
        {
            for (var y = Expansion; y < _innerSize + Expansion; y++)
            {
                var roll = rng.RandiRange(0, 100);
                _areas[x, y] = roll switch
                {
                    < 55 => Area.Forest,
                    < 85 => Area.Wasteland,
                    _ => Area.Forest
                };
            }
        }
    }

    private void GenerateStreets(RandomNumberGenerator rng, HashSet<int> streetRows, HashSet<int> streetColumns)
    {
        var maxLines = Math.Max(1, _innerSize / 6);
        var horizontalCount = rng.RandiRange(3, Math.Max(1, maxLines));
        var verticalCount = rng.RandiRange(3, Math.Max(1, maxLines));
        
        while (streetRows.Count < horizontalCount)
        {
            var row = rng.RandiRange(Expansion, _innerSize + Expansion - 1);
            if (streetRows.Contains(row + 1) || streetRows.Contains(row - 1))
                continue;
            streetRows.Add(row);
        }
        
        while (streetColumns.Count < verticalCount)
        {
            var column = rng.RandiRange(Expansion, _innerSize + Expansion - 1);
            if (streetColumns.Contains(column + 1) || streetColumns.Contains(column - 1))
                continue;
            streetColumns.Add(column);
        }
        
        foreach (var row in streetRows)
        {
            for (var column = Expansion; column < _innerSize + Expansion; column++)
                _areas[row, column] = Area.Street;
        }

        foreach (var column in streetColumns)
        {
            for (var row = Expansion; row < _innerSize + Expansion; row++)
                _areas[row, column] = Area.Street;
        }
    }

    private void PlaceHousesAtCrossings(RandomNumberGenerator rng, HashSet<int> streetRows, HashSet<int> streetColumns)
    {
        const int houseChancePercent = 70;
        foreach (var row in streetRows)
        {
            foreach (var column in streetColumns)
            {
                foreach (var (nx, ny) in GetDiagonalNeighbors(row, column, _innerSize))
                {
                    if (_areas[nx, ny] != Area.Street && _areas[nx, ny] != Area.Ocean && rng.RandiRange(0, 100) < houseChancePercent)
                    {
                        _areas[nx, ny] = Area.House;
                    }
                }
            }
        }
        
        const int roadsideHouseChance = 10;
        for (var x = Expansion; x < _innerSize + Expansion; x++)
        {
            for (var y = Expansion; y < _innerSize + Expansion; y++)
            {
                if (_areas[x, y] != Area.Street) continue;
                
                foreach (var (nx, ny) in GetOrthogonalNeighbors(x, y, _innerSize))
                {
                    if (_areas[nx, ny] == Area.Street || _areas[nx, ny] == Area.House || _areas[nx, ny] == Area.Ocean)
                        continue;
                    
                    if (rng.RandiRange(0, 100) < roadsideHouseChance)
                        _areas[nx, ny] = Area.House;
                }
            }
        }
    }

    private void DistributePonds(RandomNumberGenerator rng)
    {
        const int pondChancePercent = 4;
        for (var x = Expansion; x < _innerSize + Expansion; x++)
        {
            for (var y = Expansion; y < _innerSize + Expansion; y++)
            {
                if (_areas[x, y] == Area.Street || _areas[x, y] == Area.House)
                    continue;
                
                var nextToOcean = x == Expansion || x == _innerSize + Expansion - 1 ||
                                  y == Expansion || y == _innerSize + Expansion - 1;
                if (nextToOcean)
                    continue;

                if (rng.RandiRange(0, 100) < pondChancePercent)
                    _areas[x, y] = Area.Pond;
            }
        }
    }

    private static IEnumerable<(int, int)> GetOrthogonalNeighbors(int x, int y, int size)
    {
        if (x > 0) yield return (x - 1, y);
        if (x < size - 1) yield return (x + 1, y);
        if (y > 0) yield return (x, y - 1);
        if (y < size - 1) yield return (x, y + 1);
    }
    
    private static IEnumerable<(int, int)> GetDiagonalNeighbors(int x, int y, int size)
    {
        if (x > 0 && y > 0) yield return (x - 1, y - 1);
        if (x > 0 && y < size - 1) yield return (x - 1, y + 1);
        if (x < size - 1 && y > 0) yield return (x + 1, y - 1);
        if (x < size - 1 && y < size - 1) yield return (x + 1, y + 1);
    }

    public void Place(Action<Vector2I, Area> placeArea)
    {
        for (var x = 0; x < _areas.GetLength(0); x++)
        {
            for (var y = 0; y < _areas.GetLength(1); y++)
            {
                placeArea(new Vector2I(x, y), _areas[x, y]);
            }
        }
    }
}
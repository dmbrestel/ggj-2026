using Godot;

namespace GGJ2026.scripts.terrain;

public enum Area
{
    Street,
    Grassland,
    Wasteland,
    Pond,
    Forest,
    House,
    Ocean
}

public static class Areas
{
    public static readonly Vector2I Size = new(9, 9);
}
using Godot;
using System;

public partial class Egg : Area2D
{
    public override void _Ready()
    {
        BodyEntered += OnBodyEntered;
    }

    private void OnBodyEntered(Node2D body)
    {
        if (body is GGJ2026.scripts.Player player)
        {
            player.AddEgg();
            QueueFree();
        }
    }
}

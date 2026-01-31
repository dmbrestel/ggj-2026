using Godot;
using System;

public partial class Ammunition : Area2D
{
    [Export] public int AmmoAmount = 12;

    public override void _Ready()
    {
        BodyEntered += OnBodyEntered;
    }

    private void OnBodyEntered(Node2D body)
    {
        if (body is Player player)
        {
            player.AddAmmo(AmmoAmount);
            QueueFree();
        }
    }
}

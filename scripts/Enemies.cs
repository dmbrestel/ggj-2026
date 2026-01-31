using System;
using Godot;

namespace GGJ2026.scripts;

public partial class Enemies : Node
{
	[Export]
	public PackedScene Military { get; set; }
	
	[Export]
	public PackedScene Cultist { get; set; }
	
	[Export]
	public PackedScene Hoarders { get; set; }
	
	[Export]
	public PackedScene Robot { get; set; }

	public override void _Ready()
	{
		Instance = this;
	}
	
	public static Enemies Instance { get; private set; }

	public static PackedScene GetEnemy(Faction faction) 
	{
		return faction switch
		{
			Faction.Military => Instance.Military,
			Faction.Cultists => Instance.Cultist,
			Faction.Hoarder => Instance.Hoarders,
			Faction.Robots => Instance.Robot,
			_ => throw new ArgumentOutOfRangeException(nameof(faction), faction, null)
		};
	}

	public static Faction GetFaction(RandomNumberGenerator rng)
	{
		Faction faction;
		switch (rng.RandiRange(0, 3))
		{
			case 0:
				faction = Faction.Military;
				break;
			case 1:
				faction = Faction.Cultists;
				break;
			case 2:
				faction = Faction.Hoarder;
				break;
			case 3:
				faction = Faction.Robots;
				break;
			default:
				faction = Faction.Military;
				break;
		}
		return faction;
	}
}
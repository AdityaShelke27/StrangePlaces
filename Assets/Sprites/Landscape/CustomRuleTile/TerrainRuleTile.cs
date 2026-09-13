using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(
	fileName = "TerrainRuleTile",
	menuName = "Tiles/Terrain Rule Tile"
)]
public class TerrainRuleTile : RuleTile<TerrainRuleTile.Neighbor>
{
	public enum TerrainType
	{
		Grass,
		Sand,
		Rock,
		Water
	}

	[Header("Terrain")]
	public TerrainType terrainType;

	public class Neighbor : RuleTile.TilingRule.Neighbor
	{
		// 0 = Don't Care
		// 1 = This
		// 2 = Not This

		public const int Grass = 3;
		public const int Sand = 4;
		public const int Rock = 5;
		public const int Water = 6;
	}

	public override bool RuleMatch(int neighbor, TileBase tile)
	{
		// Handle RuleOverrideTile
		if (tile is RuleOverrideTile overrideTile)
			tile = overrideTile.m_InstanceTile;

		// Custom terrain matching
		if (tile is TerrainRuleTile terrainTile)
		{
			switch (neighbor)
			{
				case Neighbor.Grass:
					return terrainTile.terrainType == TerrainType.Grass;

				case Neighbor.Sand:
					return terrainTile.terrainType == TerrainType.Sand;

				case Neighbor.Rock:
					return terrainTile.terrainType == TerrainType.Rock;

				case Neighbor.Water:
					return terrainTile.terrainType == TerrainType.Water;
			}
		}
		else
		{
			// If the rule specifically expects a terrain,
			// a non-terrain tile does not match.
			switch (neighbor)
			{
				case Neighbor.Grass:
				case Neighbor.Sand:
				case Neighbor.Rock:
				case Neighbor.Water:
					return false;
			}
		}

		// Let normal RuleTile behavior handle:
		// This = 1
		// Not This = 2
		return base.RuleMatch(neighbor, tile);
	}
}
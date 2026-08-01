using UnityEngine;
using UnityEngine.Tilemaps;

public class WorldGenerator : MonoBehaviour
{
	[Header("References")]
	[SerializeField] private Tilemap m_GroundTilemap;
	[SerializeField] private TileBase m_GroundTile;
	[SerializeField] private TileBase m_GrassTile;
	[SerializeField] private TileBase m_SandTile;
	[SerializeField] private TileBase m_WaterTile;

	[Header("World Size")]
	[SerializeField] private int m_Width = 512;
	[SerializeField] private int m_Height = 512;

	[Header("Noise")]
	[SerializeField] private float m_NoiseScale = 0.05f;
	[SerializeField] private int m_Seed;
	private void Start()
	{
		GenerateWorld();
	}

	private void GenerateWorld()
	{
		m_GroundTilemap.ClearAllTiles();

		TileBase[] _tiles = new TileBase[m_Width * m_Height];
		int _index = 0;

		float _offsetX = RandomOffset(m_Seed);
		float _offsetY = RandomOffset(m_Seed + 1);

		int _endX = m_Width / 2;
		int _endY = m_Height / 2;
		for (int x = -_endX; x < _endX; x++)
		{
			for (int y = -_endY; y < _endY; y++)
			{
				float _noise = Mathf.PerlinNoise(x * m_NoiseScale + _offsetX, y * m_NoiseScale + _offsetY);
				if(_noise < 0.3f)
				{
					_tiles[_index] = m_WaterTile;
				}
				else if(_noise < 0.5f)
				{
					_tiles[_index] = m_SandTile;
				}
				else if(_noise < 0.75f)
				{
					_tiles[_index] = m_GrassTile;
				}
				else
				{
					_tiles[_index] = m_GroundTile;
				}

				_index++;
			}
		}

		BoundsInt _bounds = new BoundsInt(new Vector3Int(-_endX, -_endY, 0), new Vector3Int(m_Width, m_Height, 1));
		m_GroundTilemap.SetTilesBlock(_bounds, _tiles);
	}

	private float RandomOffset(int _seed)
	{
		System.Random r = new System.Random(_seed);
		return r.Next(-100000, 100000);
	}
}

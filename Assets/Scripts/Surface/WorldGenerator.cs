using System.Collections.Generic;
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

	[Header("Terrain Thresholds")]
	[SerializeField] private float m_WaterThreshold = 0.2f;
	[SerializeField] private float m_SandThreshold = 0.3f;
	[SerializeField] private float m_GrassThreshold = 0.75f;
	[SerializeField] private float m_RockThreshold = 1f;

	[Header("Noise")]
	[SerializeField] private int m_Octaves = 4;
	[SerializeField] private float m_Persistance = 0.5f;
	[SerializeField] private float m_Lacunarity = 2f;
	[SerializeField] private float m_NoiseScale = 0.05f;
	[SerializeField] private int m_Seed;
	float[,] m_Kernel =
	{
		{ 0.11f, 0.11f, 0.11f },
		{ 0.11f, 0.11f, 0.11f },
		{ 0.11f, 0.11f, 0.11f },
	};

	float[,] m_RawTerrain;
	E_TerrainTypes[,] m_Terrain;
	Dictionary<E_TerrainTypes, TileBase> m_TerrainTileMapping;

	private float m_OffsetX;
	private float m_OffsetY;
	private void Start()
	{
		m_TerrainTileMapping = new() {
			{ E_TerrainTypes.Water, m_WaterTile},
			{ E_TerrainTypes.Sand, m_SandTile},
			{ E_TerrainTypes.Grass, m_GrassTile},
			{ E_TerrainTypes.Rock, m_GroundTile},
		};
		GenerateWorld();
	}

	private void GenerateWorld()
	{
		m_GroundTilemap.ClearAllTiles();

		TileBase[] _tiles = new TileBase[m_Width * m_Height];
		m_Terrain = new E_TerrainTypes[m_Width, m_Height];
		m_RawTerrain = new float[m_Width, m_Height];

		m_OffsetX = RandomOffset(m_Seed);
		m_OffsetY = RandomOffset(m_Seed + 1);

		// WORLD GENERATION LAYERS ----------------------------------------------------------------------

		GenerateRawTerrain();
		m_RawTerrain = KerneledImage(m_RawTerrain, m_Kernel, m_Width, m_Kernel.GetLength(0));
		AssignTerrain();
		_tiles = AssignTiles(_tiles);

		// ----------------------------------------------------------------------------------------------

		int _endX = m_Width / 2;
		int _endY = m_Height / 2;

		BoundsInt _bounds = new(new Vector3Int(-_endX, -_endY, 0), new Vector3Int(m_Width, m_Height, 1));
		m_GroundTilemap.SetTilesBlock(_bounds, _tiles);
	}
	private void GenerateRawTerrain()
	{
		for (int x = 0; x < m_Width; x++)
		{
			for (int y = 0; y < m_Height; y++)
			{
				m_RawTerrain[x, y] = GenerateNoise(x, y);

			}
		}
	}
	private void AssignTerrain()
	{
		for (int x = 0; x < m_Width; x++)
		{
			for (int y = 0; y < m_Height; y++)
			{
				if (m_RawTerrain[x, y] < m_WaterThreshold) m_Terrain[x, y] = E_TerrainTypes.Water;
				else if (m_RawTerrain[x, y] < m_SandThreshold) m_Terrain[x, y] = E_TerrainTypes.Sand;
				else if (m_RawTerrain[x, y] < m_GrassThreshold) m_Terrain[x, y] = E_TerrainTypes.Grass;
				else m_Terrain[x, y] = E_TerrainTypes.Rock;
			}
		}
	}
	private TileBase[] AssignTiles(TileBase[] _tiles)
	{
		int _index = 0;
		for (int i = 0; i < m_Width; i++)
		{
			for (int j = 0; j < m_Height; j++)
			{
				_tiles[_index] = m_TerrainTileMapping[m_Terrain[i, j]];
				_index++;
			}
		}

		return _tiles;
	}
	private float GenerateNoise(int _x, int _y)
	{
		float _amplitude = 1f;
		float _frequency = 1f;
		float _noiseHeight = 0f;

		float _maxPossibleHeight = 0f;

		for(int i = 0; i < m_Octaves; i++)
		{
			float _sampleX = (_x * m_NoiseScale * _frequency) + m_OffsetX;
			float _sampleY = (_y * m_NoiseScale * _frequency) + m_OffsetY;

			float _perlin = Mathf.PerlinNoise(_sampleX, _sampleY);

			_noiseHeight += _perlin * _amplitude;

			_maxPossibleHeight += _amplitude;

			_amplitude *= m_Persistance;
			_frequency *= m_Lacunarity;
		}

		return _noiseHeight / _maxPossibleHeight;
	}
	public float[,] KerneledImage(float[,] image2d, float[,] kernel2d, int imageLength, int kernelLength)
	{
		//int imageLength = (int)Mathf.Sqrt(image.Length);
		//int kernelLength = (int)Mathf.Sqrt(kernel.Length);

		//float[][] image2d = new float[imageLength][];
		//float[][] kernel2d = new float[kernelLength][];

		//int count = 0;
		//for (int i = 0; i < imageLength; i++)
		//{
		//	image2d[i] = new float[imageLength];
		//	for (int j = 0; j < imageLength; j++)
		//	{
		//		image2d[i][j] = image[count];
		//		count++;
		//	}
		//}
		//count = 0;
		//for (int i = 0; i < kernelLength; i++)
		//{
		//	kernel2d[i] = new float[kernelLength];
		//	for (int j = 0; j < kernelLength; j++)
		//	{
		//		kernel2d[i][j] = kernel[count];
		//		count++;
		//	}
		//}

		int clampVal = imageLength - kernelLength + 1;
		float[,] newImage = new float[clampVal, clampVal];
		for (int i = 0; i < clampVal; i++)
		{
			for (int j = 0; j < clampVal; j++)
			{
				float sum = 0;
				for (int k = 0; k < kernelLength; k++)
				{
					for (int l = 0; l < kernelLength; l++)
					{
						sum += image2d[i + k, j + l] * kernel2d[k, l];
					}
				}
				newImage[i, j] = sum;
			}
		}

		return newImage;
	}
	private float RandomOffset(int _seed)
	{
		System.Random r = new System.Random(_seed);
		return r.Next(-100000, 100000);
	}
}

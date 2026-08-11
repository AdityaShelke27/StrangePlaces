using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

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
	//float[,] m_Kernel =
	//{
	//	{ 0.11f, 0.11f, 0.11f },
	//	{ 0.11f, 0.11f, 0.11f },
	//	{ 0.11f, 0.11f, 0.11f },
	//};
	[SerializeField] private GameObject m_ResourceNodePrefab;
	[Header("Resources Spawn Rate")]
	[SerializeField] private float m_VyrexReedSpawnRate = 0.02f;
	[SerializeField] private float m_KarthBambooSpawnRate = 0.02f;
	[SerializeField] private float m_LumabloomSpawnRate = 0.02f;

	[SerializeField] private float m_IronOreSpawnRate = 0.5f;
	[SerializeField] private float m_CopperOreSpawnRate = 0.3f;
	[SerializeField] private float m_TitaniumOreSpawnRate = 0.1f;

	[Header("Bunker Area")]
	[SerializeField] private float m_BunkerGrassRadius = 30f;
	[SerializeField] private float m_BunkerTransitionRadius = 30f;
	[SerializeField] private float m_BunkerGrassTarget = 0.5f;

	float m_TotalOreSpawnChance;
	float m_IronSpawnChance;
	float m_CopperSpawnChance;
	float m_TitaniumSpawnChance;
	float m_FinalIronSpawnChance;
	float m_FinalCopperSpawnChance;

	private Dictionary<Vector2, GameObject> m_ResourcesSpawns = new();
	float[,] m_Kernel =
	{
		{ 0.0625f, 0.125f, 0.0625f },
		{ 0.125f, 0.25f, 0.125f },
		{ 0.0625f, 0.125f, 0.0625f },
	};

	float[,] m_RawTerrain;
	E_TerrainTypes[,] m_Terrain;
	Dictionary<E_TerrainTypes, TileBase> m_TerrainTileMapping;

	private float m_OffsetX;
	private float m_OffsetY;

	private int m_HalfWidth;
	private int m_HalfHeight;
	private void Start()
	{
		m_TerrainTileMapping = new() {
			{ E_TerrainTypes.Water, m_WaterTile},
			{ E_TerrainTypes.Sand, m_SandTile},
			{ E_TerrainTypes.Grass, m_GrassTile},
			{ E_TerrainTypes.Rock, m_GroundTile},
		};

		m_HalfWidth = m_Width / 2;
		m_HalfHeight = m_Height / 2;

		m_TotalOreSpawnChance = m_IronOreSpawnRate + m_CopperOreSpawnRate + m_TitaniumOreSpawnRate;
		m_IronSpawnChance = m_IronOreSpawnRate / m_TotalOreSpawnChance;
		m_CopperSpawnChance = m_CopperOreSpawnRate / m_TotalOreSpawnChance;
		m_TitaniumSpawnChance = m_TitaniumOreSpawnRate / m_TotalOreSpawnChance;

		m_FinalCopperSpawnChance = m_TitaniumSpawnChance + m_CopperSpawnChance;
		m_FinalIronSpawnChance = m_TitaniumSpawnChance + m_CopperSpawnChance + m_IronSpawnChance;

		GenerateWorld();
	}

	private void GenerateWorld()
	{
		DateTime _start = DateTime.Now;
		m_GroundTilemap.ClearAllTiles();

		TileBase[] _tiles = new TileBase[m_Width * m_Height];
		m_Terrain = new E_TerrainTypes[m_Width, m_Height];
		m_RawTerrain = new float[m_Width, m_Height];

		m_OffsetX = RandomOffset(m_Seed);
		m_OffsetY = RandomOffset(m_Seed + 1);

		// WORLD GENERATION LAYERS ----------------------------------------------------------------------

		GenerateRawTerrain();
		//m_RawTerrain = KerneledImage(m_RawTerrain, m_Kernel, m_Width, m_Kernel.GetLength(0));
		AssignTerrain();
		_tiles = AssignTiles(_tiles);

		// ----------------------------------------------------------------------------------------------

		BoundsInt _bounds = new(new Vector3Int(-m_HalfWidth, -m_HalfHeight, 0), new Vector3Int(m_Width, m_Height, 1));
		m_GroundTilemap.SetTilesBlock(_bounds, _tiles);

		SpawnResources();

		StartCoroutine(BuildNavMesh());

		DateTime _end = DateTime.Now;

		TimeSpan diff = _end - _start;
		Debug.Log($"Time: {diff.TotalMilliseconds}");
		Debug.Log($"Resources: {m_ResourcesSpawns.Count}");
	}
	IEnumerator BuildNavMesh()
	{
		yield return null;

		NavMeshManager.s_BuildNavmesh?.Invoke();
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
	private bool IsPosEmpty(Vector2 pos)
	{
		return !m_ResourcesSpawns.ContainsKey(pos);
	}
	private void CreateNode(ResourceNode _nodeData, Vector2 _pos)
	{
		GameObject _nodeIns = Instantiate(m_ResourceNodePrefab, new Vector3(-m_HalfWidth + _pos.y + 0.5f, -m_HalfHeight + _pos.x + 0.5f, 0), Quaternion.identity);
		m_ResourcesSpawns.Add(_pos, _nodeIns);
		_nodeIns.GetComponent<ResourceNodeInstance>().SetResourceNodeData(_nodeData);
	}
	private bool IsWaterEdge(int x, int y)
	{
		for (int dx = -1; dx <= 1; dx++)
		{
			for (int dy = -1; dy <= 1; dy++)
			{
				if (dx == 0 && dy == 0) continue;

				E_TerrainTypes _type = m_Terrain[x + dx, y + dy];
				if (_type == E_TerrainTypes.Sand || _type == E_TerrainTypes.Grass) return true;
			}
		}

		return false;
	}
	private void SpawnOre(ResourceNode iron, ResourceNode copper, ResourceNode titanium, Vector2 pos)
	{
		float roll = Random.value;

		if (roll <= m_TitaniumSpawnChance)
		{
			CreateNode(titanium, pos);
		}
		else if (roll <= m_FinalCopperSpawnChance)
		{
			CreateNode(copper, pos);
		}
		else if (roll <= m_FinalIronSpawnChance)
		{
			CreateNode(iron, pos);
		}
	}
	private void SpawnResources()
	{
		ResourceNode vyrex = ItemDatabase.Instance.GetItemByID("vyrex-reed-node") as ResourceNode;
		ResourceNode bamboo = ItemDatabase.Instance.GetItemByID("karth-bamboo-node") as ResourceNode;
		ResourceNode lumabloom = ItemDatabase.Instance.GetItemByID("lumabloom-node") as ResourceNode;
		ResourceNode iron = ItemDatabase.Instance.GetItemByID("iron-ore-node") as ResourceNode;
		ResourceNode copper = ItemDatabase.Instance.GetItemByID("copper-ore-node") as ResourceNode;
		ResourceNode titanium = ItemDatabase.Instance.GetItemByID("titanium-ore-node") as ResourceNode;

		for (int x = 1; x < m_Width - 1; x++)
		{
			for (int y = 1; y < m_Height - 1; y++)
			{
				Vector2 pos = new(x, y);

				if (!IsPosEmpty(pos)) continue;

				switch (m_Terrain[x, y])
				{
					case E_TerrainTypes.Sand:
						if (Random.value <= m_KarthBambooSpawnRate) CreateNode(bamboo, pos);
						break;

					case E_TerrainTypes.Grass:
						if (Random.value <= m_LumabloomSpawnRate) CreateNode(lumabloom, pos);
						break;

					case E_TerrainTypes.Rock:
						SpawnOre(iron, copper, titanium, pos);

						break;

					case E_TerrainTypes.Water:
						if (Random.value <= m_VyrexReedSpawnRate && IsWaterEdge(x, y))
						{
							CreateNode(vyrex, pos);
						}
						break;
				}
			}
		}
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

		float _noise = _noiseHeight / _maxPossibleHeight;

		// Convert array coordinates to world coordinates.
		float _worldX = _x - m_HalfWidth;
		float _worldY = _y - m_HalfHeight;

		float _distanceSqr = _worldX * _worldX + _worldY * _worldY;

		float _transitionStartSqr = m_BunkerGrassRadius;
		float _transitionEnd = m_BunkerGrassRadius + m_BunkerTransitionRadius;
		float _transitionEndSqr = _transitionEnd * _transitionEnd;

		float _grassInfluence = 1f - Mathf.InverseLerp(_transitionStartSqr, _transitionEndSqr, _distanceSqr);

		// Smooth the transition.
		_grassInfluence = Mathf.SmoothStep(0f, 1f, _grassInfluence);

		return Mathf.Lerp(_noise, m_BunkerGrassTarget, _grassInfluence);
	}
	public float[,] KerneledImage(float[,] image2d, float[,] kernel2d, int imageLength, int kernelLength)
	{
		int clampVal = imageLength - kernelLength + 1;
		float[,] newImage = new float[imageLength, imageLength];
		for (int i = 0; i < imageLength; i++)
		{
			for (int j = 0; j < imageLength; j++)
			{
				if(i < clampVal && j < clampVal)
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
				else
				{
					newImage[i, j] = image2d[i, j];
				}
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

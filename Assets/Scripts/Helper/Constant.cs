using System.Collections.Generic;
using UnityEngine;

public static class Constant
{
	// ROOM IDs
	public const int ID_MECHANIC_ROOM = 0;
	public const int ID_CONSTRUCTION_ROOM = 1;
	public const int ID_STORAGE_ROOM = 2;
	public const int ID_RESEARCH_ROOM = 3;
	public const int ID_KITCHEN_ROOM = 4;
	public const int ID_ROCKETCONSTRUCTION_ROOM = 5;

	// ROOM AND STAIR SIZE
	public static readonly Vector2 SIZE_ROOM = new(7.68f, 3.84f);
	public static readonly Vector2 SIZE_STAIR = new(1.28f, 3.84f);

	// PLAYER_PREFS
	public const string PREF_ROOMSUNLOCKED = "RoomsUnlocked";
	public const string PREF_RESEARCHEDNODES = "ResearchedNodes";
	public const string PREF_SAVE_PLAYERDATA = "Save_PlayerData";

	// SCENES
	public const string SCENE_BUNKER = "BunkerScene";
	public const string SCENE_SURFACE = "SurfaceScene";

	// TAGS
	public const string TAG_PLAYER = "Player";
	public const string TAG_RESOURCE_NODE = "ResourceNode";

	public static readonly Dictionary<int, string> m_ResearchID_ToItemID = new()
	{
		{ 1, "karth-bamboo-node"},
		{ 2, "reinforced-panels"},
		{ 3, "bio-resin"},
		{ 7, "electricity"},
		{ 9, "1"}, // Construction Room
		{ 14, "5"}, // Rocket Construction Room
		{ 15, "titanium-ore"},
		{ 16, "titanium-alloy"},
		{ 10, "miner"},
		{ 17, "smelter"},
		{ 11, "harvester"},
		{ 18, "organic-refinery"},
		{ 12, "bio-reactor"},
		{ 20, "gravity-reactor"},
		{ 21, ""}, // Automation
		{ 22, ""}, // Conveyor
		{ 24, ""}, // Conveyor Speed 1
		{ 26, ""}, // Conveyor Bridge
		{ 27, ""}, // Conveyor Speed 2
		{ 23, ""}, // Bunker Input Node 1
		{ 25, ""}, // Bunker Input Node 2
		{ 13, "artifact-scanner"},
		{ 19, "research-station"},
		{ 8, "2"}, // Storage Room
		{ 28, "nano-alloy"},
		{ 30, "energy-core"},
		{ 31, "quantum-circuit"},
		{ 32, "quantum-processor"},
		{ 34, "navigation-module"},
		{ 33, "engine"},
		{ 29, "rocket-frame"},
		{ 35, "fuel-tank"},
		{ 36, "cockpit"},
	};
}

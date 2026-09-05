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

	public static readonly Dictionary<int, List<string>> m_ResearchID_ToItemID = new()
	{
		{ 0, new () {"iron-ore-node", "iron-ore", "copper-ore", "copper-ore-node", "vyrex-reed-node", "vyrex-reed", "lumabloom-node", "lumabloom", "fiber-mesh", "copper-wires", "nutrient-paste", "iron-plates", "pickaxe", "axe", "1", "4", "5", "nutrient-bar" } },
		{ 1, new () {"karth-bamboo-node", "karth-bamboo" } },								// Done
		{ 2, new () {"reinforced-panels" } },												// Done
		{ 3, new () {"bio-resin" } },														// Done
		{ 7, new () {"electricity" } },
		{ 9, new () {"1"} }, // Construction Room
		{ 14, new () {"5" } }, // Rocket Construction Room
		{ 15, new () { "titanium-ore-node", "titanium-ore" } },								// Done
		{ 16, new () {"titanium-alloy" } },
		{ 10, new () {"miner"} },															// Done
		{ 17, new () {"smelter"} },															// Done
		{ 11, new () {"harvester"} },														// Done
		{ 18, new () {"organic-refinery"} },												// Done
		{ 12, new () {"bio-reactor"} },														// Done
		{ 20, new () {"gravity-reactor"} },													// Done
		{ 21, new () {""} }, // Automation
		{ 22, new () {""} }, // Conveyor
		{ 24, new () {""} }, // Conveyor Speed 1
		{ 26, new () {""} }, // Conveyor Bridge
		{ 27, new () {""} }, // Conveyor Speed 2
		{ 23, new () {""} }, // Bunker Input Node 1
		{ 25, new () {""} }, // Bunker Input Node 2
		{ 13, new () {"artifact-scanner", "alien-artifact" } },								// Done
		{ 19, new () {"research-station"} },												// Done
		{ 8, new() { "2" }}, // Storage Room
		{ 28, new () {"nano-alloy"} },
		{ 30, new () {"energy-core"} },
		{ 31, new () {"quantum-circuit"} },
		{ 32, new () {"quantum-processor"} },
		{ 34, new () {"navigation-module"} },
		{ 33, new () {"engine"} },
		{ 29, new () {"rocket-frame"} },
		{ 35, new () {"fuel-tank"} },
		{ 36, new () {"cockpit"} },
	};
}

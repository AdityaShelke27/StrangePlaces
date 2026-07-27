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
	public const string PREF_SAVE_PLAYERDATA = "Save_PlayerData";

	// SCENES
	public const string SCENE_BUNKER = "BunkerScene";
	public const string SCENE_SURFACE = "SurfaceScene";

	// TAGS
	public const string TAG_PLAYER = "Player";
	public const string TAG_RESOURCE_NODE = "ResourceNode";
}

using UnityEngine;
using System.Collections.Generic;

// RoomData is a plain data class - no MonoBehaviour needed
// It stores all the information about a single room
public class RoomData
{
    // Room dimensions in tiles
    public int width;
    public int height;

    // Bottom left corner position of this room in world space
    public Vector2Int position;

    // What type of room this is
    public RoomType roomType;

    // What biome this room belongs to
    public BiomeType biomeType;

    // All exits in this room
    public List<ExitData> exits = new List<ExitData>();

    // Has the player visited this room
    public bool visited;

    // Has the room been fully cleared of enemies
    public bool cleared;
}

// All possible room types
public enum RoomType
{
    Combat,
    Treasure,
    Puzzle,
    Boss,
    Secret,
    Shortcut,
    Start
}

// All biome types
public enum BiomeType
{
    Egypt,
    Occult,
    Medieval,
    Wild,
    Frost,
    Storm,
    Sunfire,
    Moonlit,
    AncientTech,
    SpiritRealm,
    Jungle,
    Cavern,
    Abyssal,
    Emberforge,
    SkyRuins
}
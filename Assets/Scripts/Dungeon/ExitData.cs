using UnityEngine;
using System.Collections.Generic;

// ExitData stores everything about a single exit in a room
public class ExitData
{
    // Where on the tilemap this exit is positioned
    public Vector2Int position;

    // Which direction this exit faces
    public ExitDirection direction;

    // The weighted pool of room types this exit can lead to
    public List<ExitPoolEntry> pool = new List<ExitPoolEntry>();

    // The room this exit leads to once generated
    public RoomData connectedRoom;

    // Has the player gone through this exit yet
    public bool opened;
}

// Which direction an exit faces
public enum ExitDirection
{
    Left,
    Right,
    Up,
    Down
}

// A single entry in an exit's weighted pool
// For example: Combat with weight 60, Treasure with weight 25
public class ExitPoolEntry
{
    public RoomType roomType;
    public int weight;

    public ExitPoolEntry(RoomType type, int weight)
    {
        this.roomType = type;
        this.weight = weight;
    }
}
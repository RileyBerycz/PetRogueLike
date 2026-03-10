using UnityEngine;

// BiomeData is a ScriptableObject - this means each biome is its own
// asset in the project that you can edit in the Inspector
// To create a new biome: right click in Project panel
// Create → Dungeon → BiomeData
[CreateAssetMenu(fileName = "NewBiome", menuName = "Dungeon/BiomeData")]
public class BiomeData : ScriptableObject
{
    [Header("Identity")]
    public BiomeType biomeType;
    public string biomeName;

    [Header("Prototype Colours")]
    // These colours are just for prototyping
    // They will be replaced by tilesets later
    public Color backgroundColour = Color.grey;
    public Color wallColour = Color.white;
    public Color floorColour = Color.white;
    public Color exitColour = Color.cyan;

    [Header("Room Dimensions")]
    // How wide rooms can be in this biome in tiles
    public int minRoomWidth = 20;
    public int maxRoomWidth = 40;

    // How tall rooms can be in this biome in tiles
    public int minRoomHeight = 15;
    public int maxRoomHeight = 30;

    [Header("Generation Style")]
    // Controls how organic the room shape is
    // 0 = clean rectangular rooms
    // 1 = fully organic noise-carved rooms
    [Range(0f, 1f)]
    public float organicness = 0.5f;

    // How much vertical variation the ceiling and floor have
    [Range(0f, 1f)]
    public float verticalVariation = 0.5f;

    // How many interior platforms to generate
    public int minPlatforms = 2;
    public int maxPlatforms = 6;

    [Header("Exit Pool Weightings")]
    // These control the odds for wall exits in this biome
    [Range(0, 100)] public int combatWeight = 40;
    [Range(0, 100)] public int treasureWeight = 20;
    [Range(0, 100)] public int puzzleWeight = 15;
    [Range(0, 100)] public int secretWeight = 10;
    [Range(0, 100)] public int bossWeight = 10;
    [Range(0, 100)] public int shortcutWeight = 5;

    [Header("Boss Chance")]
    // Base chance of a boss room appearing
    public float baseBossChance = 0.05f;
    // How much boss chance increases per room cleared
    public float bossChanceIncrement = 0.03f;
}
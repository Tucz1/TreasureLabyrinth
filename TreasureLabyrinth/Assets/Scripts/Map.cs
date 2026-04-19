using System;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public enum TileType
{
    None,
    Wall,
    Floor,
    PlayerSpawn,
    EnemySpawn,
    ArtifactSpawn,
    Exit,

}

public class Node
{
    public TileType tileType;


    public Node(TileType tileType)
    {
        this.tileType = tileType;
    }
}

[System.Serializable]
public class MapDataColors
{
    public string colorRole;
    public Color32 color;
}
[Serializable]
public class MapTextureSet 
{
    public Texture2D levelTexture;
    public Texture2D mapTexture;
}

public class Map : MonoBehaviour
{

    public Dictionary<Vector2Int, Node> data;

    public MapDataColors[] mapColors;
    [SerializeField] int colorThreshold = 20;

    public Dictionary<string, Color32> mapDataColors;

    private int mapSizeX;
    private int mapSizeY;

    public GameObject wallTile;
    public GameObject floorTile;
    public GameObject playerSpawnTile;
    public GameObject enemySpawnTile;
    public GameObject artifactSpawnTile;
    public GameObject exitTileSpawn;
    public Transform levelVisuals;

    public Texture2D testingTex;

    public static event Action<Texture2D> OnMapChanged;

    public List<MapTextureSet> mapTextures;

    void Awake()
    {
        mapDataColors = new Dictionary<string, Color32>();

        for (int i = 0; i < mapColors.Length; i++)
        {
            mapDataColors.Add(mapColors[i].colorRole, mapColors[i].color);
        }

        LoadFromTexture(mapTextures[0].levelTexture);
        OnMapChanged?.Invoke(mapTextures[0].levelTexture);
    }

    void Start()
    {
        RefreshVisuals();
    }

    public void RefreshVisuals()
    {
        // Kill previous visuals

        foreach (var dictonary in data)
        {

            if (dictonary.Value.tileType == TileType.Wall)
            {
                Instantiate(wallTile, new Vector3(dictonary.Key.x, dictonary.Key.y, 0), Quaternion.identity, levelVisuals);
            }

            if (dictonary.Value.tileType == TileType.Floor)
            {
                Instantiate(floorTile, new Vector3(dictonary.Key.x, dictonary.Key.y, 0), Quaternion.identity, levelVisuals);
            }

            if (dictonary.Value.tileType == TileType.PlayerSpawn)
            {
                Instantiate(playerSpawnTile, new Vector3(dictonary.Key.x, dictonary.Key.y, 0), Quaternion.identity, levelVisuals);
            }

            if (dictonary.Value.tileType == TileType.EnemySpawn)
            {
                Instantiate(enemySpawnTile, new Vector3(dictonary.Key.x, dictonary.Key.y, 0), Quaternion.identity, levelVisuals);
            }
            
            if (dictonary.Value.tileType == TileType.ArtifactSpawn)
            {
                Instantiate(artifactSpawnTile, new Vector3(dictonary.Key.x, dictonary.Key.y, 0), Quaternion.identity, levelVisuals);
            }

            if (dictonary.Value.tileType == TileType.Exit)
            {
                Instantiate(exitTileSpawn, new Vector3(dictonary.Key.x, dictonary.Key.y, 0), Quaternion.identity, levelVisuals);
            }
        }
    }

    public void LoadFromTexture(Texture2D tex)
    {

        data = new Dictionary<Vector2Int, Node>();

        mapSizeX = tex.width;
        mapSizeY = tex.height;

        var pixels = tex.GetPixels32();

        for (int i = 0; i < mapSizeX; i++)
        {
            for (int j = 0; j < mapSizeY; j++)
            {
                var pixel = pixels[i + j * mapSizeX];
                TileType tileType = TileType.None;

                bool IsCloseEnough(Color32 c1, Color32 c2, int threshold)
                {
                    return Mathf.Abs(c1.r - c2.r) < threshold &&
                            Mathf.Abs(c1.g - c2.g) < threshold &&
                            Mathf.Abs(c1.b - c2.b) < threshold;
                }
                
                if (IsCloseEnough(pixel, mapDataColors["Wall"], colorThreshold))
                {
                    // dodo
                    tileType = TileType.Wall;
                }
                else if (IsCloseEnough(pixel, mapDataColors["Floor"], colorThreshold))
                {
                    tileType = TileType.Floor;
                }
                else if (IsCloseEnough(pixel, mapDataColors["PlayerSpawn"], colorThreshold))
                {
                    // Spawn player & change tile
                    tileType = TileType.PlayerSpawn;
                }
                else if (IsCloseEnough(pixel, mapDataColors["EnemySpawn"], colorThreshold))
                {
                    tileType = TileType.EnemySpawn;
                }
                else if (IsCloseEnough(pixel, mapDataColors["ArtifactSpawn"], colorThreshold))
                {
                    tileType = TileType.ArtifactSpawn;
                }
                else if (IsCloseEnough(pixel, mapDataColors["Exit"], colorThreshold))
                {
                    tileType = TileType.Exit;
                }




                // else { tileType = pixel.r > 128 ? TileType.Floor : TileType.Wall; }

                if (tileType == TileType.None) Debug.LogError("vevybad");

                data.Add(new Vector2Int(i, j), new Node(tileType));
            }
        }
    }
}

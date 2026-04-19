using System;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public enum TileType
{
    None,
    Wall,
    Floor,

}

public enum ColorRole
{
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

public class Map : MonoBehaviour
{

    public Dictionary<Vector2Int, Node> data;

    public MapDataColors[] mapColors;
    [SerializeField] int colorThreshold = 20;

    public Dictionary<string, Color32> mapDataColors;

    private int mapSizeX;
    private int mapSizeY;

    public GameObject testWall;
    public GameObject testFloor;
    public Transform levelVisuals;

    public Texture2D testingTex;


    void Awake()
    {
        mapDataColors = new Dictionary<string, Color32>();

        for (int i = 0; i < mapColors.Length; i++)
        {
            mapDataColors.Add(mapColors[i].colorRole, mapColors[i].color);
        }
    }

    void Start()
    {

        LoadFromTexture(testingTex);
        RefreshVisuals();
    }

    public void RefreshVisuals()
    {
        // Kill previous visuals????

        foreach (var thing in data)
        {
            // GameObject prefab;

            if (thing.Value.tileType == TileType.Wall)
            {
                Instantiate(testWall, new Vector3(thing.Key.x, thing.Key.y, 0), Quaternion.identity, levelVisuals);
            }

            if (thing.Value.tileType == TileType.Floor)
            {
                Instantiate(testFloor, new Vector3(thing.Key.x, thing.Key.y, 0), Quaternion.identity, levelVisuals);
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
                
                if (IsCloseEnough(pixel, mapDataColors["Floor"], colorThreshold))
                {
                    // dodo
                    tileType = TileType.Floor;
                }
                else if (IsCloseEnough(pixel, mapDataColors["Wall"], colorThreshold))
                {
                    tileType = TileType.Wall;
                }




                // else { tileType = pixel.r > 128 ? TileType.Floor : TileType.Wall; }

                if (tileType == TileType.None) Debug.LogError("vevybad");

                data.Add(new Vector2Int(i, j), new Node(tileType));
            }
        }
    }
}

using System.Collections.Generic;
using UnityEngine;

public enum TileType
{
    None,
    Wall,
    Floor,

}

public class Node
{
    public TileType tileType;

    public Node(TileType tileType)
    {
        this.tileType = tileType;
    }
}

public class Map : MonoBehaviour
{
    public List<Vector2Int> testingWalls;

    public Dictionary<Vector2Int, Node> data;

    public int mapSizeX = 10;
    public int mapSizeY = 10;

    public GameObject testWall;
    public GameObject testFloor;
    public Transform levelVisuals;
    
    public Texture2D testingTex;

    public void LoadDummyMap()
    {
        data = new Dictionary<Vector2Int, Node>();

        for (int i = 0; i < mapSizeX; i++)
        {
            for (int j = 0; j < mapSizeY; j++)
            {
                data.Add(new Vector2Int(i, j), new Node(TileType.Floor));
            }
        }

        foreach (var wall in testingWalls)
        {
            data[wall].tileType = TileType.Wall;
        }
    }

    void Start()
    {
        //LoadDummyMap();
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
                var tileType = pixel.r > 128 ? TileType.Floor : TileType.Wall;

                data.Add(new Vector2Int(i, j), new Node(tileType));
            }
        }
    }
}

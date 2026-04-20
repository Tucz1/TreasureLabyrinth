using UnityEngine;

public class Artifact : MonoBehaviour
{
    GridMovement player;
    Map map;




    void Start()
    {
        map = FindAnyObjectByType<Map>();
        player = FindAnyObjectByType<GridMovement>();

        Vector2Int pos = (Vector2Int)Vector3Int.RoundToInt(transform.position);

        map.data[pos].artifact = this;
    }

    public void Interact()
    {
        Vector2Int pos = (Vector2Int)Vector3Int.RoundToInt(transform.position);

        map.data[pos].tileType = TileType.Floor;
        Destroy(gameObject);
    }
}

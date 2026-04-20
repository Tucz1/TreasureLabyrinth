using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class Artifact : MonoBehaviour
{
    GridMovement player;
    Map map;
    ArtifactList artifactList;

    private SpriteRenderer objSprite;

    [SerializeField] private Discoverable m_discoverable;




    void Start()
    {
        artifactList = FindAnyObjectByType<ArtifactList>();
        map = FindAnyObjectByType<Map>();
        player = FindAnyObjectByType<GridMovement>();
        objSprite = GetComponent<SpriteRenderer>();

        Vector2Int pos = (Vector2Int)Vector3Int.RoundToInt(transform.position);

        map.data[pos].artifact = this;

        // var sprite = gameObject.GetComponent<SpriteRenderer>().sprite;
        

        var i = Random.Range(0, artifactList.artifactSprites.Count);

        

        objSprite.sprite = artifactList.artifactSprites[i];

        artifactList.artifactSprites.RemoveAt(i);



    }

    public void Interact()
    {
        ArtifactData d = new();
        d.image = objSprite.sprite;
        d.name = "Artifact";

        if (m_discoverable != null)
            m_discoverable.SetDiscovered(true);

        UIEvents.ArtifactAdded(d);

        Vector2Int pos = (Vector2Int)Vector3Int.RoundToInt(transform.position);

        map.data[pos].tileType = TileType.Floor;
        transform.parent.gameObject.SetActive(false);
    }
}

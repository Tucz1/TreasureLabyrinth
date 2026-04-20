using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class GridMovement : MonoBehaviour
{
    Map map;
    private bool isMoving;
    [SerializeField] float moveSpeed = 1f;
    public Vector2Int currentGridPos;
    PositionPlayer positionPlayer;
    public event Action InteractWithArtifact;
    EnemyAI enemy;
    public AudioClip whatToPlay;
    AudioSource myAudio;
    public UnityEvent footstepEvent;

    void Awake()
    {
        map = FindAnyObjectByType<Map>();
        myAudio = GetComponent<AudioSource>();

    }

    void Start()
    {
        positionPlayer = FindAnyObjectByType<PositionPlayer>();
        currentGridPos = (Vector2Int)Vector3Int.RoundToInt(positionPlayer.transform.position);
        
    }


    void Update()
    {
        if (isMoving) return;

        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");

        if (x != 0 || y != 0)
        {
            Vector2Int direction = new Vector2Int((int)x, (int)y);

            if (direction.x != 0) direction.y = 0;

            Vector2Int targetGridPos = currentGridPos + direction;

            Node targetNode = map.data[targetGridPos];
            bool CheckForWalkability()
            {
                return (targetNode.tileType == TileType.Floor) || 
                        targetNode.tileType == TileType.ArtifactSpawn ||
                        targetNode.tileType == TileType.EnemySpawn ||
                        targetNode.tileType == TileType.PatrolPoint ||
                        targetNode.tileType == TileType.Exit;
            }

            if (targetNode != null && CheckForWalkability())
            {
                if (targetNode.tileType == TileType.ArtifactSpawn)
                {
                    targetNode.artifact.Interact();
                    InteractWithArtifact?.Invoke();

                    map.spawnEnemy();
                    enemy = FindAnyObjectByType<EnemyAI>();
                    enemy.artifactPickedUp();
                    myAudio.PlayOneShot(whatToPlay);

                }

                currentGridPos = targetGridPos;
                StartCoroutine(SmoothMove(targetGridPos));

            }
        }
    }

    IEnumerator SmoothMove(Vector2Int targetGridPos)
    {
        isMoving = true;

        Vector3 targetWorldPos = new Vector3(targetGridPos.x, targetGridPos.y, 0f);
        footstepEvent.Invoke();

        while (Vector3.Distance(transform.position, targetWorldPos) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                targetWorldPos,
                moveSpeed * Time.deltaTime
            );

            yield return null;
        }

        transform.position = targetWorldPos;

        isMoving = false;
    }

}

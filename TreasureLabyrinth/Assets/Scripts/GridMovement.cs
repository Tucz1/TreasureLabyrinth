using System.Collections;
using UnityEngine;

public class GridMovement : MonoBehaviour
{
    Map map;
    private bool isMoving;
    [SerializeField] float moveSpeed = 1f;
    public Vector2Int currentGridPos;

    void Awake()
    {
        map = FindAnyObjectByType<Map>();

        transform.position = new Vector3(currentGridPos.x, currentGridPos.y, -1f);
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

            if (targetNode != null && targetNode.tileType == TileType.Floor)
            {
                currentGridPos = targetGridPos;
                StartCoroutine(SmoothMove(targetGridPos));

            }
        }
    }

    IEnumerator SmoothMove(Vector2Int targetGridPos)
    {
        isMoving = true;

        Vector3 targetWorldPos = new Vector3(targetGridPos.x, targetGridPos.y, -1f);

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

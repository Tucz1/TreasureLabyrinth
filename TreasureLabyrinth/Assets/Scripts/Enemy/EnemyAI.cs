using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour {
    public BFS map;
    public Vector2Int enemyCurrentGridPos;
    public Vector2Int goalPos;
    public Vector2Int pingPos;

    bool pinged = false;
    bool alert = false;
    float movespeed = 2f;
    float timer = 0f;
    float tick = 1f;

    public GridMovement player;
    Vector2Int playerPos;

    private void Awake() {

        transform.position = new Vector3 (enemyCurrentGridPos.x, enemyCurrentGridPos.y, 0);
    }

    private void chasePlayer() {
    }

    private Vector2Int generateGoalPos() {
        if (alert) goalPos = playerPos;
        if (pinged) goalPos = pingPos;

        goalPos = new Vector2Int(playerPos.x - 1, playerPos.y - 1);
        return goalPos;
    }
    private void Start() {
        if (goalPos == null) generateGoalPos();
        StartCoroutine(Move());
    }
    

    IEnumerator Move() {
        var path = map.SearchAndBuildPath(enemyCurrentGridPos, goalPos);
        if (path == null) {
            Debug.Log("No Path");
            yield return null;
        }

        foreach (var n in path) {
            transform.position = Vector2.MoveTowards(
            enemyCurrentGridPos,
            n,
            movespeed * Time.timeScale);
            enemyCurrentGridPos = n;
        }
        yield return null;
    }
}

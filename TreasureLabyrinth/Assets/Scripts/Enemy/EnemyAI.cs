using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder (100)]
public class EnemyAI : MonoBehaviour {
    public BFS map;
    public Vector2Int enemyCurrentGridPos;
    //public Vector2Int goalPos;
    public Vector2Int pingPos;
    public List<Vector2Int> path;

    bool alert = false;
    bool pinged = false;
    float movespeed = 1f;
    float timer = 0f;
    float tick = 1f;

    public GridMovement player;
    //Vector2Int playerPos;

    private void Start() {

        transform.position = new Vector3(enemyCurrentGridPos.x, enemyCurrentGridPos.y, 0);
        StartCoroutine(think());
    }

    private void chasePlayer() {
    }

    private Vector2Int generateGoalPos() {
        var goalPos = Vector2Int.zero;
        //if (alert) goalPos
        if (pinged) goalPos = pingPos;

        goalPos = new Vector2Int(player.currentGridPos.x,player.currentGridPos.y);
        return goalPos;
    }

    IEnumerator Move() {
        if (path == null) {
            Debug.Log("No Path");
            yield return null;
        }

        foreach (var n in path) {
            StartCoroutine(MoveStep(n));
        }
        yield return null;
    }
    IEnumerator MoveStep(Vector2Int target) {
        var analogTarget = (Vector3)(Vector2)target;
        while (transform.position != analogTarget) {
            transform.position = Vector2.MoveTowards(
            transform.position,
            analogTarget,
            movespeed * Time.deltaTime);
            yield return null;
        }
        enemyCurrentGridPos = target;
    }
    IEnumerator think() {
        while (true) {
            if (path == null || path.Count == 0) {
                var destination = generateGoalPos();
                path = map.SearchAndBuildPath(enemyCurrentGridPos, destination);
                print("made new path");
            }
            var next = path[0];
            path.RemoveAt(0);
            yield return MoveStep(next);
        }
    }
    //IEnumerator MoveStep(Vector2Int target) {
    //    var analogTarget = (Vector3)(Vector2)target;
    //    while (transform.position != analogTarget) {
    //        transform.position = Vector2.MoveTowards(
    //        enemyCurrentGridPos,
    //        analogTarget,
    //        movespeed * Time.timeScale);
    //        yield return null;
    //    }
    //    enemyCurrentGridPos = target;
    //    think();
    //}
    //private void think() {
    //    if (path == null || path.Count == 0) {
    //        var destination = generateGoalPos();
    //        path = map.SearchAndBuildPath(enemyCurrentGridPos, destination);
    //    }
    //    var next = path[0];
    //    path.RemoveAt(0);
    //    StartCoroutine(MoveStep(next));
    //}
}

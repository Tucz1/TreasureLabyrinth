using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[DefaultExecutionOrder (100)]
public class EnemyAI : MonoBehaviour {
    public BFS map;
    public Vector2Int enemyCurrentGridPos;
    //public Vector2Int goalPos;
    public Vector2Int pingPos;
    public Vector2Int patrolPos;
    public Vector2Int goalPos;
    public List<Vector2Int> path;

    bool playerVisible = false;
    bool pinged = false;
    bool patrollingToLPos = false;
    public float movespeed = 6f;
    float timer = 0f;
    float tick = 1f;

    public List<Vector2Int> patrolPointList;

    [SerializeField] GridMovement player;
    //Vector2Int playerPos;
    [SerializeField] private EnemyState currentState;
    private enum EnemyState {
        patrollingToPoint,
        patrollingToPlayer,
        pinged,
        alert
    }
    private void Awake() {
        player = FindAnyObjectByType<GridMovement>();
        map = FindAnyObjectByType<BFS>();
        if (player == null) Debug.Log("Player is null");
        int tPosX = (int)transform.position.x;
        int tPosY = (int)transform.position.y;
        enemyCurrentGridPos = new Vector2Int(tPosX, tPosY);
        currentState = EnemyState.patrollingToPoint;
        StartCoroutine(think());
    }

    private void chasePlayer() {
    }

    private Vector2Int generateGoalPos(Vector2Int ?goalPosInfo) {
        var goalPos = Vector2Int.zero;

        //PING LOCATION AS GOAL
        //if (pinged) goalPos = pingPos;

        //PREV POSITION AS GOAL
        if (currentState == EnemyState.patrollingToPlayer) {
            if (player == null) Debug.Log("Player is null, patrolstate");
        var lastPos = new Vector2Int(player.currentGridPos.x, player.currentGridPos.y);
            if (patrollingToLPos) {
                goalPos = (Vector2Int)goalPosInfo;
                Debug.Log("We're still going to the old position yippee");
            }
            else goalPos = lastPos;
        }
        
        //CURRENT POSITION AS GOAL
        if (currentState == EnemyState.alert) {
        goalPos = new Vector2Int(player.currentGridPos.x,player.currentGridPos.y);
        }

        //RANDOM PATROL POINT AS GOAL
        if (currentState == EnemyState.patrollingToPoint) {
            if (patrolPointList.Count < 1) {
                Debug.Log("Missing PatrolPointList, giving player location");
                goalPos = new Vector2Int(player.currentGridPos.x, player.currentGridPos.y);
            }

            else {
                Debug.Log("We shouldn't be here yet");
                int rndNum = Random.Range(0, patrolPointList.Count);
                goalPos = patrolPointList[rndNum];
            }
        }
        else
            Debug.Log($"Weird state situation: {currentState} , giving player pos.");
            goalPos = new Vector2Int(player.currentGridPos.x, player.currentGridPos.y);
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
                stateCheck();
                var destination = generateGoalPos(goalPos);
                path = map.SearchAndBuildPath(enemyCurrentGridPos, destination);
                print("made new path");
            }
            var next = path[0];
            path.RemoveAt(0);
            yield return MoveStep(next);
        }
    }

    public void artifactPickedUp() {
        StopAllCoroutines();
        path.Clear();
        patrollingToLPos = false;
        currentState = EnemyState.patrollingToPoint;
        StartCoroutine(think());
    }
    private void stateCheck() {
        var prevState = currentState;
        switch (prevState) {
            case EnemyState.patrollingToPoint:
                    currentState = EnemyState.patrollingToPlayer;
                    patrollingToLPos = true;
                break;
            case EnemyState.patrollingToPlayer:
                    currentState = EnemyState.patrollingToPoint;
                    patrollingToLPos = false;   
                break;
            case EnemyState.pinged:
                currentState = EnemyState.patrollingToPlayer;
                break;
            case EnemyState.alert:
                //Can we still see the player? if not return to patrol
                if (!playerVisible)
                    currentState = EnemyState.patrollingToPoint;
                break;
            default:
                currentState = prevState;
                Debug.Log("We ended up at default state, shouldn't happen.");
                break;
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

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BFS : MonoBehaviour {
	public Map map;
	public Vector2Int start;
	public Vector2Int goal;
	public GameObject cube;

	//bool JustSearch(Vector2Int startNode) {
	//	if (startNode == null)
	//		Debug.LogError ("Trying to run BFS with a null start node!");

	//	// Implementation straight from
	//	// http://en.wikipedia.org/wiki/Breadth-first_search

	//	var Q = new Queue<Vector2Int>();
	//	Q.Enqueue (startNode);

	//	var discovered = new HashSet<Vector2Int>();
	//	discovered.Add(startNode);

	//	while (Q.Count > 0) {
	//		var v = Q.Dequeue();
	//		// Process v.
	//		// If we're doing simple search we can just check if v is a goal node,
	//		// and return true if so. This also means a path exists from start to goal.
	//		if (v.type == NodeType.Goal)
	//			return true;
	//		foreach (Vector2Int w in v.neighbors) {
	//			// Slight addition to the BFS on Wikipedia: we have chosen to mark some
	//			// nodes as obstacles that the search is not allowed to go into, so we
	//			// have to check to make sure the newly found node is _not_ an obstacle.
	//			// If we wanted to use the exact Wikipedia code, we'd have to remove the
	//			// obstacle nodes from the graph before we run BFS.
	//			if (!discovered.Contains(w) && w.type != NodeType.Obstacle) {
	//				Q.Enqueue(w);
	//				discovered.Add(w);
	//			}
	//		}
	//	}
	//	return false; // We went through the whole graph, no goal nodes to be found.
	//}

	// Helper function that builds us the final path for the pathfinding BFS below
	List<Vector2Int> BuildPathFromBFSData(Dictionary<Vector2Int, Vector2Int?> discovered,
	                                            Vector2Int goalFound) {
		var path = new List<Vector2Int> ();
		path.Add(goalFound);
		Vector2Int? previousNode = discovered [goalFound];
		while (previousNode != null) {
			path.Add (previousNode.Value);
			previousNode = discovered [previousNode.Value];
		}
		path.Reverse ();
		return path;
	}
	private List<Vector2Int> getNeighbours(Vector2Int node) {
		List<Vector2Int> neighbours = new List<Vector2Int>();

		var nodeLeft = new Vector2Int(node.x - 1, node.y);
		if (map.data.ContainsKey(nodeLeft) &&
			map.data[nodeLeft].tileType != TileType.Wall)
			neighbours.Add(nodeLeft);

        var nodeRight = new Vector2Int(node.x + 1, node.y);
        if (map.data.ContainsKey(nodeRight) &&
            map.data[nodeRight].tileType != TileType.Wall)
            neighbours.Add(nodeRight);

        var nodeUp = new Vector2Int(node.x, node.y+1);
        if (map.data.ContainsKey(nodeUp) &&
            map.data[nodeUp].tileType != TileType.Wall)
            neighbours.Add(nodeUp);

        var nodeDown = new Vector2Int(node.x, node.y -1);
        if (map.data.ContainsKey(nodeDown) &&
            map.data[nodeDown].tileType != TileType.Wall)
            neighbours.Add(nodeDown);
        return neighbours;
	}
	// Search for goal and return path to goal if found, otherwise return null.
	// Only difference to pure search is that we do not just store the info of which nodes
	// we have visited, but also where we came from when we first visited them.
	// That lets us build a path after we hit a goal node.
	public List<Vector2Int> SearchAndBuildPath(Vector2Int startNode, Vector2Int goalNode) {
		//if (startNode == null)
		//	Debug.LogError ("Trying to run BFS with a null start node!");
		
		var Q = new Queue<Vector2Int>();
		Q.Enqueue (startNode);
		
		var discovered = new Dictionary<Vector2Int, Vector2Int?>();
		discovered.Add(startNode, null);
		
		while (Q.Count > 0) {
			var v = Q.Dequeue();
			// Process v.
			if (v == goalNode)
				return BuildPathFromBFSData(discovered, v);
			foreach (Vector2Int w in getNeighbours(v)) {
				if (!discovered.ContainsKey(w)) {
					Q.Enqueue(w);
					discovered.Add(w, v);
				}
			}
		}
		return null; // We went through the whole graph, no goal nodes to be found.
	}

	// Find a start node from the scene. We're assuming there's only one.

	void Start() {
		map = GetComponent<Map>();
		//var path = SearchAndBuildPath(start, goal);
		//if (path == null) {
		//	print("No path");
		//	return;
		//}
		//foreach (var pos in path) {
		//	Instantiate(cube, (Vector3)(Vector2)pos, Quaternion.identity);
		}
	}

	


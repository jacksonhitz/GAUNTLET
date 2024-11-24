using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Tilemaps;

public class Pathfinding : MonoBehaviour
{
    public GameObject enemy;
    public int gridWidth = 50;
    public int gridHeight = 50;
    public float cellSize = 0.2f;
    public Tilemap tilemap;

    private Node[,] grid;
    private List<Node> path;

    private void Update()
    {
        if (enemy != null)
        {
            CreateGrid(enemy.transform.position);
        }
    }

    private void CreateGrid(Vector3 origin)
    {
        grid = new Node[gridWidth, gridHeight];
        Vector2 gridOrigin = new Vector2(origin.x - (gridWidth * cellSize) / 2, origin.y - (gridHeight * cellSize) / 2);

        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                Vector3 worldPosition = gridOrigin + new Vector2(x * cellSize, y * cellSize);
                Vector3Int tilePosition = tilemap.WorldToCell(worldPosition);

                bool isWalkable = !tilemap.HasTile(tilePosition);
                grid[x, y] = new Node(new Vector3Int(x, y, 0), worldPosition, isWalkable);
            }
        }
    }

    public List<Vector3> FindPath(Vector2 startWorldPosition, Vector2 targetWorldPosition)
    {
        Node startNode = GetNodeFromWorldPosition(startWorldPosition);
        Node targetNode = GetNodeFromWorldPosition(targetWorldPosition);

        if (startNode == null || targetNode == null || !startNode.walkable || !targetNode.walkable)
        {
            path = null;
            return null;
        }

        List<Node> openSet = new List<Node>();
        HashSet<Node> closedSet = new HashSet<Node>();
        Dictionary<Node, Node> cameFrom = new Dictionary<Node, Node>();
        Dictionary<Node, int> gCost = new Dictionary<Node, int>();
        Dictionary<Node, int> fCost = new Dictionary<Node, int>();

        openSet.Add(startNode);
        gCost[startNode] = 0;
        fCost[startNode] = GetHeuristic(startNode, targetNode);

        while (openSet.Count > 0)
        {
            Node currentNode = openSet[0];
            foreach (Node node in openSet)
            {
                if (fCost[node] < fCost[currentNode] || (fCost[node] == fCost[currentNode] && gCost[node] < gCost[currentNode]))
                {
                    currentNode = node;
                }
            }

            openSet.Remove(currentNode);
            closedSet.Add(currentNode);

            if (currentNode == targetNode)
            {
                path = RetracePath(cameFrom, startNode, targetNode);
                return ConvertNodesToWorldPositions(path);
            }

            foreach (Node neighbor in GetNeighbors(currentNode))
            {
                if (closedSet.Contains(neighbor)) continue;
                if (!neighbor.walkable) continue;

                int tentativeGCost = gCost[currentNode] + 1;
                if (!openSet.Contains(neighbor) || tentativeGCost < gCost[neighbor])
                {
                    cameFrom[neighbor] = currentNode;
                    gCost[neighbor] = tentativeGCost;
                    fCost[neighbor] = gCost[neighbor] + GetHeuristic(neighbor, targetNode);

                    if (!openSet.Contains(neighbor))
                        openSet.Add(neighbor);
                }
            }
        }

        path = null;
        return null;
    }

    private List<Vector3> ConvertNodesToWorldPositions(List<Node> nodes)
    {
        List<Vector3> worldPositions = new List<Vector3>();
        foreach (Node node in nodes)
        {
            worldPositions.Add(node.worldPosition);
        }
        return worldPositions;
    }

    private Node GetNodeFromWorldPosition(Vector2 worldPosition)
    {
        Vector2 gridOrigin = new Vector2(enemy.transform.position.x - (gridWidth * cellSize) / 2, enemy.transform.position.y - (gridHeight * cellSize) / 2);
        int x = Mathf.FloorToInt((worldPosition.x - gridOrigin.x) / cellSize);
        int y = Mathf.FloorToInt((worldPosition.y - gridOrigin.y) / cellSize);

        if (x >= 0 && x < gridWidth && y >= 0 && y < gridHeight)
            return grid[x, y];

        return null;
    }

    private List<Node> GetNeighbors(Node node)
    {
        List<Node> neighbors = new List<Node>();
        Vector3Int[] directions = { Vector3Int.right, Vector3Int.left, Vector3Int.up, Vector3Int.down };

        foreach (Vector3Int direction in directions)
        {
            int neighborX = node.gridPosition.x + direction.x;
            int neighborY = node.gridPosition.y + direction.y;

            if (neighborX >= 0 && neighborX < gridWidth && neighborY >= 0 && neighborY < gridHeight)
                neighbors.Add(grid[neighborX, neighborY]);
        }

        return neighbors;
    }

    private List<Node> RetracePath(Dictionary<Node, Node> cameFrom, Node startNode, Node endNode)
    {
        List<Node> path = new List<Node>();
        Node currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = cameFrom[currentNode];
        }

        path.Reverse();
        return path;
    }

    private int GetHeuristic(Node a, Node b)
    {
        return Mathf.Abs(a.gridPosition.x - b.gridPosition.x) + Mathf.Abs(a.gridPosition.y - b.gridPosition.y);
    }

    private void OnDrawGizmos()
    {
        if (grid == null || enemy == null) return;

        foreach (Node node in grid)
        {
            Gizmos.color = node.walkable ? Color.white : Color.red;
            Gizmos.DrawCube(node.worldPosition, Vector3.one * (cellSize * 0.9f));
        }

        if (path == null) return;

        Gizmos.color = Color.green;
        for (int i = 0; i < path.Count; i++)
        {
            Gizmos.DrawSphere(path[i].worldPosition, 0.1f);

            if (i < path.Count - 1)
                Gizmos.DrawLine(path[i].worldPosition, path[i + 1].worldPosition);
        }
    }
}

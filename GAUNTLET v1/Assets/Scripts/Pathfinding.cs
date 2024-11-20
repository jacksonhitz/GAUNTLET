using System.Collections.Generic;
using UnityEngine;

public class Pathfinding : MonoBehaviour
{
    public GridManager tilemapManager;
    List<Vector3Int> path; 

    public List<Vector3Int> FindPath(Vector2 startWorldPosition, Vector2 targetWorldPosition)
    {
        Vector3Int startTile = tilemapManager.WorldToTilePosition(startWorldPosition);
        Vector3Int targetTile = tilemapManager.WorldToTilePosition(targetWorldPosition);

        if (!tilemapManager.IsWalkable(startTile) || !tilemapManager.IsWalkable(targetTile))
        {
            path = null; // Clear path if invalid
            return null;
        }

        List<Vector3Int> openSet = new List<Vector3Int>();
        HashSet<Vector3Int> closedSet = new HashSet<Vector3Int>();
        Dictionary<Vector3Int, Vector3Int> cameFrom = new Dictionary<Vector3Int, Vector3Int>();
        Dictionary<Vector3Int, int> gCost = new Dictionary<Vector3Int, int>();
        Dictionary<Vector3Int, int> fCost = new Dictionary<Vector3Int, int>();

        openSet.Add(startTile);
        gCost[startTile] = 0;
        fCost[startTile] = GetHeuristic(startTile, targetTile);

        while (openSet.Count > 0)
        {
            Vector3Int currentTile = openSet[0];
            foreach (Vector3Int tile in openSet)
            {
                if (fCost[tile] < fCost[currentTile] || (fCost[tile] == fCost[currentTile] && gCost[tile] < gCost[currentTile]))
                {
                    currentTile = tile;
                }
            }

            openSet.Remove(currentTile);
            closedSet.Add(currentTile);

            if (currentTile == targetTile)
            {
                path = RetracePath(cameFrom, startTile, targetTile); 
                return path;
            }

            foreach (Vector3Int neighbor in tilemapManager.GetNeighbors(currentTile))
            {
                if (closedSet.Contains(neighbor)) continue;

                int tentativeGCost = gCost[currentTile] + 1;
                if (!openSet.Contains(neighbor) || tentativeGCost < gCost[neighbor])
                {
                    cameFrom[neighbor] = currentTile;
                    gCost[neighbor] = tentativeGCost;
                    fCost[neighbor] = gCost[neighbor] + GetHeuristic(neighbor, targetTile);

                    if (!openSet.Contains(neighbor))
                        openSet.Add(neighbor);
                }
            }
        }

        path = null; 
        return null;
    }

    List<Vector3Int> RetracePath(Dictionary<Vector3Int, Vector3Int> cameFrom, Vector3Int startTile, Vector3Int endTile)
    {
        List<Vector3Int> path = new List<Vector3Int>();
        Vector3Int currentTile = endTile;

        while (currentTile != startTile)
        {
            path.Add(currentTile);
            currentTile = cameFrom[currentTile];
        }

        path.Reverse();
        return path;
    }

    private int GetHeuristic(Vector3Int a, Vector3Int b)
    {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
    }

    private void OnDrawGizmos()
    {
        if (path == null) return;

        Gizmos.color = Color.green;

        for (int i = 0; i < path.Count; i++)
        {
            Vector3 worldPosition = tilemapManager.TileToWorldPosition(path[i]);
            Gizmos.DrawSphere(worldPosition, 0.2f); 

            if (i < path.Count - 1)
            {
                Vector3 nextWorldPosition = tilemapManager.TileToWorldPosition(path[i + 1]);
                Gizmos.DrawLine(worldPosition, nextWorldPosition);
            }
        }
    }
}

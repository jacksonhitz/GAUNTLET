using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GridManager : MonoBehaviour
{
    public Tilemap tilemap; // Tilemap where the enemy can walk

    // Check if the tile exists in the tilemap (all tiles are walkable)
    public bool IsWalkable(Vector3Int tilePosition)
    {
        // Check if the tile exists within the tilemap (not an empty tile)
        TileBase tile = tilemap.GetTile(tilePosition);
        return tile != null; // All tiles in this map are walkable, so just check if the tile exists
    }

    public Vector3 TileToWorldPosition(Vector3Int tilePosition)
    {
        return tilemap.GetCellCenterWorld(tilePosition);
    }

    public Vector3Int WorldToTilePosition(Vector2 worldPosition)
    {
        return tilemap.WorldToCell(worldPosition);
    }

    // Get neighboring tiles (up, down, left, right) for the given tile position
    public List<Vector3Int> GetNeighbors(Vector3Int tilePosition)
    {
        List<Vector3Int> neighbors = new List<Vector3Int>();

        // List of directions to check: Up, Down, Right, Left
        foreach (Vector3Int offset in new Vector3Int[]
        {
            new Vector3Int(0, 1, 0), // Up
            new Vector3Int(0, -1, 0), // Down
            new Vector3Int(1, 0, 0), // Right
            new Vector3Int(-1, 0, 0) // Left
        })
        {
            Vector3Int neighborPosition = tilePosition + offset;
            if (IsWalkable(neighborPosition)) // All tiles in the map are walkable
            {
                neighbors.Add(neighborPosition);
            }
        }

        return neighbors;
    }
}

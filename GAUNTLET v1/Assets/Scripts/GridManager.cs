using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GridManager : MonoBehaviour
{
    public Tilemap tilemap; 
    public bool IsWalkable(Vector3Int tilePosition)
    {
        TileBase tile = tilemap.GetTile(tilePosition);
        return tile != null; 
    }

    public Vector3 TileToWorldPosition(Vector3Int tilePosition)
    {
        return tilemap.GetCellCenterWorld(tilePosition);
    }

    public Vector3Int WorldToTilePosition(Vector2 worldPosition)
    {
        return tilemap.WorldToCell(worldPosition);
    }
    public List<Vector3Int> GetNeighbors(Vector3Int tilePosition)
    {
        List<Vector3Int> neighbors = new List<Vector3Int>();

        foreach (Vector3Int offset in new Vector3Int[]
        {
            new Vector3Int(0, 1, 0), // Up
            new Vector3Int(0, -1, 0), // Down
            new Vector3Int(1, 0, 0), // Right
            new Vector3Int(-1, 0, 0) // Left
        })
        {
            Vector3Int neighborPosition = tilePosition + offset;
            if (IsWalkable(neighborPosition)) 
            {
                neighbors.Add(neighborPosition);
            }
        }

        return neighbors;
    }
}

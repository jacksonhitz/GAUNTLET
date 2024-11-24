using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
    public Vector3Int gridPosition;
    public Vector2 worldPosition;
    public bool walkable;

    public Node(Vector3Int gridPosition, Vector2 worldPosition, bool walkable)
    {
        this.gridPosition = gridPosition;
        this.worldPosition = worldPosition;
        this.walkable = walkable;
    }
}

using System.Collections.Generic;
using UnityEngine;


public enum TileType
{
    VOID,
    FLOOR,
    WALL_UP,
    WALL_DOWN,
    WALL_RIGHT,
    WALL_LEFT,
    WALL_CORNER_LARGE_RIGHT,
    WALL_CORNER_LARGE_LEFT,
    WALL_CORNER_SHORT_RIGHT,
    WALL_CORNER_SHORT_LEFT,
    LADDER
}
public class Tile
{
    public int x;
    public int y;

    public TileType tileType;

    //list of neighbours tiles (up, down, right, top) where a character could pass
    public List<Tile> reachableNeighbours;

    public Tile(int x, int y, TileType tileType = TileType.VOID)
    {
        this.x = x;
        this.y = y;
        this.tileType = tileType;
    }

    /// <summary>
    /// Set tileType.
    /// </summary>
    public void SetTileType(TileType tileType)
    {
        this.tileType = tileType;
    }

    /// <summary>
    /// Set reachable neighbours tiles.
    /// </summary>
    public void SetReachableNeighbours(List<Tile> reachableNeighbours)
    {
        this.reachableNeighbours = reachableNeighbours;
    }

    /// <summary>
    /// Check if this tile is reachable for characters.
    /// </summary>
    public bool IsReachable()
    {
        return tileType == TileType.FLOOR || tileType == TileType.LADDER;
    }

    /// <summary>
    /// Check if this tile is a wall.
    /// </summary>
    public bool IsWall()
    {
        return (tileType == TileType.WALL_UP ||
                tileType == TileType.WALL_DOWN ||
                tileType == TileType.WALL_RIGHT ||
                tileType == TileType.WALL_LEFT);
    }

    /// <summary>
    /// Calculate the distance between two tiles.
    /// </summary>
    public float Distance(Tile target)
    {
        return (Mathf.Abs(this.x - target.x) + Mathf.Abs(this.y - target.y));
    }
}

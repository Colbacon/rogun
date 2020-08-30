using System.Collections.Generic;
using UnityEngine;

public class Room
{
    public int x;
    public int y;

    public int width;
    public int height;

    public List<Tile> innerTiles;
    public List<Tile> borderTiles;

    public Room(int x, int y, int width, int height)
    {
        this.x = x;
        this.y = y;
        this.width = width;
        this.height = height;
    }

    /*
    private void SetRoomTiles()
    {
        innerTiles = new List<Tile>();
        borderTiles = new List<Tile>();

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                /
                if (x == 0 || x == width - 1 || y == 0 || y == height - 1)
                {
                    borderTiles.Add()
                }
                else
                {

                }
            }
        }
    }
    */
    /// <summary>
    /// Check if there is an intersection with another room.
    /// </summary>
    public bool IsRoomCollision(Room room)
    {
        return (x < room.x + room.width &&  // room1 left edge past room2 right
                x + width > room.x &&       // room1 right edge past room2 left
                y < room.y + room.height && // room1 bottom edge past room2 top
                y + height > room.y);       // room1 top edge past room2 bottom
    }

    /// <summary>
    /// Get distante with another room, using both center's points
    /// </summary>
    public float GetDistanceToRoom(Room room)
    {
        return Vector2.Distance(GetCenterPoint(), room.GetCenterPoint());
    }

    /// <summary>
    /// Get room's center point.
    /// </summary>
    public Vector2 GetCenterPoint()
    {
        return new Vector2(Mathf.Floor(x + (x + width)) / 2, Mathf.Floor(y + (y + height)) / 2);
    }

    public Tile GetRandomInnerTile()
    {
        Tile tile = innerTiles[Random.Range(0, innerTiles.Count)];
        
        //remove from list to avoid get one tile more than once (p.e place units or items)
        innerTiles.Remove(tile);

        return tile;
    }

}


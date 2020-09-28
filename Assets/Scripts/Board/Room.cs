﻿using System.Collections.Generic;
using UnityEngine;

public class Room
{
    public int x;
    public int y;

    public int width;
    public int height;

    //for placing units, items and decorations
    public List<_Tile> floorTiles = new List<_Tile>();
    //for placing decoration
    public List<_Tile> upWallTiles = new List<_Tile>();
    public List<_Tile> downWallTiles = new List<_Tile>();
    public List<_Tile> leftWallTiles = new List<_Tile>();
    public List<_Tile> rightWallTiles = new List<_Tile>();

    public Room(int x, int y, int width, int height)
    {
        this.x = x;
        this.y = y;
        this.width = width;
        this.height = height;
    }

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
        //return Vector2.Distance(GetCenterPoint(), room.GetCenterPoint());
        return Vector3Int.Distance(GetCenterPoint(), room.GetCenterPoint());
    }

    /// <summary>
    /// Get room's center point.
    /// </summary>
    public Vector3Int GetCenterPoint()
    {
        return new Vector3Int(Mathf.FloorToInt(x + (x + width)) / 2, Mathf.FloorToInt(y + (y + height)) / 2, 0);
    }

    public _Tile GetRandomFloorTile()
    {
        _Tile tile = floorTiles[Random.Range(0, floorTiles.Count)];
        
        //remove from list to avoid get one tile more than once (p.e place units or items)
        floorTiles.Remove(tile);

        return tile;
    }

    public _Tile GetRandomDownWallTile()
    {
        _Tile tile = downWallTiles[Random.Range(0, downWallTiles.Count)];

        //remove from list to avoid get one tile more than once (p.e place units or items)
        downWallTiles.Remove(tile);

        return tile;
    }

    public _Tile GetRandomLeftWallTile()
    {
        _Tile tile = leftWallTiles[Random.Range(0, leftWallTiles.Count)];

        //remove from list to avoid get one tile more than once (p.e place units or items)
        leftWallTiles.Remove(tile);

        return tile;
    }

    public _Tile GetRandomRightWallTile()
    {
        _Tile tile = rightWallTiles[Random.Range(0, rightWallTiles.Count)];

        //remove from list to avoid get one tile more than once (p.e place units or items)
        rightWallTiles.Remove(tile);

        return tile;
    }
}


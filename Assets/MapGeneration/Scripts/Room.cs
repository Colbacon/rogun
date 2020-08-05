using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MapGeneration
{
    public class Room
    {
        //top left position
        public int x;
        public int y;

        public int width;
        public int height;

        public List<Tile> tiles;

        public Room(int x, int y, int width, int height)
        {
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
        }

        public bool IsRoomCollision(Room room)
        {
            return (x < room.x + room.width &&  // room1 left edge past room2 right
                    x + width > room.x &&       // room1 right edge past room2 left
                    y < room.y + room.height && // room1 bottom edge past room2 top
                    y + height > room.y);       // room1 top edge past room2 bottom
        }

        public float GetDistanceToRoom(Room room)
        {
            return Vector2.Distance(GetCenterPoint(), room.GetCenterPoint());
        }

        public Vector2 GetCenterPoint()
        {
            return new Vector2(Mathf.Floor(x + (x + width)) / 2, Mathf.Floor(y + (y + height)) / 2);
        }

    }
}

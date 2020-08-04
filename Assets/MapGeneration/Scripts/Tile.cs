using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MapGeneration
{
    public enum TileType
    {
        VOID = 0,
        FLOOR = 1,
        WALL = 2,
        LADDER = 3
    }
    public class Tile
    {
        public int x;
        public int y;

        public TileType tileType;
        //public bool isObstacle;

        public List<Tile> neighbours;

        public Tile(int x, int y, TileType tileType)
        {
            this.x = x;
            this.y = y;
            this.tileType = tileType;
            //this.isObstacle = (tileType == TileType.FLOOR) ? false : true;
        }

        public void SetTileType(TileType tileType)
        {
            this.tileType = tileType;
        }
    }
}

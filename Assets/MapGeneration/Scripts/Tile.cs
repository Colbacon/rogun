using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MapGeneration
{
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

        
        //related properties to A* pathfinding
        private List<Tile> reachableNeighbours;
        /*
        public Tile parent;
        public float distanceToTarget; //h value -> heuristic
        public float cost; //g value
        public float F
        {
            get
            {
                if (distanceToTarget != -1 && cost != -1)
                    return distanceToTarget + cost;
                else
                    return -1;
            }
        }
        */ //quitar vecinos

        public Tile(int x, int y, TileType tileType = TileType.VOID)
        {
            this.x = x;
            this.y = y;
            this.tileType = tileType;
        }

        public void SetTileType(TileType tileType)
        {
            this.tileType = tileType;
        }

        public void SetReachableNeighbours(List<Tile> reachableNeighbours)
        {
            this.reachableNeighbours = reachableNeighbours;
        }

        public bool IsWall()
        {
            return (tileType == TileType.WALL_UP ||
                    tileType == TileType.WALL_DOWN ||
                    tileType == TileType.WALL_RIGHT ||
                    tileType == TileType.WALL_LEFT);
        }

        public bool IsReachable()
        {
            return tileType == TileType.FLOOR || tileType == TileType.LADDER;
        }

        public float Distance(Tile target)
        {
            return (Mathf.Abs(this.x - target.x) + Mathf.Abs(this.y - target.y));
        }
    }
}

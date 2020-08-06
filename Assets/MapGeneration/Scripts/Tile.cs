using System.Collections;
using System.Collections.Generic;

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
        LADDER
    }
    public class Tile
    {
        public int x;
        public int y;

        public TileType tileType;
        public bool isObstacle;

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

        public bool IsWall()
        {
            return (tileType == TileType.WALL_UP ||
                    tileType == TileType.WALL_DOWN ||
                    tileType == TileType.WALL_RIGHT ||
                    tileType == TileType.WALL_LEFT);
        }
    }
}

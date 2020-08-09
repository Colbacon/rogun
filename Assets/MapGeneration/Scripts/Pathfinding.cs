using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace MapGeneration
{
    public static class Pathfinding
    {

        public static Stack<Tile> AStartPathfinding(Tile start, Tile end)
        {
            Stack<Tile> path = new Stack<Tile>();

            List<Tile> openSet = new List<Tile>(); //for better perporfance priority queue or min-heap
            Dictionary<Tile, Tile> cameFrom = new Dictionary<Tile, Tile>();
            Dictionary<Tile, float> gScore = new Dictionary<Tile, float>();
            Dictionary<Tile, float> fScore = new Dictionary<Tile, float>();
            Tile current;

            openSet.Add(start);
            gScore[start] = 0;
            fScore[start] = start.Distance(end);

            while(openSet.Count > 0)
            {
                current = 
            }

            return path;
        }

    }

}

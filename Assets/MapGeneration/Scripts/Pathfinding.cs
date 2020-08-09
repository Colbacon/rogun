using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Priority_Queue;

namespace MapGeneration
{
    public static class Pathfinding
    {
        //https://en.wikipedia.org/wiki/A*_search_algorithm
        public static List<Tile> AStartPathfinding(Tile start, Tile end)
        {
            SimplePriorityQueue<Tile> openSet = new SimplePriorityQueue<Tile>();
            Dictionary<Tile, Tile> cameFrom = new Dictionary<Tile, Tile>();
            Dictionary<Tile, float> gScore = new Dictionary<Tile, float>();
            Tile current = null;
            float tentativeGScore;
            float f;

            openSet.Enqueue(start,0);
            cameFrom.Add(start, null);
            gScore.Add(start, 0);

            while(openSet.Count > 0)
            {
                Debug.Log("entrao1");
                current = openSet.Dequeue();
                if (current == end)
                    break;
                Debug.Log("entrao2: " + current.reachableNeighbours.Count);
                
                foreach (Tile neighbour in current.reachableNeighbours)
                {
                    Debug.Log("entrao3");
                    tentativeGScore = gScore[current] + 1;

                    if (!gScore.ContainsKey(neighbour) || tentativeGScore < gScore[neighbour])
                    {
                        gScore[neighbour] = tentativeGScore;
                        if (cameFrom.ContainsKey(neighbour))
                        {
                            cameFrom[neighbour] = current;
                        }
                        else
                        {
                            cameFrom.Add(neighbour, current);
                        }

                        f = tentativeGScore + current.Distance(neighbour);
                        openSet.Enqueue(neighbour, f);
                    }
                    
                }
            }

            if (current != end)
                return null;

            List<Tile> path = new List<Tile>();

            while (current != null)
            {
                path.Add(current);
                if (cameFrom.ContainsKey(current))
                {
                    current = cameFrom[current];
                }
                else
                {
                    break;
                }
            }
            path.Reverse();
            Debug.Log(" "+path.Count);
            return path;
            /*
            path.Add(start);
            return path;
            */
        }



    }

}

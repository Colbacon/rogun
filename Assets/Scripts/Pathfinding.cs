using System.Collections.Generic;
using Priority_Queue;


public static class Pathfinding
{
    /// <summary>
    /// Calculates shortest path between two reachable tiles using and heuristic function.
    /// The heuristic function used is the distance between two tiles.
    /// https://en.wikipedia.org/wiki/A*_search_algorithm
    /// </summary>
    public static List<_Tile> AStartSorthestPath(_Tile start, _Tile end, bool ignoreOccupiedTiles = false)
    {
        //set of discovered  nodes that may need to be (re-)expanded
        //the nodes are ordered by f value associated to the node.
        SimplePriorityQueue<_Tile> openSet = new SimplePriorityQueue<_Tile>();
        //for node n, cameFrom[n] is the node inmmediately preceding it on the cheapest path from start
        Dictionary<_Tile, _Tile> cameFrom = new Dictionary<_Tile, _Tile>();
        //for node n, gScore[n] is the cost of the cheapest path from start to n currently know.
        Dictionary<_Tile, float> gScore = new Dictionary<_Tile, float>();
        _Tile current = null;
        float tentativeGScore;
        float f; //f(n) = g(n) + h(n)

        openSet.Enqueue(start,0);
        cameFrom.Add(start, null);
        gScore.Add(start, 0);

        while(openSet.Count > 0)
        {
            //getting current tile with lowest f value
            current = openSet.Dequeue();
            if (current == end)
                break;
                
            foreach (_Tile neighbour in current.reachableNeighbours)
            {
                if ((!ignoreOccupiedTiles && neighbour.isOccupied) && neighbour != end) //end tile tipically will be occupied by player
                    continue;

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

        //non existing solution
        if (current != end)
            return null;

        //reconstruct the solution path
        List<_Tile> path = new List<_Tile>();

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
        return path;
    }
}


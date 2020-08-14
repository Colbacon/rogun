using System.Collections.Generic;
using Priority_Queue;


public static class Pathfinding
{
    /// <summary>
    /// Calculates shortest path between two reachable tiles using and heuristic function.
    /// The heuristic function used is the distance between two tiles.
    /// https://en.wikipedia.org/wiki/A*_search_algorithm
    /// </summary>
    public static List<Tile> AStartSorthestPath(Tile start, Tile end)
    {
        //set of discovered  nodes that may need to be (re-)expanded
        //the nodes are ordered by f value associated to the node.
        SimplePriorityQueue<Tile> openSet = new SimplePriorityQueue<Tile>();
        //for node n, cameFrom[n] is the node inmmediately preceding it on the cheapest path from start
        Dictionary<Tile, Tile> cameFrom = new Dictionary<Tile, Tile>();
        //for node n, gScore[n] is the cost of the cheapest path from start to n currently know.
        Dictionary<Tile, float> gScore = new Dictionary<Tile, float>();
        Tile current = null;
        float tentativeGScore;
        float f; //f(n) = g(n) + h(n)

        openSet.Enqueue(start,0);
        cameFrom.Add(start, null);
        gScore.Add(start, 0);

        while(openSet.Count > 0)
        {
            current = openSet.Dequeue();
            if (current == end)
                break;
                
            foreach (Tile neighbour in current.reachableNeighbours)
            {
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
                break; //needed?
            }
        }
        path.Reverse();
        return path;
    }
}


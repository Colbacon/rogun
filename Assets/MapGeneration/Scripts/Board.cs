using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using QuickGraph;
using QuickGraph.Algorithms;
using QuickGraph.Algorithms.ShortestPath;


namespace MapGeneration
{
    public static class Extensions
    {
        class AStarWrapper<TVertex, TEdge>
        where TEdge : IEdge<TVertex>
        {
            private TVertex target;
            private AStarShortestPathAlgorithm<TVertex, TEdge> innerAlgorithm;
            public AStarWrapper(AStarShortestPathAlgorithm<TVertex, TEdge> innerAlgo, TVertex root, TVertex target)
            {
                innerAlgorithm = innerAlgo;
                this.innerAlgorithm.SetRootVertex(root);
                this.target = target;
                this.innerAlgorithm.FinishVertex += new VertexAction<TVertex>(innerAlgorithm_FinishVertex);
            }
            void innerAlgorithm_FinishVertex(TVertex vertex)
            {
                if (object.Equals(vertex, target))
                    this.innerAlgorithm.Abort();
            }
            public double Compute()
            {
                this.innerAlgorithm.Compute();
                return this.innerAlgorithm.Distances[target];
            }
        }

        public static double ComputeDistanceBetween<TVertex, TEdge>(this AStarShortestPathAlgorithm<TVertex, TEdge> algo, TVertex start, TVertex end)
            where TEdge : IEdge<TVertex>
        {
            var wrap = new AStarWrapper<TVertex, TEdge>(algo, start, end);
            return wrap.Compute();
        }
    }

    public class Board : MonoBehaviour
    {

        public static int rows = 100;
        public static int columns = 100;

        public Tile[][] tileMap;

        private List<Room> rooms;
        public static int roomsToAttemp = 50;

        public GameObject _void;
        public GameObject floor;
        public GameObject wall;
        public GameObject wallUp;
        public GameObject wallDown;
        public GameObject wallRight;
        public GameObject wallLeft;
        public GameObject wallCornerShortRight;
        public GameObject wallCornerShortLeft;
        public GameObject wallCornerLargeRight;
        public GameObject wallCornerLargeLeft;
        public GameObject ladder;

        private Transform boardTransform;

        void Awake()
        {
            boardTransform = new GameObject("Board").transform;

            InitTileMap();
            CreateRooms();
            //CreateCorridors();
            CreateCorridorsMST();
            AddTilesNeighbours();
            InstantiateTiles();

            //TestQuickgraph();

            TestPathfinding();
        }

        private void TestPathfinding()
        {
            Tile start = tileMap[rooms[1].x+2][rooms[1].y+2];
            Tile end = tileMap[rooms[3].x+6][rooms[3].y+6];
            List<Tile> path = Pathfinding.AStartPathfinding(start, end);
            if (path == null)
                Debug.Log("nullito");
            for(int i = 0; i < path.Count; i++)
            {
                int x = path[i].x;
                int y = path[i].y;
                Instantiate(ladder, new Vector3(x, y, 0f), Quaternion.identity);
                Debug.Log("iteration " + i + "  x: " + x + "  y: " + y);
            }
        }
        private void TestQuickgraph()
        {
            /*
            var g = new UndirectedGraph<int, TaggedUndirectedEdge<int, int>>();
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    g.AddVerticesAndEdge(new TaggedUndirectedEdge<int, int>(i, j, i+j));
                }
            }
            var vertices = g.Vertices.ToList();
            Debug.Log("total vertices added "+ vertices.Count);
            for (int i = 0; i < vertices.Count; i++)
            {
                Debug.Log(vertices[i]);

            }
            var edges = g.Edges.ToList();
            Debug.Log("total edges added " + edges.Count);
            for (int i = 0; i < edges.Count; i++)
            {
                Debug.Log(edges[i]);

            }
            var mst = g.MinimumSpanningTreePrim(e => e.Tag).ToList();
            for (int i = 0; i < mst.Count; i++)
            {
                Debug.Log(mst[i]);
            }
            */
            //https://stackoverflow.com/questions/14557896/minimum-spanning-tree-quick-graph
           /* var g = new UndirectedGraph<int, TaggedUndirectedEdge<int, int>>();

            var e1 = new TaggedUndirectedEdge<int, int>(1, 2, 57);
            var e2 = new TaggedUndirectedEdge<int, int>(1, 4, 65);
            var e3 = new TaggedUndirectedEdge<int, int>(2, 3, 500);
            var e4 = new TaggedUndirectedEdge<int, int>(2, 4, 1);
            var e5 = new TaggedUndirectedEdge<int, int>(3, 4, 78);
            var e6 = new TaggedUndirectedEdge<int, int>(3, 5, 200);

            g.AddVerticesAndEdge(e1);
            g.AddVerticesAndEdge(e2);
            g.AddVerticesAndEdge(e3);
            g.AddVerticesAndEdge(e4);
            g.AddVerticesAndEdge(e5);
            g.AddVerticesAndEdge(e6);

            var mst = g.MinimumSpanningTreePrim(e => e.Tag).ToList();
            for (int i = 0; i < mst.Count; i++)
            {
                Debug.Log(mst[i]);
            }

            var vertices = g.Vertices.ToList();
            var d = g.ShortestPathsDijkstra(e => e.Tag, verti).ToList();
            //var astar = new AStarShortestPathAlgorithm<int, IEdge<int>>(g, x => 1.0, x => 0.0);
            //var dist = astar.ComputeDistanceBetween(2, 4);
            //astar.
            */


        }

        private void InitTileMap()
        {
            tileMap = new Tile[columns][];
            for (int i = 0; i < tileMap.Length; i++)
            {
                tileMap[i] = new Tile[rows];
            }

            for (int x = 0; x < columns; x++)
            {
                for (int y = 0; y < rows; y++)
                {
                    tileMap[x][y] = new Tile(x, y, TileType.VOID);
                }
            }
        }

        /**
         * ROOMS 
         **/
        private void CreateRooms()
        {
            rooms = new List<Room>();
            Room room;
            int x, y, width, height;

            for (int i = 0; i < roomsToAttemp; i++)
            {
                width = Random.Range(10, 15);
                height = Random.Range(10, 15);

                x = Random.Range(0, columns - width);
                y = Random.Range(0, rows - height);

                room = new Room(x, y, width, height);
                //Debug.Log("Trying room " + i);
                if (!CollidesWithBoardsRooms(room))
                {
                    //Debug.Log("--------Success room " + i);
                    rooms.Add(room);
                    UpdateRoomTiles(room);
                }
            }
        }

        private bool CollidesWithBoardsRooms(Room room)
        {
            for (int i = 0; i < rooms.Count; i++)
            {
                if (room.IsRoomCollision(rooms[i]))
                    return true;
            }

            return false;
        }

        private void UpdateRoomTiles(Room room)
        {
            TileType tileType;

            for (int x = 0; x < room.width; x++)
            {
                for (int y = 0; y < room.height; y++)
                {
                    /*
                    if (x == 0 || x == room.width -1|| y == 0 || y == room.height -1)
                    {
                        tileMap[room.x + x][room.y + y].SetTileType(TileType.WALL);
                    }
                    else
                    {
                        tileMap[room.x + x][room.y + y].SetTileType(TileType.FLOOR);
                    }*/
                    if (x == 0)
                    {
                        tileType = TileType.WALL_RIGHT;
                    }
                    else if (x == room.width - 1)
                    {
                        tileType = TileType.WALL_LEFT;
                    }
                    else if (y == 0)
                    {
                        tileType = TileType.WALL_UP;
                    }
                    else if (y == room.height - 1)
                    {
                        tileType = TileType.WALL_DOWN;
                    }
                    else
                    {
                        tileType = TileType.FLOOR;
                    }

                    tileMap[room.x + x][room.y + y].SetTileType(tileType);
                }
                //Setting corners
                tileMap[room.x][room.y].SetTileType(TileType.WALL_CORNER_SHORT_RIGHT);
                tileMap[room.x + room.width - 1][room.y].SetTileType(TileType.WALL_CORNER_SHORT_LEFT);
            }
        }

        /**
         * CORRIDORS
         **/
        private void CreateCorridorsMST()
        {
            //TODO: to improve
            var g = new UndirectedGraph<int, TaggedUndirectedEdge<int, float>>();

            for (int i = 0; i < rooms.Count; i++)
            {
                for (int j = 0; j < rooms.Count; j++)
                {
                    if(i != j)
                    {
                        g.AddVerticesAndEdge(new TaggedUndirectedEdge<int, float>(i, j, rooms[i].GetDistanceToRoom(rooms[j])));
                    }
                } 
            }
           
            
            var vertices = g.Vertices.ToList();
            //Debug.Log("total vertices added " + vertices.Count);
            /*or (int i = 0; i < vertices.Count; i++)
            {
                Debug.Log(vertices[i]);

            }
            var edges = g.Edges.ToList();
            //Debug.Log("total edges added " + edges.Count);
            for (int i = 0; i < edges.Count; i++)
            {
                Debug.Log(edges[i]);

            }
            */
            var mst = g.MinimumSpanningTreePrim(e => e.Tag).ToList();

            //Debug.Log("-------------");
            for (int i = 0; i < mst.Count; i++)
            {
                //Debug.Log(mst[i]);
                Room room1 = rooms[mst[i].Source];
                Room room2 = rooms[mst[i].Target];
                UpdateCorridorTiles(room1, room2);
            }


        }

        private void CreateCorridors()
        {
            Dictionary<Room, Room> connectedRooms = new Dictionary<Room, Room>();
            Dictionary<Room, bool> selectedRooms = new Dictionary<Room, bool>();

            float minDistance;
            float distance;
            Room candidateRoom;

            for (int i = 0; i < rooms.Count; i++)
            {
                minDistance = Mathf.Infinity;
                candidateRoom = null;

                for (int j = 0; j < rooms.Count; j++)
                {
                    /*if (selectedRooms.ContainsKey(rooms[j]))
                        Debug.Log("---Previous selected: " + rooms[j].GetCenterPoint());*/
                    //continue if is the same room or if it was previously selected
                    if (!(i == j || selectedRooms.ContainsKey(rooms[j])))
                    {
                        //Debug.Log("iterate: " + i+" con1");
                        if (!(connectedRooms.ContainsKey(rooms[j]) && connectedRooms[rooms[j]] == rooms[i]))
                        {
                            //Debug.Log("iterate: " + i + " con2");
                            distance = rooms[i].GetDistanceToRoom(rooms[j]);
                            if (distance < minDistance)
                            {
                                minDistance = distance;
                                candidateRoom = rooms[j];
                            }
                        }
                    }
                }
                
                if (candidateRoom != null)
                {
                    //Debug.Log("candidate: " + candidateRoom.GetCenterPoint());
                    connectedRooms.Add(rooms[i], candidateRoom); //to delete, no es necesario guardar la info
                    selectedRooms.Add(candidateRoom, true);
                    UpdateCorridorTiles(rooms[i], candidateRoom);
                }
            }
            var keys = connectedRooms.Keys.ToList();
            var values = connectedRooms.Values.ToList();
            for (int i = 0; i<connectedRooms.Count; i++)
            {
                Debug.Log("key-> "+keys[i].GetCenterPoint()+"  value-> "+values[i].GetCenterPoint());
            } 
        }

        private void UpdateCorridorTiles(Room room1, Room room2)
        {
            //TODO BETER GENERATION DON OVERLAB WITH OTHER WALLS IN ORDER TO SPRITES BE NICE
            //TODO fix possible out of bounds bug
            int r1x = (int)room1.GetCenterPoint().x;
            int r1y = (int)room1.GetCenterPoint().y;

            int r2x = (int)room2.GetCenterPoint().x;
            int r2y = (int)room2.GetCenterPoint().y;

            while (r1x != r2x || r1y != r2y)
            {
                if (r1x != r2x && r1y != room2.y && r1y != room2.y + room2.height)
                {
                    tileMap[r1x][r1y].SetTileType(TileType.FLOOR);
                    
                    /*if (tileMap[r1x][r1y + 1].tileType == TileType.VOID)
                        tileMap[r1x][r1y + 1].SetTileType(TileType.WALL_DOWN);

                    if (tileMap[r1x][r1y - 1].tileType == TileType.VOID)
                        tileMap[r1x][r1y - 1].SetTileType(TileType.WALL_UP);

                    //comprueba el vecino de la izquierda
                    if (tileMap[r1x - 1][r1y].tileType == TileType.WALL_LEFT)
                    {
                        tileMap[r1x - 1][r1y + 1].SetTileType(TileType.WALL_DOWN);
                        tileMap[r1x - 1][r1y - 1].SetTileType(TileType.WALL_CORNER_LARGE_LEFT);
                    }

                    //comprrueba el vecino de la derecha
                    if (tileMap[r1x + 1][r1y].tileType == TileType.WALL_RIGHT)
                    {
                        tileMap[r1x + 1][r1y + 1].SetTileType(TileType.WALL_DOWN);
                        tileMap[r1x + 1][r1y - 1].SetTileType(TileType.WALL_CORNER_LARGE_RIGHT);
                    }*/

                    r1x += (r1x < r2x) ? 1 : -1;
                }
                else
                {
                    tileMap[r1x][r1y].SetTileType(TileType.FLOOR);
                    
                    /*
                    if (tileMap[r1x + 1][r1y].tileType == TileType.VOID)
                        tileMap[r1x + 1][r1y].SetTileType(TileType.WALL_LEFT);

                    if (tileMap[r1x - 1][r1y].tileType == TileType.VOID)
                        tileMap[r1x - 1][r1y].SetTileType(TileType.WALL_RIGHT);

                    if (tileMap[r1x][r1y + 1].tileType == TileType.WALL_UP)
                    {
                        tileMap[r1x + 1][r1y + 1].SetTileType(TileType.WALL_CORNER_LARGE_LEFT);
                        tileMap[r1x - 1][r1y + 1].SetTileType(TileType.WALL_CORNER_LARGE_RIGHT);
                    }*/

                    r1y += (r1y < r2y) ? 1 : -1;
                }
            }
        }

        private void AddTilesNeighbours()
        {
            List<Tile> reachableNeighbours;

            for (int x = 0; x < columns; x++)
            {
                for (int y = 0; y < rows; y++)
                {
                    if (!tileMap[x][y].IsReachable())
                        continue;

                    reachableNeighbours = new List<Tile>();

                    if(x > 0 && tileMap[x - 1][y].IsReachable()) //left
                    {
                        reachableNeighbours.Add(tileMap[x - 1][y]);
                    }
                    if(x < columns-1 && tileMap[x + 1][y].IsReachable()) //right
                    {
                        reachableNeighbours.Add(tileMap[x + 1][y]);
                    }
                    if (y > 0 && tileMap[x][y - 1].IsReachable()) //bottom
                    {
                        reachableNeighbours.Add(tileMap[x][y - 1]);
                    }
                    if (y < rows-1  && tileMap[x][y + 1].IsReachable()) //top
                    {
                        reachableNeighbours.Add(tileMap[x][y + 1]);
                    }

                    tileMap[x][y].SetReachableNeighbours(reachableNeighbours);
                    //Debug.Log("tile (" + x + "," + "y" + y + "): " + reachableNeighbours.Count);
                }
            }
        }


        private void InstantiateTiles()
        {
            GameObject toInstantiate;
            GameObject instance;

            for (int x = 0; x < columns; x++)
            {
                for (int y = 0; y < rows; y++)
                {
                    switch (tileMap[x][y].tileType)
                    {
                        case TileType.WALL_UP:
                            toInstantiate = wallUp;
                            break;
                        case TileType.WALL_DOWN:
                            toInstantiate = wallDown;
                            break;
                        case TileType.WALL_RIGHT:
                            toInstantiate = wallRight;
                            break;
                        case TileType.WALL_LEFT:
                            toInstantiate = wallLeft;
                            break;
                        case TileType.WALL_CORNER_SHORT_RIGHT:
                            toInstantiate = wallCornerShortRight;
                            break;
                        case TileType.WALL_CORNER_SHORT_LEFT:
                            toInstantiate = wallCornerShortLeft;
                            break;
                        case TileType.WALL_CORNER_LARGE_RIGHT:
                            toInstantiate = wallCornerLargeRight;
                            break;
                        case TileType.WALL_CORNER_LARGE_LEFT:
                            toInstantiate = wallCornerLargeLeft;
                            break;
                        case TileType.FLOOR:
                            toInstantiate = floor;
                            break;
                        default:
                            toInstantiate = _void;
                            break;
                    }
                    /*
                    tileType = tileMap[x][y].tileType;
                    if (tileType == TileType.FLOOR)
                    {
                        toInstantiate = floor;
                    }
                    else if (tileType == TileType.WALL)
                    {
                        toInstantiate = wall;
                    }
                    else
                    {
                        toInstantiate = ladder;
                    }*/
                    instance = Instantiate(toInstantiate, new Vector3(x, y, 0f), Quaternion.identity) as GameObject;
                    instance.transform.SetParent(boardTransform);
                }
            }
        }
    }
}


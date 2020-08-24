using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using QuickGraph;
using QuickGraph.Algorithms;

public class Board : MonoBehaviour
{

    //bidimensional array representing the board
    public Tile[][] board;

    public static int rows = 100;
    public static int columns = 100;
    
    //number of rooms to attempt to place on board
    public static int roomsToAttemp = 1;
    public int roomsMinWidth = 10;
    public int roomsMaxWidth = 20;
    public int roomsMinHeight = 10;
    public int roomsMaxHeight = 20;
    private List<Room> rooms;

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
    public GameObject player;
    public GameObject enemy;

    public GameObject item;

    //board's transform, that will hang all instanciated prefabs (tiles, objects and characters)
    private Transform boardTransform;

    public void BoardSetUp()
    {
        boardTransform = new GameObject("Board").transform;

        InitTileMap();
        CreateRooms();
        CreateCorridorsMST();
        AddTilesNeighbours();
        InstantiateTiles();

        PlaceObjects();
    }

    private void PlaceObjects()
    {
        Room room = rooms[Random.Range(0, rooms.Count)];
        Tile tile = room.GetRandomInnerTile();

        //Make player a object from hierarchy and set not destroy on load
        tile.isOccupied = true;
        Instantiate(player, new Vector3(tile.x, tile.y, 0f), Quaternion.identity);

        tile = room.GetRandomInnerTile();
        tile.isOccupied = true;
        Instantiate(enemy, new Vector3(tile.x, tile.y, 0f), Quaternion.identity);

        tile = room.GetRandomInnerTile();
        Instantiate(ladder, new Vector3(tile.x, tile.y, 0f), Quaternion.identity);

        for(int i = 0; i < 5; i++)
        {
            tile = room.GetRandomInnerTile();
            Instantiate(item, new Vector3(tile.x, tile.y, 0f), Quaternion.identity);
        }
        
    }

    /*
    private void TestPathfinding()
    {
            
        // the code that you want to measure comes here
            
            
        Tile start = tileMap[rooms[1].x+2][rooms[1].y+2];
        Tile end = tileMap[rooms[3].x+6][rooms[3].y+6];

        var watch = System.Diagnostics.Stopwatch.StartNew();
        List<Tile> path = Pathfinding.AStartPathfinding(start, end);
        watch.Stop();
        var elapsedMs = watch.ElapsedMilliseconds;
        Debug.Log("----------TIme: " + elapsedMs);
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
    */

    /// <summary>
    /// Instantiate all tiles objects in TileMap and set them VOID type.
    /// </summary>
    private void InitTileMap()
    {
        board = new Tile[columns][];
        for (int i = 0; i < board.Length; i++)
        {
            board[i] = new Tile[rows];
        }

        for (int x = 0; x < columns; x++)
        {
            for (int y = 0; y < rows; y++)
            {
                board[x][y] = new Tile(x, y, TileType.VOID);
            }
        }
    }

    #region Rooms
    /// <summary>
    /// Try to place a number of rooms on a ramdom board's random place. If there is an
    /// intersecction with a placed room, the placement it's discarted.
    /// </summary>
    private void CreateRooms()
    {
        rooms = new List<Room>();
        Room room;
        int x, y, width, height;

        for (int i = 0; i < roomsToAttemp; i++)
        {
            width = Random.Range(roomsMinWidth, roomsMaxWidth);
            height = Random.Range(roomsMinHeight, roomsMaxHeight);

            x = Random.Range(0, columns - width);
            y = Random.Range(0, rows - height);

            room = new Room(x, y, width, height);

            if (!CollidesWithBoardsRooms(room))
            {
                rooms.Add(room);
                UpdateRoomTiles(room);
            }
        }
    }

    /// <summary>
    /// Check if there are a room collision with a placed room
    /// </summary>
    private bool CollidesWithBoardsRooms(Room room)
    {
        for (int i = 0; i < rooms.Count; i++)
        {
            if (room.IsRoomCollision(rooms[i]))
                return true;
        }
        return false;
    }

    /// <summary> //SE PUEDE FUSIONAR CON BOARD SET TILES
    /// Update board's rooms' tiles with floor and wall tiles
    /// </summary>
    private void UpdateRoomTiles(Room room)
    {
        TileType tileType;
        room.innerTiles = new List<Tile>();
        room.borderTiles = new List<Tile>();

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

                board[room.x + x][room.y + y].SetTileType(tileType);

                if (tileType == TileType.FLOOR)
                {
                    room.innerTiles.Add(board[room.x + x][room.y + y]);
                }
                else
                {
                    room.borderTiles.Add(board[room.x + x][room.y + y]);
                }
                
            }
            //Setting corners
            board[room.x][room.y].SetTileType(TileType.WALL_CORNER_SHORT_RIGHT);
            board[room.x + room.width - 1][room.y].SetTileType(TileType.WALL_CORNER_SHORT_LEFT);
        }
    }

    #endregion

    #region Corridors

    /// <summary>
    /// Create corridors between rooms using Minimun Spanning Tree (MST) algorithm, in order
    /// to connect all rooms with a minimun distance between them.
    /// </summary>
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
    /*
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
                    Debug.Log("---Previous selected: " + rooms[j].GetCenterPoint());
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
    */

    /// <summary>
    /// Update board's corridors' tiles with floor and wall tiles
    /// </summary>
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
                board[r1x][r1y].SetTileType(TileType.FLOOR);
                    
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
                board[r1x][r1y].SetTileType(TileType.FLOOR);
                    
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
    #endregion

    /// <summary>
    /// For reachable tiles, sets reachables neighbours list.
    /// </summary>
    private void AddTilesNeighbours()
    {
        List<Tile> reachableNeighbours;

        for (int x = 0; x < columns; x++)
        {
            for (int y = 0; y < rows; y++)
            {
                if (!board[x][y].IsReachable())
                    continue;

                reachableNeighbours = new List<Tile>();

                if (x > 0 && board[x - 1][y].IsReachable()) //left
                    reachableNeighbours.Add(board[x - 1][y]);

                if(x < columns-1 && board[x + 1][y].IsReachable()) //right
                    reachableNeighbours.Add(board[x + 1][y]);

                if (y > 0 && board[x][y - 1].IsReachable()) //bottom
                    reachableNeighbours.Add(board[x][y - 1]);

                if (y < rows-1  && board[x][y + 1].IsReachable()) //up
                    reachableNeighbours.Add(board[x][y + 1]);

                board[x][y].SetReachableNeighbours(reachableNeighbours);
            }
        }
    }

    /// <summary>
    /// Instantiates tiles prefabs.
    /// </summary>
    private void InstantiateTiles()
    {
        GameObject toInstantiate;
        GameObject instance;

        for (int x = 0; x < columns; x++)
        {
            for (int y = 0; y < rows; y++)
            {
                switch (board[x][y].tileType)
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

                instance = Instantiate(toInstantiate, new Vector3(x, y, 0f), Quaternion.identity) as GameObject;
                instance.transform.SetParent(boardTransform);
            }
        }
    }

    public Tile GetTile(Vector3 position)
    {
        return board[(int)position.x][(int)position.y];
    }
    
    public void SetOccupiedTile(Vector3 position, bool occupied)
    {
        board[(int)position.x][(int)position.y].isOccupied = occupied;
    }
}


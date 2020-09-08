using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using QuickGraph;
using QuickGraph.Algorithms;
using UnityEngine.Tilemaps;

public class Board : MonoBehaviour
{

    public Tilemap dungeonTileMap;
    public Tilemap debugTilemap;
    public TileBase dungeonTile;
    public TileBase debugtile;
    
    //bidimensional array representing the board
    public Tile[][] board;

    public static int rows = 50;
    public static int columns = 50;
    
    //number of rooms to attempt to place on board
    public static int roomsToAttemp = 20;
    public int roomsMinWidth;
    public int roomsMaxWidth;
    public int roomsMinHeight;
    public int roomsMaxHeight;
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
    public Item[] commonItems;
    public Item[] rareItems;

    //board's transform, that will hang all instanciated prefabs (tiles, objects and characters)
    private Transform boardTransform;

    public void BoardSetUp()
    {
        boardTransform = new GameObject("Board").transform;
        dungeonTileMap = GameObject.Find("Dungeon").GetComponent<Tilemap>();
        debugTilemap = GameObject.Find("Debug").GetComponent<Tilemap>();

        InitTileMap();
        CreateRooms();
        CreateCorridorsMST();
        LoadTilesFromTileMap();
        AddRoomsTiles();
        AddTilesNeighbours();
        //InstantiateTiles();

        PlaceObjectsOnBoard();
        //DebugPlaceObjectsOnBoard();
    }

    private void LoadTilesFromTileMap()
    {
        for(int x = 0; x < columns; x++)
        {
            for(int y = 0; y < rows; y++)
            {
                Sprite sprite = dungeonTileMap.GetSprite(new Vector3Int(x, y, 0));

                if (sprite != null)
                {
                    int idx = sprite.name.LastIndexOf("_");
                    if(idx != -1)
                    {
                        string str = sprite.name.Substring(0,idx);
                        TileType tileType = (TileType)System.Enum.Parse(typeof(TileType), str, true);
                        board[x][y].SetTileType(tileType);
                    }    
                }
            }
        }
    }

    private void PlaceObjectsOnBoard()
    {
        Room room = rooms[Random.Range(0, rooms.Count)];
        Tile tile = room.GetRandomFloorTile();

        if (Player.instance == null)
        {
            Instantiate(player, tile.GetPosition(), Quaternion.identity);
            tile.isOccupied = true;
        }
        else
        {
            Player.instance.transform.position = tile.GetPosition();
            tile.isOccupied = true;
        }

        /*
        tile = room.GetRandomFloorTile();
        tile.isOccupied = true;
        Instantiate(enemy, tile.GetPosition(), Quaternion.identity);
        */
        //room = rooms[Random.Range(0, rooms.Count)];

        
        room = rooms[Random.Range(0, rooms.Count)];
        tile = room.GetRandomFloorTile();
        tile.isOccupied = true;
        Instantiate(enemy, tile.GetPosition(), Quaternion.identity);
        
        room = rooms[Random.Range(0, rooms.Count)];
        tile = room.GetRandomFloorTile();
        tile.isOccupied = true;
        Instantiate(enemy, tile.GetPosition(), Quaternion.identity);

        room = rooms[Random.Range(0, rooms.Count)];
        tile = room.GetRandomFloorTile();
        tile.isOccupied = true;
        Instantiate(enemy, tile.GetPosition(), Quaternion.identity);
        
        room = rooms[Random.Range(0, rooms.Count)];
        tile = room.GetRandomFloorTile();
        Instantiate(ladder, tile.GetPosition(), Quaternion.identity);

        ItemDataAssigner itemData = item.GetComponent<ItemDataAssigner>();
        for (int i = 0; i < 5; i++)
        {
            room = rooms[Random.Range(0, rooms.Count)];
            tile = room.GetRandomFloorTile();
            itemData.SetItem(commonItems[Random.Range(0, commonItems.Length)]);
            Instantiate(item, tile.GetPosition(), Quaternion.identity);
        }

        for (int i = 0; i < 5; i++)
        {
            room = rooms[Random.Range(0, rooms.Count)];
            tile = room.GetRandomFloorTile();
            itemData.SetItem(rareItems[Random.Range(0, rareItems.Length)]);
            Instantiate(item, tile.GetPosition(), Quaternion.identity);
        }

    }

    private void DebugPlaceObjectsOnBoard()
    {
        Room room = rooms[Random.Range(0, rooms.Count)];
        Debug.Log("floor count: " + room.floorTiles.Count);
        Tile tile = room.GetRandomFloorTile();

        if (Player.instance == null)
        {
            Instantiate(player, tile.GetPosition(), Quaternion.identity);
            tile.isOccupied = true;
        }
        else
        {
            Player.instance.transform.position = tile.GetPosition();
            tile.isOccupied = true;
        }
        
        /*
        tile = room.GetRandomFloorTile();
        tile.isOccupied = true;
        Instantiate(enemy, tile.GetPosition(), Quaternion.identity);
        */
        //room = rooms[Random.Range(0, rooms.Count)];

        tile = room.GetRandomFloorTile();
        tile.isOccupied = true;
        Instantiate(enemy, tile.GetPosition(), Quaternion.identity);
        
        tile = room.GetRandomFloorTile();
        tile.isOccupied = true;
        Instantiate(enemy, tile.GetPosition(), Quaternion.identity);

        tile = room.GetRandomFloorTile();
        tile.isOccupied = true;
        Instantiate(enemy, tile.GetPosition(), Quaternion.identity);


        tile = room.GetRandomFloorTile();
        Instantiate(ladder, tile.GetPosition(), Quaternion.identity);
        
        ItemDataAssigner itemData = item.GetComponent<ItemDataAssigner>();
        for (int i = 0; i < 3; i++)
        {
            tile = room.GetRandomFloorTile();
            itemData.SetItem(commonItems[Random.Range(0, commonItems.Length)]);
            Instantiate(item, tile.GetPosition(), Quaternion.identity);
        }

        for (int i = 0; i < 5; i++)
        {
            tile = room.GetRandomFloorTile();
            itemData.SetItem(rareItems[Random.Range(0, rareItems.Length)]);
            Instantiate(item, tile.GetPosition(), Quaternion.identity);
        }
        
    }

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
                DrawRoomTiles(room);
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

    /// Update board's rooms' tiles with floor and wall tiles
    /// </summary>
    private void DrawRoomTiles(Room room)
    {
        for (int x = 0; x < room.width; x++)
        {
            for (int y = 0; y < room.height; y++)
            {
                dungeonTileMap.SetTile(new Vector3Int(room.x + x, room.y + y, 0), dungeonTile);
            }
        }
    }

    private void AddRoomsTiles()
    {
        foreach (Room room in rooms)
        {
            room.floorTiles = new List<Tile>();

            for (int x = 0; x < room.width; x++)
            {
                for (int y = 0; y < room.height; y++)
                {
                    //add floor tiles
                    if(board[room.x + x][room.y + y].tileType == TileType.FLOOR)
                    {
                        room.floorTiles.Add(board[room.x + x][room.y + y]);
                    }
                }
            }
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

        for (int i = 0; i < mst.Count; i++)
        {
            Room room1 = rooms[mst[i].Source];
            Room room2 = rooms[mst[i].Target];
            DrawCorridorTiles(room1, room2);
        }
    }

    private void DrawCorridorTiles(Room room1, Room room2)
    {
        int r1x = Mathf.RoundToInt(room1.GetCenterPoint().x);
        int r1y = Mathf.RoundToInt(room1.GetCenterPoint().y);

        int r2x = Mathf.RoundToInt(room2.GetCenterPoint().x);
        int r2y = Mathf.RoundToInt(room2.GetCenterPoint().y);

        while (r1x != r2x || r1y != r2y) //while not reached other room center
        {
            if (r1x != r2x) //move horizontally
            {
                r1x += (r1x < r2x) ? 1 : -1;
            }
            else
            {
                r1y += (r1y < r2y) ? 1 : -1;
            }
            Vector3Int currentPosition = new Vector3Int(r1x, r1y, 0);
            dungeonTileMap.SetTile(currentPosition, dungeonTile);
            DrawAdjacentPositions(currentPosition);
        }
    }

    private void DrawAdjacentPositions(Vector3Int position)
    {
        dungeonTileMap.SetTile(new Vector3Int(position.x - 1, position.y + 1, 0), dungeonTile); //up-left
        dungeonTileMap.SetTile(new Vector3Int(position.x, position.y + 1, 0), dungeonTile); //up
        dungeonTileMap.SetTile(new Vector3Int(position.x + 1, position.y + 1, 0), dungeonTile); //up-right
        dungeonTileMap.SetTile(new Vector3Int(position.x - 1, position.y, 0), dungeonTile); //left
        dungeonTileMap.SetTile(new Vector3Int(position.x, position.y, 0), dungeonTile); //center
        dungeonTileMap.SetTile(new Vector3Int(position.x + 1, position.y, 0), dungeonTile); //right
        dungeonTileMap.SetTile(new Vector3Int(position.x - 1, position.y - 1, 0), dungeonTile); //down-left
        dungeonTileMap.SetTile(new Vector3Int(position.x, position.y - 1, 0), dungeonTile); //down
        dungeonTileMap.SetTile(new Vector3Int(position.x + 1, position.y - 1, 0), dungeonTile); //down-right
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

                if (x > 0 && board[x - 1][y].IsReachable()) //left neighbour
                    reachableNeighbours.Add(board[x - 1][y]);

                if(x < columns-1 && board[x + 1][y].IsReachable()) //right neighbour
                    reachableNeighbours.Add(board[x + 1][y]);

                if (y > 0 && board[x][y - 1].IsReachable()) //bottom neighbour
                    reachableNeighbours.Add(board[x][y - 1]);

                if (y < rows-1  && board[x][y + 1].IsReachable()) //up neighbour
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
        int x = Mathf.RoundToInt(position.x);
        int y = Mathf.RoundToInt(position.y);
        return board[x][y];
    }
    
    public void SetOccupiedTile(Vector3 position, bool occupied)
    {
        int x = Mathf.RoundToInt(position.x);
        int y = Mathf.RoundToInt(position.y);
        board[x][y].isOccupied = occupied;
        /*if (occupied)
        {
            debugTilemap.SetTile(new Vector3Int((int)position.x, (int)position.y, 0), debugtile);
        }
        else
        {
            debugTilemap.SetTile(new Vector3Int((int)position.x, (int)position.y, 0), null);
        }*/
    }
}


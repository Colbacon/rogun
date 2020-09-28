using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using QuickGraph;
using QuickGraph.Algorithms;
using UnityEngine.Tilemaps;

public class Board : MonoBehaviour
{

    public Tilemap dungeonTileMap;
    public Tilemap dungeonObjectsTileMap;
    public Tilemap debugTilemap;
    public TileBase dungeonTile;
    public TileBase debugTile;

    public TileBase[] downWallElements;
    public TileBase[] sideWallElements;
    public TileBase[] floorElements;
    public TileBase probeTile;
    public TileBase probeAnimTile;
    
    //bidimensional array representing the board
    public _Tile[][] board;

    public static int rows = 50;
    public static int columns = 50;
    
    //number of rooms to attempt to place on board
    public static int roomsToAttemp = 3;
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

    public GameObject pawn;
    public GameObject patroller;
    public GameObject chaser;

    public GameObject item;
    public Item[] commonItems;
    public Item[] rareItems;

    //board's transform, that will hang all instanciated prefabs (tiles, objects and characters)
    private Transform boardTransform;
    private List<TaggedUndirectedEdge<int, float>> corridors;

    public void BoardSetUp()
    {
        boardTransform = new GameObject("Board").transform;
        dungeonTileMap = GameObject.Find("Dungeon").GetComponent<Tilemap>();
        dungeonObjectsTileMap = GameObject.Find("Dungeon_objects").GetComponent<Tilemap>();
        debugTilemap = GameObject.Find("Debug").GetComponent<Tilemap>();

        InitTileMap();
        CreateRooms();
        CreateCorridorsMST();
        CreateRoomsInnerWalls();
        LoadTilesFromTileMap();
        AddRoomsTiles();
        PlaceObjectsOnBoard();
        

        AddTilesNeighbours();
        PlaceUnitsAndItemsOnBoard();

    }


    private void PlaceObjectsOnBoard()
    {
        int maxObjects;
        TileBase objects;

        foreach(Room room in rooms)
        {
            maxObjects = Mathf.RoundToInt(room.rightWallTiles.Count * 0.2f);
            for (int i = 0; i < maxObjects; i++)
            {
                _Tile tile = room.GetRandomRightWallTile();
                objects = sideWallElements[Random.Range(0, sideWallElements.Count())];
                PlaceObjectTile(dungeonObjectsTileMap, objects, tile.GetPosition() + Vector3Int.right, false, false);
            }

            maxObjects = Mathf.RoundToInt(room.leftWallTiles.Count * 0.2f);
            for (int i = 0; i < maxObjects; i++)
            {
                _Tile tile = room.GetRandomLeftWallTile();
                objects = sideWallElements[Random.Range(0, sideWallElements.Count())];
                PlaceObjectTile(dungeonObjectsTileMap, objects, tile.GetPosition() + Vector3Int.left, true, false);
            }

            maxObjects = Mathf.RoundToInt(room.downWallTiles.Count * 0.2f);
            for (int i = 0; i < maxObjects; i++)
            {
                _Tile tile = room.GetRandomDownWallTile();
                objects = downWallElements[Random.Range(0, downWallElements.Count())];
                PlaceObjectTile(dungeonObjectsTileMap, objects, tile.GetPosition(), false, false);
            }


            maxObjects = Mathf.RoundToInt(room.floorTiles.Count * 0.04f);
            for (int i = 0; i < maxObjects; i++)
            {
                _Tile tile = room.GetRandomFloorTile();
                objects = floorElements[Random.Range(0, floorElements.Count())];

                bool obstacle = IsObstacle(objects);
                //UnityEngine.Tilemaps.Tile til2 = (Tile)objects;
                //Debug.Log(dungeonObjectsTileMap.GetColliderType(tile.GetPosition()) + ": " + tile.GetPosition());
                //Debug.Log("coll type: "+ til2.colliderType);

                if (obstacle)
                    debugTilemap.SetTile(tile.GetPosition(), debugTile);
                PlaceObjectTile(dungeonObjectsTileMap, objects, tile.GetPosition(), false, IsObstacle(objects));
            }
        }
        //redrawing dungeonObjectsTilemap to null on corridors, avoiding objects obstacilize room entrance
        for (int i = 0; i < corridors.Count; i++)
        {
            Room room1 = rooms[corridors[i].Source];
            Room room2 = rooms[corridors[i].Target];
            DrawCorridorTiles(dungeonObjectsTileMap, null, room1, room2, false);
        }
    }

    private bool IsObstacle(TileBase tile)
    {
        bool obstacle = false;

        System.Type type = tile.GetType();
        if (type.Equals(typeof(Tile)))
        {
            Tile auxTile = (Tile)tile;
            obstacle = auxTile.colliderType.Equals(Tile.ColliderType.Sprite);
        }
        if (type.Equals(typeof(AnimatedTile)))
        {
            AnimatedTile auxTile = (AnimatedTile)tile;
            obstacle = auxTile.m_TileColliderType.Equals(Tile.ColliderType.Sprite);
        }

        return obstacle;
    }
    
    private void PlaceUnitsAndItemsOnBoard()
    {

        if (Player.instance == null)
        {
            InstantiateGameObject(player);
        }
        else
        {
            Room room = rooms[Random.Range(0, rooms.Count)];
            _Tile tile = room.GetRandomFloorTile();
            while (tile.IsIsolated())
            {
                tile = room.GetRandomFloorTile();
            }
            Player.instance.transform.position = tile.GetPosition();
        }


        //InstantiateGameObject(patroller);
        InstantiateGameObject(chaser);
        InstantiateGameObject(patroller);


        InstantiateGameObject(ladder);

        ItemDataAssigner itemData = item.GetComponent<ItemDataAssigner>();
        for (int i = 0; i < 5; i++)
        {
            Room room = rooms[Random.Range(0, rooms.Count)];
            _Tile tile = room.GetRandomFloorTile();
            itemData.SetItem(commonItems[Random.Range(0, commonItems.Length)]);
            Instantiate(item, tile.GetPosition(), Quaternion.identity);
        }

        for (int i = 0; i < 5; i++)
        {
            Room room = rooms[Random.Range(0, rooms.Count)];
            _Tile tile = room.GetRandomFloorTile();
            itemData.SetItem(rareItems[Random.Range(0, rareItems.Length)]);
            Instantiate(item, tile.GetPosition(), Quaternion.identity);
        }

    }

    private void InstantiateGameObject(GameObject gameObject)
    {
        Room room = rooms[Random.Range(0, rooms.Count)];
        _Tile tile = room.GetRandomFloorTile();
        while (tile.IsIsolated())
        {
            tile = room.GetRandomFloorTile();
        }
        Instantiate(gameObject, tile.GetPosition(), Quaternion.identity);
        //gameObject.transform.SetParent(boardTransform);
    }

    /// <summary>
    /// Instantiate all tiles objects in TileMap and set them VOID type.
    /// </summary>
    private void InitTileMap()
    {
        board = new _Tile[columns][];
        for (int i = 0; i < board.Length; i++)
        {
            board[i] = new _Tile[rows];
        }

        for (int x = 0; x < columns; x++)
        {
            for (int y = 0; y < rows; y++)
            {
                board[x][y] = new _Tile(x, y, TileType.VOID);
            }
        }
    }

    private void LoadTilesFromTileMap()
    {
        for (int x = 0; x < columns; x++)
        {
            for (int y = 0; y < rows; y++)
            {
                Sprite sprite = dungeonTileMap.GetSprite(new Vector3Int(x, y, 0));

                if (sprite != null)
                {
                    int idx = sprite.name.LastIndexOf("_");
                    if (idx != -1)
                    {
                        string str = sprite.name.Substring(0, idx);
                        TileType tileType = (TileType)System.Enum.Parse(typeof(TileType), str, true);
                        board[x][y].SetTileType(tileType);
                    }
                }
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

    private void CreateRoomsInnerWalls()
    {
        int num;
        Vector3Int pos;

        foreach (Room room in rooms)
        {
            pos = room.GetCenterPoint();
            num = Random.Range(0, 11);
            //Debug.Log("pos -> " + pos + "  num-> " + num );

            switch (num)
            {
                case 0: //one
                    dungeonTileMap.SetTile(pos, null);
                    break;
                case 1: //two horizontal
                    dungeonTileMap.SetTile(pos, null);
                    dungeonTileMap.SetTile(pos + Vector3Int.right, null);
                    break;
                case 2: //two vertical
                    dungeonTileMap.SetTile(pos, null);
                    dungeonTileMap.SetTile(pos + Vector3Int.up, null);
                    break;
                case 3: //two diagonal 0
                    dungeonTileMap.SetTile(pos, null);
                    dungeonTileMap.SetTile(pos + Vector3Int.up + Vector3Int.right, null);
                    break;
                case 4: //two diagonal 1
                    dungeonTileMap.SetTile(pos + Vector3Int.right, null);
                    dungeonTileMap.SetTile(pos + Vector3Int.up, null);
                    break;
                case 5: //three 0
                    dungeonTileMap.SetTile(pos, null);
                    dungeonTileMap.SetTile(pos + Vector3Int.up, null);
                    dungeonTileMap.SetTile(pos + Vector3Int.right, null);
                    break;
                case 6: //three 1
                    dungeonTileMap.SetTile(pos, null);
                    dungeonTileMap.SetTile(pos + Vector3Int.up, null);
                    dungeonTileMap.SetTile(pos + Vector3Int.up + Vector3Int.right, null);
                    break;
                case 7: //three 2
                    dungeonTileMap.SetTile(pos, null);
                    dungeonTileMap.SetTile(pos + Vector3Int.right, null);
                    dungeonTileMap.SetTile(pos + Vector3Int.up + Vector3Int.right, null);
                    break;
                case 8: //three
                    dungeonTileMap.SetTile(pos + Vector3Int.up, null);
                    dungeonTileMap.SetTile(pos + Vector3Int.up + Vector3Int.right, null);
                    dungeonTileMap.SetTile(pos + Vector3Int.right, null);
                    break;
                case 9: //four
                    dungeonTileMap.SetTile(pos, null);
                    dungeonTileMap.SetTile(pos + Vector3Int.up, null);
                    dungeonTileMap.SetTile(pos + Vector3Int.right, null);
                    dungeonTileMap.SetTile(pos + Vector3Int.up + Vector3Int.right, null);
                    break;
                default: //none
                    break;
            }
        }
    }

    private void AddRoomsTiles()
    {
        _Tile tile;

        foreach (Room room in rooms)
        {
            for (int x = 0; x < room.width; x++)
            {
                for (int y = 0; y < room.height; y++)
                {
                    tile = board[room.x + x][room.y + y];
                    switch (tile.tileType)
                    {
                        case TileType.FLOOR:
                            room.floorTiles.Add(tile);
                            break;
                        case TileType.WALL_UP:
                            room.upWallTiles.Add(tile);
                            break;
                        case TileType.WALL_DOWN:
                            room.downWallTiles.Add(tile);
                            break;
                        case TileType.WALL_LEFT:
                            room.leftWallTiles.Add(tile);
                            break;
                        case TileType.WALL_RIGHT:
                            room.rightWallTiles.Add(tile);
                            break;
                    }
                }
            }
        }
    }
    #endregion

    #region Corridors

    /// <summary>
    /// Create corridors between rooms using Minimun Spanning Tree (MST) algorithm, in order
    /// to connect all rooms with a minimun distance between them. Futhermore, add extra
    /// corridors corresponding to 30% of total rooms.
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

        corridors = g.MinimumSpanningTreePrim(e => e.Tag).ToList();

        var edges = g.Edges.ToList();
        TaggedUndirectedEdge<int, float> edge;
        int extraEdges = Mathf.CeilToInt(rooms.Count * 0.30f);
        int count = 0;
        
        while(count <= extraEdges && edges.Count > 0)
        {
            edge = edges[Random.Range(0, edges.Count)];
            if (!corridors.Contains(edge))
            {
                corridors.Add(edge);
                count++;
            }
            edges.Remove(edge);
        }
        
        for (int i = 0; i < corridors.Count; i++)
        {            
            Room room1 = rooms[corridors[i].Source];
            Room room2 = rooms[corridors[i].Target];
            DrawCorridorTiles(dungeonTileMap, dungeonTile, room1, room2);
        }
    }

    private void DrawCorridorTiles(Tilemap tilemap, TileBase tile, Room room1, Room room2, bool drawAdjancentTiles = true)
    {
        int r1x = room1.GetCenterPoint().x;
        int r1y = room1.GetCenterPoint().y;

        int r2x = room2.GetCenterPoint().x;
        int r2y = room2.GetCenterPoint().y;

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
            tilemap.SetTile(currentPosition, tile);
            //temporal, hacerlo bien
            GetTile(currentPosition).SetTileType(TileType.FLOOR);
            debugTilemap.SetTile(currentPosition, null);
            if(drawAdjancentTiles)
                DrawAdjacentPositions(tilemap, tile, currentPosition);
        }
    }

    private void DrawAdjacentPositions(Tilemap tilemap, TileBase tile, Vector3Int position)
    {
        for(int x = position.x - 1; x <= position.x + 1; x++)
        {
            for (int y = position.y - 1; y <= position.y + 1; y++)
            {
                tilemap.SetTile(new Vector3Int(x, y, 0), tile);
            }
        }
    }
    #endregion


    /// <summary>
    /// For reachable tiles, sets reachables neighbours list.
    /// </summary>
    private void AddTilesNeighbours()
    {
        List<_Tile> reachableNeighbours;

        for (int x = 0; x < columns; x++)
        {
            for (int y = 0; y < rows; y++)
            {
                if (!board[x][y].IsReachable())
                    continue;

                reachableNeighbours = new List<_Tile>();

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

    private void PlaceObjectTile(Tilemap tileMap, TileBase tile, Vector3Int position, bool flip, bool obstacle)
    {
        tileMap.SetTile(position, tile);
        if (flip)
        {
            UpdateTileTransform(tileMap, position, 0f, true);
        }      
        if (obstacle)
        {
            GetTile(position).SetTileType(TileType.OBSTACLE);
            tileMap.SetColliderType(position, Tile.ColliderType.Sprite);
        }
    }

    private void UpdateTileTransform(Tilemap tileMap, Vector3 position, float rotation, bool flip)
    {
        Vector3 scaling = !flip ? Vector3.one : new Vector3(-1, 1, 1);
        Matrix4x4 matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0f, 0f, rotation), scaling);
        tileMap.SetTransformMatrix(Vector3Int.RoundToInt(position), matrix);
    }

    public _Tile GetTile(Vector3 position)
    {
        int x = Mathf.RoundToInt(position.x);
        int y = Mathf.RoundToInt(position.y);
        return board[x][y];
    }

    //TODO: control if there is no more floortiles in room
    public _Tile GetRandomFloorTile()
    {
        Room room = rooms[Random.Range(0, rooms.Count)];
        Debug.LogWarning(rooms.Count + " " + room);
        return room.GetRandomFloorTile();
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


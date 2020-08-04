using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MapGeneration
{
    public class Board : MonoBehaviour
    {

        public static int rows = 50;
        public static int columns = 50;

        public Tile[][] tileMap;

        private List<Room> rooms;
        public static int roomsToAttemp = 10;

        public GameObject floor;
        public GameObject wall;
        public GameObject ladder;

        private Transform boardTransform;

        void Awake()
        {
            boardTransform = new GameObject("Board").transform;

            InitTileMap();
            CreateRooms();
            //CreateCorridors();
            //AddTilesNeighbours();
            InstantiateTiles();
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
                width = Random.Range(4, 10);
                height = Random.Range(4, 10);

                x = Random.Range(0, columns - width);
                y = Random.Range(0, rows - height);

                room = new Room(x, y, width, height);
                Debug.Log("Trying room " + i);
                if (!CollidesWithBoardsRooms(room))
                {
                    Debug.Log("Success room " + i);
                    rooms.Add(room);
                    UpdateRoomTiles(room);
                }
            }
        }

        private bool CollidesWithBoardsRooms(Room room)
        {
            for (int i = 0; i < rooms.Count; i++)
            {
                if (room.IsRoomCollision(room))
                    return true;
            }

            return false;
        }

        private void UpdateRoomTiles(Room room)
        {
            for (int x = 0; x < room.width; x++)
            {
                for (int y = 0; y < room.height; y++)
                {
                    if (x == 0 || x == room.width || y == 0 || y == room.height)
                    {
                        //tileMap[room.x + x][room.y + y].SetTileType(TileType.FLOOR);
                    }
                    else
                    {
                        tileMap[room.x + x][room.y + y].SetTileType(TileType.FLOOR);
                    }
                }
            }
        }

        /**
         * CORRIDORS
         **/
        private void GenerateCorridors()
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
                    //continue if is the same room or if it was previously selected
                    if (i == j || selectedRooms.ContainsKey(rooms[i]))
                        continue;

                    distance = rooms[i].GetDistanceToRoom(rooms[j]);
                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        candidateRoom = rooms[j];
                    }
                }

                if (candidateRoom != null)
                {
                    connectedRooms.Add(rooms[i], candidateRoom);
                    selectedRooms.Add(candidateRoom, true);
                    UpdateCorridorTiles(rooms[i], candidateRoom);
                }
            }
        }

        private void UpdateCorridorTiles(Room room1, Room room2)
        {
            int r1x = (int)room1.GetCenterPoint().x;
            int r1y = (int)room1.GetCenterPoint().y;
            int r2x = (int)room2.GetCenterPoint().x;
            int r2y = (int)room2.GetCenterPoint().y;

            while (r1x != r2x || r1y != r2y)
            {
                if (r1x != r2x)
                {
                    //TODO update tiles
                    r1x += r1x < r2x ? 1 : -1;
                }
                else
                {
                    //TODO update tiles
                    r1y += r1y < r2y ? 1 : -1;
                }

                //TODO change surrounding tiles from void to grass
            }
        }

        private void InstantiateTiles()
        {
            TileType tileType;
            GameObject toInstantiate;
            GameObject instance;

            for (int x = 0; x < columns; x++)
            {
                for (int y = 0; y < rows; y++)
                {
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
                    }
                    instance = Instantiate(toInstantiate, new Vector3(x, y, 0f), Quaternion.identity) as GameObject;
                    instance.transform.SetParent(boardTransform);
                }
            }
        }
    }
}


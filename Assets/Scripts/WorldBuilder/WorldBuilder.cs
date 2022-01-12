using Assets.Scripts.CharacterControl;
using Assets.Scripts.Data.World;
using Assets.Scripts.Utility;
using Assets.Scripts.WorldBuilder.Bot;
using Assets.Scripts.WorldBuilder.RoomBuilder;
using Assets.Scripts.WorldBuilder.WorldManager;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.WorldBuilder
{
    public class WorldBuilder : MonoBehaviour
    {
        [SerializeField] public GameObject playerCharacter; // used to assign enemies an initial target
        [SerializeField] public GameObject roomsParent;
        [Space(10)]
        [Header("Tile sets")]
        [Space(10)]
        [SerializeField] public GameObject rock1TilePalette;

        [Space(10)]
        [Header("Enemies")]
        [Space(10)]
        [SerializeField] public GameObject floatingBot;


        private RoomManager[] _rooms;
        private List<DoorManager> _doors;


        void Awake()
        {
            BuildWorld();
            Globals.isWorldBuilt = true;
        }

        public DoorPlacement GetDoorAtPosition(Vector2 globalPosition)
        {
            foreach (DoorPlacement d in Globals.world.doorPlacements)
            {
                if (d.positionInGlobalSpace == globalPosition) return d;
            }
            return null;
        }
        public (Vector2 upperLeft, Vector2 lowerRight) GetRoomDimensions(int roomId)
        {
            try
            {
                var room = _rooms.Where(x => x.id == roomId).FirstOrDefault();
                Vector2 upperLeft = room.upperLeftInGlobalSpace;
                Vector2 lowerRight = room.lowerRightInGlobalSpace;
                return (upperLeft, lowerRight);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public void ToggleActiveRooms()
        {
            List<int> activeRooms = new List<int>();
            activeRooms.Add(Globals.currentRoom);
            activeRooms = RoomAndTileHelper.GetRoomConnections(Globals.currentRoom, 0);

            foreach (RoomManager r in _rooms)
            {
                if (activeRooms.Contains(r.id)) r.ActivateSelf();
                else r.DeActivateSelf();

                if (r.id == Globals.currentRoom) r.ActivateMasks();
                else r.DeActivateMasks();
            }
            foreach (DoorManager d in _doors)
            {
                if (activeRooms.Contains(d.doorPlacement.room1Id) ||
                    activeRooms.Contains(d.doorPlacement.room2Id)) d.ActivateSelf();
                else d.DeActivateSelf();
            }
        }

        private void BuildWorld()
        {
            CreatorBot bot = new CreatorBot();
            bot.CreateWorld(WorldSizeValues.sizes[(int)WorldSizes.SMALL], 78446);

            CreateRoomManagersFromBluePrint();
            CreateDoorManagersFromBluePrint();
        }
        //private void BuildStarterRoom()
        //{
        //    _roomSaves = RoomBuilderHelper.GetAllRoomPlacements();
        //    RoomPlacement starterRoomPlacement = _roomSaves.Where(x => x.roomName == "Cave Exit West").FirstOrDefault();
        //    RoomPlacement secondRoomPlacement = _roomSaves.Where(x => x.roomName == "Psionics Chamber").FirstOrDefault();

        //    Vector2 startingPointForRooms = new Vector2(-5.0f, 5.0F);

        //    GameObject room000 = Instantiate(new GameObject("Room000"), roomsParent.transform, false);
        //    RoomManager room0 = new RoomManager(0, rock1TilePalette, starterRoomPlacement, startingPointForRooms, room000);
        //    _rooms[0] = room0;

        //    // room000 door right is in row 6
        //    // room001 door left is in row 14
        //    // so need to start room001 14 rows above room000's right door
        //    int room000DoorRightRow = starterRoomPlacement.doors.
        //        Where(x => x.position.column == starterRoomPlacement.roomWidth - 1).First().position.row;
        //    int room001DoorLeftRow = secondRoomPlacement.doors.
        //        Where(x => x.position.column == - 1).First().position.row;
        //    float room001YPosition = startingPointForRooms.y - room000DoorRightRow + room001DoorLeftRow;
        //    Vector2 startingPointForRoom001 = new Vector2(
        //        startingPointForRooms.x + room0.roomWidthInTiles + 2,
        //        room001YPosition
        //        );
        //    GameObject room001 = Instantiate(new GameObject("Room001"), roomsParent.transform, false);
        //    RoomManager room1 = new RoomManager(1, rock1TilePalette, secondRoomPlacement, startingPointForRoom001, room001);
        //    _rooms[1] = room1;


        //    // draw the door between them
        //    Transform doorTransform = rock1TilePalette.transform.Find("Door_origin");
        //    ConnectRoomsWithDoor(room0, room1, doorTransform.gameObject, 1.0f);
        //    room0.DrawSelf();
        //    room1.DrawSelf();


        //    //room0.AddPerimiterTiles();
        //    //// tile above the floor = -7.68
        //    //room0.AddPlatformTiles(new Vector2(-1f, -4.0f), 4, 5);
        //    //room0.AddPlatformTiles(new Vector2(5f, -5f), 12, 1);
        //    //room0.AddPlatformTiles(new Vector2(5f, -1f), 5, 1);
        //    //room0.AddPlatformTiles(new Vector2(12.0f, -1f), 5, 1);
        //    //room0.AddPlatformTiles(new Vector2(5f, 3f), 12, 1);
        //    //room0.AddPlatformTiles(new Vector2(20f, 0f), 2, 7);
        //    //room0.AddPlatformTiles(new Vector2(24f, 1f), 1, 10);
        //    //room0.AddPlatformTiles(new Vector2(26f, 0f), 1, 9);
        //    //room0.AddPlatformTiles(new Vector2(28f, -1f), 1, 8);
        //    //room0.AddPlatformTiles(new Vector2(30f, -2f), 1, 7);
        //    //room0.AddStartingEnemy(floatingBot, new Vector2(26f, -4f), playerCharacter);
        //    //room0.AddStartingEnemy(floatingBot, new Vector2(28f, 0f), playerCharacter);
        //    //room0.AddStartingEnemy(floatingBot, new Vector2(30f, 0f), playerCharacter);
        //    //room0.AddStartingEnemy(floatingBot, new Vector2(32f, 0f), playerCharacter);
        //    //room0.AddStartingEnemy(floatingBot, new Vector2(34f, 0f), playerCharacter);
        //    //room0.AddStartingEnemy(floatingBot, new Vector2(36f, 0f), playerCharacter);
        //    //room0.AddStartingEnemy(floatingBot, new Vector2(26f, -5f), playerCharacter);
        //    //room0.AddStartingEnemy(floatingBot, new Vector2(28f, 1f), playerCharacter);
        //    //room0.AddStartingEnemy(floatingBot, new Vector2(30f, 1f), playerCharacter);
        //    //room0.AddStartingEnemy(floatingBot, new Vector2(32f, 1f), playerCharacter);
        //    //room0.AddStartingEnemy(floatingBot, new Vector2(34f, 1f), playerCharacter);
        //    //room0.AddStartingEnemy(floatingBot, new Vector2(36f, 1f), playerCharacter);


        //    //

        //    //Room room1 = new Room(_tileSetRock1, 20, 20, new Vector2(34.0f, 10.0F), room001);
        //    //_rooms[1] = room1;
        //    //room1.AddPerimiterTiles();
        //    //room1.AddPlatformTiles(new Vector2(38f, 6f), 1, 11);
        //    //room1.AddStartingEnemy(floatingBot, new Vector2(40f, 0f), playerCharacter);
        //    //room1.AddStartingEnemy(floatingBot, new Vector2(42f, 0f), playerCharacter);
        //    //room1.AddStartingEnemy(floatingBot, new Vector2(44f, 0f), playerCharacter);
        //    //room1.AddStartingEnemy(floatingBot, new Vector2(46f, 0f), playerCharacter);
        //    //room1.AddStartingEnemy(floatingBot, new Vector2(48f, 0f), playerCharacter);

        //    //SealTheMap();

        //}
        //private void ConnectRoomsWithDoor(RoomManager roomLeft, RoomManager roomRight, GameObject prefab, 
        //    float heightFromLeftFloorInTiles)
        //{

        //    float posX = roomLeft.lowerRightInGlobalSpace.x - 1;
        //    float posY = roomLeft.lowerRightInGlobalSpace.y + heightFromLeftFloorInTiles + Globals.doorVHeightInTiles;
        //    // now knock out perimiter blocks in each room
        //    for (int i = 0; i < Globals.doorVHeightInTiles; i++)
        //    {
        //        roomLeft.KnockOutTile(new Vector2(posX, posY - i));
        //        roomRight.KnockOutTile(new Vector2(posX + 1, posY - i));
        //    }


        //    Vector3 position = new Vector2(posX, posY);
        //    Quaternion rotation = new Quaternion(0, 0, 0, 0);
        //    GameObject gameObject = Instantiate(prefab, position, rotation, roomsParent.transform);

        //    _doors.Add(new DoorManager(gameObject)
        //    {
        //        room1Id = roomLeft.id,
        //        room2Id = roomRight.id,
        //        positionInGlobalSpace = position
        //    });
        //}
        private void CreateDoorManagersFromBluePrint()
        {
            _doors = new List<DoorManager>();
            foreach (DoorPlacement d in Globals.world.doorPlacements)
            {
                Transform doorTransform = rock1TilePalette.transform.Find("Door_origin");
                if (d.isHorizontal) doorTransform = rock1TilePalette.transform.Find("HorizontalDoor_origin");
                Quaternion rotation = new Quaternion(0, 0, 0, 0);
                GameObject gameobject = Instantiate(
                    doorTransform.gameObject, d.positionInGlobalSpace, rotation, roomsParent.transform);

                DoorManager doorManager = new DoorManager(gameobject);
                doorManager.doorPlacement = d;
                _doors.Add(doorManager);

                doorManager.DeActivateSelf();
            }
        }
        private void CreateRoomManagersFromBluePrint()
        {
            int howManyRooms = Globals.world.rooms.Count;
            _rooms = new RoomManager[howManyRooms];

            for (int i = 0; i < howManyRooms; i++)
            {
                RoomPlacement blueprint = Globals.world.rooms[i];

                string unityRoomName = string.Format("Room{0}", blueprint.id);
                GameObject roomGameObject = Instantiate(new GameObject(unityRoomName), roomsParent.transform, false);
                RoomManager room = new RoomManager(blueprint.id, rock1TilePalette, blueprint, roomGameObject);
                _rooms[i] = room;
                room.DrawSelf();
                room.DeActivateSelf();
            }
        }
        private bool IsDoorTileAtPosition(Vector2 position)
        {
            foreach (DoorPlacement d in Globals.world.doorPlacements)
            {
                for (int i = 0; i < Globals.doorVHeightInTiles; i++)
                {
                    for (int i2 = 0; i2 < Globals.doorHWidthInTiles; i2++)
                    {
                        Vector2 trialPosition = new Vector2(d.positionInGlobalSpace.x + i2, d.positionInGlobalSpace.y - i);
                        if (trialPosition == position) return true;
                    }
                }
            }
            return false;
        }
        private void SealTheMap()
        {

            // todo: seal the map in a teleporter that bring the player character back to start

            //    int minX = 0;
            //    int maxX = 0;
            //    int minY = 0;
            //    int maxY = 0;

            //    // find the dimensions of all the rooms together
            //    foreach(Room r in _rooms)
            //    {
            //        if (r.upperLeftInGlobalSpace.x < minX) minX = (int)Mathf.Floor(r.upperLeftInGlobalSpace.x);
            //        if (r.lowerRightInGlobalSpace.x > maxX) maxX = (int)Mathf.Floor(r.lowerRightInGlobalSpace.x) + 1;
            //        if (r.lowerRightInGlobalSpace.y < minY) minY = (int)Mathf.Floor(r.lowerRightInGlobalSpace.y);
            //        if (r.upperLeftInGlobalSpace.x > maxY) maxY = (int)Mathf.Floor(r.upperLeftInGlobalSpace.y) + 1;
            //    }

            //    // now pad by 5
            //    minX -= 5;
            //    maxX += 5;
            //    minY -= 5;
            //    maxY += 5;

            //    // now add blocks if they're not inside of a room
            //    for(int i = minY; i <= maxY; i++)
            //    {
            //        for (int i2 = minX; i2 <= maxX; i2++)
            //        {
            //            Vector2 position = new Vector2(i2, i);
            //            if(WhichRoomAreWeIn(position) == -1)
            //            {
            //                if(!IsDoorTileAtPosition(position))
            //                {
            //                    Transform prefabTransform = rock1TileSet.transform.Find("1_origin");
            //                    GameObject prefab = prefabTransform.gameObject;
            //                    Quaternion rotation = new Quaternion(0, 0, 0, 0);
            //                    UnityEngine.Object.Instantiate(prefab, position, rotation, roomsParent.transform);
            //                }
            //            }
            //        }
            //    }

        }
    }
}

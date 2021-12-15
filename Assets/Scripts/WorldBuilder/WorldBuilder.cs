using Assets.Scripts.CharacterControl;
using System;
using System.Collections.Generic;
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
        [SerializeField] public GameObject rock1TileSet;
        //[SerializeField] public GameObject rock1Base;
        //[SerializeField] public GameObject rock1Top;
        //[SerializeField] public GameObject rock1Bottom;
        //[SerializeField] public GameObject rock1Left;
        //[SerializeField] public GameObject rock1Right;
        //[SerializeField] public GameObject rock1CornerUpLeft;
        //[SerializeField] public GameObject rock1CornerUpRight;
        //[SerializeField] public GameObject rock1CornerBottomLeft;
        //[SerializeField] public GameObject rock1CornerBottomRight;
        //[SerializeField] public GameObject rock1EndCapUp;
        //[SerializeField] public GameObject rock1EndCapDown;
        //[SerializeField] public GameObject rock1EndCapLeft;
        //[SerializeField] public GameObject rock1EndCapRight;
        //[SerializeField] public GameObject rock1SingleWide;
        //[SerializeField] public GameObject rock1SingleTall;
        [SerializeField] public GameObject rock1Door;
        [Space(10)]
        [Header("Enemies")]
        [Space(10)]
        [SerializeField] public GameObject floatingBot;
        private Room[] _rooms;
        private List<Door> _doors;

        private TileSet _tileSetRock1; 

        public Door GetDoorAtPosition(Vector2 globalPosition)
        {
            foreach(Door d in _doors)
            {
                if (d.positionInGlobalSpace == globalPosition) return d;             
            }
            return null;
        }
        public int WhichRoomAreWeIn(Vector2 currentLocation)
        {
            for(int i = 0; i < _rooms.Length; i++)
            {
                if(_rooms[i].upperLeftInGlobalSpace.x < currentLocation.x
                    && _rooms[i].lowerRightInGlobalSpace.x > currentLocation.x
                    && _rooms[i].upperLeftInGlobalSpace.y > currentLocation.y
                    && _rooms[i].lowerRightInGlobalSpace.y < currentLocation.y)
                {
                    return i;
                }
            }
            return 0;
        }
        void Awake()
        {
            int howManyRooms = 2;
            PopulateTileSets();

            _rooms = new Room[howManyRooms];
            _doors = new List<Door>();
            BuildStarterRoom();
        }

        public (Vector2 upperLeft, Vector2 lowerRight) GetRoomDimensions(int roomId)
        {
            Vector2 upperLeft = _rooms[roomId].upperLeftInGlobalSpace;
            Vector2 lowerRight = _rooms[roomId].lowerRightInGlobalSpace;
            return (upperLeft, lowerRight);
        }
        private void BuildStarterRoom()
        {
            GameObject room000 = Instantiate(new GameObject("Room000"), roomsParent.transform, false);

            Room room0 = new Room(_tileSetRock1, 40, 20, new Vector2(-6.0f, 10.0F), room000);
            _rooms[0] = room0;


            Transform tile8 = rock1TileSet.transform.Find("8_origin");
            Vector3 position = Vector3.zero;
            Quaternion rotation = new Quaternion(0, 0, 0, 0);
            UnityEngine.Object.Instantiate(tile8, position, rotation, roomsParent.transform);

            //room0.AddPerimiterTiles();
            //// tile above the floor = -7.68
            //room0.AddPlatformTiles(new Vector2(-1f, -4.0f), 4, 5);
            //room0.AddPlatformTiles(new Vector2(5f, -5f), 12, 1);
            //room0.AddPlatformTiles(new Vector2(5f, -1f), 5, 1);
            //room0.AddPlatformTiles(new Vector2(12.0f, -1f), 5, 1);
            //room0.AddPlatformTiles(new Vector2(5f, 3f), 12, 1);
            //room0.AddPlatformTiles(new Vector2(20f, 0f), 2, 7);
            //room0.AddPlatformTiles(new Vector2(24f, 1f), 1, 10);
            //room0.AddPlatformTiles(new Vector2(26f, 0f), 1, 9);
            //room0.AddPlatformTiles(new Vector2(28f, -1f), 1, 8);
            //room0.AddPlatformTiles(new Vector2(30f, -2f), 1, 7);
            //room0.AddStartingEnemy(floatingBot, new Vector2(26f, -4f), playerCharacter);
            //room0.AddStartingEnemy(floatingBot, new Vector2(28f, 0f), playerCharacter);
            //room0.AddStartingEnemy(floatingBot, new Vector2(30f, 0f), playerCharacter);
            //room0.AddStartingEnemy(floatingBot, new Vector2(32f, 0f), playerCharacter);
            //room0.AddStartingEnemy(floatingBot, new Vector2(34f, 0f), playerCharacter);
            //room0.AddStartingEnemy(floatingBot, new Vector2(36f, 0f), playerCharacter);
            //room0.AddStartingEnemy(floatingBot, new Vector2(26f, -5f), playerCharacter);
            //room0.AddStartingEnemy(floatingBot, new Vector2(28f, 1f), playerCharacter);
            //room0.AddStartingEnemy(floatingBot, new Vector2(30f, 1f), playerCharacter);
            //room0.AddStartingEnemy(floatingBot, new Vector2(32f, 1f), playerCharacter);
            //room0.AddStartingEnemy(floatingBot, new Vector2(34f, 1f), playerCharacter);
            //room0.AddStartingEnemy(floatingBot, new Vector2(36f, 1f), playerCharacter);


            //GameObject room001 = Instantiate(new GameObject("Room001"), roomsParent.transform, false);

            //Room room1 = new Room(_tileSetRock1, 20, 20, new Vector2(34.0f, 10.0F), room001);
            //_rooms[1] = room1;
            //room1.AddPerimiterTiles();
            //room1.AddPlatformTiles(new Vector2(38f, 6f), 1, 11);
            //room1.AddStartingEnemy(floatingBot, new Vector2(40f, 0f), playerCharacter);
            //room1.AddStartingEnemy(floatingBot, new Vector2(42f, 0f), playerCharacter);
            //room1.AddStartingEnemy(floatingBot, new Vector2(44f, 0f), playerCharacter);
            //room1.AddStartingEnemy(floatingBot, new Vector2(46f, 0f), playerCharacter);
            //room1.AddStartingEnemy(floatingBot, new Vector2(48f, 0f), playerCharacter);



            // draw the door between them
            //ConnectRoomsWithDoor(room0, room1, rock1Door, 1f);
            //room0.DrawSelf();
            //room1.DrawSelf();
        }
        private void PopulateTileSets()
        {
            _tileSetRock1 = new TileSet();
            //_tileSetRock1.basePrefab = rock1Base;
            //_tileSetRock1.topPrefab = rock1Top;
            //_tileSetRock1.bottomPrefab = rock1Bottom;
            //_tileSetRock1.leftPrefab = rock1Left;
            //_tileSetRock1.rightPrefab = rock1Right;
            //_tileSetRock1.cornerUpLeftPrefab = rock1CornerUpLeft;
            //_tileSetRock1.cornerUpRightPrefab = rock1CornerUpRight;
            //_tileSetRock1.cornerDownLeftPrefab = rock1CornerBottomLeft;
            //_tileSetRock1.cornerDownRightPrefab = rock1CornerBottomRight;
            //_tileSetRock1.endCapUpPrefab = rock1EndCapUp;
            //_tileSetRock1.endCapDownPrefab = rock1EndCapDown;
            //_tileSetRock1.endCapLeftPrefab = rock1EndCapLeft;
            //_tileSetRock1.endCapRightPrefab = rock1EndCapRight;
            //_tileSetRock1.singleTallPrefab = rock1SingleTall;
            //_tileSetRock1.singleWidePrefab = rock1SingleWide;
        }
        private void ConnectRoomsWithDoor(Room roomLeft, Room roomRight, GameObject prefab, 
            float heightFromLeftFloorInTiles)
        {
            
            float posX = roomLeft.lowerRightInGlobalSpace.x - 1;
            float posY = roomLeft.lowerRightInGlobalSpace.y + heightFromLeftFloorInTiles + Globals.doorHeightInTiles;
            // now knock out perimiter blocks in each room
            for (int i = 0; i < Globals.doorHeightInTiles; i++)
            {
                roomLeft.KnockOutTile(new Vector2(posX, posY - i));
                roomRight.KnockOutTile(new Vector2(posX + 1, posY - i));
            }


            Vector3 position = new Vector2(posX, posY);
            Quaternion rotation = new Quaternion(0, 0, 0, 0);
            Instantiate(prefab, position, rotation, roomsParent.transform);

            _doors.Add(new Door() { roomLeft = roomLeft, roomRight = roomRight, positionInGlobalSpace = position });
        }
    }
}

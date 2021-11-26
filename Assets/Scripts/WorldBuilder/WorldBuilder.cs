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
        [SerializeField] public GameObject roomsParent;
        [Space(10)]
        [Header("Tile sets")]
        [Space(10)]
        [SerializeField] public GameObject rock1Base;
        [SerializeField] public GameObject rock1Top;
        [SerializeField] public GameObject rock1Bottom;
        [SerializeField] public GameObject rock1Left;
        [SerializeField] public GameObject rock1Right;
        [SerializeField] public GameObject rock1CornerUpLeft;
        [SerializeField] public GameObject rock1CornerUpRight;
        [SerializeField] public GameObject rock1CornerBottomLeft;
        [SerializeField] public GameObject rock1CornerBottomRight;
        [SerializeField] public GameObject rock1EndCapUp;
        [SerializeField] public GameObject rock1EndCapDown;
        [SerializeField] public GameObject rock1EndCapLeft;
        [SerializeField] public GameObject rock1EndCapRight;
        [SerializeField] public GameObject rock1SingleWide;
        [SerializeField] public GameObject rock1SingleTall;
        public Room[] rooms;

        private TileSet _tileSetRock1; 

        void Start()
        {
            int howManyRooms = 1;
            PopulateTileSets();

            rooms = new Room[howManyRooms];
            BuildStarterRoom();
        }

        public (Vector2 upperLeft, Vector2 lowerRight) GetRoomDimensions(int roomId)
        {
            Vector2 upperLeft = rooms[roomId].upperLeftInGlobalSpace;
            Vector2 lowerRight = rooms[roomId].lowerRightInGlobalSpace;
            return (upperLeft, lowerRight);
        }
        private void BuildStarterRoom()
        {
             
            GameObject room000 = Instantiate(new GameObject("Room000"), roomsParent.transform, false);

            Room r = new Room(_tileSetRock1, 40, 20, new Vector2(-6.0f, 10.0F), room000);
            rooms[0] = r;
            r.AddPerimiterTiles();
            // tile above the floor = -7.68
            r.AddPlatformTiles(new Vector2(-1f, -4.0f), 4, 5);
            r.AddPlatformTiles(new Vector2(5f, -5f), 12, 1);
            r.AddPlatformTiles(new Vector2(5f, -1f), 5, 1);
            r.AddPlatformTiles(new Vector2(12.0f, -1f), 5, 1);
            r.AddPlatformTiles(new Vector2(5f, 3f), 12, 1);
            r.AddPlatformTiles(new Vector2(20f, 0f), 2, 7);
            r.AddPlatformTiles(new Vector2(29f, 6f), 1, 11);
            r.DrawSelf();
        }
        private void PopulateTileSets()
        {
            _tileSetRock1 = new TileSet();
            _tileSetRock1.basePrefab = rock1Base;
            _tileSetRock1.topPrefab = rock1Top;
            _tileSetRock1.bottomPrefab = rock1Bottom;
            _tileSetRock1.leftPrefab = rock1Left;
            _tileSetRock1.rightPrefab = rock1Right;
            _tileSetRock1.cornerUpLeftPrefab = rock1CornerUpLeft;
            _tileSetRock1.cornerUpRightPrefab = rock1CornerUpRight;
            _tileSetRock1.cornerDownLeftPrefab = rock1CornerBottomLeft;
            _tileSetRock1.cornerDownRightPrefab = rock1CornerBottomRight;
            _tileSetRock1.endCapUpPrefab = rock1EndCapUp;
            _tileSetRock1.endCapDownPrefab = rock1EndCapDown;
            _tileSetRock1.endCapLeftPrefab = rock1EndCapLeft;
            _tileSetRock1.endCapRightPrefab = rock1EndCapRight;
            _tileSetRock1.singleTallPrefab = rock1SingleTall;
            _tileSetRock1.singleWidePrefab = rock1SingleWide;
        }
    }
}

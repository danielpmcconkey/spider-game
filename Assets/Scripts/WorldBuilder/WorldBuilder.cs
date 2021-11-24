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

            Room r = new Room(_tileSetRock1, 40, 20, new Vector2(-19.2f, 9.6F), room000);
            rooms[0] = r;
            r.DrawSelf();
        }
        private void PopulateTileSets()
        {
            _tileSetRock1 = new TileSet();
            _tileSetRock1.basePrefab = rock1Base;
            _tileSetRock1.topPrefab = rock1Top;

        }
    }
}

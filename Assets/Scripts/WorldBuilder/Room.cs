using Assets.Scripts.CharacterControl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.WorldBuilder
{
    public class Room
    {
        public int id;
        public Vector2 upperLeftInGlobalSpace;
        public Vector2 lowerRightInGlobalSpace;
        public int roomWidthInTiles;
        public int roomHeightInTiles;
        public float roomWidthInUnityMeters;
        public float roomHeightInUnityMeters;
        public ControllableCharacter[] enemies;
        private TileSet _tileSet;


        private Block[] _blocks;
        private const float _blockWidthInUnityMeters = 0.96f;
        private const float _blockHeightInUnityMeters = 0.96f;
        private GameObject _roomGameObject;



        public Room(TileSet tileSet, int roomWidthInTiles, int roomHeightInTiles, 
            Vector2 upperLeftInGlobalSpace, GameObject roomsGameObject)
        {
            _tileSet = tileSet;
            this.roomWidthInTiles = roomWidthInTiles;
            this.roomHeightInTiles = roomHeightInTiles;
            this.upperLeftInGlobalSpace = upperLeftInGlobalSpace;
            lowerRightInGlobalSpace = new Vector2(
                upperLeftInGlobalSpace.x + (_blockWidthInUnityMeters * roomWidthInTiles),
                upperLeftInGlobalSpace.y - (_blockHeightInUnityMeters * roomHeightInTiles));
            _roomGameObject = roomsGameObject;

            roomWidthInUnityMeters = roomWidthInTiles * _blockWidthInUnityMeters;
            roomHeightInUnityMeters = roomHeightInTiles * _blockHeightInUnityMeters;

            _blocks = new Block[roomWidthInTiles * roomHeightInTiles];
        }
        public void DrawSelf()
        {
            DrawPerimiter();
        }

        private void DrawPerimiter()
        {
            DrawFloor();
        }
        private void DrawFloor()
        {
            float x = upperLeftInGlobalSpace.x;
            float y = upperLeftInGlobalSpace.y - (roomHeightInTiles * _blockHeightInUnityMeters);
            for (int i = 0; i < roomWidthInTiles; i++)
            {
                AddBlockToUnity(_tileSet.basePrefab, x, y);
                x += _blockWidthInUnityMeters;
            }
        }

        private void AddBlockToUnity(GameObject prefab, float x, float y)
        {
            Vector3 position = new Vector3(x, y);
            Quaternion rotation = new Quaternion(0, 0, 0, 0);
            //if (prefab == baseRockPrefab)
            //{
            //    // random chance to alternate out the tile
            //    int rando = RNG.getRandomInt(0, 120);
            //    if (rando < 10)
            //    {
            //        prefab = baseRockAltPrefab;
            //    }
            //    else if (rando < 20)
            //    {
            //        prefab = baseRockAlt2Prefab;
            //    }
            //    else if (rando < 30)
            //    {
            //        prefab = baseRockAlt3Prefab;
            //    }

            //}
            GameObject obj = UnityEngine.Object.Instantiate(prefab, position, rotation, _roomGameObject.transform);

            //obj.transform.localScale = new Vector3(2, 2, 2);
        }

    }
}

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
            AddPerimiterBlocks();
            for(int i = 0; i < _blocks.Length; i++)
            {
                if(_blocks[i] != null)
                {
                    AddBlockToUnity(_blocks[i]);
                }
            }
        }

        private void AddPerimiterBlocks()
        {
            AddFloorBlocks();
            AddCeilingBlocks();
            AddLeftWallBlocks();
            AddRightWallBlocks();
        }
        private void AddCeilingBlocks()
        {
            float x = upperLeftInGlobalSpace.x;
            float y = upperLeftInGlobalSpace.y;
            for (int i = 0; i < roomWidthInTiles; i++)
            {
                int blockIndex = GetBlockIndexFromUnityPosition(x, y);
                if(_blocks[blockIndex] != null)
                {
                    string burp = "true";
                }
                _blocks[blockIndex] = new Block()
                {
                    prefab = _tileSet.basePrefab,
                    positionInRoom = new Vector2(x, y)
                };
                // AddBlockToUnity(_tileSet.basePrefab, x, y);
                
                x += _blockWidthInUnityMeters;
            }
        }
        private void AddFloorBlocks()
        {
            float x = upperLeftInGlobalSpace.x;
            float y = upperLeftInGlobalSpace.y - ((roomHeightInTiles - 1) * _blockHeightInUnityMeters);
            for (int i = 0; i < roomWidthInTiles; i++)
            {
                GameObject prefab = _tileSet.topPrefab;
                if (i == 0 || i == roomWidthInTiles - 1)
                {
                    prefab = _tileSet.basePrefab;
                }

                int blockIndex = GetBlockIndexFromUnityPosition(x, y);
                _blocks[blockIndex] = new Block()
                {
                    prefab = prefab,
                    positionInRoom = new Vector2(x, y)
                };
                x += _blockWidthInUnityMeters;
            }
        }
        private void AddLeftWallBlocks()
        {
            float x = upperLeftInGlobalSpace.x;
            float y = upperLeftInGlobalSpace.y - _blockHeightInUnityMeters;
            // assume floor and ceiling are already drawn
            for (int i = 0; i < roomHeightInTiles - 1; i++)
            {
                int blockIndex = GetBlockIndexFromUnityPosition(x, y);
                _blocks[blockIndex] = new Block()
                {
                    prefab = _tileSet.basePrefab,
                    positionInRoom = new Vector2(x, y)
                };

                y -= _blockHeightInUnityMeters;
            }
        }
        private void AddRightWallBlocks()
        {
            float x = upperLeftInGlobalSpace.x + ((roomWidthInTiles - 1) * _blockWidthInUnityMeters);
            float y = upperLeftInGlobalSpace.y - _blockHeightInUnityMeters;
            // assume floor and ceiling are already drawn
            for (int i = 0; i < roomHeightInTiles - 1; i++)
            {
                int blockIndex = GetBlockIndexFromUnityPosition(x, y);
                _blocks[blockIndex] = new Block()
                {
                    prefab = _tileSet.basePrefab,
                    positionInRoom = new Vector2(x, y)
                };

                y -= _blockHeightInUnityMeters;
            }
        }
        private int GetBlockIndexFromUnityPosition(float x, float y)
        {
            int xAsHundredTimes = (int)Math.Round(Math.Round(x, 2) * 100);
            int yAsHundredTimes = (int)Math.Round(Math.Round(y, 2) * 100);
            int xUlAsHundredTimes = (int)Math.Round(Math.Round(upperLeftInGlobalSpace.x, 2) * 100);
            int yUlAsHundredTimes = (int)Math.Round(Math.Round(upperLeftInGlobalSpace.y, 2) * 100);
            int blockWidthAsHundredTimes = (int)Math.Round(Math.Round(_blockWidthInUnityMeters, 2) * 100);
            int blockHeightAsHundredTimes = (int)Math.Round(Math.Round(_blockHeightInUnityMeters, 2) * 100);

            // remember that unity y gets lower as we go downward
            // but our index goes from up-left to down-right
            //
            // also, I rounded and multiplied by 100 because floats 
            // were doing weird things with the math
            int numRowsDown =  (yUlAsHundredTimes - yAsHundredTimes) / blockHeightAsHundredTimes;
            int valueInFirstColumnOfThatRow = numRowsDown * roomWidthInTiles;
            int numColsIn = (xAsHundredTimes - xUlAsHundredTimes) / blockWidthAsHundredTimes;
            return valueInFirstColumnOfThatRow + numColsIn; 
        }

        private void AddBlockToUnity(Block block)
        {
            Vector3 position = block.positionInRoom;
            Quaternion rotation = new Quaternion(0, 0, 0, 0);
            GameObject obj = UnityEngine.Object.Instantiate(block.prefab, position, rotation, _roomGameObject.transform);
        }

    }
}

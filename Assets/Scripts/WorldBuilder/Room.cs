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
        private List<Enemy> _startingEnemies;
        private TileSet _tileSet;


        private Tile[] _tiles;
        private GameObject _roomGameObject;



        #region public methods
        public Room(TileSet tileSet, int roomWidthInTiles, int roomHeightInTiles,
            Vector2 upperLeftInGlobalSpace, GameObject roomsGameObject)
        {
            _tileSet = tileSet;
            this.roomWidthInTiles = roomWidthInTiles;
            this.roomHeightInTiles = roomHeightInTiles;
            this.upperLeftInGlobalSpace = upperLeftInGlobalSpace;
            lowerRightInGlobalSpace = new Vector2(
                upperLeftInGlobalSpace.x + (Globals.tileWidthInUnityMeters * roomWidthInTiles),
                upperLeftInGlobalSpace.y - (Globals.tileHeightInUnityMeters * roomHeightInTiles));
            _roomGameObject = roomsGameObject;

            roomWidthInUnityMeters = roomWidthInTiles * Globals.tileWidthInUnityMeters;
            roomHeightInUnityMeters = roomHeightInTiles * Globals.tileHeightInUnityMeters;

            _tiles = new Tile[roomWidthInTiles * roomHeightInTiles];
            _startingEnemies = new List<Enemy>();
        }
        public void AddPlatformTiles(Vector2 platformUpLeft, int numTilesWide, int numTilesTall)
        {
            float y = platformUpLeft.y;

            for (int row = 0; row < numTilesTall; row++)
            {
                // re-zero x every row
                float x = platformUpLeft.x;

                for (int column = 0; column < numTilesWide; column++)
                {
                    int tileIndex = GetTileIndexFromUnityPosition(x, y);

                    GameObject prefab = _tileSet.basePrefab;
                    if (row == 0)
                    {
                        prefab = _tileSet.topPrefab;
                        if (column == 0)
                        {
                            prefab = _tileSet.cornerUpLeftPrefab;
                            if (numTilesTall == 1) prefab = _tileSet.endCapLeftPrefab;
                            if (numTilesWide == 1) prefab = _tileSet.endCapUpPrefab;
                        }
                        else if (column == numTilesWide - 1)
                        {
                            prefab = _tileSet.cornerUpRightPrefab;
                            if (numTilesTall == 1) prefab = _tileSet.endCapRightPrefab;
                        }
                        else if (numTilesTall == 1) prefab = _tileSet.singleTallPrefab;
                    }
                    else
                    {
                        if (column == 0)
                        {
                            prefab = _tileSet.leftPrefab;
                            if (row == numTilesTall - 1)
                            {
                                prefab = _tileSet.cornerDownLeftPrefab;
                            }
                        }
                        else if (column == numTilesWide - 1)
                        {
                            prefab = _tileSet.rightPrefab;
                            if (row == numTilesTall - 1)
                            {
                                prefab = _tileSet.cornerDownRightPrefab;
                            }
                        }
                        if (numTilesWide == 1)
                        {
                            prefab = _tileSet.singleWidePrefab;
                            if(row == numTilesTall - 1)
                            {
                                prefab = _tileSet.endCapDownPrefab;
                            }
                        }
                        
                    }

                    _tiles[tileIndex] = new Tile()
                    {
                        prefab = prefab,
                        positionInGlobalSpace = new Vector2(x, y)
                    };

                    x += Globals.tileWidthInUnityMeters;
                }
                y -= Globals.tileHeightInUnityMeters;
            }
        }
        public void AddPerimiterTiles()
        {
            AddFloorTiles();
            AddCeilingTiles();
            AddLeftWallTiles();
            AddRightWallTiles();
        }
        public void AddStartingEnemy(GameObject prefab, Vector2 positionInGlobalSpace, GameObject playerCharacter)
        {
            _startingEnemies.Add(new Enemy()
            {
                prefab = prefab,
                positionInGlobalSpace = positionInGlobalSpace,
                targetCharacter = playerCharacter
            });
        }
        public void DrawSelf()
        {
            for (int i = 0; i < _tiles.Length; i++)
            {
                if (_tiles[i] != null)
                {
                    AddTileToUnity(_tiles[i]);
                }
            }
            foreach(Enemy e in _startingEnemies)
            {
                GameObject enemyObj = DrawPrefab(e.prefab, e.positionInGlobalSpace);
            }
        }
        public void KnockOutTile(Vector2 positionInGlobalSpace)
        {
            _tiles[GetTileIndexFromUnityPosition(positionInGlobalSpace.x, positionInGlobalSpace.y)] = null;
        }
        #endregion

        #region private methods
        private void AddCeilingTiles()
        {
            float x = upperLeftInGlobalSpace.x;
            float y = upperLeftInGlobalSpace.y;
            for (int i = 0; i < roomWidthInTiles; i++)
            {
                GameObject prefab = _tileSet.bottomPrefab;
                if (i == 0 || i == roomWidthInTiles - 1)
                {
                    prefab = _tileSet.basePrefab;
                }


                int tileIndex = GetTileIndexFromUnityPosition(x, y);

                _tiles[tileIndex] = new Tile()
                {
                    prefab = prefab,
                    positionInGlobalSpace = new Vector2(x, y)
                };

                x += Globals.tileWidthInUnityMeters;
            }
        }
        private void AddFloorTiles()
        {
            float x = upperLeftInGlobalSpace.x;
            float y = upperLeftInGlobalSpace.y - ((roomHeightInTiles - 1) * Globals.tileHeightInUnityMeters);
            for (int i = 0; i < roomWidthInTiles; i++)
            {
                GameObject prefab = _tileSet.topPrefab;
                if (i == 0 || i == roomWidthInTiles - 1)
                {
                    prefab = _tileSet.basePrefab;
                }

                int tileIndex = GetTileIndexFromUnityPosition(x, y);
                _tiles[tileIndex] = new Tile()
                {
                    prefab = prefab,
                    positionInGlobalSpace = new Vector2(x, y)
                };
                x += Globals.tileWidthInUnityMeters;
            }
        }
        private void AddLeftWallTiles()
        {
            float x = upperLeftInGlobalSpace.x;
            float y = upperLeftInGlobalSpace.y - Globals.tileHeightInUnityMeters;
            // assume floor and ceiling are already drawn
            for (int i = 0; i < roomHeightInTiles - 2; i++)
            {
                GameObject prefab = _tileSet.rightPrefab;
                
                int tileIndex = GetTileIndexFromUnityPosition(x, y);
                _tiles[tileIndex] = new Tile()
                {
                    prefab = prefab,
                    positionInGlobalSpace = new Vector2(x, y)
                };

                y -= Globals.tileHeightInUnityMeters;
            }
        }
        private void AddRightWallTiles()
        {
            float x = upperLeftInGlobalSpace.x + ((roomWidthInTiles - 1) * Globals.tileWidthInUnityMeters);
            float y = upperLeftInGlobalSpace.y - Globals.tileHeightInUnityMeters;
            // assume floor and ceiling are already drawn
            for (int i = 0; i < roomHeightInTiles - 2; i++)
            {
                GameObject prefab = _tileSet.leftPrefab;
                
                int tileIndex = GetTileIndexFromUnityPosition(x, y);
                _tiles[tileIndex] = new Tile()
                {
                    prefab = prefab,
                    positionInGlobalSpace = new Vector2(x, y)
                };

                y -= Globals.tileHeightInUnityMeters;
            }
        }
        private void AddTileToUnity(Tile tile)
        {
            // check to see if we've burried a top tile
            if(tile.prefab == _tileSet.topPrefab)
            {
                int tileIndexAbove = GetTileIndexFromUnityPosition(tile.positionInGlobalSpace.x,
                    tile.positionInGlobalSpace.y + Globals.tileHeightInUnityMeters);
                if (_tiles[tileIndexAbove] != null) tile.prefab = _tileSet.basePrefab;
            }
            // now draw it
            DrawPrefab(tile.prefab, tile.positionInGlobalSpace);
        }
        private GameObject DrawPrefab(GameObject prefab, Vector2 positionInGlobalSpace)
        {
            Vector3 position = positionInGlobalSpace;
            Quaternion rotation = new Quaternion(0, 0, 0, 0);
            return UnityEngine.Object.Instantiate(prefab, position, rotation, _roomGameObject.transform);
        }
        private int GetTileIndexFromUnityPosition(float x, float y)
        {
            int xAsHundredTimes = (int)Math.Round(Math.Round(x, 2) * 100);
            int yAsHundredTimes = (int)Math.Round(Math.Round(y, 2) * 100);
            int xUlAsHundredTimes = (int)Math.Round(Math.Round(upperLeftInGlobalSpace.x, 2) * 100);
            int yUlAsHundredTimes = (int)Math.Round(Math.Round(upperLeftInGlobalSpace.y, 2) * 100);
            int tileWidthAsHundredTimes = (int)Math.Round(Math.Round(Globals.tileWidthInUnityMeters, 2) * 100);
            int tileHeightAsHundredTimes = (int)Math.Round(Math.Round(Globals.tileHeightInUnityMeters, 2) * 100);

            // remember that unity y gets lower as we go downward
            // but our index goes from up-left to down-right
            //
            // also, I rounded and multiplied by 100 because floats 
            // were doing weird things with the math
            int numRowsDown = (yUlAsHundredTimes - yAsHundredTimes) / tileHeightAsHundredTimes;
            int valueInFirstColumnOfThatRow = numRowsDown * roomWidthInTiles;
            int numColsIn = (xAsHundredTimes - xUlAsHundredTimes) / tileWidthAsHundredTimes;
            return valueInFirstColumnOfThatRow + numColsIn;
        }

        #endregion


    }
}

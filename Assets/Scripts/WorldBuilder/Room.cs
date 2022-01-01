using Assets.Scripts.CharacterControl;
using Assets.Scripts.WorldBuilder.RoomBuilder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering.Universal;

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
        private GameObject _tileSet;
        


        private RoomSave _roomSave;
        private GameObject _roomGameObject;



        #region public methods
        public Room(int id, GameObject tileSet, RoomSave roomSave, Vector2 upperLeftInGlobalSpace, GameObject roomsGameObject)
        {
            this.id = id;
            _tileSet = tileSet;
            _roomSave = roomSave;
            roomWidthInTiles = _roomSave.roomWidth;
            roomHeightInTiles = _roomSave.roomHeight;
            this.upperLeftInGlobalSpace = upperLeftInGlobalSpace;
            lowerRightInGlobalSpace = new Vector2(
                upperLeftInGlobalSpace.x + (Globals.tileWidthInUnityMeters * roomWidthInTiles),
                upperLeftInGlobalSpace.y - (Globals.tileHeightInUnityMeters * roomHeightInTiles));
            _roomGameObject = roomsGameObject;

            roomWidthInUnityMeters = roomWidthInTiles * Globals.tileWidthInUnityMeters;
            roomHeightInUnityMeters = roomHeightInTiles * Globals.tileHeightInUnityMeters;

            _startingEnemies = new List<Enemy>();
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
            for (int i = 0; i < _roomSave.tiles.Length; i++)
            {
                TilePlacement tilePlacement = _roomSave.tiles[i];
                if (tilePlacement.tileNum > 0)
                {
                    Tile tile = new Tile();
                    tile.positionInGlobalSpace = GetGlobalPositionFromTileIndex(i);
                    Transform prefabTransform = _tileSet.transform.Find(string.Format("{0}_origin", tilePlacement.tileNum));
                    tile.prefab = prefabTransform.gameObject;
                    AddTileToUnity(tile);
                    // random lights
                    if (Utility.RNG.GetRandomInt(0, 20) == 7) // 1 in 20
                    {
                        AddRandomLight(tile.positionInGlobalSpace);
                    }
                }
            }
            AddDecorations();

            foreach (Enemy e in _startingEnemies)
            {
                GameObject enemyObj = DrawPrefab(e.prefab, e.positionInGlobalSpace);
            }
        }


        public void KnockOutTile(Vector2 positionInGlobalSpace)
        {
            //_tiles[GetTileIndexFromUnityPosition(positionInGlobalSpace.x, positionInGlobalSpace.y)] = null;
        }
        #endregion

        #region private methods
        private void AddDecorations()
        {
            const float floorAndWallPlacementOdds = 0.75f;
            // floor decorations
            int[] tilesThatCanHaveFloorDecor = new int[] { 10, 11, 12, 13, 31 };
            AddDecorationsOfType(tilesThatCanHaveFloorDecor, "FloorDecor{0}_origin", 7, floorAndWallPlacementOdds);
            // floor decorations on corner tiles
            int[] tilesThatCanHaveFloorOnConerDecor = new int[] { 18, 19, 20, 21, 26, 27, 28, 29 };
            AddDecorationsOfType(tilesThatCanHaveFloorOnConerDecor, "CornerTopDecor{0}_origin", 4, floorAndWallPlacementOdds);
            // left wall decorations
            int[] tilesThatCanHaveLeftWallDecor = new int[] { 2, 3, 4, 5, 32 };
            AddDecorationsOfType(tilesThatCanHaveLeftWallDecor, "SideLeftDecor{0}_origin", 2, floorAndWallPlacementOdds);
            // left wall decorations on corner tiles
            int[] tilesThatCanHaveLeftWallOnCornerDecor = new int[] { 18, 19, 22, 23, 26, 27, 29, 30, 32 };
            AddDecorationsOfType(tilesThatCanHaveLeftWallOnCornerDecor, "CornerLeftDecor{0}_origin", 2, floorAndWallPlacementOdds);
            // right wall decorations
            int[] tilesThatCanHaveRightWallDecor = new int[] { 6, 7, 8, 9, 32 };
            AddDecorationsOfType(tilesThatCanHaveRightWallDecor, "SideRightDecor{0}_origin", 2, floorAndWallPlacementOdds);
            // right wall decorations on corner tiles
            int[] tilesThatCanHaveRightWallOnCornerDecor = new int[] { 20, 21, 24, 25, 26, 28, 29, 30, 32 };
            AddDecorationsOfType(tilesThatCanHaveRightWallOnCornerDecor, "CornerRightDecor{0}_origin", 2, floorAndWallPlacementOdds);
            // ceiling decorations
            int[] tilesThatCanHaveCeilingDecor = new int[] { 14, 15, 16, 17, 31 };
            AddDecorationsOfType(tilesThatCanHaveCeilingDecor, "CeilingDecor{0}_origin", 7, floorAndWallPlacementOdds);
            //// ceiling decorations on corner tiles
            //int[] tilesThatCanHaveCeilingOnConerDecor = new int[] { 18, 19, 20, 21, 26, 27, 28, 29 };
            //AddDecorationsOfType(tilesThatCanHaveCeilingOnConerDecor, "CornerCeilingDecor{0}_origin", 2, floorAndWallPlacementOdds);

        }
        private void AddDecorationsOfType(int[] tilesThatHaveThisDecor, string prefabNameString, 
            int numberOfOptions, float oddsOfPlacement)
        {
            
            for (int i = 0; i < _roomSave.tiles.Length; i++)
            {
                TilePlacement tilePlacement = _roomSave.tiles[i];

                
                if (Utility.RNG.GetRandomPercent() < oddsOfPlacement &&
                    tilesThatHaveThisDecor.Contains(tilePlacement.tileNum))
                {
                    int spriteNum = Utility.RNG.GetRandomInt(1, numberOfOptions);
                    Tile tile = new Tile();
                    tile.positionInGlobalSpace = GetGlobalPositionFromTileIndex(i);
                    Transform prefabTransform = _tileSet.transform.Find(string.Format(prefabNameString, spriteNum));
                    tile.prefab = prefabTransform.gameObject;

                    GameObject decoration = AddTileToUnity(tile);
                    AssignChildLightsToRoom(decoration);
                }
            }
        }
        private void AddRandomLight(Vector2 positionInGlobalSpace)
        {
            var prefab = _tileSet.transform.Find("SmallRoomLight");
            GameObject gameObject = DrawPrefab(prefab.gameObject, positionInGlobalSpace);
            LightSwitch lightSwitch = gameObject.GetComponent<LightSwitch>();
            lightSwitch.roomId = id;
        }
        private GameObject AddTileToUnity(Tile tile)
        {
            
            // now draw it
            return DrawPrefab(tile.prefab, tile.positionInGlobalSpace);
        }
        private void AssignChildLightsToRoom(GameObject decoration)
        {
            var lights = //decoration.GetComponents<Light2D>();
            decoration.GetComponentsInChildren<Light2D>();
            foreach(var light in lights)
            {
                LightSwitch lightSwitch = light.GetComponent<LightSwitch>();
                lightSwitch.roomId = id;
            }
        }
        private GameObject DrawPrefab(GameObject prefab, Vector2 positionInGlobalSpace)
        {
            Vector3 position = positionInGlobalSpace;
            Quaternion rotation = new Quaternion(0, 0, 0, 0);
            return UnityEngine.Object.Instantiate(prefab, position, rotation, _roomGameObject.transform);
        }
        private Vector2 GetGlobalPositionFromTileIndex(int index)
        {
            Vector2 position = Vector2.zero;
            int row = (int)Mathf.Floor(index / (float)roomWidthInTiles);
            int column = (int)Mathf.Floor(index % roomWidthInTiles);
            position.x = upperLeftInGlobalSpace.x + (column * Globals.tileWidthInUnityMeters);
            position.y = upperLeftInGlobalSpace.y - (row * Globals.tileHeightInUnityMeters);
            return position;
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

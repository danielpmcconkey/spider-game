using Assets.Scripts.CharacterControl;
using Assets.Scripts.Data.World;
using Assets.Scripts.Events;
using Assets.Scripts.Utility;
using Assets.Scripts.WorldBuilder.RoomBuilder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Assets.Scripts.WorldBuilder.WorldManager
{
    public class RoomManager : ActivatableGameObjectManager
    {
        public int id;
        public Vector2 upperLeftInGlobalSpace;
        public Vector2 lowerRightInGlobalSpace;
        public int roomWidthInTiles;
        public int roomHeightInTiles;
        public float roomWidthInUnityMeters;
        public float roomHeightInUnityMeters;
        private GameObject _tilePalette;
        private RoomPlacement _roomPlacement;

        private List<RoomMaskManager> _roomMasks;
        private List<EnemyManager> _startingEnemies;



        #region public methods
        public RoomManager(int id, GameObject tilePalette, RoomPlacement roomPlacement, GameObject roomsGameObject) 
            : base(roomsGameObject)
        {
            this.id = id;
            _tilePalette = tilePalette;
            _roomPlacement = roomPlacement;
            roomWidthInTiles = _roomPlacement.roomWidthInTiles;
            roomHeightInTiles = _roomPlacement.roomHeightInTiles;
            this.upperLeftInGlobalSpace = _roomPlacement.upperLeftInGlobalSpace;
            lowerRightInGlobalSpace = new Vector2(
                upperLeftInGlobalSpace.x + MeasurementConverter.TilesXToUnityMeters(roomWidthInTiles),
                upperLeftInGlobalSpace.y - MeasurementConverter.TilesYToUnityMeters(roomHeightInTiles));

            roomWidthInUnityMeters = MeasurementConverter.TilesXToUnityMeters(roomWidthInTiles);
            roomHeightInUnityMeters = MeasurementConverter.TilesYToUnityMeters(roomHeightInTiles);

            _startingEnemies = new List<EnemyManager>();
            BuildMasksFromBlueprint();
        }
        private void BuildMasksFromBlueprint()
        {
            _roomMasks = new List<RoomMaskManager>();
            foreach(RoomMaskPlacement maskPlacement in _roomPlacement.roomMasks)
            {
                Transform maskTransform = _tilePalette.transform.Find("Mask");
                Quaternion rotation = new Quaternion(0, 0, 0, 0);
                GameObject maskGameobject = GameObject.Instantiate(
                    maskTransform.gameObject, maskPlacement.positionInGlobalSpace, rotation, _gameObject.transform);
                RoomMaskManager roomMaskManager = new RoomMaskManager(maskGameobject);
                maskGameobject.transform.localScale = maskPlacement.scale;
                _roomMasks.Add(roomMaskManager);
            }
        }
        public void ActivateMasks()
        {
            foreach (RoomMaskManager mask in _roomMasks)
            {
                mask.ActivateSelf();
            }
        }
        //public void AddStartingEnemy(GameObject prefab, Vector2 positionInGlobalSpace, GameObject playerCharacter)
        //{
        //    _startingEnemies.Add(new EnemyManager()
        //    {
        //        prefab = prefab,
        //        positionInGlobalSpace = positionInGlobalSpace,
        //        targetCharacter = playerCharacter
        //    });
        //}
        public void DeActivateMasks()
        {
            foreach(RoomMaskManager mask in _roomMasks)
            {
                mask.DeActivateSelf();
            }
        }
        public void DrawSelf()
        {
            for (int i = 0; i < _roomPlacement.tiles.Length; i++)
            {
                TilePlacement tilePlacement = _roomPlacement.tiles[i];
                if (tilePlacement != null && tilePlacement.tileNum > 0)
                {
                    TileManager tile = new TileManager();
                    tile.tilePlacement = tilePlacement;
                    Transform prefabTransform = _tilePalette.transform.Find(string.Format("{0}_origin", tilePlacement.tileNum));
                    tile.prefab = prefabTransform.gameObject;
                    AddTileToUnity(tile);
                    // random lights
                    if (Utility.RNG.GetRandomInt(0, 20) == 7) // 1 in 20
                    {
                        AddRandomLight(tilePlacement.positionInGlobalSpace);
                    }
                }
            }
            AddDecorations();

            foreach (EnemyManager e in _startingEnemies)
            {
                GameObject enemyObj = DrawPrefab(e.prefab, e.enemyPlacement.positionInGlobalSpace);
            }
        }
        //public void KnockOutTile(Vector2 positionInGlobalSpace)
        //{
        //    //_tiles[GetTileIndexFromUnityPosition(positionInGlobalSpace.x, positionInGlobalSpace.y)] = null;
        //}
        //public void SwapRoomMasks(List<RoomMaskManager> masks)
        //{
        //    _roomMasks = masks;
        //}
        #endregion

        #region private methods
        private void AddDecorations()
        {
            const float floorAndWallPlacementOdds = 1.0f;
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
            // ceiling decorations on corner tiles
            int[] tilesThatCanHaveCeilingOnConerDecor = new int[] { 22, 23, 24, 25, 26, 27, 28, 30 };
            AddDecorationsOfType(tilesThatCanHaveCeilingOnConerDecor, "CornerCeilingDecor{0}_origin", 2, floorAndWallPlacementOdds);

        }
        private void AddDecorationsOfType(int[] tilesThatHaveThisDecor, string prefabNameString, 
            int numberOfOptions, float oddsOfPlacement)
        {
            
            for (int i = 0; i < _roomPlacement.tiles.Length; i++)
            {
                TilePlacement tilePlacement = _roomPlacement.tiles[i];

                
                if (tilePlacement != null && Utility.RNG.GetRandomPercent() <= oddsOfPlacement &&
                    tilesThatHaveThisDecor.Contains(tilePlacement.tileNum))
                {
                    int spriteNum = Utility.RNG.GetRandomInt(1, numberOfOptions);
                    TileManager tile = new TileManager();
                    //tile.positionInGlobalSpace = GetGlobalPositionFromTileIndex(i);
                    Transform prefabTransform = _tilePalette.transform.Find(string.Format(prefabNameString, spriteNum));
                    tile.prefab = prefabTransform.gameObject;

                    GameObject decoration = AddTileToUnity(tile);
                    AssignChildLightsToRoom(decoration);
                }
            }
        }
        private void AddRandomLight(Vector2 positionInGlobalSpace)
        {
            var prefab = _tilePalette.transform.Find("SmallRoomLight");
            GameObject gameObject = DrawPrefab(prefab.gameObject, positionInGlobalSpace);
            LightSwitch lightSwitch = gameObject.GetComponent<LightSwitch>();
            lightSwitch.roomId = id;
        }
        private GameObject AddTileToUnity(TileManager tile)
        {
            
            // now draw it
            return DrawPrefab(tile.prefab, tile.tilePlacement.positionInGlobalSpace);
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
            return UnityEngine.Object.Instantiate(prefab, position, rotation, _gameObject.transform);
        }
        private Vector2 GetGlobalPositionFromTileIndex(int index)
        {
            try
            {
                Vector2 position = Vector2.zero;
                int row = (int)Mathf.Floor(index / (float)roomWidthInTiles);
                int column = (int)Mathf.Floor(index % roomWidthInTiles);
                position.x = upperLeftInGlobalSpace.x + MeasurementConverter.TilesXToUnityMeters(column);
                position.y = upperLeftInGlobalSpace.y - MeasurementConverter.TilesYToUnityMeters(row);
                return position;
            }
            catch (Exception)
            {

                throw;
            }
        }
        //private int GetTileIndexFromUnityPosition(float x, float y)
        //{
        //    int xAsHundredTimes = (int)Math.Round(Math.Round(x, 2) * 100);
        //    int yAsHundredTimes = (int)Math.Round(Math.Round(y, 2) * 100);
        //    int xUlAsHundredTimes = (int)Math.Round(Math.Round(upperLeftInGlobalSpace.x, 2) * 100);
        //    int yUlAsHundredTimes = (int)Math.Round(Math.Round(upperLeftInGlobalSpace.y, 2) * 100);
        //    int tileWidthAsHundredTimes = (int)Math.Round(Math.Round(MeasurementConverter.TilesXToUnityMeters(1), 2) * 100);
        //    int tileHeightAsHundredTimes = (int)Math.Round(Math.Round(MeasurementConverter.TilesYToUnityMeters(1), 2) * 100);

        //    // remember that unity y gets lower as we go downward
        //    // but our index goes from up-left to down-right
        //    //
        //    // also, I rounded and multiplied by 100 because floats 
        //    // were doing weird things with the math
        //    int numRowsDown = (yUlAsHundredTimes - yAsHundredTimes) / tileHeightAsHundredTimes;
        //    int valueInFirstColumnOfThatRow = numRowsDown * roomWidthInTiles;
        //    int numColsIn = (xAsHundredTimes - xUlAsHundredTimes) / tileWidthAsHundredTimes;
        //    return valueInFirstColumnOfThatRow + numColsIn;
        //}
        #endregion


    }
}

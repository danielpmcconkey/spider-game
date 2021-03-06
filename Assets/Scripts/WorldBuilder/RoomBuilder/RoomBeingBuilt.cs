using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.WorldBuilder.RoomBuilder
{
    public class RoomBeingBuilt
    {
        public int tileWidth = 48;
        public int tileHeight = 48;
        public string roomName;
        public int roomWidth = -1;
        public int roomHeight = -1;
        public TilePlacement[] tiles;
        public List<Position> doors;

        private List<int> _topRowTiles;
        private List<int> _bottomRowTiles;
        private List<int> _leftColumnTiles;
        private List<int> _rightColumnTiles;
        private LineRenderer _gridLinePrefab;
        private GameObject _gridParent;
        private GameObject _tileSet;
        private GameObject _tileParent;
        private GameObject _mouseTriggerSquare;


        private bool _hasGridBeenDrawn;

        public RoomBeingBuilt(RoomSave restore, LineRenderer gridLinePrefab, GameObject gridParent, GameObject tileSet,
            GameObject tileParent, GameObject mouseTriggerSquare)
        {
            _gridLinePrefab = gridLinePrefab;
            _gridParent = gridParent;
            _tileSet = tileSet;
            _tileParent = tileParent;
            _mouseTriggerSquare = mouseTriggerSquare;

            roomName = restore.roomName;
            tileWidth = restore.tileWidth;
            tileHeight = restore.tileHeight;
            roomWidth = restore.roomWidth;
            roomHeight = restore.roomHeight;
            tiles = restore.tiles;
            if (restore.doors != null) doors = new List<Position>(restore.doors);
            else doors = new List<Position>();
            AddPerimeterTiles(true);
            _hasGridBeenDrawn = false;
        }

        public void ReverseDoor(int row, int column)
        {
            bool isDoorAlreadyThere = false;
            List<Position> newDoors = new List<Position>();

            foreach (var d in doors)
            {
                if (d.row == row && d.column == column)
                {
                    isDoorAlreadyThere = true;
                }
                else newDoors.Add(d);
            }
            if (!isDoorAlreadyThere)
            {
                newDoors.Add(new Position() { row = row, column = column });
            }
            doors = newDoors;
        }
        
        public void DrawRoom(EditMode editMode)
        {
            if (roomWidth > 0 && roomHeight > 0)
            {
                DrawTiles();
                if(!_hasGridBeenDrawn) DrawGrid();
                DrawDoors();
                if (editMode == EditMode.DOOR)
                {

                }
            }
        }
        

        public int GetTileNumbFromMousePosition(Vector2 mouseDownLocation)
        {
            float mouseXToUse = mouseDownLocation.x;
            float mouseYToUse = mouseDownLocation.y;
            // if < the zero line or > right border, set it 
            // to the closest in-bounds point
            float minX = 0;
            float maxX = roomWidth * tileWidth / Globals.pixelsInAUnityMeter;
            float minY = roomHeight * tileHeight / Globals.pixelsInAUnityMeter * -1;
            float maxY = 0;


            if (mouseXToUse < minX) mouseXToUse = minX;
            if (mouseXToUse > maxX) mouseXToUse = maxX;
            if (mouseYToUse < minY) mouseYToUse = minY;
            if (mouseYToUse > maxY) mouseYToUse = maxY;

            // now map to row and column
            int column = (int)Math.Floor(mouseXToUse / ((float)tileWidth / Globals.pixelsInAUnityMeter));
            int row = (int)Math.Floor(mouseYToUse / ((float)tileHeight / Globals.pixelsInAUnityMeter)) * -1;
            column -= 1;    // account for the border column
            row -= 1;   // account for the border row, but Y is inverse
            int tileNum = (row * roomWidth) + column;
            return tileNum;
        }
        public void ReverseTile(int tileNum)
        {
            if (tileNum < 0) return;
            if (tileNum >= roomWidth * roomHeight) return;
            if (tiles[tileNum].isSolid)
            {
                tiles[tileNum] = new TilePlacement() { isSolid = false, tileNum = -1 };
            }
            else
            {
                tiles[tileNum] = new TilePlacement() { isSolid = true, tileNum = 2 };
            }
        }
        
        public void SetRoomDimensions(int width, int height)
        {
            roomWidth = width;
            roomHeight = height;
            tiles = new TilePlacement[width * height];
            doors = new List<Position>();
            AddPerimeterTiles();
        }

        private void AddPerimeterTiles(bool fromFileLoad = false)
        {
            _topRowTiles = new List<int>();
            _bottomRowTiles = new List<int>();
            _leftColumnTiles = new List<int>();
            _rightColumnTiles = new List<int>();

            int bottomRowStart = 0 + (roomHeight * roomWidth) - roomWidth;
            // floor and ceiling
            for (int i = 0; i < roomWidth; i++)
            {
                if (!fromFileLoad)
                {
                    tiles[i] = new TilePlacement() { isSolid = true, tileNum = 2 };
                    tiles[i + bottomRowStart] = new TilePlacement() { isSolid = true, tileNum = 2 };
                }
                _topRowTiles.Add(i);
                _bottomRowTiles.Add(i + bottomRowStart);
            }
            // left and right
            for (int i = 0; i < roomWidth * roomHeight; i += roomWidth)
            {
                if (!fromFileLoad)
                {
                    tiles[i] = new TilePlacement() { isSolid = true, tileNum = 2 };
                    tiles[i + roomWidth - 1] = new TilePlacement() { isSolid = true, tileNum = 2 };
                }
                _leftColumnTiles.Add(i);
                _rightColumnTiles.Add(i + roomWidth - 1);
            }
        }
        private void DrawDoorConnectors()
        {
            if (doors == null) return;

            foreach (Position d in doors)
            {
                float x = d.column * tileWidth;
                float y = d.row * tileHeight;
                throw new NotImplementedException();
            }
        }
        private void DrawDoors()
        {
            if (doors == null) doors = new List<Position>();
            foreach (Position d in doors)
            {
                float x = (d.column + 1) * tileWidth / Globals.pixelsInAUnityMeter;
                float y = (d.row + 1) * tileHeight / Globals.pixelsInAUnityMeter * -1;

                Transform prefabTransform = _tileSet.transform.Find("Door_origin");
                DrawPrefab(prefabTransform.gameObject, new Vector3(x, y, 20), _tileParent);
            }
        }
        private void DrawGrid()
        {
            const int startX = 0;
            const int startY = 0;

            EraseGrid();

            float lineWidth = (roomWidth + 2) * tileWidth / Globals.pixelsInAUnityMeter;
            float lineHeight = (roomHeight + 2) * tileHeight / Globals.pixelsInAUnityMeter;

            float horizontalLineLeft = startX;
            float horizontalLineRight = startX + lineWidth;
            float verticalLineTop = startY;
            float verticalLineBottom = startY - lineHeight;

            // vertical lines
            for (float x = startX; x <= lineWidth + startX; x += tileWidth / Globals.pixelsInAUnityMeter)
            {
                LineRenderer line = LineRenderer.Instantiate(_gridLinePrefab, _gridParent.transform, false);
                line.SetPosition(0, new Vector2(x, verticalLineTop));
                line.SetPosition(1, new Vector2(x, verticalLineBottom));
            }
            // horizontal lines
            for (float y = startY; y >= startY - lineHeight; y -= tileHeight / Globals.pixelsInAUnityMeter)
            {
                LineRenderer line = LineRenderer.Instantiate(_gridLinePrefab, _gridParent.transform, false);
                line.SetPosition(0, new Vector2(horizontalLineLeft, y));
                line.SetPosition(1, new Vector2(horizontalLineRight, y));
            }

            // draw the trigger squares
            for (float y = startY; y >= startY - lineHeight; y -= tileHeight / Globals.pixelsInAUnityMeter)
            {
                for (float x = startX; x <= lineWidth + startX; x += tileWidth / Globals.pixelsInAUnityMeter)
                {
                    DrawPrefab(_mouseTriggerSquare, new Vector3(x, y, 0), _gridParent);
                }
            }

            // finally set the bool to true so we don't do this every frame
            _hasGridBeenDrawn = true;
        }
        private GameObject DrawPrefab(GameObject prefab, Vector3 positionInGlobalSpace, GameObject parent)
        {
            Quaternion rotation = new Quaternion(0, 0, 0, 0);
            return UnityEngine.Object.Instantiate(prefab, positionInGlobalSpace, rotation, parent.transform);
        }
        private void DrawSprite(int spriteNum, float x, float y)
        {
            Transform prefabTransform = _tileSet.transform.Find(string.Format("{0}_origin", spriteNum));
            DrawPrefab(prefabTransform.gameObject, new Vector3(x, y, 20), _tileParent);
        }
        private void DrawTiles()
        {
            EraseTiles();
            for (int i = 0; i < roomWidth * roomHeight; i++)
            {
                int column = i % roomWidth;
                int row = (int)Math.Floor(i / (float)roomWidth);

                TileNeighbors neighbors = GetTileNeighbors(i);

                int tileNumToDraw = -1;

                bool shouldDrawATile = false;
                if (tiles[i].isSolid)
                {
                    shouldDrawATile = true;



                    if (neighbors.isDown && neighbors.isDownLeft && neighbors.isDownRight && neighbors.isLeft
                        && neighbors.isRight && neighbors.isUp && neighbors.isUpLeft && neighbors.isUpRight)
                    {
                        tileNumToDraw = 1;
                    }
                    else if (neighbors.isUp && neighbors.isDown && neighbors.isUpRight && neighbors.isRight &&
                        neighbors.isDownRight && !neighbors.isLeft)
                    {
                        tileNumToDraw = 2;
                    }
                    else if (!neighbors.isUpRight && neighbors.isDown && neighbors.isUp && neighbors.isRight
                        && neighbors.isDownRight && !neighbors.isLeft)
                    {
                        tileNumToDraw = 3;
                    }
                    else if (neighbors.isUp && neighbors.isDown && neighbors.isUpRight && neighbors.isRight
                        && !neighbors.isDownRight && !neighbors.isLeft)
                    {
                        tileNumToDraw = 4;
                    }
                    else if (!neighbors.isLeft && !neighbors.isUpRight && !neighbors.isDownRight && neighbors.isRight
                        && neighbors.isDown && neighbors.isUp)
                    {
                        tileNumToDraw = 5;
                    }
                    else if (neighbors.isDown && neighbors.isDownLeft && neighbors.isLeft && !neighbors.isRight
                        && neighbors.isUp && neighbors.isUpLeft)
                    {
                        tileNumToDraw = 6;
                    }
                    else if (neighbors.isDown && neighbors.isDownLeft && neighbors.isLeft && !neighbors.isRight
                        && neighbors.isUp && !neighbors.isUpLeft)
                    {
                        tileNumToDraw = 7;
                    }
                    else if (neighbors.isDown && neighbors.isUpLeft && neighbors.isLeft && !neighbors.isRight
                        && neighbors.isUp && !neighbors.isDownLeft)
                    {
                        tileNumToDraw = 8;
                    }
                    else if (neighbors.isDown && !neighbors.isUpLeft && neighbors.isLeft && !neighbors.isRight
                        && neighbors.isUp && !neighbors.isDownLeft)
                    {
                        tileNumToDraw = 9;
                    }
                    else if (!neighbors.isUp && neighbors.isLeft && neighbors.isRight && neighbors.isDown
                        && neighbors.isDownLeft && neighbors.isDownRight)
                    {
                        tileNumToDraw = 10;
                    }
                    else if (!neighbors.isUp && neighbors.isLeft && neighbors.isRight && neighbors.isDown
                        && !neighbors.isDownLeft && neighbors.isDownRight)
                    {
                        tileNumToDraw = 11;
                    }
                    else if (!neighbors.isUp && neighbors.isLeft && neighbors.isRight && neighbors.isDown
                        && neighbors.isDownLeft && !neighbors.isDownRight)
                    {
                        tileNumToDraw = 12;
                    }
                    else if (!neighbors.isUp && neighbors.isLeft && neighbors.isRight && neighbors.isDown
                        && !neighbors.isDownLeft && !neighbors.isDownRight)
                    {
                        tileNumToDraw = 13;
                    }
                    else if (!neighbors.isDown && neighbors.isLeft && neighbors.isRight && neighbors.isUp
                        && neighbors.isUpLeft && neighbors.isUpRight)
                    {
                        tileNumToDraw = 14;
                    }
                    else if (!neighbors.isDown && neighbors.isLeft && neighbors.isRight && neighbors.isUp
                        && !neighbors.isUpLeft && neighbors.isUpRight)
                    {
                        tileNumToDraw = 15;
                    }
                    else if (!neighbors.isDown && neighbors.isLeft && neighbors.isRight && neighbors.isUp
                        && neighbors.isUpLeft && !neighbors.isUpRight)
                    {
                        tileNumToDraw = 16;
                    }
                    else if (!neighbors.isDown && neighbors.isLeft && neighbors.isRight && neighbors.isUp
                        && !neighbors.isUpLeft && !neighbors.isUpRight)
                    {
                        tileNumToDraw = 17;
                    }
                    else if (neighbors.isDown && neighbors.isDownRight && !neighbors.isLeft
                        && neighbors.isRight && !neighbors.isUp)
                    {
                        tileNumToDraw = 18;
                    }
                    else if (neighbors.isDown && !neighbors.isDownRight && !neighbors.isLeft
                        && neighbors.isRight && !neighbors.isUp)
                    {
                        tileNumToDraw = 19;
                    }
                    else if (neighbors.isDown && neighbors.isDownLeft && neighbors.isLeft
                        && !neighbors.isRight && !neighbors.isUp)
                    {
                        tileNumToDraw = 20;
                    }
                    else if (neighbors.isDown && !neighbors.isDownLeft && neighbors.isLeft
                        && !neighbors.isRight && !neighbors.isUp)
                    {
                        tileNumToDraw = 21;
                    }
                    else if (!neighbors.isDown && !neighbors.isLeft
                        && neighbors.isRight && neighbors.isUp && neighbors.isUpRight)
                    {
                        tileNumToDraw = 22;
                    }
                    else if (!neighbors.isDown && !neighbors.isLeft
                        && neighbors.isRight && neighbors.isUp && !neighbors.isUpRight)
                    {
                        tileNumToDraw = 23;
                    }
                    else if (!neighbors.isDown && neighbors.isLeft
                        && !neighbors.isRight && neighbors.isUp && neighbors.isUpLeft)
                    {
                        tileNumToDraw = 24;
                    }
                    else if (!neighbors.isDown && neighbors.isLeft
                        && !neighbors.isRight && neighbors.isUp && !neighbors.isUpLeft)
                    {
                        tileNumToDraw = 25;
                    }
                    else if (!neighbors.isDown && !neighbors.isLeft
                        && !neighbors.isRight && !neighbors.isUp)
                    {
                        tileNumToDraw = 26;
                    }
                    else if (!neighbors.isDown && !neighbors.isLeft
                        && neighbors.isRight && !neighbors.isUp)
                    {
                        tileNumToDraw = 27;
                    }
                    else if (!neighbors.isDown && neighbors.isLeft
                        && !neighbors.isRight && !neighbors.isUp)
                    {
                        tileNumToDraw = 28;
                    }
                    else if (neighbors.isDown && !neighbors.isLeft
                        && !neighbors.isRight && !neighbors.isUp)
                    {
                        tileNumToDraw = 29;
                    }
                    else if (!neighbors.isDown && !neighbors.isLeft
                        && !neighbors.isRight && neighbors.isUp)
                    {
                        tileNumToDraw = 30;
                    }
                    else if (!neighbors.isDown && neighbors.isLeft
                        && neighbors.isRight && !neighbors.isUp)
                    {
                        tileNumToDraw = 31;
                    }
                    else if (neighbors.isDown && !neighbors.isLeft
                        && !neighbors.isRight && neighbors.isUp)
                    {
                        tileNumToDraw = 32;
                    }
                    else if (neighbors.isDown && neighbors.isDownLeft && neighbors.isDownRight && neighbors.isLeft
                        && neighbors.isRight && neighbors.isUp && !neighbors.isUpLeft && neighbors.isUpRight)
                    {
                        tileNumToDraw = 33;
                    }
                    else if (neighbors.isDown && neighbors.isDownLeft && neighbors.isDownRight && neighbors.isLeft
                        && neighbors.isRight && neighbors.isUp && neighbors.isUpLeft && !neighbors.isUpRight)
                    {
                        tileNumToDraw = 34;
                    }
                    else if (neighbors.isDown && !neighbors.isDownLeft && neighbors.isDownRight && neighbors.isLeft
                        && neighbors.isRight && neighbors.isUp && neighbors.isUpLeft && neighbors.isUpRight)
                    {
                        tileNumToDraw = 35;
                    }
                    else if (neighbors.isDown && neighbors.isDownLeft && !neighbors.isDownRight && neighbors.isLeft
                        && neighbors.isRight && neighbors.isUp && neighbors.isUpLeft && neighbors.isUpRight)
                    {
                        tileNumToDraw = 36;
                    }
                    else if (neighbors.isDown && neighbors.isDownLeft && neighbors.isDownRight && neighbors.isLeft
                        && neighbors.isRight && neighbors.isUp && !neighbors.isUpLeft && !neighbors.isUpRight)
                    {
                        tileNumToDraw = 37;
                    }
                    else if (neighbors.isDown && !neighbors.isDownLeft && !neighbors.isDownRight && neighbors.isLeft
                        && neighbors.isRight && neighbors.isUp && neighbors.isUpLeft && neighbors.isUpRight)
                    {
                        tileNumToDraw = 38;
                    }
                    else if (neighbors.isDown && !neighbors.isDownLeft && neighbors.isDownRight && neighbors.isLeft
                        && neighbors.isRight && neighbors.isUp && !neighbors.isUpLeft && neighbors.isUpRight)
                    {
                        tileNumToDraw = 39;
                    }
                    else if (neighbors.isDown && neighbors.isDownLeft && !neighbors.isDownRight && neighbors.isLeft
                        && neighbors.isRight && neighbors.isUp && neighbors.isUpLeft && !neighbors.isUpRight)
                    {
                        tileNumToDraw = 40;
                    }
                    else if (neighbors.isDown && !neighbors.isDownLeft && !neighbors.isDownRight && neighbors.isLeft
                        && neighbors.isRight && neighbors.isUp && !neighbors.isUpLeft && !neighbors.isUpRight)
                    {
                        tileNumToDraw = 41;
                    }
                    if (neighbors.isDown && !neighbors.isDownLeft && !neighbors.isDownRight && neighbors.isLeft
                        && neighbors.isRight && neighbors.isUp && !neighbors.isUpLeft && neighbors.isUpRight)
                    {
                        tileNumToDraw = 55;
                    }
                    if (neighbors.isDown && !neighbors.isDownLeft && !neighbors.isDownRight && neighbors.isLeft
                        && neighbors.isRight && neighbors.isUp && neighbors.isUpLeft && !neighbors.isUpRight)
                    {
                        tileNumToDraw = 56;
                    }
                    if (neighbors.isDown && !neighbors.isDownLeft && neighbors.isDownRight && neighbors.isLeft
                        && neighbors.isRight && neighbors.isUp && !neighbors.isUpLeft && !neighbors.isUpRight)
                    {
                        tileNumToDraw = 57;
                    }
                    if (neighbors.isDown && neighbors.isDownLeft && !neighbors.isDownRight && neighbors.isLeft
                        && neighbors.isRight && neighbors.isUp && !neighbors.isUpLeft && !neighbors.isUpRight)
                    {
                        tileNumToDraw = 58;
                    }
                    tiles[i].tileNum = tileNumToDraw;
                }
                else
                {
                    // check for hollow tiles
                    if (!neighbors.isDown && neighbors.isLeft
                        && !neighbors.isRight && neighbors.isUp && neighbors.isUpLeft)
                    {
                        tileNumToDraw = 42;
                        shouldDrawATile = true;
                    }
                    else if (!neighbors.isDown && !neighbors.isLeft
                        && neighbors.isRight && neighbors.isUp && neighbors.isUpRight)
                    {
                        tileNumToDraw = 43;
                        shouldDrawATile = true;
                    }
                    else if (neighbors.isDown && neighbors.isDownLeft && neighbors.isLeft
                        && !neighbors.isRight && !neighbors.isUp)
                    {
                        tileNumToDraw = 44;
                        shouldDrawATile = true;
                    }
                    else if (neighbors.isDown && neighbors.isDownRight && !neighbors.isLeft
                        && neighbors.isRight && !neighbors.isUp)
                    {
                        tileNumToDraw = 45;
                        shouldDrawATile = true;
                    }
                    else if (!neighbors.isDown && neighbors.isLeft
                        && neighbors.isRight && neighbors.isUp && neighbors.isUpLeft && neighbors.isUpRight)
                    {
                        tileNumToDraw = 46;
                        shouldDrawATile = true;
                    }
                    else if (!neighbors.isUp && neighbors.isLeft
                        && neighbors.isRight && neighbors.isDown && neighbors.isDownLeft && neighbors.isDownRight)
                    {
                        tileNumToDraw = 47;
                        shouldDrawATile = true;
                    }
                    else if (neighbors.isDown && neighbors.isDownLeft && neighbors.isLeft
                        && !neighbors.isRight && neighbors.isUp && neighbors.isUpLeft)
                    {
                        tileNumToDraw = 48;
                        shouldDrawATile = true;
                    }
                    else if (neighbors.isDown && neighbors.isDownRight && neighbors.isRight
                        && !neighbors.isLeft && neighbors.isUp && neighbors.isUpRight)
                    {
                        tileNumToDraw = 49;
                        shouldDrawATile = true;
                    }
                    else if (neighbors.isDown && neighbors.isDownLeft && neighbors.isDownRight && neighbors.isLeft
                        && neighbors.isRight && neighbors.isUp && !neighbors.isUpLeft && neighbors.isUpRight)
                    {
                        tileNumToDraw = 50;
                        shouldDrawATile = true;
                    }
                    else if (neighbors.isDown && neighbors.isDownLeft && neighbors.isDownRight && neighbors.isLeft
                        && neighbors.isRight && neighbors.isUp && neighbors.isUpLeft && !neighbors.isUpRight)
                    {
                        tileNumToDraw = 51;
                        shouldDrawATile = true;
                    }
                    else if (neighbors.isDown && !neighbors.isDownLeft && neighbors.isDownRight && neighbors.isLeft
                        && neighbors.isRight && neighbors.isUp && neighbors.isUpLeft && neighbors.isUpRight)
                    {
                        tileNumToDraw = 52;
                        shouldDrawATile = true;
                    }
                    else if (neighbors.isDown && neighbors.isDownLeft && !neighbors.isDownRight && neighbors.isLeft
                        && neighbors.isRight && neighbors.isUp && neighbors.isUpLeft && neighbors.isUpRight)
                    {
                        tileNumToDraw = 53;
                        shouldDrawATile = true;
                    }
                    else if (neighbors.isDown && neighbors.isDownLeft && neighbors.isDownRight && neighbors.isLeft
                        && neighbors.isRight && neighbors.isUp && neighbors.isUpLeft && neighbors.isUpRight)
                    {
                        tileNumToDraw = 54;
                        shouldDrawATile = true;
                    }
                    else if (neighbors.isDown && neighbors.isDownLeft && !neighbors.isDownRight && neighbors.isLeft
                        && neighbors.isRight && neighbors.isUp && !neighbors.isUpLeft && neighbors.isUpRight)
                    {
                        tileNumToDraw = 59;
                        shouldDrawATile = true;
                    }
                    else if (neighbors.isDown && !neighbors.isDownLeft && neighbors.isDownRight && neighbors.isLeft
                        && neighbors.isRight && neighbors.isUp && neighbors.isUpLeft && !neighbors.isUpRight)
                    {
                        tileNumToDraw = 60;
                        shouldDrawATile = true;
                    }
                    // update the array with a tile to draw
                    if (shouldDrawATile)
                    {
                        tiles[i].tileNum = tileNumToDraw;
                    }
                    else
                    {
                        tiles[i].tileNum = -1;
                    }
                }
                if (shouldDrawATile)
                {
                    DrawSprite(
                        tileNumToDraw, 
                        (column + 1) * tileWidth / Globals.pixelsInAUnityMeter, 
                        (row + 1) * tileHeight / Globals.pixelsInAUnityMeter * -1);
                }
            }

        }
        private void EraseAllFromParent(GameObject parent)
        {
            var children = new List<GameObject>();
            foreach (Transform child in parent.transform) children.Add(child.gameObject);
            children.ForEach(child => GameObject.Destroy(child));
        }
        private void EraseGrid()
        {
            EraseAllFromParent(_gridParent);
        }
        private void EraseTiles()
        {
            EraseAllFromParent(_tileParent);
        }
        private Position GetSpriteSheetPositionOfSpriteNum(int spriteNum)
        {
            Position position = new Position() { column = 0, row = 0 };
            switch (spriteNum)
            {
                case 0:
                    position.column = 0;
                    position.row = 0;
                    break;
                case 1:
                    position.column = 1;
                    position.row = 0;
                    break;
                case 2:
                    position.column = 2;
                    position.row = 0;
                    break;
                case 3:
                    position.column = 3;
                    position.row = 0;
                    break;
                case 4:
                    position.column = 4;
                    position.row = 0;
                    break;
                case 5:
                    position.column = 5;
                    position.row = 0;
                    break;
                case 6:
                    position.column = 6;
                    position.row = 0;
                    break;

                case 7:
                    position.column = 0;
                    position.row = 1;
                    break;
                case 8:
                    position.column = 1;
                    position.row = 1;
                    break;
                case 9:
                    position.column = 2;
                    position.row = 1;
                    break;
                case 10:
                    position.column = 3;
                    position.row = 1;
                    break;
                case 11:
                    position.column = 4;
                    position.row = 1;
                    break;
                case 12:
                    position.column = 5;
                    position.row = 1;
                    break;
                case 13:
                    position.column = 6;
                    position.row = 1;
                    break;

                case 14:
                    position.column = 0;
                    position.row = 2;
                    break;
                case 15:
                    position.column = 1;
                    position.row = 2;
                    break;
                case 16:
                    position.column = 2;
                    position.row = 2;
                    break;
                case 17:
                    position.column = 3;
                    position.row = 2;
                    break;
                case 18:
                    position.column = 4;
                    position.row = 2;
                    break;
                case 19:
                    position.column = 5;
                    position.row = 2;
                    break;
                case 20:
                    position.column = 6;
                    position.row = 2;
                    break;

                case 21:
                    position.column = 0;
                    position.row = 3;
                    break;
                case 22:
                    position.column = 1;
                    position.row = 3;
                    break;
                case 23:
                    position.column = 2;
                    position.row = 3;
                    break;
                case 24:
                    position.column = 3;
                    position.row = 3;
                    break;
                case 25:
                    position.column = 4;
                    position.row = 3;
                    break;
                case 26:
                    position.column = 5;
                    position.row = 3;
                    break;
                case 27:
                    position.column = 6;
                    position.row = 3;
                    break;

                case 28:
                    position.column = 0;
                    position.row = 4;
                    break;
                case 29:
                    position.column = 1;
                    position.row = 4;
                    break;
                case 30:
                    position.column = 2;
                    position.row = 4;
                    break;
                case 31:
                    position.column = 3;
                    position.row = 4;
                    break;
                case 32:
                    position.column = 4;
                    position.row = 4;
                    break;
                case 33:
                    position.column = 5;
                    position.row = 4;
                    break;
                case 34:
                    position.column = 6;
                    position.row = 4;
                    break;

                case 35:
                    position.column = 0;
                    position.row = 5;
                    break;
                case 36:
                    position.column = 1;
                    position.row = 5;
                    break;
                case 37:
                    position.column = 2;
                    position.row = 5;
                    break;
                case 38:
                    position.column = 3;
                    position.row = 5;
                    break;
                case 39:
                    position.column = 4;
                    position.row = 5;
                    break;
                case 40:
                    position.column = 5;
                    position.row = 5;
                    break;
                case 41:
                    position.column = 6;
                    position.row = 5;
                    break;

                case 42:
                    position.column = 0;
                    position.row = 6;
                    break;
                case 43:
                    position.column = 1;
                    position.row = 6;
                    break;
                case 44:
                    position.column = 2;
                    position.row = 6;
                    break;
                case 45:
                    position.column = 3;
                    position.row = 6;
                    break;
                case 46:
                    position.column = 4;
                    position.row = 6;
                    break;
                case 47:
                    position.column = 5;
                    position.row = 6;
                    break;
                case 48:
                    position.column = 6;
                    position.row = 6;
                    break;

                case 49:
                    position.column = 0;
                    position.row = 7;
                    break;
                case 50:
                    position.column = 1;
                    position.row = 7;
                    break;
                case 51:
                    position.column = 2;
                    position.row = 7;
                    break;
                case 52:
                    position.column = 3;
                    position.row = 7;
                    break;
                case 53:
                    position.column = 4;
                    position.row = 7;
                    break;
                case 54:
                    position.column = 5;
                    position.row = 7;
                    break;
                case 55:
                    position.column = 6;
                    position.row = 7;
                    break;
                case 56:
                    position.column = 0;
                    position.row = 8;
                    break;
                case 57:
                    position.column = 1;
                    position.row = 8;
                    break;
                case 58:
                    position.column = 2;
                    position.row = 8;
                    break;
                case 59:
                    position.column = 3;
                    position.row = 8;
                    break;
                case 60:
                    position.column = 4;
                    position.row = 8;
                    break;

            }

            return position;
        }
        private TileNeighbors GetTileNeighbors(int tileNum)
        {
            TileNeighbors neighbors = new TileNeighbors();
            bool isInTopRow = _topRowTiles.Contains(tileNum);
            bool isInBottomRow = _bottomRowTiles.Contains(tileNum);
            bool isInLeftColumn = _leftColumnTiles.Contains(tileNum);
            bool isInRightColumn = _rightColumnTiles.Contains(tileNum);

            // up left
            if (isInTopRow || isInLeftColumn)
            {
                neighbors.isUpLeft = true;
            }
            else
            {
                int upLeftNum = tileNum - 1 - roomWidth;
                neighbors.isUpLeft = tiles[upLeftNum].isSolid;
                if (IsDoorAtPosition(upLeftNum)) neighbors.isUpLeft = true;
            }
            // up
            if (isInTopRow)
            {
                neighbors.isUp = true;
            }
            else
            {
                int upNum = tileNum - roomWidth;
                neighbors.isUp = tiles[upNum].isSolid;
                if (IsDoorAtPosition(upNum)) neighbors.isUp = true;
            }
            // upRight
            if (isInTopRow || isInRightColumn)
            {
                neighbors.isUpRight = true;
            }
            else
            {
                int upRightNum = tileNum - roomWidth + 1;
                neighbors.isUpRight = tiles[upRightNum].isSolid;
                if (IsDoorAtPosition(upRightNum)) neighbors.isUpRight = true;
            }
            // left
            if (isInLeftColumn)
            {
                neighbors.isLeft = true;
            }
            else
            {
                int leftNum = tileNum - 1;
                neighbors.isLeft = tiles[leftNum].isSolid;
                if (IsDoorAtPosition(leftNum)) neighbors.isLeft = true;
            }
            // right
            if (isInRightColumn)
            {
                neighbors.isRight = true;
            }
            else
            {
                int rightNum = tileNum + 1;
                neighbors.isRight = tiles[rightNum].isSolid;
                if (IsDoorAtPosition(rightNum)) neighbors.isRight = true;
            }
            // down left
            if (isInBottomRow || isInLeftColumn)
            {
                neighbors.isDownLeft = true;
            }
            else
            {
                int downLeftNum = tileNum - 1 + roomWidth;
                neighbors.isDownLeft = tiles[downLeftNum].isSolid;
                if (IsDoorAtPosition(downLeftNum)) neighbors.isDownLeft = true;
            }
            // down
            if (isInBottomRow)
            {
                neighbors.isDown = true;
            }
            else
            {
                int downNum = tileNum + roomWidth;
                neighbors.isDown = tiles[downNum].isSolid;
                if (IsDoorAtPosition(downNum)) neighbors.isDown = true;
            }
            // downRight
            if (isInBottomRow || isInRightColumn)
            {
                neighbors.isDownRight = true;
            }
            else
            {
                int downRightNum = tileNum + roomWidth + 1;
                neighbors.isDownRight = tiles[downRightNum].isSolid;
                if (IsDoorAtPosition(downRightNum)) neighbors.isDownRight = true;
            }
            return neighbors;
        }
        private bool IsDoorAtPosition(int tileIndex)
        {
            foreach (var door in doors)
            {
                // get the index of the left brick in each of the door's rows
                int indexAtDoorUL = (door.row * roomWidth) + door.column;
                int indexAtDoorML = indexAtDoorUL + roomWidth;
                int indexAtDoorBL = indexAtDoorML + roomWidth;

                // if door is on the left wall, check the right side of it
                if (door.column == -1)
                {
                    // upper row
                    if (indexAtDoorUL + 1 == tileIndex) return true;
                    // middle row
                    if (indexAtDoorML + 1 == tileIndex) return true;
                    // bottom row
                    if (indexAtDoorBL + 1 == tileIndex) return true;
                }
                // if door is on the right wall, check the left side of it
                else if (door.column == roomWidth - 1)
                {
                    // upper row
                    if (indexAtDoorUL == tileIndex) return true;
                    // middle row
                    if (indexAtDoorML == tileIndex) return true;
                    // bottom row
                    if (indexAtDoorBL == tileIndex) return true;
                }
            }
            return false;
        }
    }
}

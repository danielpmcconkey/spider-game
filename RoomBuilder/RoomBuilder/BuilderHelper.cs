using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Text.Json;

namespace RoomBuilder
{
    [Serializable]
    public struct Position
    {
        public int column { get; set; }
        public int row { get; set; }
    }
    [Serializable]
    public struct TilePlacement
    {
        public bool isSolid { get; set; }
        public int tileNum { get; set; }
    }
    [Serializable]
    public struct RoomSave
    {
        public string roomName { get; set; }
        public int tileWidth { get; set; }
        public int tileHeight { get; set; }
        public int roomWidth { get; set; }
        public int roomHeight { get; set; }
        public TilePlacement[] tiles { get; set; }
        public Position[] doors { get; set; }
    }
    
    public struct TileNeighbors
    {
        public bool isUpLeft;
        public bool isUp;
        public bool isUpRight;
        public bool isLeft;
        public bool isRight;
        public bool isDownLeft;
        public bool isDown;
        public bool isDownRight;
    }
    public class BuilderHelper
    {
        public Image spriteSheetMainImage;
        public Image doorImage;
        public int tileWidth = 48;
        public int tileHeight = 48;
        public string roomName;
        public int roomWidth = -1;
        public int roomHeight = -1;

        private List<int> _topRowTiles;
        private List<int> _bottomRowTiles;
        private List<int> _leftColumnTiles;
        private List<int> _rightColumnTiles;

        private TilePlacement[] _tiles;
        private List<Position> _doors;

        public string SerializeRoom()
        {
            RoomSave save = new RoomSave();
            save.roomName = roomName;
            save.tileWidth = tileWidth;
            save.tileHeight = tileHeight;
            save.roomWidth = roomWidth;
            save.roomHeight = roomHeight;
            save.tiles = _tiles;
            save.doors = _doors.ToArray();
            string json = JsonSerializer.Serialize(save);
            return json;
        }
        public void AddDoor(int row, int column)
        {
            _doors.Add(new Position() { row = row, column = column });
        }
        public void DeSerializeRoom(string jsonFromFile)
        {
            RoomSave restore = JsonSerializer.Deserialize<RoomSave>(jsonFromFile);
            roomName = restore.roomName;
            tileWidth = restore.tileWidth;
            tileHeight = restore.tileHeight;
            roomWidth = restore.roomWidth;
            roomHeight = restore.roomHeight;
            _tiles = restore.tiles;
            if (restore.doors != null) _doors = new List<Position>(restore.doors);
            else _doors = new List<Position>();
            AddPerimeterTiles(true);
        }
        public void LoadSpriteSheetImage(string path)
        {
            spriteSheetMainImage = Image.FromFile(path);
        }
        public void LoadDoorImage(string path)
        {
            doorImage = Image.FromFile(path);
        }
        public void SetRoomDimensions(int width, int height)
        {
            roomWidth = width;
            roomHeight = height;
            _tiles = new TilePlacement[width * height];
            _doors = new List<Position>();
            AddPerimeterTiles();
        }
        public void DrawRoom(PaintEventArgs e)
        {
            if (roomWidth > 0 && roomHeight > 0)
            {
                DrawTiles(e);
                DrawGrid(e);
                DrawDoors(e);
            }
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
                    _tiles[i] = new TilePlacement() { isSolid = true, tileNum = 2 };
                    _tiles[i + bottomRowStart] = new TilePlacement() { isSolid = true, tileNum = 2 };
                }
                _topRowTiles.Add(i);
                _bottomRowTiles.Add(i + bottomRowStart);
            }
            // left and right
            for (int i = 0; i < roomWidth * roomHeight; i+= roomWidth)
            {
                if (!fromFileLoad)
                {
                    _tiles[i] = new TilePlacement() { isSolid = true, tileNum = 2 };
                    _tiles[i + roomWidth - 1] = new TilePlacement() { isSolid = true, tileNum = 2 };
                }
                _leftColumnTiles.Add(i);
                _rightColumnTiles.Add(i + roomWidth - 1);
            }
        }
        private void DrawDoors(PaintEventArgs e)
        {
            if (_doors == null) _doors = new List<Position>();
            foreach(Position d in _doors)
            {
                float x = d.column * tileWidth;
                float y = d.row * tileHeight;
                RectangleF srcRect = new RectangleF(0, 0, 2 * tileWidth, 3 * tileHeight);
                GraphicsUnit units = GraphicsUnit.Pixel;
                e.Graphics.DrawImage(doorImage, x, y, srcRect, units);
            }
        }
        private void DrawTiles(PaintEventArgs e)
        {
            
            for(int i = 0; i < roomWidth * roomHeight; i++)
            {
                int column = i % roomWidth;
                int row = (int)Math.Floor(i / (float)roomWidth);

                TileNeighbors neighbors = GetTileNeighbors(i);

                int tileNumToDraw = -1;

                bool shouldDrawATile = false;
                if (_tiles[i].isSolid)
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
                        && !neighbors.isRight && !neighbors.isUp )
                    {
                        tileNumToDraw = 20;
                    }
                    else if (neighbors.isDown && !neighbors.isDownLeft && neighbors.isLeft
                        && !neighbors.isRight && !neighbors.isUp)
                    {
                        tileNumToDraw = 21;
                    }
                    else if (!neighbors.isDown && !neighbors.isLeft
                        && neighbors.isRight && neighbors.isUp  && neighbors.isUpRight)
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
                        && neighbors.isRight && neighbors.isUp && !neighbors.isUpLeft && neighbors.isUpRight )
                    {
                        tileNumToDraw = 33;
                    }
                    else if (neighbors.isDown && neighbors.isDownLeft && neighbors.isDownRight && neighbors.isLeft
                        && neighbors.isRight && neighbors.isUp && neighbors.isUpLeft && !neighbors.isUpRight)
                    {
                        tileNumToDraw = 34;
                    }
                    else if (neighbors.isDown && !neighbors.isDownLeft && neighbors.isDownRight && neighbors.isLeft
                        && neighbors.isRight && neighbors.isUp && neighbors.isUpLeft  && neighbors.isUpRight)
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
                    _tiles[i].tileNum = tileNumToDraw;
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
                        && !neighbors.isRight && !neighbors.isUp )
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
                        _tiles[i].tileNum = tileNumToDraw;
                    }
                    else
                    {
                        _tiles[i].tileNum = -1;
                    }
                }
                if (shouldDrawATile) DrawSprite(e, tileNumToDraw, column * tileWidth, row * tileHeight);
            }

        }
        private void DrawGrid(PaintEventArgs e)
        {
            const int startX = 0;
            const int startY = 0;
            int lineWidth = roomWidth * tileWidth;
            int lineHeight = roomHeight * tileHeight;
            using (Pen pen = new Pen( Color.Gray, 1))
            {
                for (int x = startX; x <= lineWidth + startX; x += tileWidth)
                {

                    e.Graphics.DrawLine(pen, x, startY, x, startY + lineHeight);
                }
                for (int y = startY; y <= lineHeight + startY; y += tileHeight)
                {

                    e.Graphics.DrawLine(pen, startX, y, startX + lineWidth, y);
                }
            }
        }
        public void DrawSprite(PaintEventArgs e, int spriteNum, float x, float y)
        {
            Position position = GetSpriteSheetPositionOfSpriteNum(spriteNum);
            RectangleF srcRect = new RectangleF(position.column * tileWidth, position.row * tileHeight, tileWidth, tileHeight);
            GraphicsUnit units = GraphicsUnit.Pixel;
            e.Graphics.DrawImage(spriteSheetMainImage, x, y, srcRect, units);
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

            }

            return position;
        }
        public int GetTileNumbFromMousePosition(Point mouseDownLocation)
        {
            int column = (int)Math.Floor(mouseDownLocation.X / (float)tileWidth);
            int row = (int)Math.Floor(mouseDownLocation.Y / (float)tileHeight);
            int tileNum = (row * roomWidth) + column;
            return tileNum;
        }
        public void ReverseTile(int tileNum)
        {
            if (_tiles[tileNum].isSolid)
            {
                _tiles[tileNum] = new TilePlacement() { isSolid = false, tileNum = -1 };
            }
            else
            {
                _tiles[tileNum] = new TilePlacement() { isSolid = true, tileNum = 2 };
            }
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
                neighbors.isUpLeft = _tiles[upLeftNum].isSolid;
            }
            // up
            if (isInTopRow)
            {
                neighbors.isUp = true;
            }
            else
            {
                int upNum = tileNum - roomWidth;
                neighbors.isUp = _tiles[upNum].isSolid;
            }
            // upRight
            if (isInTopRow || isInRightColumn)
            {
                neighbors.isUpRight = true;
            }
            else
            {
                int upRightNum = tileNum - roomWidth + 1;
                neighbors.isUpRight = _tiles[upRightNum].isSolid;
            }
            // left
            if (isInLeftColumn)
            {
                neighbors.isLeft = true;
            }
            else
            {
                int leftNum = tileNum - 1;
                neighbors.isLeft = _tiles[leftNum].isSolid;
            }
            // right
            if (isInRightColumn)
            {
                neighbors.isRight = true;
            }
            else
            {
                int rightNum = tileNum + 1;
                neighbors.isRight = _tiles[rightNum].isSolid;
            }
            // down left
            if (isInBottomRow || isInLeftColumn)
            {
                neighbors.isDownLeft = true;
            }
            else
            {
                int downLeftNum = tileNum - 1 + roomWidth;
                neighbors.isDownLeft = _tiles[downLeftNum].isSolid;
            }
            // down
            if (isInBottomRow)
            {
                neighbors.isDown = true;
            }
            else
            {
                int downNum = tileNum + roomWidth;
                neighbors.isDown = _tiles[downNum].isSolid;
            }
            // downRight
            if (isInBottomRow || isInRightColumn)
            {
                neighbors.isDownRight = true;
            }
            else
            {
                int downRightNum = tileNum + roomWidth + 1;
                neighbors.isDownRight = _tiles[downRightNum].isSolid;
            }
            return neighbors;
        }
    }
}

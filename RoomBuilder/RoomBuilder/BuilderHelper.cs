using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace RoomBuilder
{
    public struct Position { public int column; public int row; }
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
        public Image image;
        public int tileWidth = 48;
        public int tileHeight = 48;

        private int _roomWidth = -1;
        private int _roomHeight = -1;
        private List<int> _topRowTiles;
        private List<int> _bottomRowTiles;
        private List<int> _leftColumnTiles;
        private List<int> _rightColumnTiles;

        private (bool isSolid, int tileNum)[] _tiles;

        public void LoadImage(string path)
        {
            image = Image.FromFile(path);
        }
        public void SetRoomDimensions(int width, int height)
        {
            _roomWidth = width;
            _roomHeight = height;
            _tiles = new (bool,int) [width * height];
            AddPerimeterTiles();
        }
        public void DrawRoom(PaintEventArgs e)
        {
            if (_roomWidth > 0 && _roomHeight > 0)
            {
                DrawTiles(e);
                DrawGrid(e);
            }
        }
        private void AddPerimeterTiles()
        {
            _topRowTiles = new List<int>();
            _bottomRowTiles = new List<int>();
            _leftColumnTiles = new List<int>();
            _rightColumnTiles = new List<int>();

            int bottomRowStart = 0 + (_roomHeight * _roomWidth) - _roomWidth;
            // floor and ceiling
            for (int i = 0; i < _roomWidth; i++)
            {
                _tiles[i] = (true, 2);
                _tiles[i + bottomRowStart] = (true, 2);
                _topRowTiles.Add(i);
                _bottomRowTiles.Add(i + bottomRowStart);
            }
            // left and right
            for (int i = 0; i < _roomWidth * _roomHeight; i+= _roomWidth)
            {
                _tiles[i] = (true, 2);
                _tiles[i + _roomWidth - 1] = (true, 2);
                _leftColumnTiles.Add(i);
                _rightColumnTiles.Add(i + _roomWidth - 1);
            }
        }
        private void DrawTiles(PaintEventArgs e)
        {
            
            for(int i = 0; i < _roomWidth * _roomHeight; i++)
            {
                int column = i % _roomWidth;
                int row = (int)Math.Floor(i / (float)_roomWidth);

                TileNeighbors neighbors = GetTileNeighbors(i);

                int tileNumToDraw = -1;

                bool shouldDrawATile = false;
                if (_tiles[i].isSolid)
                {
                    shouldDrawATile = true;



                    if (neighbors.isDown && neighbors.isDownLeft && neighbors.isDownRight && neighbors.isLeft 
                        && neighbors.isRight && neighbors.isUp && neighbors.isUpLeft && neighbors.isUpRight)
                    {
                        tileNumToDraw = 2;
                    }
                    else if (neighbors.isUp && neighbors.isDown && neighbors.isUpRight && neighbors.isRight &&
                        neighbors.isDownRight && !neighbors.isLeft)
                    {
                        tileNumToDraw = 11;
                    }
                    else if (!neighbors.isUpRight && neighbors.isDown && neighbors.isUp && neighbors.isRight
                        && neighbors.isDownRight && !neighbors.isLeft)
                    {
                        tileNumToDraw = 44;
                    }
                    else if (neighbors.isUp && neighbors.isDown && neighbors.isUpRight && neighbors.isRight
                        && !neighbors.isDownRight && !neighbors.isLeft)
                    {
                        tileNumToDraw = 45;
                    }
                    else if (!neighbors.isLeft && !neighbors.isUpRight && !neighbors.isDownRight && neighbors.isRight
                        && neighbors.isDown && neighbors.isUp)
                    {
                        tileNumToDraw = 39;
                    }
                    else if (neighbors.isDown && neighbors.isDownLeft && neighbors.isLeft && !neighbors.isRight
                        && neighbors.isUp && neighbors.isUpLeft)
                    {
                        tileNumToDraw = 12;
                    }
                    else if (neighbors.isDown && neighbors.isDownLeft && neighbors.isLeft && !neighbors.isRight
                        && neighbors.isUp && !neighbors.isUpLeft)
                    {
                        tileNumToDraw = 42;
                    }
                    else if (neighbors.isDown && neighbors.isUpLeft && neighbors.isLeft && !neighbors.isRight
                        && neighbors.isUp && !neighbors.isDownLeft)
                    {
                        tileNumToDraw = 43;
                    }
                    else if (neighbors.isDown && !neighbors.isUpLeft && neighbors.isLeft && !neighbors.isRight
                        && neighbors.isUp && !neighbors.isDownLeft)
                    {
                        tileNumToDraw = 40;
                    }
                    else if (!neighbors.isUp && neighbors.isLeft && neighbors.isRight && neighbors.isDown 
                        && neighbors.isDownLeft && neighbors.isDownRight)
                    {
                        tileNumToDraw = 13;
                    }
                    else if (!neighbors.isUp && neighbors.isLeft && neighbors.isRight && neighbors.isDown
                        && !neighbors.isDownLeft && neighbors.isDownRight)
                    {
                        tileNumToDraw = 47;
                    }
                    else if (!neighbors.isUp && neighbors.isLeft && neighbors.isRight && neighbors.isDown
                        && neighbors.isDownLeft && !neighbors.isDownRight)
                    {
                        tileNumToDraw = 46;
                    }
                    else if (!neighbors.isUp && neighbors.isLeft && neighbors.isRight && neighbors.isDown
                        && !neighbors.isDownLeft && !neighbors.isDownRight)
                    {
                        tileNumToDraw = 53;
                    }
                    else if (!neighbors.isDown && neighbors.isLeft && neighbors.isRight && neighbors.isUp 
                        && neighbors.isUpLeft && neighbors.isUpRight)
                    {
                        tileNumToDraw = 14;
                    }
                    else if (!neighbors.isDown && neighbors.isLeft && neighbors.isRight && neighbors.isUp 
                        && !neighbors.isUpLeft && neighbors.isUpRight)
                    {
                        tileNumToDraw = 41;
                    }
                    else if (!neighbors.isDown && neighbors.isLeft && neighbors.isRight && neighbors.isUp
                        && neighbors.isUpLeft && !neighbors.isUpRight)
                    {
                        tileNumToDraw = 54;
                    }
                    else if (!neighbors.isDown && neighbors.isLeft && neighbors.isRight && neighbors.isUp
                        && !neighbors.isUpLeft && !neighbors.isUpRight)
                    {
                        tileNumToDraw = 52;
                    }
                    else if (neighbors.isDown && neighbors.isDownRight && !neighbors.isLeft
                        && neighbors.isRight && !neighbors.isUp)
                    {
                        tileNumToDraw = 5;
                    }
                    else if (neighbors.isDown && !neighbors.isDownRight && !neighbors.isLeft
                        && neighbors.isRight && !neighbors.isUp)
                    {
                        tileNumToDraw = 36;
                    }
                    else if (neighbors.isDown && neighbors.isDownLeft && neighbors.isLeft
                        && !neighbors.isRight && !neighbors.isUp )
                    {
                        tileNumToDraw = 6;
                    }
                    else if (neighbors.isDown && !neighbors.isDownLeft && neighbors.isLeft
                        && !neighbors.isRight && !neighbors.isUp)
                    {
                        tileNumToDraw = 35;
                    }
                    else if (!neighbors.isDown && !neighbors.isLeft
                        && neighbors.isRight && neighbors.isUp  && neighbors.isUpRight)
                    {
                        tileNumToDraw = 3;
                    }
                    else if (!neighbors.isDown && !neighbors.isLeft
                        && neighbors.isRight && neighbors.isUp && !neighbors.isUpRight)
                    {
                        tileNumToDraw = 37;
                    }
                    else if (!neighbors.isDown && neighbors.isLeft
                        && !neighbors.isRight && neighbors.isUp && neighbors.isUpLeft)
                    {
                        tileNumToDraw = 4;
                    }
                    else if (!neighbors.isDown && neighbors.isLeft
                        && !neighbors.isRight && neighbors.isUp && !neighbors.isUpLeft)
                    {
                        tileNumToDraw = 38;
                    }
                    else if (!neighbors.isDown && !neighbors.isLeft
                        && !neighbors.isRight && !neighbors.isUp)
                    {
                        tileNumToDraw = 1;
                    }
                    else if (!neighbors.isDown && !neighbors.isLeft
                        && neighbors.isRight && !neighbors.isUp)
                    {
                        tileNumToDraw = 8;
                    }
                    else if (!neighbors.isDown && neighbors.isLeft
                        && !neighbors.isRight && !neighbors.isUp)
                    {
                        tileNumToDraw = 9;
                    }
                    else if (neighbors.isDown && !neighbors.isLeft
                        && !neighbors.isRight && !neighbors.isUp)
                    {
                        tileNumToDraw = 10;
                    }
                    else if (!neighbors.isDown && !neighbors.isLeft
                        && !neighbors.isRight && neighbors.isUp)
                    {
                        tileNumToDraw = 7;
                    }
                    else if (!neighbors.isDown && neighbors.isLeft
                        && neighbors.isRight && !neighbors.isUp)
                    {
                        tileNumToDraw = 15;
                    }
                    else if (neighbors.isDown && !neighbors.isLeft
                        && !neighbors.isRight && neighbors.isUp)
                    {
                        tileNumToDraw = 16;
                    }
                    else if (neighbors.isDown && neighbors.isDownLeft && neighbors.isDownRight && neighbors.isLeft
                        && neighbors.isRight && neighbors.isUp && !neighbors.isUpLeft && neighbors.isUpRight )
                    {
                        tileNumToDraw = 17;
                    }
                    else if (neighbors.isDown && neighbors.isDownLeft && neighbors.isDownRight && neighbors.isLeft
                        && neighbors.isRight && neighbors.isUp && neighbors.isUpLeft && !neighbors.isUpRight)
                    {
                        tileNumToDraw = 18;
                    }
                    else if (neighbors.isDown && !neighbors.isDownLeft && neighbors.isDownRight && neighbors.isLeft
                        && neighbors.isRight && neighbors.isUp && neighbors.isUpLeft  && neighbors.isUpRight)
                    {
                        tileNumToDraw = 19;
                    }
                    else if (neighbors.isDown && neighbors.isDownLeft && !neighbors.isDownRight && neighbors.isLeft
                        && neighbors.isRight && neighbors.isUp && neighbors.isUpLeft && neighbors.isUpRight)
                    {
                        tileNumToDraw = 20;
                    }
                    else if (neighbors.isDown && neighbors.isDownLeft && neighbors.isDownRight && neighbors.isLeft
                        && neighbors.isRight && neighbors.isUp && !neighbors.isUpLeft && !neighbors.isUpRight)
                    {
                        tileNumToDraw = 22;
                    }
                    else if (neighbors.isDown && !neighbors.isDownLeft && neighbors.isDownRight && neighbors.isLeft
                        && neighbors.isRight && neighbors.isUp && !neighbors.isUpLeft && neighbors.isUpRight)
                    {
                        tileNumToDraw = 24;
                    }
                    else if (neighbors.isDown && !neighbors.isDownLeft && !neighbors.isDownRight && neighbors.isLeft
                        && neighbors.isRight && neighbors.isUp && neighbors.isUpLeft && neighbors.isUpRight)
                    {
                        tileNumToDraw = 23;
                    }
                    else if (neighbors.isDown && neighbors.isDownLeft && !neighbors.isDownRight && neighbors.isLeft
                        && neighbors.isRight && neighbors.isUp && neighbors.isUpLeft && !neighbors.isUpRight)
                    {
                        tileNumToDraw = 25;
                    }
                    else if (neighbors.isDown && !neighbors.isDownLeft && !neighbors.isDownRight && neighbors.isLeft
                        && neighbors.isRight && neighbors.isUp && !neighbors.isUpLeft && !neighbors.isUpRight)
                    {
                        tileNumToDraw = 21;
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
                        tileNumToDraw = 26;
                        shouldDrawATile = true;
                    }
                    else if (!neighbors.isDown && !neighbors.isLeft
                        && neighbors.isRight && neighbors.isUp && neighbors.isUpRight)
                    {
                        tileNumToDraw = 27;
                        shouldDrawATile = true;
                    }
                    else if (neighbors.isDown && neighbors.isDownLeft && neighbors.isLeft
                        && !neighbors.isRight && !neighbors.isUp )
                    {
                        tileNumToDraw = 28;
                        shouldDrawATile = true;
                    }
                    else if (neighbors.isDown && neighbors.isDownRight && !neighbors.isLeft
                        && neighbors.isRight && !neighbors.isUp)
                    {
                        tileNumToDraw = 29;
                        shouldDrawATile = true;
                    }
                    else if (!neighbors.isDown && neighbors.isLeft
                        && neighbors.isRight && neighbors.isUp && neighbors.isUpLeft && neighbors.isUpRight)
                    {
                        tileNumToDraw = 31;
                        shouldDrawATile = true;
                    }
                    else if (!neighbors.isUp && neighbors.isLeft
                        && neighbors.isRight && neighbors.isDown && neighbors.isDownLeft && neighbors.isDownRight)
                    {
                        tileNumToDraw = 32;
                        shouldDrawATile = true;
                    }
                    else if (neighbors.isDown && neighbors.isDownLeft && neighbors.isLeft
                        && !neighbors.isRight && neighbors.isUp && neighbors.isUpLeft)
                    {
                        tileNumToDraw = 33;
                        shouldDrawATile = true;
                    }
                    else if (neighbors.isDown && neighbors.isDownRight && neighbors.isRight
                        && !neighbors.isLeft && neighbors.isUp && neighbors.isUpRight)
                    {
                        tileNumToDraw = 34;
                        shouldDrawATile = true;
                    }
                    else if (neighbors.isDown && neighbors.isDownLeft && neighbors.isDownRight && neighbors.isLeft
                        && neighbors.isRight && neighbors.isUp && !neighbors.isUpLeft && neighbors.isUpRight)
                    {
                        tileNumToDraw = 48;
                        shouldDrawATile = true;
                    }
                    else if (neighbors.isDown && neighbors.isDownLeft && neighbors.isDownRight && neighbors.isLeft
                        && neighbors.isRight && neighbors.isUp && neighbors.isUpLeft && !neighbors.isUpRight)
                    {
                        tileNumToDraw = 49;
                        shouldDrawATile = true;
                    }
                    else if (neighbors.isDown && !neighbors.isDownLeft && neighbors.isDownRight && neighbors.isLeft
                        && neighbors.isRight && neighbors.isUp && neighbors.isUpLeft && neighbors.isUpRight)
                    {
                        tileNumToDraw = 50;
                        shouldDrawATile = true;
                    }
                    else if (neighbors.isDown && neighbors.isDownLeft && !neighbors.isDownRight && neighbors.isLeft
                        && neighbors.isRight && neighbors.isUp && neighbors.isUpLeft && neighbors.isUpRight)
                    {
                        tileNumToDraw = 51;
                        shouldDrawATile = true;
                    }
                    else if (neighbors.isDown && neighbors.isDownLeft && neighbors.isDownRight && neighbors.isLeft
                        && neighbors.isRight && neighbors.isUp && neighbors.isUpLeft && neighbors.isUpRight)
                    {
                        tileNumToDraw = 30;
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
                    if (shouldDrawATile) _tiles[i].tileNum = tileNumToDraw;
                }
                if (shouldDrawATile) DrawSprite(e, tileNumToDraw, column * tileWidth, row * tileHeight);
            }

        }
        private void DrawGrid(PaintEventArgs e)
        {
            const int startX = 0;
            const int startY = 0;
            int lineWidth = _roomWidth * tileWidth;
            int lineHeight = _roomHeight * tileHeight;
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
            Position position = GetPositionOfSpriteNum(spriteNum);
            RectangleF srcRect = new RectangleF(position.column * tileWidth, position.row * tileHeight, tileWidth, tileHeight);
            GraphicsUnit units = GraphicsUnit.Pixel;
            e.Graphics.DrawImage(image, x, y, srcRect, units);
        }
        private Position GetPositionOfSpriteNum(int spriteNum)
        {
            Position position = new Position() { column = 0, row = 0 };
            switch (spriteNum)
            {
                case 2:
                    position.column = 0;
                    position.row = 0;
                    break;
                case 11:
                    position.column = 1;
                    position.row = 0;
                    break;
                case 44:
                    position.column = 2;
                    position.row = 0;
                    break;
                case 45:
                    position.column = 3;
                    position.row = 0;
                    break;
                case 39:
                    position.column = 4;
                    position.row = 0;
                    break;
                case 12:
                    position.column = 5;
                    position.row = 0;
                    break;
                case 42:
                    position.column = 6;
                    position.row = 0;
                    break;
                case 43:
                    position.column = 0;
                    position.row = 1;
                    break;
                case 40:
                    position.column = 1;
                    position.row = 1;
                    break;
                case 13:
                    position.column = 2;
                    position.row = 1;
                    break;
                case 47:
                    position.column = 3;
                    position.row = 1;
                    break;
                case 46:
                    position.column = 4;
                    position.row = 1;
                    break;
                case 53:
                    position.column = 5;
                    position.row = 1;
                    break;
                case 14:
                    position.column = 6;
                    position.row = 1;
                    break;
                case 41:
                    position.column = 0;
                    position.row = 2;
                    break;            
                case 54:              
                    position.column = 1; 
                    position.row = 2;
                    break;            
                case 52:              
                    position.column = 2; 
                    position.row = 2;
                    break;            
                case 5:              
                    position.column = 3; 
                    position.row = 2;
                    break;            
                case 36:              
                    position.column = 4; 
                    position.row = 2;
                    break;            
                case 6:              
                    position.column = 5; 
                    position.row = 2;
                    break;            
                case 35:              
                    position.column = 6; 
                    position.row = 2;
                    break;
                case 3:
                    position.column = 0;
                    position.row = 3;
                    break;
                case 37:
                    position.column = 1;
                    position.row = 3;
                    break;
                case 4:
                    position.column = 2;
                    position.row = 3;
                    break;
                case 38:
                    position.column = 3;
                    position.row = 3;
                    break;
                case 1:
                    position.column = 4;
                    position.row = 3;
                    break;
                case 8:
                    position.column = 5;
                    position.row = 3;
                    break;
                case 9:
                    position.column = 6;
                    position.row = 3;
                    break;
                case 10:
                    position.column = 0;
                    position.row = 4;
                    break;
                case 7:
                    position.column = 1;
                    position.row = 4;
                    break;
                case 15:
                    position.column = 2;
                    position.row = 4;
                    break;
                case 16:
                    position.column = 3;
                    position.row = 4;
                    break;
                case 17:
                    position.column = 4;
                    position.row = 4;
                    break;
                case 18:
                    position.column = 5;
                    position.row = 4;
                    break;
                case 19:
                    position.column = 6;
                    position.row = 4;
                    break;
                case 20:
                    position.column = 0;
                    position.row = 5;
                    break;
                case 22:
                    position.column = 1;
                    position.row = 5;
                    break;
                case 23:
                    position.column = 2;
                    position.row = 5;
                    break;
                case 24:
                    position.column = 3;
                    position.row = 5;
                    break;
                case 25:
                    position.column = 4;
                    position.row = 5;
                    break;
                case 21:
                    position.column = 5;
                    position.row = 5;
                    break;
                case 26:
                    position.column = 6;
                    position.row = 5;
                    break;
                case 27:
                    position.column = 0;
                    position.row = 6;
                    break;
                case 28:
                    position.column = 1;
                    position.row = 6;
                    break;
                case 29:
                    position.column = 2;
                    position.row = 6;
                    break;
                case 31:
                    position.column = 3;
                    position.row = 6;
                    break;
                case 32:
                    position.column = 4;
                    position.row = 6;
                    break;
                case 33:
                    position.column = 5;
                    position.row = 6;
                    break;
                case 34:
                    position.column = 6;
                    position.row = 6;
                    break;
                case 48:
                    position.column = 0;
                    position.row = 7;
                    break;
                case 49:
                    position.column = 1;
                    position.row = 7;
                    break;
                case 50:
                    position.column = 2;
                    position.row = 7;
                    break;
                case 51:
                    position.column = 3;
                    position.row = 7;
                    break;
                case 30:
                    position.column = 4;
                    position.row = 7;
                    break;
                case 55:
                    position.column = 5;
                    position.row = 7;
                    break;
                case 56:
                    position.column = 6;
                    position.row = 7;
                    break;
                case 57:
                    position.column = 0;
                    position.row = 8;
                    break;
                case 58:
                    position.column = 1;
                    position.row = 8;
                    break;
                case 59:
                    position.column = 2;
                    position.row = 8;
                    break;
                case 60:
                    position.column = 3;
                    position.row = 8;
                    break;
            }

            return position;
        }
        public int GetTileNumbFromMousePosition(Point mouseDownLocation)
        {
            int column = (int)Math.Floor(mouseDownLocation.X / (float)tileWidth);
            int row = (int)Math.Floor(mouseDownLocation.Y / (float)tileHeight);
            int tileNum = (row * _roomWidth) + column;
            return tileNum;
        }
        public void ReverseTile(int tileNum)
        {
            if (_tiles[tileNum].isSolid)
            {
                _tiles[tileNum] = (false, -1);
            }
            else
            {
                _tiles[tileNum] = (true, 2);
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
                int upLeftNum = tileNum - 1 - _roomWidth; 
                neighbors.isUpLeft = _tiles[upLeftNum].isSolid;
            }
            // up
            if (isInTopRow)
            {
                neighbors.isUp = true;
            }
            else
            {
                int upNum = tileNum - _roomWidth;
                neighbors.isUp = _tiles[upNum].isSolid;
            }
            // upRight
            if (isInTopRow || isInRightColumn)
            {
                neighbors.isUpRight = true;
            }
            else
            {
                int upRightNum = tileNum - _roomWidth + 1;
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
                int downLeftNum = tileNum - 1 + _roomWidth;
                neighbors.isDownLeft = _tiles[downLeftNum].isSolid;
            }
            // down
            if (isInBottomRow)
            {
                neighbors.isDown = true;
            }
            else
            {
                int downNum = tileNum + _roomWidth;
                neighbors.isDown = _tiles[downNum].isSolid;
            }
            // downRight
            if (isInBottomRow || isInRightColumn)
            {
                neighbors.isDownRight = true;
            }
            else
            {
                int downRightNum = tileNum + _roomWidth + 1;
                neighbors.isDownRight = _tiles[downRightNum].isSolid;
            }
            return neighbors;
        }
    }
}

using Assets.Scripts.WorldBuilder.RoomBuilder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Utility
{
    public static class GridHelper
    {
        public static Position GetPositionFromOrdinal(int gridWidth, int ordinal)
        {
            Position p = new Position();
            p.row = (int)Math.Floor(ordinal / (float)gridWidth);
            p.column = ordinal % gridWidth;
            return p;
        }
        public static int GetOrdinalFromPosition(int gridWidth, Position position)
        {
            return (position.row * gridWidth) + position.column;
        }
    }
}

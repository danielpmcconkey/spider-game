using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Data.World
{
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
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Data.World
{
    public class WorldGridCell
    {
        public int row;
        public int column;
        public int roomId;

        // whethere there are cells above, below, etc. that share the same room
        public bool isCellAbove;
        public bool isCellBelow;
        public bool isCellLeft;
        public bool isCellRight;
        public bool isCellLowerLeft;
        public bool isCellLowerRight;
        public bool isCellUpperLeft;
        public bool isCellUpperRight;

        // positions of the corners within the room's tile grid
        public int cellUpLeftOrdinal;
        public int cellUpRightOrdinal;
        public int cellLowRightOrdinal;
        public int cellLowLeftOrdinal;
    }
}

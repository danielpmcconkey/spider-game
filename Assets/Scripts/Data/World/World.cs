using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Data.World
{
    public class World
    {
        public int worldWidthInStandardRooms;
        public int worldHeightInStandardRooms;
        public WorldGridCell[] worldGrid;
        public List<RoomPlacement> rooms;
        public List<DoorPlacement> doorPlacements;
    }
}

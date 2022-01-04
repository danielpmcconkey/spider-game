using Assets.Scripts.WorldBuilder.Bot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.WorldBuilder
{
    public struct WorldGridCell
    {
        public int row;
        public int column;
        public int roomId;
    }
    public class World
    {
        public int worldWidthInStandardRooms;
        public int worldHeightInStandardRooms;
        public WorldGridCell[] worldGrid;
        public List<RoomBlueprint> rooms;
    }
}

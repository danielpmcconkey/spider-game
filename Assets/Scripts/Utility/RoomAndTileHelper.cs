using Assets.Scripts.Data.World;
using Assets.Scripts.WorldBuilder.RoomBuilder;
using Assets.Scripts.WorldBuilder.WorldManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Utility
{
    public static class RoomAndTileHelper
    {
        public static int GetGridOrdinalFromPosition(int gridWidth, Position position)
        {
            return (position.row * gridWidth) + position.column;
        }
        public static Position GetPositionFromGridOrdinal(int gridWidth, int ordinal)
        {
            Position p = new Position();
            p.row = (int)Math.Floor(ordinal / (float)gridWidth);
            p.column = ordinal % gridWidth;
            return p;
        }
        public static List<int> GetRoomConnections(int starterRoomId, int layers = -1)
        {
            // layers is the number of layers of recurrsion. if -1 then no limit
            List<int> connections = new List<int>();
            connections.Add(starterRoomId);
            connections = AddRoomConnectionsRecurrsive(connections, starterRoomId, layers);
            return connections;
        }

        private static List<int> AddRoomConnectionsRecurrsive(List<int> knownConnectedIds, int roomId, int layers = -1)
        {
            List<DoorPlacement> doorsInThisRoom = Globals.world.doorPlacements.
                Where(x => x.room1Id == roomId || x.room2Id == roomId).ToList();
            foreach (DoorPlacement d in doorsInThisRoom)
            {
                if (!knownConnectedIds.Contains(d.room1Id))
                {
                    knownConnectedIds.Add(d.room1Id);
                    if (layers == -1) knownConnectedIds = AddRoomConnectionsRecurrsive(knownConnectedIds, d.room1Id, -1);
                    else if (layers > 0) knownConnectedIds = AddRoomConnectionsRecurrsive(knownConnectedIds, d.room1Id, --layers);
                }
                if (!knownConnectedIds.Contains(d.room2Id))
                {
                    knownConnectedIds.Add(d.room2Id);
                    if (layers == -1) knownConnectedIds = AddRoomConnectionsRecurrsive(knownConnectedIds, d.room2Id, -1);
                    else if (layers > 0) knownConnectedIds = AddRoomConnectionsRecurrsive(knownConnectedIds, d.room2Id, --layers);
                }
            }
            return knownConnectedIds;
        }
    }
}

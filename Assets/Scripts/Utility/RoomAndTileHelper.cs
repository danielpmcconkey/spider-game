using Assets.Scripts.Data.World;
using Assets.Scripts.WorldBuilder.RoomBuilder;
using Assets.Scripts.WorldBuilder.WorldManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Utility
{
    public static class RoomAndTileHelper
    {
        public static Vector2 GetGlobalPositionFromRoomTileIndex(RoomPlacement room, int index)
        {
            try
            {
                Vector2 position = Vector2.zero;
                int row = (int)Mathf.Floor(index / (float)room.roomWidthInTiles);
                int column = (int)Mathf.Floor(index % room.roomWidthInTiles);
                position.x = room.upperLeftInGlobalSpace.x + MeasurementConverter.TilesXToUnityMeters(column);
                position.y = room.upperLeftInGlobalSpace.y - MeasurementConverter.TilesYToUnityMeters(row);
                return position;
            }
            catch (Exception)
            {

                throw;
            }
        }
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
        public static Position GetTilePositionInRoomFromWorldSpaceLocation(RoomPlacement room, Vector2 positionInWorldSpace)
        {
            Position outPosition = new Position();
            int ordinal = GetRoomTileIndexFromWorldSpaceLocation(room, positionInWorldSpace);
            outPosition = GetPositionFromGridOrdinal(room.roomWidthInTiles, ordinal);
            return outPosition;
        }
        public static int GetRoomTileIndexFromWorldSpaceLocation(RoomPlacement room, Vector2 positionInWorldSpace)
        {
            int xAsHundredTimes = (int)Math.Round(Math.Round(positionInWorldSpace.x, 2) * 100);
            int yAsHundredTimes = (int)Math.Round(Math.Round(positionInWorldSpace.y, 2) * 100);
            int xUlAsHundredTimes = (int)Math.Round(Math.Round(room.upperLeftInGlobalSpace.x, 2) * 100);
            int yUlAsHundredTimes = (int)Math.Round(Math.Round(room.upperLeftInGlobalSpace.y, 2) * 100);
            int tileWidthAsHundredTimes = (int)Math.Round(Math.Round(MeasurementConverter.TilesXToUnityMeters(1), 2) * 100);
            int tileHeightAsHundredTimes = (int)Math.Round(Math.Round(MeasurementConverter.TilesYToUnityMeters(1), 2) * 100);

            // remember that unity y gets lower as we go downward
            // but our index goes from up-left to down-right
            //
            // also, I rounded and multiplied by 100 because floats 
            // were doing weird things with the math
            int numRowsDown = (yUlAsHundredTimes - yAsHundredTimes) / tileHeightAsHundredTimes;
            int valueInFirstColumnOfThatRow = numRowsDown * room.roomWidthInTiles;
            int numColsIn = (xAsHundredTimes - xUlAsHundredTimes) / tileWidthAsHundredTimes;
            return valueInFirstColumnOfThatRow + numColsIn;
        }
        public static bool IsInBottomRow(int gridWidth, int ordinal, int gridHeight)
        {
            if (Mathf.FloorToInt(ordinal / (float)gridWidth) == gridHeight - 1) return true;
            return false;
        }
        public static bool IsInLeftColumn(int gridWidth, int ordinal)
        {
            if (ordinal % gridWidth == 0) return true;
            return false;
        }
        public static bool IsInRightColumn(int gridWidth, int ordinal)
        {
            if (ordinal % gridWidth == gridWidth - 1) return true;
            return false;
        }
        public static bool IsInTopRow(int gridWidth, int ordinal)
        {
            if (ordinal < gridWidth) return true;
            return false;
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

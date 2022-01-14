using Assets.Scripts.Data.World;
using Assets.Scripts.Utility;
using Assets.Scripts.WorldBuilder.RoomBuilder;
using Assets.Scripts.WorldBuilder.WorldManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.WorldBuilder.Bot
{
    public class CreatorBot
    {
        private WorldSize size;

        private TilePlacement[] _worldTiles;
        private int _worldWidthInTiles;
        private int _worldHeightInTiles;

        public void CreateWorld(WorldSize size, int rngSeed)
        {
            this.size = size;
            RNG.initWithSeed(rngSeed);
            Globals.world = new World();
            SizeWorld();
            // create a grid of empty rooms, all equally sized
            AddEmptyWorldGrid();
            // now combine some of them (adding all possible doors)
            CombineGridCellsIntoRooms();
            // remove unneeded doors
            FinalizeRoomDoors();
            // actually turn them into rooms and add perimiter tiles
            MakeRoomsFromGrid();
            FleshOutRoomCellDetails();
            FillInRoomPerimiters();
            // add the miscellaneous platforms
            AddPlatformsToRoom();
            // put masking objects around rooms and do this before knocking out tiles behind doors
            AddRoomMasks();
            // remove any tiles that are row replaced by doors
            KnockOutTilesBehindDoors();
            // update tile types (only do this after you've added all the platforms
            UpdateTileTypes();
            //decorateRooms();



            
        }


        private void AddBaseTilePlacement(RoomPlacement room, int roomTileGridOrdinal)
        {
            // add a base tile (tile map 1) to the room and to the global tile grid

            TilePlacement tilePlacement = new TilePlacement();
            tilePlacement.roomId = room.id;
            tilePlacement.isSolid = true;
            tilePlacement.tileNum = 1;
            Position posInRoom = RoomAndTileHelper.GetPositionFromGridOrdinal(room.roomWidthInTiles, roomTileGridOrdinal);
            tilePlacement.rowInRoom = posInRoom.row;
            tilePlacement.columnInRoom = posInRoom.column;
            float x = room.upperLeftInGlobalSpace.x + (MeasurementConverter.TilesXToUnityMeters(posInRoom.column));
            float y = room.upperLeftInGlobalSpace.y - (MeasurementConverter.TilesYToUnityMeters(posInRoom.row));
            tilePlacement.positionInGlobalSpace = new Vector2(x, y);

            // add it to the room grid
            try
            {
                room.tiles[roomTileGridOrdinal] = tilePlacement;
            }
            catch (Exception)
            {
                //return;
                throw;
                
            }

            // add it to the world grid
            Position posInWorldTileGrid = new Position() { row = 0, column = 0 };
            posInWorldTileGrid.column += MeasurementConverter.UnityMetersToTilesX(room.upperLeftInGlobalSpace.x);
            posInWorldTileGrid.column += posInRoom.column;
            posInWorldTileGrid.row += -1 * MeasurementConverter.UnityMetersToTilesY(room.upperLeftInGlobalSpace.y);
            posInWorldTileGrid.row += posInRoom.row;
            int worldGridOrdinal = RoomAndTileHelper.GetGridOrdinalFromPosition(_worldWidthInTiles, posInWorldTileGrid);
            _worldTiles[worldGridOrdinal] = tilePlacement;
        }
        private void AddEmptyWorldGrid()
        {
            // create a grid of empty rooms, all equally sized
            // also add a door between each grid cell
            // we'll later take away doors


            Globals.world.worldGrid = new WorldGridCell[
                Globals.world.worldHeightInStandardRooms * Globals.world.worldWidthInStandardRooms];
            Globals.world.doorPlacements = new List<DoorPlacement>();

            int roomId = 0;
            for (int i = 0; i < Globals.world.worldHeightInStandardRooms; i++)
            {
                for (int i2 = 0; i2 < Globals.world.worldWidthInStandardRooms; i2++)
                {
                    WorldGridCell cell = new WorldGridCell() { row = i, column = i2, roomId = roomId };
                    Globals.world.worldGrid[roomId] = cell;

                    // add a door down and right
                    if (i < Globals.world.worldHeightInStandardRooms - 1)
                    {
                        // add a door down
                        int randTilesIn = RNG.GetRandomInt(0, Globals.standardRoomWidth - 4 - Globals.doorHWidthInTiles); // the 4 removes the perimiter
                        float posX =
                            ConvertGridColumnToUnityX(i2) // left-most point of the room
                            + MeasurementConverter.TilesXToUnityMeters(2) // forward 2 for the room perimiter
                            + MeasurementConverter.TilesXToUnityMeters(randTilesIn); // forward by rand number of tiles
                        float posY =
                            ConvertGridRowToUnityY(i) // top of the room
                            - MeasurementConverter.TilesYToUnityMeters(Globals.standardRoomHeight) // bottom of the room
                            + MeasurementConverter.TilesYToUnityMeters(2); // back up 2 tiles for the room perimiter

                        DoorPlacement d = new DoorPlacement()
                        {
                            room1Id = roomId,
                            room2Id = roomId + Globals.world.worldWidthInStandardRooms,
                            isHorizontal = true,
                            positionInGlobalSpace = new Vector2(posX, posY),
                        };
                        Globals.world.doorPlacements.Add(d);
                    }
                    if (i2 < Globals.world.worldWidthInStandardRooms - 1)
                    {
                        // add a door right
                        int randTilesIn = RNG.GetRandomInt(0, Globals.standardRoomHeight - 4 - Globals.doorVHeightInTiles); // the 4 removes the perimiter
                        float posX =
                            ConvertGridColumnToUnityX(i2) // left-most point of the room
                            + MeasurementConverter.TilesXToUnityMeters(Globals.standardRoomWidth) // right-most point
                            - MeasurementConverter.TilesXToUnityMeters(2); // back up 2 for the room perimiter
                        float posY =
                            ConvertGridRowToUnityY(i) // top of the room
                            - MeasurementConverter.TilesYToUnityMeters(2) // forward 2 for the room perimiter
                            - MeasurementConverter.TilesYToUnityMeters(randTilesIn); // forward by rand number of tiles

                        DoorPlacement d = new DoorPlacement()
                        {
                            room1Id = roomId,
                            room2Id = roomId + 1,
                            isHorizontal = false,
                            positionInGlobalSpace = new Vector2(posX, posY),
                        };
                        Globals.world.doorPlacements.Add(d);
                    }

                    roomId++;
                }
            }
        }
        private void AddPlatformsForDoors()
        {
            foreach(RoomPlacement room in Globals.world.rooms)
            {
                var doorsInRoom = Globals.world.doorPlacements.Where(x => x.room1Id == room.id || x.room2Id == room.id);
                foreach(DoorPlacement door in doorsInRoom)
                {
                    Position doorPosInRoom = WhereIsDoorInRoomSpace(door, room);

                    if (door.isHorizontal && door.room2Id == room.id)
                    {
                        // this room is lower room; add a platform just below it
                        int row = doorPosInRoom.row + Globals.doorHHeightInTiles + 3;
                        int col = doorPosInRoom.column + 1;
                        Position upLeftPos = new Position() { row = row, column = col};
                        AddPlatformToRoom(room, upLeftPos, 3, 1);                        
                    }
                    if (!door.isHorizontal && door.room1Id == room.id)
                    {
                        // this room is left room; add a platform to the right of it
                        int row = doorPosInRoom.row + Globals.doorVHeightInTiles;
                        int col = doorPosInRoom.column - 3;
                        Position upLeftPos = new Position() { row = row, column = col };
                        AddPlatformToRoom(room, upLeftPos, 3, 1);
                    }
                    if (!door.isHorizontal && door.room2Id == room.id)
                    {
                        // this room is right room; add a platform to the left of it
                        int row = doorPosInRoom.row + Globals.doorVHeightInTiles;
                        int col = doorPosInRoom.column + 3 + 1;
                        Position upLeftPos = new Position() { row = row, column = col };
                        AddPlatformToRoom(room, upLeftPos, 3, 1);
                    }
                }
            }
        }
        private void AddPlatformToRoom(RoomPlacement room, Position upLeftPos, int numCols, int numRows)
        {
            for(int i = 0; i < numRows; i++)
            {
                // rows
                for (int i2 = 0; i2 < numCols; i2++)
                {
                    // columns
                    Position placementPos = new Position() { row = upLeftPos.row + i, column = upLeftPos.column + i2 };
                    int ordinal = RoomAndTileHelper.GetGridOrdinalFromPosition(room.roomWidthInTiles, placementPos);
                    AddBaseTilePlacement(room, ordinal);
                }
            }
        }
        private void AddPlatformsToRoom()
        {
            // add ledges around doors
            AddPlatformsForDoors();
        }
        //private List<int> AddRoomConnectionsRecurrsive(List<int> knownConnectedIds, int roomId, int layers = -1)
        //{
        //    // layers is the number of layers of recurrsion. if -1 then no limit

        //    List<DoorPlacement> doorsInThisRoom = Globals.world.doorPlacements.
        //        Where(x => x.room1Id == roomId || x.room2Id == roomId).ToList();
        //    foreach (DoorPlacement d in doorsInThisRoom)
        //    {
        //        if (!knownConnectedIds.Contains(d.room1Id))
        //        {
        //            knownConnectedIds.Add(d.room1Id);
        //            if (layers == -1) knownConnectedIds = AddRoomConnectionsRecurrsive(knownConnectedIds, d.room1Id, -1);
        //            else if (layers > 0) knownConnectedIds = AddRoomConnectionsRecurrsive(knownConnectedIds, d.room1Id, --layers);
        //        }
        //        if (!knownConnectedIds.Contains(d.room2Id))
        //        {
        //            knownConnectedIds.Add(d.room2Id);
        //            if (layers == -1) knownConnectedIds = AddRoomConnectionsRecurrsive(knownConnectedIds, d.room2Id, -1);
        //            else if (layers > 0) knownConnectedIds = AddRoomConnectionsRecurrsive(knownConnectedIds, d.room2Id, --layers);
        //        }
        //    }
        //    return knownConnectedIds;
        //}
        private void AddRoomMask(RoomPlacement r)
        {
            // at this point in room building, the only blocks left are our perimiter tiles

            r.roomMasks = new List<RoomMaskPlacement>();

            const float maskDistanceAroundRoom = 5f;

            // get overall boundaries
            Vector2 maskBoundUL = new Vector2(r.upperLeftInGlobalSpace.x - maskDistanceAroundRoom,
                r.upperLeftInGlobalSpace.y + maskDistanceAroundRoom);
            Vector2 maskBoundLR = new Vector2(r.lowerRightInGlobalSpace.x + maskDistanceAroundRoom,
                r.lowerRightInGlobalSpace.y - maskDistanceAroundRoom);

            // start with the easy ones: the borders
            // left border
            RoomMaskPlacement leftBorder = new RoomMaskPlacement();
            leftBorder.positionInGlobalSpace = new Vector2(
                maskBoundUL.x + (maskDistanceAroundRoom / 2),
                (maskBoundUL.y + maskBoundLR.y) / 2
                );
            leftBorder.scale = new Vector2(maskDistanceAroundRoom, Mathf.Abs(maskBoundLR.y - maskBoundUL.y));
            r.roomMasks.Add(leftBorder);
            // right border
            RoomMaskPlacement rightBorder = new RoomMaskPlacement();
            rightBorder.positionInGlobalSpace = new Vector2(
                maskBoundLR.x - (maskDistanceAroundRoom / 2),
                (maskBoundUL.y + maskBoundLR.y) / 2
                );
            rightBorder.scale = new Vector2(maskDistanceAroundRoom, Mathf.Abs(maskBoundLR.y - maskBoundUL.y));
            r.roomMasks.Add(rightBorder);
            // top border
            RoomMaskPlacement topBorder = new RoomMaskPlacement();
            topBorder.positionInGlobalSpace = new Vector2(
                (maskBoundUL.x + maskBoundLR.x) / 2,
                maskBoundUL.y - (maskDistanceAroundRoom / 2)
                );
            topBorder.scale = new Vector2(Mathf.Abs(maskBoundLR.x - maskBoundUL.x), maskDistanceAroundRoom);
            r.roomMasks.Add(topBorder);
            // bottom border
            RoomMaskPlacement bottomBorder = new RoomMaskPlacement();
            bottomBorder.positionInGlobalSpace = new Vector2(
                (maskBoundUL.x + maskBoundLR.x) / 2,
                maskBoundLR.y + (maskDistanceAroundRoom / 2)
                );
            bottomBorder.scale = new Vector2(Mathf.Abs(maskBoundLR.x - maskBoundUL.x), maskDistanceAroundRoom);
            r.roomMasks.Add(bottomBorder);

            // now try to fill in any empty room grid cells
            var cellsInRoom = Globals.world.worldGrid.Where(x => x.roomId == r.id);
            int topMostRow = cellsInRoom.Min(x => x.row);
            int bottomMostRow = cellsInRoom.Max(x => x.row);
            int leftMostColumn = cellsInRoom.Min(x => x.column);
            int rightMostColumn = cellsInRoom.Max(x => x.column);

            for(int i = topMostRow; i <= bottomMostRow; i++)
            {
                for(int i2 = leftMostColumn; i2 <= rightMostColumn; i2++)
                {
                    if(cellsInRoom.Where(x => x.row == i && x.column == i2).Count() > 0)
                    {
                        // there's a cell here, no need to create a mask
                    }
                    else
                    {
                        float posX = ConvertGridColumnToUnityX(i2) + 
                            (MeasurementConverter.TilesXToUnityMeters(Globals.standardRoomWidth / 2f));
                        float posY = ConvertGridRowToUnityY(i) -
                            (MeasurementConverter.TilesYToUnityMeters(Globals.standardRoomHeight / 2f));
                        float scaleX = MeasurementConverter.TilesXToUnityMeters(Globals.standardRoomWidth);
                        float scaleY = MeasurementConverter.TilesYToUnityMeters(Globals.standardRoomHeight);
                        RoomMaskPlacement cellMask = new RoomMaskPlacement();
                        cellMask.positionInGlobalSpace = new Vector2(posX, posY);
                        cellMask.scale = new Vector2(scaleX, scaleY);
                        r.roomMasks.Add(cellMask);
                    }
                }
            }



        }
        private void AddRoomMasks()
        {
            foreach(RoomPlacement r in Globals.world.rooms)
            {
                AddRoomMask(r);
            }
        }
        private bool AreAllRoomsConnected()
        {
            // used to determine if we removed one door too many

            
            // get a list of all room IDs we have
            List<int> roomIds = Globals.world.worldGrid.GroupBy(x => x.roomId).Select(grp => grp.First().roomId).ToList();
            // now recurssively add each room as its connections
            List<int> knownConnectedIds = RoomAndTileHelper.GetRoomConnections(roomIds[0]);
            // now check to make sure all rooms are connected
            foreach(int id in roomIds)
            {
                if (!knownConnectedIds.Contains(id)) return false;
            }
            return true;
        }
        private void CombineGridCellsIntoRooms(int cursor = 0)
        {
            

            // recursive function to join multiple cells to one another
            int numJoinsRequired = (int)Math.Round(Globals.world.worldGrid.Length * 0.5f, 0);
            if (cursor < numJoinsRequired)
            {
                // find room to start the combo
                int targetCellToCombine = RNG.GetRandomInt(0, Globals.world.worldGrid.Length);
                Position targetPosition = GetPositionFromGridOrdinal(targetCellToCombine);

                // now find room to combine with
                // 0 = above
                // 1 = right
                // 2 = below
                // 3 = left
                int direction = RNG.GetRandomInt(0, 4);
                
                // check if room exists in that direction
                if (
                    (direction == 0 && targetPosition.row == 0) || 
                    (direction == 1 && targetPosition.column == Globals.world.worldWidthInStandardRooms - 1) || 
                    (direction == 2 && targetPosition.row == Globals.world.worldHeightInStandardRooms - 1) || 
                    (direction == 3 && targetPosition.column == 0)
                    )
                {
                    // try again, no room in that direction
                    CombineGridCellsIntoRooms(cursor);
                }
                else
                {
                    Position candidatePosition = new Position() { row = targetPosition.row, column = targetPosition.column };
                    if (direction == 0) candidatePosition.row -= 1;
                    if (direction == 1) candidatePosition.column += 1;
                    if (direction == 2) candidatePosition.row += 1;
                    if (direction == 3) candidatePosition.column -= 1;

                    // are they already combined?
                    WorldGridCell targetCell = Globals.world.worldGrid[targetCellToCombine];
                    WorldGridCell candidateCell = Globals.world.worldGrid[GetGridOrdinalFromPosition(candidatePosition)];
                    if (targetCell.roomId == candidateCell.roomId)
                    {
                        // try again, already combined
                        CombineGridCellsIntoRooms(cursor);
                    }
                    else
                    {
                        // actually combine these two
                        CombineTwoGridCells(targetCell.roomId, candidateCell.roomId);

                        // run it again until we're good
                        CombineGridCellsIntoRooms(cursor + 1);
                    }
                }
            }
            else return;
        }
        private void CombineTwoGridCells(int roomId1, int roomId2)
        {
            // first remove any doors between these cells
            List<DoorPlacement> newDoors = new List<DoorPlacement>();
            foreach(DoorPlacement d in Globals.world.doorPlacements)
            {
                if (d.room1Id == roomId1 && d.room2Id == roomId2 ||
                    d.room1Id == roomId2 && d.room2Id == roomId1)
                {
                    // get rid of it by not adding
                }
                else newDoors.Add(d);
            }
            Globals.world.doorPlacements = newDoors;

            // now combine cells by updating their roomIds

            int newRoomId = (roomId1 < roomId2) ? roomId1 : roomId2;
            for (int i = 0; i < Globals.world.worldGrid.Length; i++)
            {
                if (Globals.world.worldGrid[i].roomId == roomId1 ||
                    Globals.world.worldGrid[i].roomId == roomId2)
                {
                    Globals.world.worldGrid[i].roomId = newRoomId;
                }
            }

            // now update any doors to use the new ID
            foreach (DoorPlacement d in Globals.world.doorPlacements)
            {
                if(d.room1Id == roomId1 || d.room1Id == roomId2)
                {
                    d.room1Id = newRoomId;
                }
                if (d.room2Id == roomId1 || d.room2Id == roomId2)
                {
                    d.room2Id = newRoomId;
                }
            }

        }
        private float ConvertGridColumnToUnityX(int gridColumn)
        {
            float start = Globals.unityWorldUpLeftX;
            return start + MeasurementConverter.TilesXToUnityMeters(Globals.standardRoomWidth * gridColumn);
        }
        private float ConvertGridRowToUnityY(int gridRow)
        {
            float start = Globals.unityWorldUpLeftY;
            return start - MeasurementConverter.TilesYToUnityMeters(Globals.standardRoomHeight * gridRow);
        }
        private void FillInRoomPerimiters()
        {
            foreach (RoomPlacement room in Globals.world.rooms)
            {
                List<WorldGridCell> cells = Globals.world.worldGrid.Where(x => x.roomId == room.id).ToList();
                foreach (var cell in cells)
                {
                    // fill in the base perimeter

                    // if there's no room above add a ceiling
                    if (!cell.isCellAbove)
                    {
                        for (int i = cell.cellUpLeftOrdinal; i <= cell.cellUpRightOrdinal; i++)
                        {
                            AddBaseTilePlacement(room, i);
                            AddBaseTilePlacement(room, i + room.roomWidthInTiles);
                        }
                    }
                    // if there's no room to the right add the right wall
                    if (!cell.isCellRight)
                    {
                        for (int i = cell.cellUpRightOrdinal; i <= cell.cellLowRightOrdinal; i += room.roomWidthInTiles)
                        {
                            try
                            {
                                AddBaseTilePlacement(room, i);
                                AddBaseTilePlacement(room, i - 1);
                            }
                            catch (Exception)
                            {

                                throw;
                            }
                        }
                    }
                    // if there's no room below add a floor
                    if (!cell.isCellBelow)
                    {
                        for (int i = cell.cellLowLeftOrdinal; i <= cell.cellLowRightOrdinal; i++)
                        {
                            AddBaseTilePlacement(room, i);
                            AddBaseTilePlacement(room, i - room.roomWidthInTiles);
                        }
                    }
                    // if there's no room to the left add the left wall
                    if (!cell.isCellLeft)
                    {
                        for (int i = cell.cellUpLeftOrdinal; i <= cell.cellLowLeftOrdinal; i += room.roomWidthInTiles)
                        {
                            AddBaseTilePlacement(room, i);
                            AddBaseTilePlacement(room, i + 1);
                        }
                    }

                    // now fill in the jagged corners. 
                    // in the above graphic example, column 6, row 4 would
                    // have a "staircase" corner in the lower left

                    // no room below, room to the left, room to the lower-left
                    if (!cell.isCellBelow && cell.isCellLeft && cell.isCellLowerLeft)
                    {
                        // extend the floor 2 tiles to the left
                        for (int i = cell.cellLowLeftOrdinal - 2; i < cell.cellLowLeftOrdinal; i++)
                        {
                            AddBaseTilePlacement(room, i);
                            AddBaseTilePlacement(room, i - room.roomWidthInTiles);
                        }
                    }
                    // no room below, room to the right, room to the lower-right
                    if (!cell.isCellBelow && cell.isCellRight && cell.isCellLowerRight)
                    {
                        // extend the floor 2 tiles to the right
                        for (int i = cell.cellLowRightOrdinal + 1; i <= cell.cellLowRightOrdinal + 2; i++)
                        {
                            AddBaseTilePlacement(room, i);
                            AddBaseTilePlacement(room, i - room.roomWidthInTiles);
                        }
                    }
                    // no room above, room to the left, room to the upper-left
                    if (!cell.isCellAbove && cell.isCellLeft && cell.isCellUpperLeft)
                    {
                        // extend the ceiling 2 tiles to the left
                        for (int i = cell.cellUpLeftOrdinal - 2; i < cell.cellUpLeftOrdinal; i++)
                        {
                            AddBaseTilePlacement(room, i);
                            AddBaseTilePlacement(room, i + room.roomWidthInTiles);
                        }
                    }
                    // no room above, room to the right, room to the upper-right
                    if (!cell.isCellAbove && cell.isCellRight && cell.isCellUpperRight)
                    {
                        // extend the floor 2 tiles to the right
                        for (int i = cell.cellUpRightOrdinal + 1; i <= cell.cellUpRightOrdinal + 2; i++)
                        {
                            AddBaseTilePlacement(room, i);
                            AddBaseTilePlacement(room, i + room.roomWidthInTiles);
                        }
                    }
                }
            }
        }
        private void FinalizeRoomDoors()
        {
            // The doors are already there. We need to remove as many as we can
            // while still leaving all rooms connected


            // remove half of the doors
            int doorRemovalCountTarget = (int)Math.Floor(Globals.world.doorPlacements.Count * 0.5f) + 1;
            int doorRemovalCount = 0;

            int attemptCount = 0;
            while(doorRemovalCount < doorRemovalCountTarget)
            {
                int indexToRemove = RNG.GetRandomInt(0, Globals.world.doorPlacements.Count);
                DoorPlacement sacrifice = Globals.world.doorPlacements[indexToRemove];
                Globals.world.doorPlacements.RemoveAt(indexToRemove);

                if (AreAllRoomsConnected()) doorRemovalCount++;
                else
                {
                    // add back in the sacrificial door to keep all rooms connected
                    Globals.world.doorPlacements.Add(sacrifice);
                    attemptCount++;
                    if (attemptCount > 500)
                    {
                        return; // throw new Exception("Burp");
                    }
                }
            }
            
        }
        private void FleshOutRoomCellDetails()
        {
            foreach (RoomPlacement room in Globals.world.rooms)
            {
                List<WorldGridCell> cells = Globals.world.worldGrid.Where(x => x.roomId == room.id).ToList();
                for(int i = 0; i < cells.Count; i++)                
                {
                    var cell = cells[i];
                    // get the starting points of the cell within the room
                    /*
                        *               3         4         5         6         7   <-- grid column
                        *                        10        20        30        40   <-- room column
                        *               01234567890123456789012345678901234567890123456789
                        * 3           0 |--------||--------||########||########||--------|
                        * 3           1 |--------||--------||########||########||--------|
                        * 3           2 |--------||--------||########||########||--------|
                        * 4           3 |--------||########||########||########||########|
                        * 4           4 |--------||########||########||########||########|
                        * 4           5 |--------||########||########||########||########|
                        * 5           6 |--------||########||########||--------||--------|
                        * 5           7 |--------||########||########||--------||--------|
                        * 5           8 |--------||########||########||--------||--------|
                        *               01234567890123456789012345678901234567890123456789
                        * ^ Grid row
                        *             ^ Room row    
                        * */
                    int cellUpLeftColumn = (cell.column - cells.Min(x => x.column)) * Globals.standardRoomWidth;
                    int cellUpLeftRow = (cell.row - cells.Min(x => x.row)) * Globals.standardRoomHeight;
                    cell.cellUpLeftOrdinal = RoomAndTileHelper.GetGridOrdinalFromPosition(room.roomWidthInTiles,
                        new Position() { row = cellUpLeftRow, column = cellUpLeftColumn });

                    int cellUpRightColumn = cellUpLeftColumn + Globals.standardRoomWidth - 1;
                    int cellUpRightRow = cellUpLeftRow;
                    cell.cellUpRightOrdinal = RoomAndTileHelper.GetGridOrdinalFromPosition(room.roomWidthInTiles,
                        new Position() { row = cellUpRightRow, column = cellUpRightColumn });

                    int cellLowLeftColumn = cellUpLeftColumn;
                    int cellLowLeftRow = cellUpLeftRow + Globals.standardRoomHeight - 1;
                    cell.cellLowLeftOrdinal = RoomAndTileHelper.GetGridOrdinalFromPosition(room.roomWidthInTiles,
                        new Position() { row = cellLowLeftRow, column = cellLowLeftColumn });

                    int cellLowRightColumn = cellUpRightColumn;
                    int cellLowRightRow = cellLowLeftRow;
                    cell.cellLowRightOrdinal = RoomAndTileHelper.GetGridOrdinalFromPosition(room.roomWidthInTiles,
                        new Position() { row = cellLowRightRow, column = cellLowRightColumn });

                    cell.isCellAbove = (cells.Where(x => x.row == cell.row - 1 && x.column == cell.column).Count() == 0) ?
                        false : true;
                    cell.isCellRight = (cells.Where(x => x.column == cell.column + 1 && x.row == cell.row).Count() == 0) ?
                        false : true;
                    cell.isCellBelow = (cells.Where(x => x.row == cell.row + 1 && x.column == cell.column).Count() == 0) ?
                        false : true;
                    cell.isCellLeft = (cells.Where(x => x.column == cell.column - 1 && x.row == cell.row).Count() == 0) ?
                        false : true;
                    cell.isCellUpperRight = (cells.Where(x => x.column == cell.column + 1 && x.row == cell.row - 1).Count() == 0) ?
                        false : true;
                    cell.isCellUpperLeft = (cells.Where(x => x.column == cell.column - 1 && x.row == cell.row - 1).Count() == 0) ?
                        false : true;
                    cell.isCellLowerRight = (cells.Where(x => x.column == cell.column + 1 && x.row == cell.row + 1).Count() == 0) ?
                        false : true;
                    cell.isCellLowerLeft = (cells.Where(x => x.column == cell.column - 1 && x.row == cell.row + 1).Count() == 0) ?
                        false : true;
                    
                   
                }
            }
        }
        private int GetGridOrdinalFromPosition(Position position)
        {
            return RoomAndTileHelper.GetGridOrdinalFromPosition(Globals.world.worldWidthInStandardRooms, position);
        }
        private Position GetPositionFromGridOrdinal(int gridOrdinal)
        {
            return RoomAndTileHelper.GetPositionFromGridOrdinal(Globals.world.worldWidthInStandardRooms, gridOrdinal);
        }
        private (TileNeighbors neighbors, List<TilePlacement> list) GetTileNeighbors(int tileNum)
        {
            TileNeighbors neighbors = new TileNeighbors();
            List<TilePlacement> outList = new List<TilePlacement>();

            // get whether this tile is on the periphery to avoid
            // index out of range exceptions and to keep from checking
            // the left-most tile of the row below you when you want
            // to check right
            bool isInTopRow = RoomAndTileHelper.IsInTopRow(_worldWidthInTiles, tileNum);
            bool isInBottomRow = RoomAndTileHelper.IsInBottomRow(_worldWidthInTiles, tileNum, _worldHeightInTiles);
            bool isInLeftColumn = RoomAndTileHelper.IsInLeftColumn(_worldWidthInTiles, tileNum);
            bool isInRightColumn = RoomAndTileHelper.IsInRightColumn(_worldWidthInTiles, tileNum);

            // up left
            int upLeftNum = tileNum - 1 - _worldWidthInTiles;
            if (isInTopRow || isInLeftColumn)
            {
                neighbors.isUpLeft = true;
            }
            else if (_worldTiles[upLeftNum] != null && _worldTiles[upLeftNum].isSolid)
            {
                neighbors.isUpLeft = true;
                if(upLeftNum > 0 && upLeftNum <_worldTiles.Length) outList.Add(_worldTiles[upLeftNum]);
            }

            // up
            int upNum = tileNum - _worldWidthInTiles;
            if (isInTopRow)
            {
                neighbors.isUp = true;
            }
            else if (_worldTiles[upNum] != null && _worldTiles[upNum].isSolid)
            {
                neighbors.isUp = true;
                if (upNum > 0 && upNum < _worldTiles.Length) outList.Add(_worldTiles[upNum]);
            }

            // upRight
            int upRightNum = tileNum - _worldWidthInTiles + 1;
            if (isInTopRow || isInRightColumn)
            {
                neighbors.isUpRight = true;
            }
            else if (_worldTiles[upRightNum] != null && _worldTiles[upRightNum].isSolid)
            {
                neighbors.isUpRight = true;
                if (upRightNum > 0 && upRightNum < _worldTiles.Length) outList.Add(_worldTiles[upRightNum]);
            }

            // left
            int leftNum = tileNum - 1;
            if (isInLeftColumn)
            {
                neighbors.isLeft = true;
            }
            else if (_worldTiles[leftNum] != null && _worldTiles[leftNum].isSolid)
            {
                neighbors.isLeft = true;
                if (leftNum > 0 && leftNum < _worldTiles.Length) outList.Add(_worldTiles[leftNum]);
            }

            // right
            int rightNum = tileNum + 1;
            if (isInRightColumn)
            {
                neighbors.isRight = true;
            }
            else if (_worldTiles[rightNum] != null && _worldTiles[rightNum].isSolid)
            {
                neighbors.isRight = true;
                if (rightNum > 0 && rightNum < _worldTiles.Length) outList.Add(_worldTiles[rightNum]);
            }

            // down left
            int downLeftNum = tileNum - 1 + _worldWidthInTiles;
            if (isInBottomRow || isInLeftColumn)
            {
                neighbors.isDownLeft = true;
            }
            else if (_worldTiles[downLeftNum] != null && _worldTiles[downLeftNum].isSolid)
            {
                neighbors.isDownLeft = true;
                if (downLeftNum > 0 && downLeftNum < _worldTiles.Length) outList.Add(_worldTiles[downLeftNum]);
            }

            // down
            int downNum = tileNum + _worldWidthInTiles;
            if (isInBottomRow)
            {
                neighbors.isDown = true;
            }
            else if (_worldTiles[downNum] != null && _worldTiles[downNum].isSolid)
            {
                neighbors.isDown = true;
                if (downNum > 0 && downNum < _worldTiles.Length) outList.Add(_worldTiles[downNum]);
            }

            // downRight
            int downRightNum = tileNum + _worldWidthInTiles + 1;
            if (isInBottomRow || isInRightColumn)
            {
                neighbors.isDownRight = true;
            }
            else if (_worldTiles[downRightNum] != null && _worldTiles[downRightNum].isSolid)
            {
                neighbors.isDownRight = true;
                if (downRightNum > 0 && downRightNum < _worldTiles.Length) outList.Add(_worldTiles[downRightNum]);
            }

            return (neighbors, outList);
        }
        private void KnockOutTilesAtPosition(Vector2 knockOutPosition, int roomId)
        {
            RoomPlacement r = Globals.world.rooms.Where(x => x.id == roomId).FirstOrDefault();
            for(int i = 0; i < r.tiles.Length; i++)
            {
                var t = r.tiles[i];
                if (t != null && t.isSolid)
                {
                    Position tilePositionLocal = RoomAndTileHelper.GetPositionFromGridOrdinal(r.roomWidthInTiles, i);
                    Vector2 tilePositionGlobal = new Vector2(
                        r.upperLeftInGlobalSpace.x + MeasurementConverter.TilesXToUnityMeters(tilePositionLocal.column),
                        r.upperLeftInGlobalSpace.y - MeasurementConverter.TilesYToUnityMeters(tilePositionLocal.row)
                        );
                    if(tilePositionGlobal == knockOutPosition)
                    {
                        r.tiles[i].isSolid = false;
                        r.tiles[i].tileNum = 0;
                    }
                }
            }            
        }
        private void KnockOutTilesBehindDoors()
        {
            foreach (DoorPlacement d in Globals.world.doorPlacements)
            {
                int tilesX = (d.isHorizontal) ? Globals.doorHWidthInTiles : Globals.doorVWidthInTiles;
                int tilesY = (d.isHorizontal) ? Globals.doorHHeightInTiles : Globals.doorVHeightInTiles;
                for (int i = 0; i < tilesY; i++)
                {
                    for (int i2 = 0; i2 < tilesX; i2++)
                    {
                        float posX = d.positionInGlobalSpace.x + (MeasurementConverter.TilesXToUnityMeters(i2));
                        float posY = d.positionInGlobalSpace.y - (MeasurementConverter.TilesYToUnityMeters(i));
                        KnockOutTilesAtPosition(new Vector2(posX, posY), d.room1Id);
                        KnockOutTilesAtPosition(new Vector2(posX, posY), d.room2Id);
                    }
                }
            }
        }
        private void MakeRoomsFromGrid()
        {
            List<int> roomIds = Globals.world.worldGrid.GroupBy(x => x.roomId).Select(grp => grp.First().roomId).ToList();
            Globals.world.rooms = new List<RoomPlacement>();
            foreach(int id in roomIds)
            {
                // grab all grid cells
                List<WorldGridCell> cells = Globals.world.worldGrid.Where(x => x.roomId == id).ToList();

                // map the bounding rectangle
                float upLeftX = ConvertGridColumnToUnityX(cells.Min(x => x.column));
                float upLeftY = ConvertGridRowToUnityY(cells.Min(x => x.row));
                

                float lowerRightX = ConvertGridColumnToUnityX(cells.Max(x => x.column)) + 
                    (MeasurementConverter.TilesXToUnityMeters(Globals.standardRoomWidth - 1));

                // min row is the "highest"
                int minRow = cells.Min(x => x.row);
                int minRowTopInTiles = minRow * Globals.standardRoomHeight;
                // max row is the lowest
                int maxRow = cells.Max(x => x.row);
                int maxRowTopInTiles = maxRow * Globals.standardRoomHeight;
                int maxRowBottomInTiles = maxRowTopInTiles + Globals.standardRoomHeight;

                int heightInTiles = Math.Abs(minRowTopInTiles - maxRowBottomInTiles);
                float distanceY = MeasurementConverter.TilesYToUnityMeters(heightInTiles);
                float lowerRightY = upLeftY - distanceY;

                int widthInTiles = MeasurementConverter.UnityMetersToTilesX(Math.Abs(lowerRightX - upLeftX) + 1);

                RoomPlacement r = new RoomPlacement()
                {
                    id = id,
                    lowerRightInGlobalSpace = new Vector2(lowerRightX, lowerRightY),
                    upperLeftInGlobalSpace = new Vector2(upLeftX, upLeftY),
                    tiles = new TilePlacement[widthInTiles * heightInTiles],
                    roomWidthInTiles = widthInTiles,
                    roomHeightInTiles = heightInTiles,
                    roomWidthInUnityMeters = MeasurementConverter.TilesXToUnityMeters(widthInTiles),
                    roomHeightInUnityMeters = MeasurementConverter.TilesYToUnityMeters(heightInTiles),
                    doors = new List<DoorPlacement>(),
                };
                
                Globals.world.rooms.Add(r);
            }
        }
        private void SizeWorld()
        {
            Globals.world.worldWidthInStandardRooms = RNG.GetRandomInt(size.minWidth, size.maxWidth + 1);
            Globals.world.worldHeightInStandardRooms = RNG.GetRandomInt(size.minHeight, size.maxHeight + 1);
            _worldWidthInTiles = Globals.world.worldWidthInStandardRooms * Globals.standardRoomWidth;
            _worldHeightInTiles = Globals.world.worldHeightInStandardRooms * Globals.standardRoomHeight;
            _worldTiles = new TilePlacement[_worldWidthInTiles * _worldHeightInTiles];
        }
        private void UpdateTileTypes()
        {
            for(int i = 0; i < _worldTiles.Length; i++)
            {
                
                var neighborsCheck = GetTileNeighbors(i);
                TileNeighbors neighbors = neighborsCheck.neighbors;

                int tileNumToDraw = -1;

                
                if (_worldTiles[i] != null && _worldTiles[i].isSolid)
                {
                    


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
                    if (!neighbors.isUpLeft && !neighbors.isDownRight && neighbors.isUp && neighbors.isUpRight
                        && neighbors.isLeft && neighbors.isRight && neighbors.isDownLeft && neighbors.isDown)
                    {
                        tileNumToDraw = 61;
                    }
                    if (neighbors.isUpLeft && neighbors.isDownRight && neighbors.isUp && !neighbors.isUpRight
                        && neighbors.isLeft && neighbors.isRight && !neighbors.isDownLeft && neighbors.isDown)
                    {
                        tileNumToDraw = 62;
                    }
                    _worldTiles[i].tileNum = tileNumToDraw;
                }
                else
                {
                    bool shouldDrawATile = false;


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
                        // need to create a tilePlacement object as there wouldn't have been one already
                        TilePlacement tilePlacement = new TilePlacement();
                        tilePlacement.isSolid = false;
                        Position posInWorld = RoomAndTileHelper.GetPositionFromGridOrdinal(_worldWidthInTiles, i);
                        tilePlacement.positionInGlobalSpace = new Vector2(
                            MeasurementConverter.TilesXToUnityMeters(posInWorld.column),
                            -1 * MeasurementConverter.TilesYToUnityMeters(posInWorld.row)
                            );                        
                        tilePlacement.roomId = WhichRoomIsAWorldTileIn(i, neighborsCheck.list);
                        tilePlacement.tileNum = tileNumToDraw;

                        RoomPlacement room = Globals.world.rooms.Where(x => x.id == tilePlacement.roomId).FirstOrDefault();
                        Position posInRoom = RoomAndTileHelper.GetTilePositionInRoomFromWorldSpaceLocation(
                            room, tilePlacement.positionInGlobalSpace);
                        tilePlacement.columnInRoom = posInRoom.column;
                        tilePlacement.rowInRoom = posInRoom.row;
                        int ordinalInRoom = RoomAndTileHelper.GetGridOrdinalFromPosition(room.roomWidthInTiles, posInRoom);

                        // now add it to both the room and global list
                        _worldTiles[i] = tilePlacement;
                        try
                        {
                            room.tiles[ordinalInRoom] = tilePlacement;
                        }
                        catch (Exception)
                        {

                            throw;
                        }
                    }
                    else
                    {
                        // do nothing
                    }
                }
            }
        }
        private Position WhereIsDoorInRoomSpace(DoorPlacement door, RoomPlacement room)
        {
            float roomTop = room.upperLeftInGlobalSpace.y;
            float roomLeft = room.upperLeftInGlobalSpace.x;
            float doorTop = door.positionInGlobalSpace.y;
            float doorLeft = door.positionInGlobalSpace.x;
            int row = -1 * MeasurementConverter.UnityMetersToTilesY(doorTop - roomTop);
            int col = MeasurementConverter.UnityMetersToTilesX(doorLeft - roomLeft);
            return new Position() { row = row, column = col };
        }
        private int WhichRoomIsAWorldTileIn(int worldTileOrdinal, List<TilePlacement> neighbors = null)
        {
            // first check if there's already a tile there with a room
            TilePlacement existingTile = _worldTiles[worldTileOrdinal];
            if(existingTile != null)
            {
                return existingTile.roomId;
            }

            // no luck. go the hard way
            // check all neighbors. Whichever room has the highest count
            // go w/ that
            List<TilePlacement> neighborsToUse = neighbors;
            if (neighbors == null)
            {
                var neighborsCheck = GetTileNeighbors(worldTileOrdinal);
                neighborsToUse = neighborsCheck.list;
            }

            try
            {
                var counts = neighborsToUse
                        .GroupBy(x => x.roomId)
                        .Select(g => new
                        {
                            roomId = g.Key,
                            count = g.Count()
                        });
                int maxCount = counts.Max(x => x.count);
                int roomId = counts.Where(y => y.count == maxCount).FirstOrDefault().roomId;

                return roomId;
            }
            catch (Exception)
            {

                throw;
            }

            

        }

    }
}

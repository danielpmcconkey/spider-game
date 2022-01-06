using Assets.Scripts.Utility;
using Assets.Scripts.WorldBuilder.RoomBuilder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.WorldBuilder.Bot
{
    public class WBBot
    {
        private World world;
        private WorldSize size;

        public World CreateWorld(WorldSize size, int rngSeed)
        {
            this.size = size;
            RNG.initWithSeed(rngSeed);
            world = new World();
            SizeWorld();
            // create a grid of empty rooms, all equally sized
            AddEmptyWorldGrid();
            // now combine some of them (adding all possible doors)
            CombineGridCellsIntoRooms();
            // remove unneeded doors
            FinalizeRoomDoors();
            // actually turn them into rooms and add perimiter tiles
            MakeRoomsFromGrid();
            KnockOutTilesBehindDoors();
            //decorateRooms();



            return world;
        }


        private void AddEmptyWorldGrid()
        {
            // create a grid of empty rooms, all equally sized
            // also add a door between each grid cell
            // we'll later take away doors
            
            
            world.worldGrid = new WorldGridCell[world.worldHeightInStandardRooms * world.worldWidthInStandardRooms];
            world.doors = new List<Door>();

            int roomId = 0;
            for (int i = 0; i < world.worldHeightInStandardRooms; i++)
            {
                for (int i2 = 0; i2 < world.worldWidthInStandardRooms; i2++)
                {
                    WorldGridCell cell = new WorldGridCell() { row = i, column = i2, roomId = roomId };
                    world.worldGrid[roomId] = cell;

                    // add a door down and right
                    if (i < world.worldHeightInStandardRooms - 1)
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

                        Door d = new Door(null)
                        {
                            room1Id = roomId,
                            room2Id = roomId + world.worldWidthInStandardRooms,
                            isHorizontal = true,
                            positionInGlobalSpace = new Vector2(posX, posY),
                        };
                        world.doors.Add(d);
                    }
                    if (i2 < world.worldWidthInStandardRooms - 1)
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

                        Door d = new Door(null)
                        {
                            room1Id = roomId,
                            room2Id = roomId + 1,
                            isHorizontal = false,
                            positionInGlobalSpace = new Vector2(posX, posY),
                        };
                        world.doors.Add(d);
                    }

                    roomId++;
                }
            }
        }
        private List<int> AddRoomConnectionsRecurrsive(List<int> knownConnectedIds, int roomId, int layers = -1)
        {
            // layers is the number of layers of recurrsion. if -1 then no limit

            List<Door> doorsInThisRoom = world.doors.Where(x => x.room1Id == roomId || x.room2Id == roomId).ToList();
            foreach (Door d in doorsInThisRoom)
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
        private bool AreAllRoomsConnected()
        {
            // used to determine if we removed one door too many

            // create an empty "connected room IDs" container. we'll go through the connections
            // and add them into this container. at the end, we make sure that all IDs are in 
            // this container
            List<int> knownConnectedIds = new List<int>();
            // get a list of all room IDs we have
            List<int> roomIds = world.worldGrid.GroupBy(x => x.roomId).Select(grp => grp.First().roomId).ToList();
            // start with the first room
            knownConnectedIds.Add(roomIds[0]);
            // now recurssively add each room as its connections
            AddRoomConnectionsRecurrsive(knownConnectedIds, roomIds[0]);
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
            int numJoinsRequired = (int)Math.Round(world.worldGrid.Length * 0.5f, 0);
            if (cursor < numJoinsRequired)
            {
                // find room to start the combo
                int targetCellToCombine = RNG.GetRandomInt(0, world.worldGrid.Length);
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
                    (direction == 1 && targetPosition.column == world.worldWidthInStandardRooms - 1) || 
                    (direction == 2 && targetPosition.row == world.worldHeightInStandardRooms - 1) || 
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
                    WorldGridCell targetCell = world.worldGrid[targetCellToCombine];
                    WorldGridCell candidateCell = world.worldGrid[GetGridOrdinalFromPosition(candidatePosition)];
                    if (targetCell.roomId == candidateCell.roomId)
                    {
                        // try again, already combined
                        CombineGridCellsIntoRooms(cursor);
                    }
                    else
                    {
                        // actually combine these two
                        CombineTwoGridCells(targetCell, candidateCell);

                        // run it again until we're good
                        CombineGridCellsIntoRooms(cursor + 1);
                    }
                }
            }
            else return;
        }
        private void CombineTwoGridCells(WorldGridCell targetCell, WorldGridCell candidateCell)
        {
            // first remove any doors between these cells
            List<Door> newDoors = new List<Door>();
            foreach(Door d in world.doors)
            {
                if (d.room1Id == targetCell.roomId && d.room2Id == candidateCell.roomId ||
                    d.room1Id == candidateCell.roomId && d.room2Id == targetCell.roomId)
                {
                    // get rid of it by not adding
                }
                else newDoors.Add(d);
            }
            world.doors = newDoors;

            // now combine cells by updating their roomIds

            int newRoomId = (targetCell.roomId < candidateCell.roomId) ? targetCell.roomId : candidateCell.roomId;
            for (int i = 0; i < world.worldGrid.Length; i++)
            {
                if (world.worldGrid[i].roomId == targetCell.roomId ||
                    world.worldGrid[i].roomId == candidateCell.roomId)
                {
                    world.worldGrid[i].roomId = newRoomId;
                }
            }

            // now update any doors to use the new ID
            foreach (Door d in world.doors)
            {
                if(d.room1Id == targetCell.roomId || d.room1Id == candidateCell.roomId)
                {
                    d.room1Id = newRoomId;
                }
                if (d.room2Id == targetCell.roomId || d.room2Id == candidateCell.roomId)
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
        private void FillInRoomsPerimiter(RoomBlueprint room, List<WorldGridCell> cells, int widthInTiles)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var cell in cells)
            {
                sb.AppendLine(string.Format("new cell: {0}, {1}, {2}", cell.roomId, cell.row, cell.column));
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
                int cellUpLeftOrdinal = GridHelper.GetOrdinalFromPosition(widthInTiles,
                    new Position() { row = cellUpLeftRow, column = cellUpLeftColumn });

                int cellUpRightColumn = cellUpLeftColumn + Globals.standardRoomWidth - 1;
                int cellUpRightRow = cellUpLeftRow;
                int cellUpRightOrdinal = GridHelper.GetOrdinalFromPosition(widthInTiles,
                    new Position() { row = cellUpRightRow, column = cellUpRightColumn });

                int cellLowLeftColumn = cellUpLeftColumn;
                int cellLowLeftRow = cellUpLeftRow + Globals.standardRoomHeight - 1;
                int cellLowLeftOrdinal = GridHelper.GetOrdinalFromPosition(widthInTiles,
                    new Position() { row = cellLowLeftRow, column = cellLowLeftColumn });

                int cellLowRightColumn = cellUpRightColumn;
                int cellLowRightRow = cellLowLeftRow;
                int cellLowRightOrdinal = GridHelper.GetOrdinalFromPosition(widthInTiles,
                    new Position() { row = cellLowRightRow, column = cellLowRightColumn });

                // if there's no room above add a ceiling
                if (cells.Where(x => x.row == cell.row - 1 && x.column == cell.column).Count() == 0)
                {
                    for(int i = cellUpLeftOrdinal; i <= cellUpRightOrdinal; i++)
                    {                        
                        room.tiles[i] = new TilePlacement() { isSolid = true, tileNum = 1 };
                        room.tiles[i + widthInTiles] = new TilePlacement() { isSolid = true, tileNum = 1 };
                    }
                }
                // if there's no room to the right add the right wall
                if (cells.Where(x => x.column == cell.column + 1 && x.row == cell.row).Count() == 0)
                {
                    for (int i = cellUpRightOrdinal; i <= cellLowRightOrdinal; i += widthInTiles)
                    {
                        try
                        {
                            room.tiles[i] = new TilePlacement() { isSolid = true, tileNum = 1 };
                            room.tiles[i - 1] = new TilePlacement() { isSolid = true, tileNum = 1 };
                        }
                        catch (Exception)
                        {

                            throw;
                        }
                    }
                }
                // if there's no room below add a floor
                if (cells.Where(x => x.row == cell.row + 1 && x.column == cell.column).Count() == 0)
                {
                    for (int i = cellLowLeftOrdinal; i <= cellLowRightOrdinal; i++)
                    {
                        room.tiles[i] = new TilePlacement() { isSolid = true, tileNum = 1 };
                        room.tiles[i - widthInTiles] = new TilePlacement() { isSolid = true, tileNum = 1 };
                    }
                }
                // if there's no room to the left add the left wall
                if (cells.Where(x => x.column == cell.column - 1 && x.row == cell.row).Count() == 0)
                {
                    for (int i = cellUpLeftOrdinal; i <= cellLowLeftOrdinal; i += widthInTiles)
                    {
                        room.tiles[i] = new TilePlacement() { isSolid = true, tileNum = 1 };
                        room.tiles[i + 1] = new TilePlacement() { isSolid = true, tileNum = 1 };
                    }
                }
            }

        }
        private void FinalizeRoomDoors()
        {
            // The doors are already there. We need to remove as many as we can
            // while still leaving all rooms connected


            // remove half of the doors
            int doorRemovalCountTarget = (int)Math.Floor(world.doors.Count * 0.5f) + 1;
            int doorRemovalCount = 0;

            while(doorRemovalCount < doorRemovalCountTarget)
            {
                int indexToRemove = RNG.GetRandomInt(0, world.doors.Count);
                Door sacrifice = world.doors[indexToRemove];
                world.doors.RemoveAt(indexToRemove);

                if (AreAllRoomsConnected()) doorRemovalCount++;
                else
                {
                    // add back in the sacrificial door to keep all rooms connected
                    world.doors.Add(sacrifice);
                }
            }            
        }
        private int GetGridOrdinalFromPosition(Position position)
        {
            return GridHelper.GetOrdinalFromPosition(world.worldWidthInStandardRooms, position);
        }
        private Position GetPositionFromGridOrdinal(int gridOrdinal)
        {
            return GridHelper.GetPositionFromOrdinal(world.worldWidthInStandardRooms, gridOrdinal);
        }
        private void KnockOutTilesAtPosition(Vector2 knockOutPosition, int roomId)
        {
            RoomBlueprint r = world.rooms.Where(x => x.id == roomId).FirstOrDefault();
            for(int i = 0; i < r.tiles.Length; i++)
            {
                var t = r.tiles[i];
                if (t.isSolid)
                {
                    Position tilePositionLocal = GridHelper.GetPositionFromOrdinal(r.roomWidthInTiles, i);
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
            foreach (Door d in world.doors)
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
            List<int> roomIds = world.worldGrid.GroupBy(x => x.roomId).Select(grp => grp.First().roomId).ToList();
            world.rooms = new List<RoomBlueprint>();
            foreach(int id in roomIds)
            {
                // grab all grid cells
                List<WorldGridCell> cells = world.worldGrid.Where(x => x.roomId == id).ToList();

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
                float distanceY = heightInTiles * MeasurementConverter.TilesYToUnityMeters(Globals.standardRoomHeight);
                float lowerRightY = upLeftY - distanceY;

                

                // create the empty tiles array
                int widthInTiles = MeasurementConverter.UnityMetersToTilesX(Math.Abs(lowerRightX - upLeftX) + 1);

                RoomBlueprint r = new RoomBlueprint()
                {
                    id = id,
                    lowerRightInGlobalSpace = new Vector2(lowerRightX, lowerRightY),
                    upperLeftInGlobalSpace = new Vector2(upLeftX, upLeftY),
                    tiles = new TilePlacement[widthInTiles * heightInTiles],
                    roomWidthInTiles = widthInTiles,
                    roomHeightInTiles = heightInTiles,
                    roomWidthInUnityMeters = MeasurementConverter.TilesXToUnityMeters(widthInTiles),
                    roomHeightInUnityMeters = MeasurementConverter.TilesYToUnityMeters(heightInTiles),
                    doors = new List<Door>(),
                };
                

                

                FillInRoomsPerimiter(r, cells, widthInTiles);

                world.rooms.Add(r);


            }
        }
        private void SizeWorld()
        {
            world.worldWidthInStandardRooms = RNG.GetRandomInt(size.minWidth, size.maxWidth + 1);
            world.worldHeightInStandardRooms = RNG.GetRandomInt(size.minHeight, size.maxHeight + 1);
        }


    }
}

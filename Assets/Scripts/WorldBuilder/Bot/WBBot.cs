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
            // now combine some of them
            CombineGridCellsIntoRooms();
            // actually turn them into rooms and add perimiter tiles
            MakeRoomsFromGrid();


            //joinRooms();
            //decorateRooms();



            return world;
        }

        private void AddCeilingToRoom(RoomBlueprint room, WorldGridCell cell)
        {

        }
        private void AddEmptyWorldGrid()
        {
            // create a grid of empty rooms, all equally sized
            
            
            world.worldGrid = new WorldGridCell[world.worldHeightInStandardRooms * world.worldWidthInStandardRooms];

            int roomId = 0;
            for (int i = 0; i < world.worldHeightInStandardRooms; i++)
            {
                for (int i2 = 0; i2 < world.worldWidthInStandardRooms; i2++)
                {
                    WorldGridCell cell = new WorldGridCell() { row = i, column = i2, roomId = roomId };
                    world.worldGrid[roomId] = cell;
                    roomId++;
                }
            }
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
                        int newRoomId = (targetCell.roomId < candidateCell.roomId) ? targetCell.roomId : candidateCell.roomId;
                        for(int i = 0; i < world.worldGrid.Length; i++)
                        {
                            if (world.worldGrid[i].roomId == targetCell.roomId || 
                                world.worldGrid[i].roomId == candidateCell.roomId)
                            {
                                world.worldGrid[i].roomId = newRoomId;
                            }
                        }

                        // run it again until we're good
                        CombineGridCellsIntoRooms(cursor + 1);
                    }
                }
            }
            else return;
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
                if (cells.Where(x => x.row == cell.row - 1).Count() == 0)
                {
                    for(int i = cellUpLeftOrdinal; i <= cellUpRightOrdinal; i++)
                    {                        
                        room.tiles[i] = new TilePlacement() { isSolid = true, tileNum = 1 };
                        room.tiles[i + widthInTiles] = new TilePlacement() { isSolid = true, tileNum = 1 };
                    }
                }
                // if there's no room to the right add the right wall
                if (cells.Where(x => x.column == cell.column + 1).Count() == 0)
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
                if (cells.Where(x => x.row == cell.row + 1).Count() == 0)
                {
                    for (int i = cellLowLeftOrdinal; i <= cellLowRightOrdinal; i++)
                    {
                        room.tiles[i] = new TilePlacement() { isSolid = true, tileNum = 1 };
                        room.tiles[i - widthInTiles] = new TilePlacement() { isSolid = true, tileNum = 1 };
                    }
                }
                // if there's no room to the left add the left wall
                if (cells.Where(x => x.column == cell.column - 1).Count() == 0)
                {
                    for (int i = cellUpLeftOrdinal; i <= cellLowLeftOrdinal; i += widthInTiles)
                    {
                        room.tiles[i] = new TilePlacement() { isSolid = true, tileNum = 1 };
                        room.tiles[i + 1] = new TilePlacement() { isSolid = true, tileNum = 1 };
                    }
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

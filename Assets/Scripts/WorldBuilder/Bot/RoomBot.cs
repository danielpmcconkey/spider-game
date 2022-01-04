using Assets.Scripts.Utility;
using Assets.Scripts.WorldBuilder.RoomBuilder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.WorldBuilder.Bot
{
    //public static class RoomBot
    //{
    //    public static void AddPerimiterTiles(RoomBlueprint room)
    //    {
    //        float startX = room.upperLeftInGlobalSpace.x;
    //        float startY = room.upperLeftInGlobalSpace.y;
    //        float endX = startX + room.roomWidthInTiles;
    //        float endY = startY + room.roomHeightInUnityMeters;

    //        int tileCount = (room.roomWidthInTiles) * (room.roomHeightInTiles);
    //        room.tiles = new TilePlacement[tileCount];

    //        int tilePosition = 0;

    //        for (float i = startY; i < endY; i += MeasurementConverter.TilesYToUnityMeters(1))
    //        {
    //            for (float i2 = startX; i2 < endX; i2 += MeasurementConverter.TilesXToUnityMeters(1))
    //            {
    //                bool isPerimiter = false;
    //                if (i == startY) isPerimiter = true;
    //                if (i == endY - MeasurementConverter.TilesYToUnityMeters(1)) isPerimiter = true;
    //                if (i2 == startX) isPerimiter = true;
    //                if (i2 == endX - MeasurementConverter.TilesXToUnityMeters(1)) isPerimiter = true;
    //                if (isPerimiter)
    //                {
    //                    TilePlacement tilePlacement = new TilePlacement();
    //                    tilePlacement.isSolid = true;
    //                    room.tiles[tilePosition] = tilePlacement;
    //                }
    //                tilePosition++;
    //            }
    //        }

    //    }
    //}
}

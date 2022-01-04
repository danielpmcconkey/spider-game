using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Utility
{
    
    public static class MeasurementConverter
    {
        public static float TilesXToUnityMeters(int tiles)
        {
            // if Globals.tileWidthInUnityMeters = 0.5 then 2 
            // tiles = 1 unity meter. if 
            // Globals.tileWidthInUnityMeters = 0.25 then 4
            // tiles = 1 unity meter
            return tiles * Globals.tileWidthInUnityMeters;
        }
        public static float TilesYToUnityMeters(int tiles)
        {
            // if Globals.tileWidthInUnityMeters = 0.5 then 2 
            // tiles = 1 unity meter. if 
            // Globals.tileWidthInUnityMeters = 0.25 then 4
            // tiles = 1 unity meter
            return tiles * Globals.tileHeightInUnityMeters;
        }
        public static int TilesXToPixels(int tiles)
        {
            return (int)MathF.Round(TilesXToUnityMeters(tiles) * Globals.pixelsInAUnityMeter, 0);
        }
        public static int TilesYToPixels(int tiles)
        {
            return (int)MathF.Round(TilesYToUnityMeters(tiles) * Globals.pixelsInAUnityMeter, 0);
        }
        public static int UnityMetersToTilesX(float unityMeters)
        {
            // if Globals.tileWidthInUnityMeters = 0.5 then 4 
            // tiles = 2 unity meters. if 
            // Globals.tileWidthInUnityMeters = 0.25 then 8
            // tiles = 2 unity meters

            return (int)MathF.Round(unityMeters / Globals.tileWidthInUnityMeters, 0);
        }
        public static int UnityMetersToTilesY(float unityMeters)
        {
            // if Globals.tileHeightInUnityMeters = 0.5 then 4 
            // tiles = 2 unity meters. if 
            // Globals.tileHeightInUnityMeters = 0.25 then 8
            // tiles = 2 unity meters

            return (int)MathF.Round(unityMeters / Globals.tileHeightInUnityMeters, 0);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Utility
{
    public static class Globals
    {
        public const float tileWidthInUnityMeters = 1.0f;
        public const float tileHeightInUnityMeters = 1.0f;
        public const int doorHeightInTiles = 3;
        public const int doorWidthInTiles = 4;
        public const float pixelsInAUnityMeter = 48f;
        public static int currentRoom;
        public static float roomTransitionTimeInSeconds = 1.5f;
    }
}

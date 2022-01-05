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
        public const int doorVHeightInTiles = 3;
        public const int doorVWidthInTiles = 4;
        public const int doorHHeightInTiles = 4;
        public const int doorHWidthInTiles = 5;
        public const float pixelsInAUnityMeter = 48f;
        public static int currentRoom;
        public static float roomTransitionTimeInSeconds = 1.5f;
        public const int standardRoomWidth = 25;
        public const int standardRoomHeight = 15;
        public const float unityWorldUpLeftX = 0;
        public const float unityWorldUpLeftY = 0;
        public static bool isWorldBuilt = false;
    }
}

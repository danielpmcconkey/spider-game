using Assets.Scripts.WorldBuilder.RoomBuilder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Data.World
{
    public class RoomPlacement
    {
        public int id;
        public Vector2 upperLeftInGlobalSpace;
        public Vector2 lowerRightInGlobalSpace;
        public int roomWidthInTiles;
        public int roomHeightInTiles;
        public float roomWidthInUnityMeters;
        public float roomHeightInUnityMeters;
        public TilePlacement[] tiles;
        public List<DoorPlacement> doors;
        public List<RoomMaskPlacement> roomMasks;
    }
}

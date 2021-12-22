using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.WorldBuilder.RoomBuilder
{
    [Serializable]
    public struct RoomSave
    {
        public string fileName;
        public string roomName;
        public int tileWidth;
        public int tileHeight;
        public int roomWidth;
        public int roomHeight;
        public TilePlacement[] tiles;
        public List<Door> doors;
    }
}

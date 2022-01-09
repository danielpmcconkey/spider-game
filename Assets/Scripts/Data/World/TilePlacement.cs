using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Data.World
{
    public class TilePlacement
    {
        public bool isSolid;
        public int tileNum;
        public Vector2 positionInGlobalSpace;
        public int roomId;
        public int rowInRoom;
        public int columnInRoom;
    }
}

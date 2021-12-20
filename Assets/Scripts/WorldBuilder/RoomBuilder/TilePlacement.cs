using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.WorldBuilder.RoomBuilder
{
    [Serializable]
    public struct TilePlacement
    {
        public bool isSolid;
        public int tileNum;
    }
}

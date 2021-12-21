using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.WorldBuilder.RoomBuilder
{
    [Serializable]
    public struct DoorConnection
    {
        public Guid doorIn;
        public Guid doorOut;
        public bool isImpossible;
        public bool requiresHighJump;
        public bool requiresWallWalk;
        public bool requiresCeilingWalk;
        public bool requiresGrapple;
        public bool requiresUserCapability01;
        public bool requiresUserCapability02;
        public bool requiresUserCapability03;
        public bool requiresUserCapability04;
        public bool requiresUserCapability05;
        public bool requiresUserCapability06;
        public bool requiresUserCapability07;
        public bool requiresUserCapability08;
    }
}

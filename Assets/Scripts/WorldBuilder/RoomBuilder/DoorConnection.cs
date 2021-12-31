using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.WorldBuilder.RoomBuilder
{
    [Serializable]
    public class DoorConnection
    {
        public string doorConnectionId; // it's stupid to store guids as strings, but the unity serializer won't serialize a guid
        public string doorInId; // it's stupid to store guids as strings, but the unity serializer won't serialize a guid
        public string doorOutId; // it's stupid to store guids as strings, but the unity serializer won't serialize a guid
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

        public DoorConnection(string doorInId, string doorOutId)
        {
            doorConnectionId = Guid.NewGuid().ToString();
            this.doorInId = doorInId;
            this.doorOutId = doorOutId;
        }
    }
}

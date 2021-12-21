using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.WorldBuilder.RoomBuilder
{
    [Serializable]
    public struct Door
    {
        public Guid guid;
        public Position position;
        public DoorConnection[] doorConnections;   // used to determine what movement capabilities are required to go from one room to the next
    }
}

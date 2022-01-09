//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Assets.Scripts.WorldBuilder.RoomBuilder
//{
//    [Serializable]
//    public class Door
//    {
//        public string guid; // it's stupid to store guids as strings, but the unity serializer won't serialize a guid
//        public Position position;
//        public List<DoorConnection> doorConnections; // used to determine what movement capabilities are required to go from one room to the next

//        public Door()
//        {
//            guid = Guid.NewGuid().ToString();
//            doorConnections = new List<DoorConnection>();
//        }
//        public Door(int row, int column)
//        {
//            guid = Guid.NewGuid().ToString();
//            doorConnections = new List<DoorConnection>();
//            position = new Position() { row = row, column = column };
//        }
//    }
//}

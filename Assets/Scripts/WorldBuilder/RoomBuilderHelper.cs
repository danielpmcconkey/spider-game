using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.WorldBuilder
{
    /* 
     * note, the two structs here are essentially
     * copies of those with the same name in the
     * RoomBuilder project. I couldn't use a shared
     * class because the unity JSON deserializer
     * works different than the newtonsoft one I
     * used in the other project. Newtonsoft 
     * requires serializable members to be properties
     * while unity requires that they're not
     * 
     * */
    [Serializable]
    public struct TilePlacement
    {
        public bool isSolid;
        public int tileNum;
    }
    [Serializable]
    public struct RoomSave
    {
        public string roomName;
        public int tileWidth;
        public int tileHeight;
        public int roomWidth;
        public int roomHeight;
        public TilePlacement[] tiles;
    }
    public static class RoomBuilderHelper
    {
        public static RoomSave DeserializeFromJson(string json)
        {
            RoomSave save = JsonUtility.FromJson<RoomSave>(json);
            return save;
        }
    }
}

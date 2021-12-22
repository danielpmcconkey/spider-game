using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.WorldBuilder.RoomBuilder
{
    public static class RoomBuilderHelper
    {
        public static RoomSave DeserializeFromJson(string json)
        {
            RoomSave save = JsonUtility.FromJson<RoomSave>(json);
            for(int i = 0;  i < save.doors.Count; i++)
            {
                var door = save.doors[i];
                if (door.guid == null) door.guid = Guid.NewGuid().ToString();
            }
            return save;
        }
        public static RoomSave[] GetAllRoomSaves()
        {
            TextAsset[] roomTemplates = Resources.LoadAll<TextAsset>("RoomTemplates");
            RoomSave[] roomSaves = new RoomSave[roomTemplates.Length];
            for (int i = 0; i < roomSaves.Length; i++)
            {
                
                string json = roomTemplates[i].text;
                roomSaves[i] = DeserializeFromJson(json);
            }
            return roomSaves;
        }
        public static string Serialize(RoomBeingBuilt room, string fileName)
        {
            RoomSave save = new RoomSave();
            save.fileName = fileName;
            save.roomName = room.roomName;
            save.tileWidth = room.tileWidth;
            save.tileHeight = room.tileHeight;
            save.roomWidth = room.roomWidth;
            save.roomHeight = room.roomHeight;
            save.tiles = room.tiles;
            save.doors = room.doors;
            string json = JsonUtility.ToJson(save);
            return json;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Events
{
    public class GameEvents : MonoBehaviour
    {
        public static GameEvents current;
        

        private void Awake()
        {
            // singleton pattern allows us to have a "static" class inherit from MonoBehaviour
            current = this;
        }

        #region Game events
        public event Action<float> onContactDamageForPlayer;
        public event Action<Vector2> onRoomTransitionTriggerEnter;
        //public event Action<Vector2> onDoorwayTriggerEnter;
        //public event Action<Vector2> onDoorwayTriggerExit;

        public void ContactDamageForPlayerTriggerEnter(float damageAmount)
        {
            if (onContactDamageForPlayer != null)
            {
                onContactDamageForPlayer(damageAmount);
            }
        }
        public void RoomTransitionTriggerEnter(Vector2 doorPosition)
        {
            if (onRoomTransitionTriggerEnter != null)
            {
                onRoomTransitionTriggerEnter(doorPosition);
            }
        }
        //public void DoorwayTriggerEnter(Vector2 doorPosition)
        //{
        //    if (onDoorwayTriggerEnter != null)
        //    {
        //        onDoorwayTriggerEnter(doorPosition);
        //    }
        //}
        //public void DoorwayTriggerExit(Vector2 doorPosition)
        //{
        //    if (onDoorwayTriggerExit != null)
        //    {
        //        onDoorwayTriggerExit(doorPosition);
        //    }
        //}
        #endregion



    }
}

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
        public event Action onDoorwayTriggerEnter;
        public event Action onDoorwayTriggerExit;

        private void Awake()
        {
            current = this;

        }

        
        public void DoorwayTriggerEnter()
        {
            if(onDoorwayTriggerEnter != null)
            {
                onDoorwayTriggerEnter();
            }
        }
        public void DoorwayTriggerExit()
        {
            if (onDoorwayTriggerExit != null)
            {
                onDoorwayTriggerExit();
            }
        }
    }
}

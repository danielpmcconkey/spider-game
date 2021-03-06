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
        public event Action<Vector2> onBuilderSquareMouseDown;
        public event Action<Vector2> onBuilderSquareMouseEnter;
        public event Action<float> onContactDamageForPlayer;
        public event Action<Vector2> onDoorwayTriggerEnter;
        public event Action<Vector2> onDoorwayTriggerExit;

        private void Awake()
        {
            current = this;

        }
        public void BuilderSquareMouseDown(Vector2 position)
        {
            onBuilderSquareMouseDown?.Invoke(position);
        }
        public void BuilderSquareMouseEnter(Vector2 position)
        {
            onBuilderSquareMouseEnter?.Invoke(position);
        }
        public void ContactDamageForPlayerTriggerEnter(float damageAmount)
        {
            if(onContactDamageForPlayer != null)
            {
                onContactDamageForPlayer(damageAmount);
            }
        }
        public void DoorwayTriggerEnter(Vector2 doorPosition)
        {
            if(onDoorwayTriggerEnter != null)
            {
                onDoorwayTriggerEnter(doorPosition);
            }
        }
        public void DoorwayTriggerExit(Vector2 doorPosition)
        {
            if (onDoorwayTriggerExit != null)
            {
                onDoorwayTriggerExit(doorPosition);
            }
        }        
    }
}

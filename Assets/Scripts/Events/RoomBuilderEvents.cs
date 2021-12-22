using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Events
{
    public class RoomBuilderEvents : MonoBehaviour
    {
        public static RoomBuilderEvents current;


        private void Awake()
        {
            // singleton pattern allows us to have a "static" class inherit from MonoBehaviour
            current = this;
        }
    
        #region RoomBuilder Events
        public event Action<Vector2> onBuilderSquareMouseDown;
        public event Action<Vector2> onBuilderSquareMouseEnter;
        public event Action<string> onBuilderDependencyMouseDown;

        public void BuilderDependencyMouseDown(string doorConnectionId)
        {
            onBuilderDependencyMouseDown?.Invoke(doorConnectionId);
        }
        public void BuilderSquareMouseDown(Vector2 position)
        {
            onBuilderSquareMouseDown?.Invoke(position);
        }
        public void BuilderSquareMouseEnter(Vector2 position)
        {
            onBuilderSquareMouseEnter?.Invoke(position);
        }
        #endregion
    }
}

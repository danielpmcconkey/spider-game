using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.WorldBuilder
{
    public class Door : ActivatableGameObject
    {
        public int room1Id; // left or up
        public int room2Id; // right or down
        public Vector2 positionInGlobalSpace;
        public bool isLocked = false;
        public bool isHorizontal = false;

        public Door(GameObject doorGameObject)
        {
            _gameObject = doorGameObject;
        }

        
    }
}

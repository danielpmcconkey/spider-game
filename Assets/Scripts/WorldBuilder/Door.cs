using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.WorldBuilder
{
    public class Door
    {
        public Room roomLeft;
        public Room roomRight;
        public Vector2 positionInGlobalSpace;
        public bool _isLocked = false;
    }
}

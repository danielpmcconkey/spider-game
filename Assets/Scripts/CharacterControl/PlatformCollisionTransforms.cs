using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.CharacterControl
{
    public class PlatformCollisionTransforms
    {
        public Transform bellyCheckTransform;
        public Transform forwardCheckTransform;
        public Transform ceilingCheckTransform;
        public LayerMask whatIsPlatform;
    }
}

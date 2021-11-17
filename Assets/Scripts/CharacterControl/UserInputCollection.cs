using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.CharacterControl
{
    public struct UserInputCollection
    {
        public float moveHPressure;
        public float moveVPressure;
        public bool isJumpPressed;
        public bool isJumpReleased;
        public bool isJumpHeldDown;
        public bool isGrappleButtonPressed;
        public bool isGrappleButtonReleased;
        public bool isGrappleButtonHeldDown;
    }
}

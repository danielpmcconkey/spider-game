using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.CharacterControl
{
    public struct UserInputCollection
    {
        public float moveHPressure { get; set; }
        public float moveVPressure { get; set; }
        public bool isJumpPressed { get; set; }
        public bool isJumpReleased { get; set; }
        public bool isJumpHeldDown { get; set; }
        public bool isGrappleButtonPressed { get; set; }
        public bool isGrappleButtonReleased { get; set; }
        public bool isGrappleButtonHeldDown { get; set; }
        public bool isPrimaryFireButtonPressed { get; set; }
        public bool isPrimaryFireButtonReleased { get; set; }
        public bool isPrimaryFireButtonHeldDown { get; set; }
        public float mouseX { get; set; }
        public float mouseY { get; set; }
    }
}

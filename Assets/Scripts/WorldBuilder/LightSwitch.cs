using Assets.Scripts.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Assets.Scripts.WorldBuilder
{
    public class LightSwitch : MonoBehaviour
    {
        [NonSerialized]
        public int roomId = 0;
        public Light2D mylight;
        public float startingIntensity;
        public float dimmerSpeed = 2f;

        private void Update()
        {
            if (Globals.currentRoom == roomId)
            {
                raiseIntensity();
            }
            else lowerIntensity();
            
        }
        private void raiseIntensity()
        {
            if(mylight.intensity < startingIntensity)
            {
                mylight.intensity += (dimmerSpeed * Time.deltaTime);
            }
            if (mylight.intensity > startingIntensity)
            {
                mylight.intensity = startingIntensity;
            }
        }
        private void lowerIntensity()
        {
            if (mylight.intensity > 0)
            {
                mylight.intensity -= (dimmerSpeed * Time.deltaTime);
            }
            if (mylight.intensity < 0)
            {
                mylight.intensity = 0;
            }
        }

    }
}

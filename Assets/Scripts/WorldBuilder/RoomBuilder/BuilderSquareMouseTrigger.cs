using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Assets.Scripts.Events;

namespace Assets.Scripts.WorldBuilder.RoomBuilder
{
    public class BuilderSquareMouseTrigger : MonoBehaviour
    {
        private void OnMouseDown()
        {
            // use teh parent's position because the colider is always x=.5 y=-.5
            GameEvents.current.BuilderSquareMouseDown(transform.parent.position);
        }
        private void OnMouseEnter()
        {
            // use teh parent's position because the colider is always x=.5 y=-.5
            GameEvents.current.BuilderSquareMouseEnter(transform.parent.position);
        }
    }
}

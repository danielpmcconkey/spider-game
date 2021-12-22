using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Assets.Scripts.Events;

namespace Assets.Scripts.WorldBuilder.RoomBuilder
{
    class BuilderDependencyMouseTrigger : MonoBehaviour
    {
        public string doorConnectionId;

        private void OnMouseDown()
        {
            RoomBuilderEvents.current.BuilderDependencyMouseDown(doorConnectionId);
        }
    }
}

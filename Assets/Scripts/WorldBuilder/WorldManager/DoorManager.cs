using Assets.Scripts.Data.World;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.WorldBuilder.WorldManager
{
    public class DoorManager : ActivatableGameObjectManager
    {
        public DoorPlacement doorPlacement;

        public DoorManager(GameObject doorGameObject) :base(doorGameObject) { }        
    }
}

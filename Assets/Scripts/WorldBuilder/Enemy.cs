using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.WorldBuilder
{
    public class Enemy
    {
        public GameObject prefab;
        public Vector2 positionInGlobalSpace;
        public GameObject targetCharacter;
    }
}

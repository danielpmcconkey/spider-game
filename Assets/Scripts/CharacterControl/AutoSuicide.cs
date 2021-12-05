using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.CharacterControl
{
    public class AutoSuicide : MonoBehaviour
    {
        // attache this script to anything (like a bullet)
        // that needs to be killed after a timer
        [SerializeField] public float lifeTimeInSeconds = 0.25f;
        private float deathTimer = 0f;

        private void Update()
        {
            deathTimer += Time.deltaTime;
            if(deathTimer > lifeTimeInSeconds)
            {
                Destroy(gameObject);
            }
        }
    }
}

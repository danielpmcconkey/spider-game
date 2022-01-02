using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Events
{
    public class RoomTransitionTrigger : MonoBehaviour
    {
        [SerializeField] public GameObject doorOrigin;

        private void OnTriggerEnter2D(Collider2D collider)
        {
            if (collider.CompareTag("Player"))
            {
                GameEvents.current.RoomTransitionTriggerEnter(doorOrigin.transform.position);
            }
        }
    }
}

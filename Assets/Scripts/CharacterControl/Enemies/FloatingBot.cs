using Assets.Scripts.Combat;
using Assets.Scripts.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.CharacterControl.Enemies
{
    public class FloatingBot : FloatingAiCharacter
    {
        private void OnTriggerEnter2D(Collider2D collider)
        {
            if (collider.CompareTag("Player"))
            {
                GameEvents.current.ContactDamageForPlayerTriggerEnter(contactDamageDealt);
                Die();
            }
        }
        protected override void Die()
        {
            base.Die();
        }
    }
}

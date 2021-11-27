using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.CharacterControl
{
    public class FloatingAiCharacter: AiControlledCharacter
    {
        protected override bool IsGroundedRayCast()
        {
            return false;
        }
        protected override void ApplyForcesFloating()
        {
            base.ApplyForcesFloating();

            // add air friction if not agroed
            if(!isAgroed)
            {
                const float airFriction = 0.01f;

                if (rigidBody2D.velocity.x > airFriction)
                    rigidBody2D.velocity = new Vector2(rigidBody2D.velocity.x - airFriction, rigidBody2D.velocity.y);
                else if (rigidBody2D.velocity.x < -airFriction)
                    rigidBody2D.velocity = new Vector2(rigidBody2D.velocity.x + airFriction, rigidBody2D.velocity.y);
                else rigidBody2D.velocity = new Vector2(0, rigidBody2D.velocity.y);

                if (rigidBody2D.velocity.y > airFriction)
                    rigidBody2D.velocity = new Vector2(rigidBody2D.velocity.x, rigidBody2D.velocity.y - airFriction);
                else if (rigidBody2D.velocity.y < -airFriction)
                    rigidBody2D.velocity = new Vector2(rigidBody2D.velocity.x, rigidBody2D.velocity.y + airFriction);
                else rigidBody2D.velocity = new Vector2(rigidBody2D.velocity.x, 0);
            }
        }
    }
}

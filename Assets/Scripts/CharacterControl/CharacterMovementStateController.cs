using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.CharacterControl
{
    public class CharacterMovementStateController
    {
        public MovementState currentMovementState { get; }
        private ControllableCharacter _character;
        private bool _isDebugModeOn;





        public CharacterMovementStateController(ControllableCharacter character, MovementState initialMovementState)
        {
            _character = character;
            currentMovementState = initialMovementState;
            _isDebugModeOn = character.isDebugModeOn;
        }
        public void UpdateCurrentState()
        {
            

            

            /*
             * possible triggers:
             *      TRIGGER_JUMP occurs when isJumpPressed and (GROUNDED or TETHERED)
                    TRIGGER_LANDING occurs when FLOATING and (),
                    TRIGGER_FALL,
                    TRIGGER_CORNER,
                    TRIGGER_GRAPPLE_ATTEMPT,
                    TRIGGER_GRAPPLE_SUCCESS,
                    TRIGGER_GRAPPLE_RELEASE,
                    TRIGGER_GRAPPLE_REACHED_ANCHOR,
             * 
             * possible transitions:
             *      FLOATING to GROUNDED occurs at TRIGGER_LANDING
             *      FLOATING to TETHERED occurs at TRIGGER_GRAPPLE_SUCCESS while FLOATING
             *      //FLOATING to CORNERING cannot happen
             *      GROUNDED to FLOATING occurs at TRIGGER_JUMP
             *      GROUNDED to TETHERED occurs at TRIGGER_GRAPPLE_SUCCESS while GROUNDED
             *      //GROUNDED to CORNERING occurs at TRIGGER_CORNER
             *      TETHERED to FLOATING occurs at TRIGGER_GRAPPLE_RELEASE
             *      TETHERED to GROUNDED occurs at (TRIGGER_GRAPPLE_RELEASE while GROUNDED) or TRIGGER_GRAPPLE_REACHED_ANCHOR
             *      //TETHERED to CORNERING cannot happen
             *      
             *      */

        }
    }
}

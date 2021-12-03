using Assets.Scripts.Utility;
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
        internal MovementState currentMovementState { get; private set; }
        internal List<MovementTrigger> triggers { get; private set; }

        private ControllableCharacter _character;
        private float _currentCorneringCounter;
        private bool _isClutchEngaged;  // the clutch prevents state changes during transitions

        internal CharacterMovementStateController(ControllableCharacter character, MovementState initialMovementState)
        {
            _character = character;
            currentMovementState = initialMovementState;

            triggers = new List<MovementTrigger>();
            _currentCorneringCounter = 0f;
            _isClutchEngaged = false;
        }
        internal void ResetClutch()
        {
            _isClutchEngaged = false;
        }
        internal void UpdateCurrentState()
        {
            // call this once per frame. it updates the current state
            // and triggers the controllable character to act

            if (_isClutchEngaged) return;

            DetermineTriggers();

            // cycle through each active trigger and set the state in order
            foreach (MovementTrigger t in triggers)
            {
                /*
                 * possible state transitions:
                 *      010  FLOATING to GROUNDED occurs at TRIGGER_LANDING
                 *      020  FLOATING to TETHERED occurs at TRIGGER_GRAPPLE_SUCCESS while FLOATING
                 *      030  FLOATING to JUMP_ACCELERATING cannot happen
                 *      040  GROUNDED to FLOATING occurs at TRIGGER_FALL
                 *      050  GROUNDED to TETHERED occurs at TRIGGER_GRAPPLE_SUCCESS while GROUNDED
                 *      060  GROUNDED to JUMP_ACCELERATING occurs at TRIGGER_JUMP
                 *      070  TETHERED to FLOATING occurs at TRIGGER_GRAPPLE_RELEASE
                 *      080  TETHERED to GROUNDED occurs at TRIGGER_GRAPPLE_REACHED_ANCHOR
                 *      090  TETHERED to JUMP_ACCELERATING occurs at TRIGGER_JUMP
                 *      100  JUMP_ACCELERATING to FLOATING occurs at TRIGGER_FALL
                 *      110  JUMP_ACCELERATING to GROUNDED occurs at TRIGGER_LANDING
                 *      120  JUMP_ACCELERATING to TETHERED occurs at TRIGGER_GRAPPLE_SUCCESS
                 *      
                 *      */

                MovementState newState = currentMovementState;

                switch (currentMovementState)
                {
                    case MovementState.FLOATING:
                        // 010
                        if (t == MovementTrigger.TRIGGER_LANDING)
                            newState = MovementState.GROUNDED;
                        // 020
                        if (t == MovementTrigger.TRIGGER_GRAPPLE_SUCCESS)
                            newState = MovementState.TETHERED;
                        // 030 cannot happen
                        
                        break;
                    case MovementState.GROUNDED:
                        // 040
                        if (t == MovementTrigger.TRIGGER_FALL)
                            newState = MovementState.FLOATING;
                        // 050
                        if (t == MovementTrigger.TRIGGER_GRAPPLE_SUCCESS)
                            newState = MovementState.TETHERED;
                        // 060
                        if (t == MovementTrigger.TRIGGER_JUMP)
                            newState = MovementState.JUMP_ACCELERATING;
                        break;
                    case MovementState.TETHERED:
                        // 070
                        if (t == MovementTrigger.TRIGGER_GRAPPLE_RELEASE)
                            newState = MovementState.FLOATING;
                        // 080
                        if (t == MovementTrigger.TRIGGER_GRAPPLE_STRUCK_GROUND)
                            newState = MovementState.GROUNDED;
                        // 090
                        if (t == MovementTrigger.TRIGGER_JUMP)
                            newState = MovementState.JUMP_ACCELERATING;
                        break;
                    case MovementState.JUMP_ACCELERATING:
                        // 100
                        if (t == MovementTrigger.TRIGGER_FALL)
                            newState = MovementState.FLOATING;
                        // 110
                        if (t == MovementTrigger.TRIGGER_LANDING)
                            newState = MovementState.GROUNDED;
                        // 120
                        if (t == MovementTrigger.TRIGGER_GRAPPLE_SUCCESS)
                            newState = MovementState.TETHERED;
                        break;
                }
                SetState(newState);
            }
        }
        private void DetermineTriggers()
        {
            triggers = new List<MovementTrigger>();
            
            // check for TRIGGER_JUMP
            if (_character.userInput.isJumpPressed
                 && (currentMovementState == MovementState.GROUNDED 
                 || currentMovementState == MovementState.TETHERED))
            {
                _character.HandleTrigger(MovementTrigger.TRIGGER_JUMP);
                triggers.Add(MovementTrigger.TRIGGER_JUMP);
                ResetCorneringCounter();
            }
            // check for TRIGGER_LANDING
            if (currentMovementState == MovementState.FLOATING
                && !_character.characterContactsCurrentFrame.isTouchingNothing)
            {
                // you can't always land. you either need to be touching 
                // with the character's base, or you need to be able to 
                // have a special ability
                bool canLand = false;
                if (_character.characterContactsCurrentFrame.isTouchingPlatformWithBase)
                {
                    // you can always land on your base
                    canLand = true;
                }
                else if (_character.characterContactsCurrentFrame.isTouchingPlatformForward
                    && _character.canWallCrawl)
                {
                    canLand = true;
                }
                else if (_character.characterContactsCurrentFrame.isTouchingPlatformWithTop
                    && _character.canCeilingCrawl)
                {
                    canLand = true;
                }
                if (canLand)
                {
                    _character.HandleTrigger(MovementTrigger.TRIGGER_LANDING);
                    triggers.Add(MovementTrigger.TRIGGER_LANDING);
                    ResetCorneringCounter();
                }
                
            }
            // check for TRIGGER_FALL
            if (
                    // walking off a platform
                    (currentMovementState == MovementState.GROUNDED 
                    && _character.characterContactsCurrentFrame.isTouchingNothing)
                    // or running out of jump juice
                    || (currentMovementState == MovementState.JUMP_ACCELERATING
                    && _character.currentJumpThrust >= _character.jumpThrustLimit)
                    // or releasing the jump button while jump accellerating
                    || (currentMovementState == MovementState.JUMP_ACCELERATING
                    && _character.userInput.isJumpReleased)
                )
            {
                _character.HandleTrigger(MovementTrigger.TRIGGER_FALL);
                triggers.Add(MovementTrigger.TRIGGER_FALL);
                ResetCorneringCounter();
            }
            // check for TRIGGER_CORNER
            if (ShouldTriggerCornering())
            {
                _character.HandleTrigger(MovementTrigger.TRIGGER_CORNER);
                triggers.Add(MovementTrigger.TRIGGER_CORNER);
                ResetCorneringCounter();
            }
            if (_character.canGrapple)
            {
                bool grappleSuccess = false;
                // check for TRIGGER_GRAPPLE_ATTEMPT
                if (_character.userInput.isGrappleButtonPressed
                     && (currentMovementState == MovementState.GROUNDED
                     || currentMovementState == MovementState.FLOATING
                     || currentMovementState == MovementState.JUMP_ACCELERATING))
                {
                    grappleSuccess = _character.HandleTrigger(MovementTrigger.TRIGGER_GRAPPLE_ATTEMPT);
                    triggers.Add(MovementTrigger.TRIGGER_GRAPPLE_ATTEMPT);
                    ResetCorneringCounter();
                }
                // check for TRIGGER_GRAPPLE_SUCCESS
                if (grappleSuccess)
                {
                    _character.HandleTrigger(MovementTrigger.TRIGGER_GRAPPLE_SUCCESS);
                    triggers.Add(MovementTrigger.TRIGGER_GRAPPLE_SUCCESS);
                    ResetCorneringCounter();
                }
                // check for TRIGGER_GRAPPLE_RELEASE
                if (currentMovementState == MovementState.TETHERED
                    && _character.userInput.isGrappleButtonReleased)
                {
                    _character.HandleTrigger(MovementTrigger.TRIGGER_GRAPPLE_RELEASE);
                    triggers.Add(MovementTrigger.TRIGGER_GRAPPLE_RELEASE);
                    ResetCorneringCounter();
                }
                // check for TRIGGER_GRAPPLE_STRUCK_GROUND
                if (currentMovementState == MovementState.TETHERED
                    && !_character.characterContactsCurrentFrame.isTouchingNothing)
                {
                    _character.HandleTrigger(MovementTrigger.TRIGGER_GRAPPLE_STRUCK_GROUND);
                    triggers.Add(MovementTrigger.TRIGGER_GRAPPLE_STRUCK_GROUND);
                    ResetCorneringCounter();
                }
            }
        }
        private void ResetCorneringCounter()
        {
            _currentCorneringCounter = 0f;
        }
        private void SetState(MovementState newState)
        {
            if (currentMovementState != newState)
            {
                LoggerCustom.DEBUG(string.Format("State change: {0} to {1}",
                    currentMovementState.ToString(), newState.ToString()));

                currentMovementState = newState;

                // turn on the clutch to prevent new state transitions
                // before the this one can be acted on
                _isClutchEngaged = true;
            }
        }
        private bool ShouldTriggerCornering()
        {
            if (_character.canWallCrawl
                && _character.characterContactsCurrentFrame.isTouchingPlatformWithBase 
                && _character.characterContactsCurrentFrame.isTouchingPlatformForward
                && (
                        // can wall crawl and moving toward a wall
                        (_character.canWallCrawl
                        && (
                            // facing right + positive moveHPressure
                            (_character.characterOrienter.headingDirection == FacingDirection.RIGHT
                            && _character.userInput.moveHPressure > 0)
                            || // facing left + negative moveHPressure
                            (_character.characterOrienter.headingDirection == FacingDirection.LEFT
                            && _character.userInput.moveHPressure < 0)
                        ))
                        || // can ceiling crawl and moving toward a ceiling
                        (_character.canCeilingCrawl
                        && (
                            // facing up + positive moveVPressure
                            (_character.characterOrienter.headingDirection == FacingDirection.UP
                            && _character.userInput.moveVPressure > 0)
                        ))
                        || // facing down + negative moveVPressure. no special capability req'd
                        (_character.characterOrienter.headingDirection == FacingDirection.DOWN
                        && _character.userInput.moveVPressure < 0)
                    )
                )
            {
                // check if we've done this enough time
                _currentCorneringCounter += Time.deltaTime;
                LoggerCustom.DEBUG("At a corner. Current counter: " + _currentCorneringCounter);
                if (_currentCorneringCounter >= _character.corneringTimeRequired)
                {
                    LoggerCustom.DEBUG("triggering a corner");
                    return true;
                }
            }
            return false;
        }
    }
}

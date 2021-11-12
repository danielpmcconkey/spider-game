using Assets.Scripts.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.CharacterControl
{
    public class CharacterStateMachine
    {
        //private int stateTransitionCurrentCountdown; // while this is above zero don't check to see if new interaction w/ platform object. decrement every frame
        public CharacterState playerStateCurrentFrame;
        public CharacterState playerStatePriorFrame;

        private GameObject _gameObject;
        private bool _isTouchingNothing = true;
        private bool _isTouchingPlatformWithBelly;
        private bool _isTouchingPlatformForward;
        private bool _isTouchingPlatformWithBack;   // meaning the top of the spider
        private CharacterOrienter _characterOrienter;
        private CharacterMovementCapabilities _movementCapabilities;
        private UserInputCollection _userInput;
        private PlatformCollisionTransforms _collisionTransforms;
        private const float _platformContactCheckRadius = .2f; // Radius of the overlap circle to determine if touching a platform
        private float _maxJumpThrust;


        #region public methods
        public CharacterStateMachine(GameObject gameObject, PlatformCollisionTransforms collisionTransforms, 
            float maxJumpThrust, CharacterOrienter characterOrienter)
        {
            _gameObject = gameObject;
            _collisionTransforms = collisionTransforms;
            _maxJumpThrust = maxJumpThrust;
            _characterOrienter = characterOrienter;

            playerStateCurrentFrame = CharacterState.IDLE;
            playerStatePriorFrame = CharacterState.IDLE;
        }
        public void CheckState(CharacterMovementCapabilities movementCapabilities,
            UserInputCollection userInput,
            float currentJumpThrust)
        {
            /*
             * check the "state" of the player every frame. This means to check whether
             * we're landed, landing, running, falling, jumping, etc. Do not move the
             * transform in this method. Only update variables so that movement can 
             * happen in the FixedUpdate() method
             * */

            _movementCapabilities = movementCapabilities;
            _userInput = userInput;

            bool wasTouchingNothing = _isTouchingNothing;
            bool wasTouchingPlatformWithBelly = _isTouchingPlatformWithBelly;
            bool wasTouchingPlatformForward = _isTouchingPlatformForward;

            // find out what which platforms we're touching
            // and with what parts of the tank
            PopulatePlatformCollisionVars();

            // figure out if we're beginning a jump, feuling an 
            // existing jump, or reading to start falling again
            CheckJumpStates(currentJumpThrust);

            // determine whether we're landing or free falling
            if (_isTouchingNothing && wasTouchingNothing
                && playerStateCurrentFrame != CharacterState.ADDIND_THRUST_TO_JUMP
                && playerStateCurrentFrame != CharacterState.TRIGGER_JUMP
                && playerStateCurrentFrame != CharacterState.EARLY_JUMP_CYCLE
                && playerStateCurrentFrame != CharacterState.TRIGGER_FALL)
            {
                SetState(CharacterState.FALLING);
            }
            else if (_isTouchingNothing && !wasTouchingNothing
                && playerStateCurrentFrame != CharacterState.ADDIND_THRUST_TO_JUMP
                && playerStateCurrentFrame != CharacterState.TRIGGER_JUMP
                && playerStateCurrentFrame != CharacterState.EARLY_JUMP_CYCLE
                && playerStateCurrentFrame != CharacterState.EARLY_LANDING_CYCLE_H
                && playerStateCurrentFrame != CharacterState.EARLY_LANDING_CYCLE_V)
            {
                SetState(CharacterState.TRIGGER_FALL);
            }
            else if (_isTouchingNothing && !wasTouchingNothing
                && playerStateCurrentFrame != CharacterState.ADDIND_THRUST_TO_JUMP
                && playerStateCurrentFrame != CharacterState.TRIGGER_JUMP
                && playerStateCurrentFrame != CharacterState.EARLY_JUMP_CYCLE
                && playerStateCurrentFrame != CharacterState.EARLY_LANDING_CYCLE_H
                && playerStateCurrentFrame != CharacterState.EARLY_LANDING_CYCLE_V)
            {
                SetState(CharacterState.FALLING);
            }
            else if (wasTouchingNothing && _isTouchingPlatformWithBelly
                && playerStateCurrentFrame != CharacterState.EARLY_JUMP_CYCLE
                && _characterOrienter.thrustingDirection == FacingDirection.UP
                && playerStateCurrentFrame != CharacterState.EARLY_LANDING_CYCLE_H
                && playerStateCurrentFrame != CharacterState.EARLY_LANDING_CYCLE_V)
            {
                SetState(CharacterState.TRIGGER_LANDING_H);
            }
            else if (wasTouchingNothing && _isTouchingPlatformForward
                && playerStateCurrentFrame != CharacterState.EARLY_JUMP_CYCLE
                && _characterOrienter.thrustingDirection == FacingDirection.UP
                && movementCapabilities.canWallCrawl)
            {
                SetState(CharacterState.TRIGGER_LANDING_V);
            }
            else if (wasTouchingNothing && _isTouchingPlatformWithBack
                && playerStateCurrentFrame != CharacterState.EARLY_JUMP_CYCLE
                && _characterOrienter.thrustingDirection == FacingDirection.UP
                && movementCapabilities.canCeilingCrawl)
            {
                SetState(CharacterState.TRIGGER_LANDING_CEILING);
                LoggerCustom.INFO("BITCHES");
            }
            else if (wasTouchingNothing && _isTouchingPlatformForward
                && playerStateCurrentFrame != CharacterState.EARLY_LANDING_CYCLE_H
                && playerStateCurrentFrame != CharacterState.EARLY_LANDING_CYCLE_V
                && playerStateCurrentFrame != CharacterState.EARLY_JUMP_CYCLE
                && _characterOrienter.thrustingDirection == FacingDirection.UP
                && !movementCapabilities.canWallCrawl)
            {
                SetState(CharacterState.TRIGGER_FALL);
            }
            // figure out if we're running
            else if (playerStateCurrentFrame != CharacterState.TRIGGER_JUMP
                && playerStateCurrentFrame != CharacterState.EARLY_JUMP_CYCLE
                && playerStateCurrentFrame != CharacterState.ADDIND_THRUST_TO_JUMP
                && _isTouchingPlatformWithBelly
                && (
                        // facing left/right + moveHPressure
                        ((_characterOrienter.headingDirection == FacingDirection.LEFT 
                            || _characterOrienter.headingDirection == FacingDirection.RIGHT)
                        && userInput.moveHPressure != 0)
                        ||
                        // facing up/down + moveVPressure
                        ((_characterOrienter.headingDirection == FacingDirection.UP 
                            || _characterOrienter.headingDirection == FacingDirection.DOWN)
                        && userInput.moveVPressure != 0)
                    )
                )
            {
                SetState(CharacterState.RUNNING);
            }
            // figure out if we're idle
            // check we're not jumping before we put it in idle
            else if (playerStateCurrentFrame != CharacterState.ADDIND_THRUST_TO_JUMP
                && playerStateCurrentFrame != CharacterState.TRIGGER_JUMP
                && playerStateCurrentFrame != CharacterState.EARLY_JUMP_CYCLE
                && playerStateCurrentFrame != CharacterState.TRIGGER_LANDING_V
                && playerStateCurrentFrame != CharacterState.TRIGGER_LANDING_CEILING
                && playerStateCurrentFrame != CharacterState.TRIGGER_FALL)
            {
                SetState(CharacterState.IDLE);

            }



            if (playerStatePriorFrame != playerStateCurrentFrame)
            {
                LoggerCustom.DEBUG(string.Format("State change: {0} to {1}",
                    playerStatePriorFrame.ToString(), playerStateCurrentFrame.ToString()));
            }
            playerStatePriorFrame = playerStateCurrentFrame;

        }
        public void SetState(CharacterState newState)
        {
            playerStateCurrentFrame = newState;
        }
        #endregion

        #region private methods
        private bool CanWeBeginJump()
        {
            bool canWeJump = false;

            if (playerStateCurrentFrame == CharacterState.IDLE
                || playerStateCurrentFrame == CharacterState.RUNNING)
            {
                if (_isTouchingPlatformWithBelly)
                {
                    //if (stateTransitionCurrentCountdown == 0)
                    //{
                    canWeJump = true;
                    //}
                }
            }

            return canWeJump;
        }
        private void CheckJumpStates(float currentJumpThrust)
        {
            // check to see if we are beginning a jump
            if (_userInput.isJumpPressed && CanWeBeginJump())
            {
                SetState(CharacterState.TRIGGER_JUMP);
            }
            // check to see if we need to start jumping
            // and start falling
            else if (playerStateCurrentFrame == CharacterState.EARLY_JUMP_CYCLE
                || playerStateCurrentFrame == CharacterState.ADDIND_THRUST_TO_JUMP)
            {
                if (_userInput.isJumpHeldDown)
                {
                    SetState(CharacterState.ADDIND_THRUST_TO_JUMP);
                    if (currentJumpThrust > _maxJumpThrust)
                    {
                        SetState(CharacterState.TRIGGER_FALL);
                    }
                }
                if (_userInput.isJumpReleased)
                {
                    SetState(CharacterState.TRIGGER_FALL);
                }
            }
        }
        private bool IsCollidingWithPlatform(Transform checkingTransform)
        {
            Collider2D[] collisions = Physics2D.OverlapCircleAll(
                checkingTransform.position, _platformContactCheckRadius, _collisionTransforms.whatIsPlatform);

            for (int i = 0; i < collisions.Length; i++)
            {
                if (collisions[i].gameObject != _gameObject)
                {
                    return true;
                }
            }

            return false;
        }
        private void PopulatePlatformCollisionVars()
        {
            _isTouchingPlatformWithBelly = false;
            _isTouchingPlatformForward = false;
            _isTouchingPlatformWithBelly = IsCollidingWithPlatform(_collisionTransforms.bellyCheckTransform);
            _isTouchingPlatformForward = IsCollidingWithPlatform(_collisionTransforms.forwardCheckTransform);
            _isTouchingPlatformWithBack = IsCollidingWithPlatform(_collisionTransforms.ceilingCheckTransform);
            _isTouchingNothing = (!_isTouchingPlatformWithBelly && !_isTouchingPlatformForward) ? true : false;
        } 
        #endregion
    }
}

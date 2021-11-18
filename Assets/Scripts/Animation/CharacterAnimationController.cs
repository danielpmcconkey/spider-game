using Assets.Scripts.CharacterControl;
using Assets.Scripts.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Animation
{
    public class CharacterAnimationController
    {
        public AnimationState currentState;
        private Animator _animator;
        private bool _isDebugModeOn = false;

        public CharacterAnimationController(Animator animator, bool isDebugModeOn)
        {
            _animator = animator;
            _isDebugModeOn = isDebugModeOn;
        }
        public void SetState()
        {
            currentState = AnimationState.IDLE_RIGHT;
            // todo: rework the animator state to match our new states
            SetAnimatorVariables();
        }
        public void SetState(DEPRECATED_CharacterState characterState, CharacterOrienter characterOrienter)
        {
            AnimationState priorState = currentState;
            AnimationState newState = AnimationState.IDLE_RIGHT;

            // new state logic
            bool isStarboardFacingCamera = IsStarboardFacingCamera(characterState, characterOrienter);
            switch (characterState)
            {
                case DEPRECATED_CharacterState.DEPRECATED_FALLING:
                case DEPRECATED_CharacterState.TRIGGER_FALL:
                    newState = (isStarboardFacingCamera) ? AnimationState.FALLING_RIGHT : AnimationState.FALLING_LEFT;
                    break;
                case DEPRECATED_CharacterState.RUNNING:
                    newState = (isStarboardFacingCamera) ? AnimationState.RUNNING_RIGHT : AnimationState.RUNNING_LEFT;
                    break;
                case DEPRECATED_CharacterState.TRIGGER_JUMP:
                case DEPRECATED_CharacterState.EARLY_JUMP_CYCLE:
                case DEPRECATED_CharacterState.ADDIND_THRUST_TO_JUMP:
                    newState = (isStarboardFacingCamera) ? AnimationState.JUMPING_RIGHT : AnimationState.JUMPING_LEFT;
                    break;
                case DEPRECATED_CharacterState.TRIGGER_LANDING_H:
                case DEPRECATED_CharacterState.TRIGGER_LANDING_V:
                case DEPRECATED_CharacterState.TRIGGER_LANDING_CEILING:
                case DEPRECATED_CharacterState.EARLY_LANDING_CYCLE_H:
                case DEPRECATED_CharacterState.EARLY_LANDING_CYCLE_V:
                    newState = (isStarboardFacingCamera) ? AnimationState.IDLE_RIGHT : AnimationState.IDLE_LEFT;
                    break;
                case DEPRECATED_CharacterState.DEPRECATED_IDLE:
                default:
                    newState = (isStarboardFacingCamera) ? AnimationState.IDLE_RIGHT : AnimationState.IDLE_LEFT;
                    break;
            }
            // end new state logic

            if (_isDebugModeOn && newState != priorState)
            {
                LoggerCustom.DEBUG(string.Format("Changing animation state from {0} to {1}", priorState, newState));
            }
            currentState = newState;
            SetAnimatorVariables();
        }
        private bool IsStarboardFacingCamera(DEPRECATED_CharacterState characterState, CharacterOrienter characterOrienter)
        {
            switch (characterOrienter.headingDirection)
            {
                case FacingDirection.RIGHT:
                    if (characterOrienter.thrustingDirection == FacingDirection.UP)
                    {
                        return true;
                    }
                    else if (characterOrienter.thrustingDirection == FacingDirection.DOWN)
                    {
                        return false;
                    }
                    break;
                case FacingDirection.LEFT:
                    if (characterOrienter.thrustingDirection == FacingDirection.UP)
                    {
                        return false;
                    }
                    else if (characterOrienter.thrustingDirection == FacingDirection.DOWN)
                    {
                        return true;
                    }
                    break;
                case FacingDirection.UP:
                    if (characterOrienter.thrustingDirection == FacingDirection.LEFT)
                    {
                        return true;
                    }
                    else if (characterOrienter.thrustingDirection == FacingDirection.RIGHT)
                    {
                        return false;
                    }
                    break;
                case FacingDirection.DOWN:
                    if (characterOrienter.thrustingDirection == FacingDirection.LEFT)
                    {
                        return false;
                    }
                    else if (characterOrienter.thrustingDirection == FacingDirection.RIGHT)
                    {
                        return true;
                    }
                    break;
            }
            return true;
        }
        private void SetAnimatorVariables()
        {
            (string name, bool value)[] animationVars = new (string name, bool value)[] {
                ("isIdleRight", false),
                ("isIdleLeft", false),
                ("isMovingRight", false),
                ("isMovingLeft", false),
                ("isJumpingRight", false),
                ("isJumpingLeft", false),
                ("isFallingRight", false),
                ("isFallingLeft", false),
            };

            switch(currentState)
            {
                case AnimationState.IDLE_LEFT:
                    animationVars[1].value = true;
                    break;
                case AnimationState.RUNNING_LEFT:
                    animationVars[3].value = true;
                    break;
                case AnimationState.FALLING_LEFT:
                    animationVars[7].value = true;
                    break;
                case AnimationState.JUMPING_LEFT:
                    animationVars[5].value = true;
                    break;
                case AnimationState.IDLE_RIGHT:
                    animationVars[0].value = true;
                    break;
                case AnimationState.RUNNING_RIGHT:
                    animationVars[2].value = true;
                    break;
                case AnimationState.FALLING_RIGHT:
                    animationVars[6].value = true;
                    break;
                case AnimationState.JUMPING_RIGHT:
                    animationVars[4].value = true;
                    break;
            }

            for (int i = 0; i < animationVars.Length; i++)
            {
                _animator.SetBool(animationVars[i].name, animationVars[i].value);
            }
        }

        
    }
}

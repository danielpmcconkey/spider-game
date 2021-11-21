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
        private ControllableCharacter _character;
        private bool _isDebugModeOn = false;

        public CharacterAnimationController(ControllableCharacter character, Animator animator, bool isDebugModeOn)
        {
            _character = character;
            _animator = animator;
            _isDebugModeOn = isDebugModeOn;
        }
        public void UpdateCurrentState()
        {
            AnimationState priorState = currentState;
            AnimationState newState = AnimationState.IDLE_RIGHT;

            // new state logic
            bool isStarboardFacingCamera = IsStarboardFacingCamera();
            
            switch (_character.GetCurrentState())
            {
                case MovementState.FLOATING:
                case MovementState.TETHERED:
                    if(_character.rigidBody2D.velocity.y > 2f)
                    {
                        newState = (isStarboardFacingCamera) ? AnimationState.JUMPING_RIGHT : AnimationState.JUMPING_LEFT;
                    }
                    else newState = (isStarboardFacingCamera) ? AnimationState.FALLING_RIGHT : AnimationState.FALLING_LEFT;
                    break;
                case MovementState.GROUNDED:
                    if(_character.userInput.moveHPressure > 0.1f 
                        && _character.characterOrienter.headingDirection == FacingDirection.RIGHT)
                    {
                        newState = (isStarboardFacingCamera) ? AnimationState.RUNNING_RIGHT : AnimationState.RUNNING_LEFT;
                    }
                    else if (_character.userInput.moveHPressure < -0.1f
                        && _character.characterOrienter.headingDirection == FacingDirection.LEFT)
                    {
                        newState = (isStarboardFacingCamera) ? AnimationState.RUNNING_RIGHT : AnimationState.RUNNING_LEFT;
                    }
                    else if (_character.userInput.moveVPressure > 0.1f
                        && _character.characterOrienter.headingDirection == FacingDirection.UP)
                    {
                        newState = (isStarboardFacingCamera) ? AnimationState.RUNNING_RIGHT : AnimationState.RUNNING_LEFT;
                    }
                    else if (_character.userInput.moveVPressure < -0.1f
                        && _character.characterOrienter.headingDirection == FacingDirection.DOWN)
                    {
                        newState = (isStarboardFacingCamera) ? AnimationState.RUNNING_RIGHT : AnimationState.RUNNING_LEFT;
                    }
                    else
                    {
                        newState = (isStarboardFacingCamera) ? AnimationState.IDLE_RIGHT : AnimationState.IDLE_LEFT;
                    }
                    break;
                case MovementState.JUMP_ACCELERATING:
                    newState = (isStarboardFacingCamera) ? AnimationState.JUMPING_RIGHT : AnimationState.JUMPING_LEFT;
                    break;
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
        private bool IsStarboardFacingCamera()
        {
            switch (_character.characterOrienter.headingDirection)
            {
                case FacingDirection.RIGHT:
                    if (_character.characterOrienter.thrustingDirection == FacingDirection.UP)
                    {
                        return true;
                    }
                    else if (_character.characterOrienter.thrustingDirection == FacingDirection.DOWN)
                    {
                        return false;
                    }
                    break;
                case FacingDirection.LEFT:
                    if (_character.characterOrienter.thrustingDirection == FacingDirection.UP)
                    {
                        return false;
                    }
                    else if (_character.characterOrienter.thrustingDirection == FacingDirection.DOWN)
                    {
                        return true;
                    }
                    break;
                case FacingDirection.UP:
                    if (_character.characterOrienter.thrustingDirection == FacingDirection.LEFT)
                    {
                        return true;
                    }
                    else if (_character.characterOrienter.thrustingDirection == FacingDirection.RIGHT)
                    {
                        return false;
                    }
                    break;
                case FacingDirection.DOWN:
                    if (_character.characterOrienter.thrustingDirection == FacingDirection.LEFT)
                    {
                        return false;
                    }
                    else if (_character.characterOrienter.thrustingDirection == FacingDirection.RIGHT)
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

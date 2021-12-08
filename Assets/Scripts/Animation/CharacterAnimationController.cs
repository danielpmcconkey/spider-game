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
        private AnimationState _priorState;
        private Animator _animator;
        private ControllableCharacter _character;
        

        public CharacterAnimationController(ControllableCharacter character, Animator animator)
        {
            _character = character;
            _animator = animator;
        }
        public void UpdateCurrentState()
        {
            _priorState = currentState;
            AnimationState newState = AnimationState.IDLE;

            if (!_character.isAlive)
            {
                newState = AnimationState.DEAD;
            }
            else
            {
                switch (_character.GetCurrentState())
                {
                    case MovementState.FLOATING:
                    case MovementState.TETHERED:
                        if (_character.rigidBody2D.velocity.y > 2f)
                        {
                            newState = AnimationState.JUMPING;
                        }
                        else newState = AnimationState.FALLING;
                        break;
                    case MovementState.GROUNDED:
                        if (_character.userInput.moveHPressure > 0.1f
                            && _character.characterOrienter.headingDirection == FacingDirection.RIGHT)
                        {
                            newState = AnimationState.RUNNING;
                        }
                        else if (_character.userInput.moveHPressure < -0.1f
                            && _character.characterOrienter.headingDirection == FacingDirection.LEFT)
                        {
                            newState = AnimationState.RUNNING;
                        }
                        else if (_character.userInput.moveVPressure > 0.1f
                            && _character.characterOrienter.headingDirection == FacingDirection.UP)
                        {
                            newState = AnimationState.RUNNING;
                        }
                        else if (_character.userInput.moveVPressure < -0.1f
                            && _character.characterOrienter.headingDirection == FacingDirection.DOWN)
                        {
                            newState = AnimationState.RUNNING;
                        }
                        else
                        {
                            newState = AnimationState.IDLE;
                        }
                        break;
                    case MovementState.JUMP_ACCELERATING:
                        newState = AnimationState.JUMPING;
                        break;
                    default:
                        newState = AnimationState.IDLE;
                        break;
                }

                // after checking the state controller, now check if the character
                // is taking damage
                if (_character.isTakingDamage)
                {
                    newState = AnimationState.TAKING_DAMAGE;
                }

                if (newState != _priorState)
                {
                    LoggerCustom.DEBUG(string.Format("Changing animation state from {0} to {1}", _priorState, newState));
                }
            }
            currentState = newState;
            SetAnimatorVariables();
        }
        private void SetAnimatorVariables()
        {
            if (_priorState == currentState) return;    // don't start an already playing animation
            switch (currentState)
            {
                case AnimationState.RUNNING:
                    _animator.Play(_character.movingAnimationName);
                    break;
                case AnimationState.FALLING:
                    _animator.Play(_character.fallingAnimationName);
                    break;
                case AnimationState.JUMPING:
                    _animator.Play(_character.jumpingAnimationName);
                    break;
                case AnimationState.TAKING_DAMAGE:
                    _animator.Play(_character.damageAnimationName);
                    break;
                case AnimationState.DEAD:
                    _animator.Play(_character.deathAnimationName);
                    break;
                case AnimationState.IDLE:
                default:
                    _animator.Play(_character.idleAnimationName);
                    break;
            }
        }

        
    }
}

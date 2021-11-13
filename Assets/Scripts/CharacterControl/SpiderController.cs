using Assets.Scripts;
using Assets.Scripts.Animation;
using Assets.Scripts.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;



namespace Assets.Scripts.CharacterControl
{
    public class SpiderController : MonoBehaviour
    {
        #region Vars set in Unity
        [Header("Debug")]
        [Space(10)]
        [SerializeField] public bool isDebugModeOn = false;
        [SerializeField] public string debugLogFileDirectory = string.Empty; // C:\Users\Dan\AppData\LocalLow\SpiderController\
        public UnityEngine.UI.Text debugTextBox;

        [Header("Movement parameters")]
        [Space(10)]
        [SerializeField] public float runSpeed = 55f;
        [SerializeField] public float initialJumpThrust = 10f;
        [SerializeField] public float jumpThrustPerSecond = 80f;
        [SerializeField] public float maxJumpThrust = 30f;
        [Range(0, .3f)] [SerializeField] public float movementSmoothing = .05f;  // How much to smooth out the movement
        [SerializeField] public float gravityOnPlayer = 40f;
        [SerializeField] public float rotationDegreesPerSecond = 720;

        [Header("Movement abilities")]
        [Space(10)]
        [SerializeField] public bool isHorizontalMovementInAirAllowed = true;
        [SerializeField] public bool canWallCrawl = true;
        [SerializeField] public bool canCeilingCrawl = true;

        [Header("Movement references")]
        [Space(10)]
        [SerializeField] public Animator animator;
        [SerializeField] public Rigidbody2D rigidBody2D;
        [SerializeField] public Transform bellyCheckTransform;
        [SerializeField] public Transform forwardCheckTransform;
        [SerializeField] public Transform ceilingCheckTransform;
        [SerializeField] public LayerMask whatIsPlatform;  // A mask determining what is ground to the character 
        #endregion

        // private movement vars
        private CharacterMovementCapabilities _movementCapabilities;
        private CharacterStateMachine _stateMachine;
        private UserInputCollection _userInput;
        private CharacterOrienter _characterOrienter;
        private float _currentJumpThrust;
        private Vector3 _velocity = Vector3.zero; // I don't know what it was but it was in the script I copied from
        private CharacterAnimationController _characterAnimationController;
        private Vector3 _startRotation;
        private Vector3 _targetRotation;
        private float _clutchCountDown = 0f; // when > 0, then the clutch is engaged, disallowing state transitions
        private float _numSecondsToEngageClutch = 0.25f;

        #region overrides of the base class
        private void Awake()
        {
            _userInput = new UserInputCollection()
            {
                moveHPressure = 0f,
                moveVPressure = 0f,
                isJumpPressed = false,
                isJumpReleased = false,
                isJumpHeldDown = false,
            };
            _movementCapabilities = new CharacterMovementCapabilities()
            {
                canWallCrawl = this.canWallCrawl,
                canCeilingCrawl = this.canCeilingCrawl,
                isHorizontalMovementInAirAllowed = this.isHorizontalMovementInAirAllowed,
            };
            _characterOrienter = new CharacterOrienter(isDebugModeOn);

            PlatformCollisionTransforms collisionTransforms = new PlatformCollisionTransforms()
            {
                bellyCheckTransform = bellyCheckTransform,
                forwardCheckTransform = forwardCheckTransform,
                ceilingCheckTransform = ceilingCheckTransform,
                whatIsPlatform = whatIsPlatform
            };
            _stateMachine = new CharacterStateMachine(gameObject, collisionTransforms, maxJumpThrust, 
                _characterOrienter, isDebugModeOn);
            
            _characterAnimationController = new CharacterAnimationController(animator, isDebugModeOn);



            LoggerCustom.Init(isDebugModeOn, debugLogFileDirectory, Time.frameCount);


            LoggerCustom.INFO("**********************************************************************************");
            LoggerCustom.INFO("BEGINNING NEW GAME");
            LoggerCustom.INFO("**********************************************************************************");
            LogAllVarsState();

        }
        private void Update()
        {
            LoggerCustom.SetFrameCount(Time.frameCount);
            CheckUserInupt();
            _stateMachine.CheckState(_movementCapabilities, _userInput, _currentJumpThrust);
            _characterAnimationController.SetState(_stateMachine.playerStateCurrentFrame, _characterOrienter);
            // act on the trigger states. these states
            // shouldn't move the character. Just trigger
            // certain events that will be acted on in the
            // next FixedUpdate()
            if (_stateMachine.playerStateCurrentFrame == CharacterState.TRIGGER_JUMP)
            {
                BeginJump();
            }
            if (_stateMachine.playerStateCurrentFrame == CharacterState.TRIGGER_FALL)
            {
                BeginFall();
            }
            if (_stateMachine.playerStateCurrentFrame == CharacterState.TRIGGER_LANDING_H)
            {
                Land();
            }
            if (_stateMachine.playerStateCurrentFrame == CharacterState.TRIGGER_LANDING_V)
            {
                LandV();
            }
            if (_stateMachine.playerStateCurrentFrame == CharacterState.TRIGGER_LANDING_CEILING)
            {
                LandCeiling();
            }

            if (_clutchCountDown > 0f)
            {
                _stateMachine.SetClutch(true);
                _clutchCountDown -= Time.deltaTime;
            }
            else _stateMachine.SetClutch(false);

        }
        private void FixedUpdate()
        {
            // here is where we check the state and perform the appropriate 
            // action

            float thrustWeMightAdd = (jumpThrustPerSecond * Time.deltaTime);
            
            if (_stateMachine.playerStateCurrentFrame == CharacterState.ADDIND_THRUST_TO_JUMP)
            {
                ApplyJumpForce(thrustWeMightAdd);
            }
            
            
            if (_stateMachine.playerStateCurrentFrame == CharacterState.RUNNING)
            {
                ApplyDirectionalMovementForceH();
                ApplyDirectionalMovementForceV();
            }
            if (
                (_stateMachine.playerStateCurrentFrame == CharacterState.FALLING
                || _stateMachine.playerStateCurrentFrame == CharacterState.ADDIND_THRUST_TO_JUMP)
                && _movementCapabilities.isHorizontalMovementInAirAllowed)
            {
                ApplyDirectionalMovementForceH();
            }
            if (_stateMachine.playerStateCurrentFrame == CharacterState.IDLE)
            {
                // apply 0 force to stop sliding
                if (_characterOrienter.headingDirection == FacingDirection.RIGHT
                    || _characterOrienter.headingDirection == FacingDirection.LEFT)
                {
                    ApplyDirectionalMovementForceH();
                }
                if (_characterOrienter.headingDirection == FacingDirection.UP
                    || _characterOrienter.headingDirection == FacingDirection.DOWN)
                {
                    ApplyDirectionalMovementForceV();
                }
            }

            // apply artifical gravity based on which way we're facing
            ApplyArtificalGravity();

            // slowly rotatate character towards target
            RotateCharacterByStep();

            if (isDebugModeOn) WriteDebugInfoToUi();
        }
        void OnDestroy()
        {
            if (isDebugModeOn)
            {
                LogAllVarsState();
                LoggerCustom.INFO("**********************************************************************************");
                LoggerCustom.INFO("GAME OVER");
                LoggerCustom.INFO("**********************************************************************************");

                // write out the log to a log file
                LoggerCustom.WriteLogToFile();

            }
        }
        #endregion

        #region controller methods
        private void ApplyArtificalGravity()
        {
            FacingDirection formerGravityDirection = _characterOrienter.gravityDirection;
            float thrustMultiplier = gravityOnPlayer * Time.deltaTime;

            if (_characterOrienter.headingDirection == FacingDirection.RIGHT
                || _characterOrienter.headingDirection == FacingDirection.LEFT)
            {
                if (_characterOrienter.thrustingDirection == FacingDirection.UP)
                {
                    _characterOrienter.SetGravityDirection(FacingDirection.DOWN);
                    thrustMultiplier *= -1f;
                }
                else if (_characterOrienter.thrustingDirection == FacingDirection.DOWN)
                {
                    _characterOrienter.SetGravityDirection(FacingDirection.UP);
                    thrustMultiplier *= 1f;
                }
                rigidBody2D.velocity += new Vector2(0, thrustMultiplier);
            }
            if (_characterOrienter.headingDirection == FacingDirection.UP
                || _characterOrienter.headingDirection == FacingDirection.DOWN)
            {
                if (_characterOrienter.thrustingDirection == FacingDirection.LEFT)
                {
                    _characterOrienter.SetGravityDirection(FacingDirection.RIGHT);
                    thrustMultiplier *= 1f;
                }
                else if (_characterOrienter.thrustingDirection == FacingDirection.RIGHT)
                {
                    _characterOrienter.SetGravityDirection(FacingDirection.LEFT);
                    thrustMultiplier *= -1f;
                }
                rigidBody2D.velocity += new Vector2(thrustMultiplier, 0);
            }
        }
        private void ApplyDirectionalMovementForceH()
        {
            if (_characterOrienter.headingDirection == FacingDirection.RIGHT
                || _characterOrienter.headingDirection == FacingDirection.LEFT)
            {
                MoveH(_userInput.moveHPressure * runSpeed * Time.deltaTime);
            }
        }
        private void ApplyDirectionalMovementForceV()
        {
            if (_characterOrienter.headingDirection == FacingDirection.UP
                || _characterOrienter.headingDirection == FacingDirection.DOWN)
            {
                MoveV(_userInput.moveVPressure * runSpeed * Time.deltaTime);
            }
        }
        private void ApplyJumpForce(float force)
        {
            _currentJumpThrust += force;

            LoggerCustom.DEBUG(string.Format("Adding jump force of {0}. Current total thrust of {1}.", force, _currentJumpThrust));

            Vector2 thrust = new Vector2(0f, force);
            switch (_characterOrienter.thrustingDirection)
            {
                case FacingDirection.UP:
                    thrust = new Vector2(0f, force);
                    break;
                case FacingDirection.DOWN:
                    thrust = new Vector2(0f, -force);
                    break;
                case FacingDirection.RIGHT:
                    thrust = new Vector2(force, 0f);
                    break;
                case FacingDirection.LEFT:
                    thrust = new Vector2(-force, 0f);
                    break;
            }
            //rigidBody2D.AddForce(thrust);
            rigidBody2D.velocity += thrust;
        }
        private void BeginFall()
        {
            LoggerCustom.DEBUG("Begin falling");

            FacingDirection targetHeading = _characterOrienter.headingDirection;
            if (_characterOrienter.headingDirection == FacingDirection.UP && _characterOrienter.thrustingDirection == FacingDirection.LEFT)
            {
                targetHeading = FacingDirection.RIGHT;
            }
            if (_characterOrienter.headingDirection == FacingDirection.UP && _characterOrienter.thrustingDirection == FacingDirection.RIGHT)
            {
                targetHeading = FacingDirection.LEFT;
            }
            if (_characterOrienter.headingDirection == FacingDirection.DOWN && _characterOrienter.thrustingDirection == FacingDirection.LEFT)
            {
                targetHeading = FacingDirection.LEFT;
            }
            if (_characterOrienter.headingDirection == FacingDirection.DOWN && _characterOrienter.thrustingDirection == FacingDirection.RIGHT)
            {
                targetHeading = FacingDirection.RIGHT;
            }
            SetFacingDirections(targetHeading, FacingDirection.UP);


            _currentJumpThrust = 0f;
            _stateMachine.SetState(CharacterState.FALLING);
        }
        private void BeginJump()
        {
            LoggerCustom.DEBUG("Begin jump");
            _currentJumpThrust = 0;
            ApplyJumpForce(initialJumpThrust);
            _stateMachine.SetState(CharacterState.EARLY_JUMP_CYCLE);
        }
        private void CheckUserInupt()
        {
            // every frame check whether user has changed input
            _userInput.moveHPressure = Input.GetAxisRaw("Horizontal");
            _userInput.moveVPressure = Input.GetAxisRaw("Vertical");
            _userInput.isJumpPressed = Input.GetButtonDown("Jump");
            _userInput.isJumpReleased = Input.GetButtonUp("Jump");
            _userInput.isJumpHeldDown = Input.GetButton("Jump");


            if (_userInput.isJumpPressed)
            {
                LoggerCustom.DEBUG("Pressed jump");
            }
            if (_userInput.isJumpReleased)
            {
                LoggerCustom.DEBUG("Released jump");
            }
        }
        private void Land()
        {
            LoggerCustom.DEBUG("LandH");
            FacingDirection priorHeading = _characterOrienter.headingDirection;

            StopH();
            StopV();
            SetFacingDirections(_characterOrienter.headingDirection, FacingDirection.UP);
            _currentJumpThrust = 0f;

            if (priorHeading == FacingDirection.LEFT || priorHeading == FacingDirection.RIGHT)
            {
                _stateMachine.SetState(CharacterState.IDLE);
            }
            else
            {
                _stateMachine.SetState(CharacterState.EARLY_LANDING_CYCLE_H);
            }
        }
        private void LandV()
        {
            LoggerCustom.DEBUG("LandV");
            StopH();
            StopV();
            
            if (_characterOrienter.headingDirection == FacingDirection.RIGHT)
            {
                SetFacingDirections(FacingDirection.UP, FacingDirection.LEFT);
            }
            else if (_characterOrienter.headingDirection == FacingDirection.LEFT)
            {
                SetFacingDirections(FacingDirection.UP, FacingDirection.RIGHT);
            }

            _currentJumpThrust = 0f;
            _stateMachine.SetState(CharacterState.EARLY_LANDING_CYCLE_V);
        }
        private void LandCeiling()
        {
            LoggerCustom.DEBUG("LandCeiling");
            StopH();
            StopV();

            SetFacingDirections(_characterOrienter.headingDirection, FacingDirection.DOWN);

            _currentJumpThrust = 0f;
            _stateMachine.SetState(CharacterState.EARLY_LANDING_CYCLE_V);
        }
        private void MoveH(float movementValue)
        {
            //LogDebugMessage("MoveH: " + movementValue);

            // Move the character by finding the target velocity
            Vector3 targetVelocity = new Vector2(movementValue * 10f, rigidBody2D.velocity.y);
            // And then smoothing it out and applying it to the character
            rigidBody2D.velocity = Vector3.SmoothDamp(rigidBody2D.velocity, targetVelocity, ref _velocity, movementSmoothing);

            // If the input is moving the player right and the player is facing left...
            if (movementValue > 0 && _characterOrienter.headingDirection == FacingDirection.LEFT)
            {
                // ... flip the player.
                SetFacingDirections(FacingDirection.RIGHT, _characterOrienter.thrustingDirection);
            }
            // Otherwise if the input is moving the player left and the player is facing right...
            else if (movementValue < 0 && _characterOrienter.headingDirection == FacingDirection.RIGHT)
            {
                // ... flip the player.
                SetFacingDirections(FacingDirection.LEFT, _characterOrienter.thrustingDirection);
            }
        }
        private void MoveV(float movementValue)
        {
            // Move the character by finding the target velocity
            Vector3 targetVelocity = new Vector2(rigidBody2D.velocity.x, movementValue * 10f);
            // And then smoothing it out and applying it to the character
            rigidBody2D.velocity = Vector3.SmoothDamp(rigidBody2D.velocity, targetVelocity, ref _velocity, movementSmoothing);

            // If the input is moving the player right and the player is facing left...
            if (movementValue < 0 && _characterOrienter.headingDirection == FacingDirection.UP)
            {
                // ... flip the player.
                SetFacingDirections(FacingDirection.DOWN, _characterOrienter.thrustingDirection);
            }
            // Otherwise if the input is moving the player left and the player is facing right...
            else if (movementValue > 0 && _characterOrienter.headingDirection == FacingDirection.DOWN)
            {
                // ... flip the player.
                SetFacingDirections(FacingDirection.UP, _characterOrienter.thrustingDirection);
            }
        }
        private void RotateCharacterByStep()
        {
            float degreesNow = rotationDegreesPerSecond * Time.deltaTime;
            Quaternion targetQuaternion = Quaternion.Euler(_targetRotation);
            transform.localRotation = Quaternion.RotateTowards(transform.localRotation, targetQuaternion, degreesNow);
        }
        private void SetFacingDirections(FacingDirection heading, FacingDirection thrusting)
        {
            if (_characterOrienter.headingDirection == heading && _characterOrienter.thrustingDirection == thrusting)
            {
                return;
            }

            // we've got a change in orientation
            RotationTarget rotator = new RotationTarget() {
                currentHeadingDirection = _characterOrienter.headingDirection,
                currentThrustingDirection = _characterOrienter.thrustingDirection,
                targetHeadingDirection = heading,
                targetThrustingDirection = thrusting,
            };
            rotator = RotationHelper.getRotationVectors(rotator);
            // update our starting rotation so we
            // have a baseline for timing rotations
            _startRotation = rotator.startingRotation;
            _targetRotation = rotator.targetRotation;

            _characterOrienter.SetHeadingDirection(heading);
            _characterOrienter.SetThrustingDirection(thrusting);

            // set the clutch to ensure we don't have any state
            // transitions while rotating
            _clutchCountDown = _numSecondsToEngageClutch;
        }
        private void StopH()
        {
            rigidBody2D.velocity = new Vector2(0, rigidBody2D.velocity.y);
        }
        private void StopV()
        {
            rigidBody2D.velocity = new Vector2(rigidBody2D.velocity.x, 0);
        }
        #endregion

        #region utilities
        private void LogAllVarsState()
        {
            LoggerCustom.DEBUG("Logging state of all controller variables");
            LoggerCustom.INFO("**********************************************************************************");
            LoggerCustom.DEBUG(string.Format("{0}|{1}", "isDebugModeOn", isDebugModeOn));
            LoggerCustom.DEBUG(string.Format("{0}|{1}", "debugLogFileDirectory", debugLogFileDirectory));
            LoggerCustom.DEBUG(string.Format("{0}|{1}", "runSpeed", runSpeed));
            LoggerCustom.DEBUG(string.Format("{0}|{1}", "jumpThrustPerSecond", jumpThrustPerSecond));
            LoggerCustom.DEBUG(string.Format("{0}|{1}", "maxJumpThrust", maxJumpThrust));
            LoggerCustom.DEBUG(string.Format("{0}|{1}", "initialJumpThrust", initialJumpThrust));
            LoggerCustom.DEBUG(string.Format("{0}|{1}", "rotationDegreesPerSecond", rotationDegreesPerSecond));
            LoggerCustom.DEBUG(string.Format("{0}|{1}", "movementSmoothing", movementSmoothing));
            LoggerCustom.DEBUG(string.Format("{0}|{1}", "gravityOnPlayer", gravityOnPlayer));
            LoggerCustom.DEBUG(string.Format("{0}|{1}", "isHorizontalMovementInAirAllowed", _movementCapabilities.isHorizontalMovementInAirAllowed));
            LoggerCustom.DEBUG(string.Format("{0}|{1}", "canWallCrall", _movementCapabilities.canWallCrawl));
            LoggerCustom.DEBUG(string.Format("{0}|{1}", "canCeilingCrawl", _movementCapabilities.canCeilingCrawl));
            LoggerCustom.DEBUG(string.Format("{0}|{1}", "isHorizontalMovementInAirAllowed", _movementCapabilities.isHorizontalMovementInAirAllowed));
            LoggerCustom.INFO("**********************************************************************************");
        }
        private void WriteDebugInfoToUi()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(string.Format("Heading direction: {0}", _characterOrienter.headingDirection));
            sb.AppendLine(string.Format("Thrusting direction: {0}", _characterOrienter.thrustingDirection));
            sb.AppendLine(string.Format("Gravity direction: {0}", _characterOrienter.gravityDirection));
            sb.AppendLine(string.Format("Current jump thrust: {0}", _currentJumpThrust));
            sb.AppendLine(string.Format("Animation: {0}", _characterAnimationController.currentState));

            debugTextBox.text = sb.ToString();
        }
        #endregion
    }
}

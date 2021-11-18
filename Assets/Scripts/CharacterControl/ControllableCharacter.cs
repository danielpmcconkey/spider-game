using Assets.Scripts.Animation;
using Assets.Scripts.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.CharacterControl
{
    public class ControllableCharacter : MonoBehaviour
    {
        #region unity properties
        [Header("Debug")]
        [Space(10)]
        [SerializeField] public bool isDebugModeOn = false;
        [SerializeField] public string debugLogFileDirectory = string.Empty; // C:\Users\Dan\AppData\LocalLow\SpiderController\
        [Header("Movement parameters")]
        [Space(10)]
        [SerializeField] public float horizontalAcceleration = 5.5f;
        [SerializeField] public float initialJumpThrust = 10f;
        [SerializeField] public float jumpThrustPerSecond = 80f;
        [SerializeField] public float maxJumpThrust = 30f;
        [Range(0, .3f)] [SerializeField] public float movementSmoothing = .05f;  // How much to smooth out the movement
        [SerializeField] public float gravityOnCharacter = 40f;
        [SerializeField] public float rotationDegreesPerSecond = 720;
        [SerializeField] public float corneringTimeRequired = 0.5f; // time it takes in seconds to begin to crawl up a wall
        [SerializeField] public float grappleBeamForce = 35f;
        [SerializeField] public float grappleBeamMaxDistance = 8f;
        [SerializeField] public float horizontalVelocityLimit = 6f;

        [Header("Movement abilities")]
        [Space(10)]
        [SerializeField] public bool isHorizontalMovementInAirAllowed = true;
        [SerializeField] public bool canWallCrawl = true;
        [SerializeField] public bool canCeilingCrawl = true;
        [SerializeField] public bool canGrapple = true;

        [Header("Movement references")]
        [Space(10)]
        [SerializeField] public Animator animator;
        [SerializeField] public Rigidbody2D rigidBody2D;
        [SerializeField] public Transform bellyCheckTransform;
        [SerializeField] public Transform forwardCheckTransform;
        [SerializeField] public Transform ceilingCheckTransform;
        [SerializeField] public LayerMask whatIsPlatform;  // A mask determining what is ground to the character 
        //todo: consider moving grapple beam stuff into the spider controller class
        [SerializeField] public Transform targetingReticuleTransform;
        [SerializeField] public LineRenderer grappleBeam;
        [SerializeField] public DistanceJoint2D grappleBeamJoint;
        #endregion unity properties

        #region fields not set in unity
        // publically exposed read-only values
        public CharacterContacts characterContactsCurrentFrame { get; private set; }
        public UserInputCollection userInput { get {return _userInput; } }
        public CharacterOrienter characterOrienter { get; private set; }
        public float currentJumpThrust { get; private set; }
        public CharacterAnimationController characterAnimationController { get; private set; }

        // private and protected members
        protected CharacterMovementStateController _stateController;
        private const float _platformContactCheckRadius = .2f; // Radius of the overlap circle to determine if touching a platform
        protected UserInputCollection _userInput;
        protected Vector2 _forcesAccumulated;
        private Vector3 _targetRotation;
        const float moveHValueNormalizer = 1000f; // multiply this value by the Horizontal acceleration parameter to make the force and velocity values seem on the same scale

        #endregion



        #region Unity implementation methods
        protected virtual void Awake()
        {
            _userInput = new UserInputCollection()
            {
                moveHPressure = 0f,
                moveVPressure = 0f,
                isJumpPressed = false,
                isJumpReleased = false,
                isJumpHeldDown = false,
            };

            characterOrienter = new CharacterOrienter(isDebugModeOn);

            //PlatformCollisionTransforms collisionTransforms = new PlatformCollisionTransforms()
            //{
            //    bellyCheckTransform = bellyCheckTransform,
            //    forwardCheckTransform = forwardCheckTransform,
            //    ceilingCheckTransform = ceilingCheckTransform,
            //    whatIsPlatform = whatIsPlatform
            //};
            _stateController = new CharacterMovementStateController(this, MovementState.FLOATING);

            characterAnimationController = new CharacterAnimationController(animator, isDebugModeOn);

            _forcesAccumulated = Vector2.zero;


        }
        protected virtual void FixedUpdate()
        {
            // apply the accumulation of physical forces
            rigidBody2D.velocity += _forcesAccumulated;
            if(rigidBody2D.velocity.x > horizontalVelocityLimit)
            {
                rigidBody2D.velocity = new Vector2(horizontalVelocityLimit, rigidBody2D.velocity.y);
            }
            if (rigidBody2D.velocity.x < (horizontalVelocityLimit * - 1))
            {
                rigidBody2D.velocity = new Vector2((horizontalVelocityLimit * -1), rigidBody2D.velocity.y);
            }
            // reset the accumulation
            _forcesAccumulated = Vector2.zero;


        }
        private void Update()
        {
            if (isDebugModeOn) LoggerCustom.SetFrameCount(Time.frameCount);
            CheckUserInput();
            PopulatePlatformCollisionVars();
            _stateController.UpdateCurrentState();
            characterAnimationController.SetState();

            // respond to H and V movement inputs
            if (
                _stateController.currentMovementState == MovementState.GROUNDED
                || _stateController.currentMovementState == MovementState.TETHERED
                || (_stateController.currentMovementState == MovementState.FLOATING && isHorizontalMovementInAirAllowed)
                || (_stateController.currentMovementState == MovementState.JUMP_ACCELERATING && isHorizontalMovementInAirAllowed)
                )
            {
                AddDirectionalMovementForceH();
            }
            if (_stateController.currentMovementState == MovementState.GROUNDED
                || _stateController.currentMovementState == MovementState.TETHERED)
            {
                AddDirectionalMovementForceV();
            }

            // respond to jump button held down
            if(_stateController.currentMovementState == MovementState.JUMP_ACCELERATING)
            {
                if (_userInput.isJumpHeldDown)
                {
                    if (currentJumpThrust < maxJumpThrust)
                    {
                        AddJumpForce((jumpThrustPerSecond * Time.deltaTime));
                    }
                }
            }

            // make sure the grapple beam's position zero stays with the character
            if (_stateController.currentMovementState == MovementState.TETHERED)
                UpdateGrappleBeamLine();

            // apply artifical gravity based on which way we're facing
            AddArtificalGravity();
        }
        #endregion



        protected virtual void CheckUserInput()
        {

        }
        private void AddArtificalGravity()
        {
            FacingDirection formerGravityDirection = characterOrienter.gravityDirection;
            float thrustMultiplier = gravityOnCharacter * Time.deltaTime;

            if (characterOrienter.headingDirection == FacingDirection.RIGHT
                || characterOrienter.headingDirection == FacingDirection.LEFT)
            {
                if (characterOrienter.thrustingDirection == FacingDirection.UP)
                {
                    characterOrienter.SetGravityDirection(FacingDirection.DOWN);
                    thrustMultiplier *= -1f;
                }
                else if (characterOrienter.thrustingDirection == FacingDirection.DOWN)
                {
                    characterOrienter.SetGravityDirection(FacingDirection.UP);
                    thrustMultiplier *= 1f;
                }
                _forcesAccumulated += new Vector2(0, thrustMultiplier);
            }
            if (characterOrienter.headingDirection == FacingDirection.UP
                || characterOrienter.headingDirection == FacingDirection.DOWN)
            {
                if (characterOrienter.thrustingDirection == FacingDirection.LEFT)
                {
                    characterOrienter.SetGravityDirection(FacingDirection.RIGHT);
                    thrustMultiplier *= 1f;
                }
                else if (characterOrienter.thrustingDirection == FacingDirection.RIGHT)
                {
                    characterOrienter.SetGravityDirection(FacingDirection.LEFT);
                    thrustMultiplier *= -1f;
                }
                _forcesAccumulated += new Vector2(thrustMultiplier, 0);
            }
        }
        private void AddDirectionalMovementForceH()
        {
            if (_userInput.moveHPressure == 0) return;
            if (characterOrienter.headingDirection == FacingDirection.RIGHT
                || characterOrienter.headingDirection == FacingDirection.LEFT)
            {
                MoveH(_userInput.moveHPressure * horizontalAcceleration * moveHValueNormalizer * Time.deltaTime);
            }
        }
        private void AddDirectionalMovementForceV()
        {
            if (characterOrienter.headingDirection == FacingDirection.UP
                || characterOrienter.headingDirection == FacingDirection.DOWN)
            {
                MoveV(_userInput.moveVPressure * horizontalAcceleration * moveHValueNormalizer * Time.deltaTime);
            }
        }
        private void AddJumpForce(float force)
        {
            currentJumpThrust += force;

            LoggerCustom.DEBUG(string.Format("Adding jump force of {0}. Current total thrust of {1}.", force, currentJumpThrust));

            Vector2 thrust = new Vector2(0f, force);
            switch (characterOrienter.thrustingDirection)
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
            _forcesAccumulated += thrust;
        }
        internal bool HandleTrigger(MovementTrigger t)
        {
            bool success = false;
            switch(t)
            {
                case MovementTrigger.TRIGGER_JUMP:
                    success = TriggerJump();
                    break;
                case MovementTrigger.TRIGGER_LANDING:
                    success = TriggerLanding();
                    break;
                case MovementTrigger.TRIGGER_FALL:
                    success = TriggerFall();
                    break;
                case MovementTrigger.TRIGGER_CORNER:
                    success = TriggerCorner();
                    break;
                case MovementTrigger.TRIGGER_GRAPPLE_ATTEMPT:
                    success = TriggerGrappleAttempt();
                    break;
                case MovementTrigger.TRIGGER_GRAPPLE_SUCCESS:
                    success = true; // nothing to do here as action was already done in the attempt
                    break;
                case MovementTrigger.TRIGGER_GRAPPLE_RELEASE:
                    success = TriggerGrappleRelease();
                    break;
                case MovementTrigger.TRIGGER_GRAPPLE_REACHED_ANCHOR:
                    success = TriggerGrappleReachedAnchor();
                    break;
            }
            return success;
        }
        private bool IsCollidingWithPlatform(Transform checkingTransform)
        {
            Collider2D[] collisions = Physics2D.OverlapCircleAll(
                checkingTransform.position, _platformContactCheckRadius, whatIsPlatform);

            for (int i = 0; i < collisions.Length; i++)
            {
                if (collisions[i].gameObject != gameObject)
                {
                    return true;
                }
            }

            return false;
        }
        private void LandFloor()
        {
            LoggerCustom.DEBUG("LandH");
            
            //StopH();
            //StopV();
            SetFacingDirections(characterOrienter.headingDirection, FacingDirection.UP);
            currentJumpThrust = 0f;
        }
        private void LandWall()
        {
            LoggerCustom.DEBUG("LandV");
            //StopH();
            //StopV();

            if (characterOrienter.headingDirection == FacingDirection.RIGHT)
            {
                SetFacingDirections(FacingDirection.UP, FacingDirection.LEFT);
            }
            else if (characterOrienter.headingDirection == FacingDirection.LEFT)
            {
                SetFacingDirections(FacingDirection.UP, FacingDirection.RIGHT);
            }

            currentJumpThrust = 0f;
            //_stateMachine.SetState(DEPRECATED_CharacterState.EARLY_LANDING_CYCLE_V);
        }
        private void LandCeiling()
        {
            LoggerCustom.DEBUG("LandCeiling");
            //StopH();
            //StopV();

            SetFacingDirections(characterOrienter.headingDirection, FacingDirection.DOWN);

            currentJumpThrust = 0f;
            //_stateMachine.SetState(DEPRECATED_CharacterState.EARLY_LANDING_CYCLE_V);
        }
        private void MoveH(float movementValue)
        {
            //LogDebugMessage("MoveH: " + movementValue);
            

            // Move the character by finding the target velocity
            Vector2 targetVelocity = new Vector2(movementValue, 0);
            // And then smoothing it out and applying it to the character
            _forcesAccumulated += targetVelocity * Time.deltaTime;

            // If the input is moving the player right and the player is facing left...
            if (movementValue > 0 && characterOrienter.headingDirection == FacingDirection.LEFT)
            {
                // ... flip the player.
                SetFacingDirections(FacingDirection.RIGHT, characterOrienter.thrustingDirection);
            }
            // Otherwise if the input is moving the player left and the player is facing right...
            else if (movementValue < 0 && characterOrienter.headingDirection == FacingDirection.RIGHT)
            {
                // ... flip the player.
                SetFacingDirections(FacingDirection.LEFT, characterOrienter.thrustingDirection);
            }
        }
        private void MoveV(float movementValue)
        {
            // Move the character by finding the target velocity
            Vector2 targetVelocity = new Vector2(0, movementValue);
            _forcesAccumulated += targetVelocity * Time.deltaTime;

            // If the input is moving the player right and the player is facing left...
            if (movementValue < 0 && characterOrienter.headingDirection == FacingDirection.UP)
            {
                // ... flip the player.
                SetFacingDirections(FacingDirection.DOWN, characterOrienter.thrustingDirection);
            }
            // Otherwise if the input is moving the player left and the player is facing right...
            else if (movementValue > 0 && characterOrienter.headingDirection == FacingDirection.DOWN)
            {
                // ... flip the player.
                SetFacingDirections(FacingDirection.UP, characterOrienter.thrustingDirection);
            }
        }
        private void PopulatePlatformCollisionVars()
        {
            //characterContactsPriorFrame = characterContactsCurrentFrame;
            characterContactsCurrentFrame = new CharacterContacts();
            if (IsCollidingWithPlatform(bellyCheckTransform))
            {
                characterContactsCurrentFrame.isTouchingPlatformWithBase = true;
            }
            else
            {
                characterContactsCurrentFrame.isTouchingPlatformWithBase = false;
            }
            if (IsCollidingWithPlatform(forwardCheckTransform))
            {
                characterContactsCurrentFrame.isTouchingPlatformForward = true;
            }
            else
            {
                characterContactsCurrentFrame.isTouchingPlatformForward = false;
            }
            if (IsCollidingWithPlatform(ceilingCheckTransform))
            {
                characterContactsCurrentFrame.isTouchingPlatformWithTop = true;
            }
            else
            {
                characterContactsCurrentFrame.isTouchingPlatformWithTop = false;
            }
            if (!characterContactsCurrentFrame.isTouchingPlatformWithBase
                && !characterContactsCurrentFrame.isTouchingPlatformForward
                && !characterContactsCurrentFrame.isTouchingPlatformWithTop)
            {
                characterContactsCurrentFrame.isTouchingNothing = true;
            }
            else
            {
                characterContactsCurrentFrame.isTouchingNothing = false;
            }
        }
        private void SetFacingDirections(FacingDirection heading, FacingDirection thrusting)
        {
            if (characterOrienter.headingDirection == heading && characterOrienter.thrustingDirection == thrusting)
            {
                return;
            }

            // we've got a change in orientation
            RotationTarget rotator = new RotationTarget()
            {
                currentHeadingDirection = characterOrienter.headingDirection,
                currentThrustingDirection = characterOrienter.thrustingDirection,
                targetHeadingDirection = heading,
                targetThrustingDirection = thrusting,
            };
            rotator = RotationHelper.getRotationVectors(rotator);
            // update our starting rotation so we
            // have a baseline for timing rotations
            //_startRotation = rotator.startingRotation;
            _targetRotation = rotator.targetRotation;

            transform.localRotation = Quaternion.Euler(_targetRotation);

            characterOrienter.SetHeadingDirection(heading);
            characterOrienter.SetThrustingDirection(thrusting);

            // set the clutch to ensure we don't have any state
            // transitions while rotating
            //_clutchCountDown = _numSecondsToEngageClutch;
        }
        //private void StopH()
        //{
        //    LoggerCustom.DEBUG("StopH");
        //    rigidBody2D.velocity = new Vector2(0, rigidBody2D.velocity.y);
        //}
        //private void StopV()
        //{
        //    LoggerCustom.DEBUG("StopV");
        //    rigidBody2D.velocity = new Vector2(rigidBody2D.velocity.x, 0);
        //}
        private void RotateCharacterByStep()
        {
            // keep this code as a reference. We're going to rotate 
            // the character immediately so that we don't have to fuss
            // with the clutch any more. But keep this here as a reference
            float degreesNow = rotationDegreesPerSecond * Time.deltaTime;
            Quaternion targetQuaternion = Quaternion.Euler(_targetRotation);
            transform.localRotation = Quaternion.RotateTowards(transform.localRotation, targetQuaternion, degreesNow);
        }
        
        private bool TriggerCorner()
        {
            LoggerCustom.DEBUG("Begin cornering");

            // calculate target orientation and gravity
            // new heading should be old thrusting direction
            // new thrusting should be the opposite of old
            // heading direction. gravity should be old 
            // heading direction
            FacingDirection oldHeading = characterOrienter.headingDirection;
            FacingDirection oldThrusting = characterOrienter.thrustingDirection;
            FacingDirection newHeading = oldThrusting;
            FacingDirection newThrusting = characterOrienter.GetOppositeDirection(oldHeading);
            FacingDirection newGravity = oldHeading;

            // update orientation
            SetFacingDirections(newHeading, newThrusting);

            return true;
        }
        private bool TriggerFall()
        {
            LoggerCustom.DEBUG("Begin falling");

            FacingDirection targetHeading = characterOrienter.headingDirection;
            if (characterOrienter.headingDirection == FacingDirection.UP && characterOrienter.thrustingDirection == FacingDirection.LEFT)
            {
                targetHeading = FacingDirection.RIGHT;
            }
            if (characterOrienter.headingDirection == FacingDirection.UP && characterOrienter.thrustingDirection == FacingDirection.RIGHT)
            {
                targetHeading = FacingDirection.LEFT;
            }
            if (characterOrienter.headingDirection == FacingDirection.DOWN && characterOrienter.thrustingDirection == FacingDirection.LEFT)
            {
                targetHeading = FacingDirection.LEFT;
            }
            if (characterOrienter.headingDirection == FacingDirection.DOWN && characterOrienter.thrustingDirection == FacingDirection.RIGHT)
            {
                targetHeading = FacingDirection.RIGHT;
            }
            SetFacingDirections(targetHeading, FacingDirection.UP);


            currentJumpThrust = 0f;
            //_stateMachine.SetState(DEPRECATED_CharacterState.DEPRECATED_FALLING);
            return true;
        }
        private bool TriggerJump()
        {
            LoggerCustom.DEBUG("Begin jump");
            currentJumpThrust = 0;
            AddJumpForce(initialJumpThrust);
            return true;
        }
        private bool TriggerGrappleAttempt()
        {
            LoggerCustom.DEBUG("Begin grapple beam attempt");
            currentJumpThrust = 0;

            // how to raycast https://www.youtube.com/watch?v=wkKsl1Mfp5M
            // raycasting starts at 13:38 

            // grapple tutorial:
            // part 1: https://www.youtube.com/watch?v=sHhzWlrTgJo&t=791s
            // part 2: https://www.youtube.com/watch?v=DTFgQIs5iMY&t=183s

            // set up the ray cast
            Vector2 firePoint = ceilingCheckTransform.position;
            Vector2 targetDirection = new Vector2(targetingReticuleTransform.position.x, targetingReticuleTransform.position.y)
                - firePoint;
            float distanceBetween = targetDirection.magnitude;
            Vector2 normalizedDirection = targetDirection / distanceBetween;

            // fire it and see what it hit
            RaycastHit2D hitInfo = Physics2D.Raycast(firePoint, normalizedDirection,
                grappleBeamMaxDistance, whatIsPlatform.value);

            bool didHit = false;
            if (hitInfo)
            {
                Rigidbody2D anchor = hitInfo.collider.gameObject.GetComponent<Rigidbody2D>();
                if (anchor != null)
                {
                    LoggerCustom.DEBUG(string.Format("Grapple hit {0} at {1}, {2}", hitInfo.transform.name,
                        hitInfo.transform.position.x, hitInfo.transform.position.y));

                    grappleBeamJoint.enabled = true;
                    grappleBeamJoint.connectedBody = anchor;
                    grappleBeamJoint.connectedAnchor = hitInfo.point - new Vector2(
                        hitInfo.collider.transform.position.x, hitInfo.collider.transform.position.y);
                    grappleBeamJoint.distance = Vector2.Distance(transform.position, hitInfo.point);

                    didHit = true;

                    // draw our line
                    grappleBeam.enabled = true;
                    grappleBeam.SetPosition(0, firePoint);
                    grappleBeam.SetPosition(1, hitInfo.point);

                    // now give a push in that direction
                    Vector2 thrust = normalizedDirection * grappleBeamForce * 10;
                    _forcesAccumulated += thrust;
                }


            }
            if (!didHit)
            {
                // draw the "miss" line
            }

            return didHit;
        }
        private bool TriggerGrappleReachedAnchor()
        {
            grappleBeam.enabled = false;
            FacingDirection newHeading = characterOrienter.thrustingDirection;
            SetFacingDirections(newHeading, FacingDirection.DOWN);
            return true;
        }
        private bool TriggerGrappleRelease()
        {
            grappleBeam.enabled = false;
            return true;
        }
        private bool TriggerLanding()
        {
            if (characterContactsCurrentFrame.isTouchingPlatformWithBase)
            {
                LandFloor();
                return true;
            }
            if (characterContactsCurrentFrame.isTouchingPlatformForward)
            {
                LandWall();
                return true;
            }
            if (characterContactsCurrentFrame.isTouchingPlatformWithTop)
            {
                LandCeiling();
                return true;
            }
            return false;
        }
        private void UpdateGrappleBeamLine()
        {
            grappleBeam.SetPosition(0, ceilingCheckTransform.position);

        }
    }
}

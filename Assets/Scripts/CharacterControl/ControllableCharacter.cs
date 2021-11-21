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
    internal class ControllableCharacter : MonoBehaviour
    {
        #region unity properties
        [Header("Debug")]
        [Space(10)]
        [SerializeField] public bool isDebugModeOn = false;
        [SerializeField] public string debugLogFileDirectory = string.Empty; // C:\Users\Dan\AppData\LocalLow\SpiderController\
        [Header("Movement parameters")]
        [Space(10)]
        [Range(0, 1f)] [SerializeField] public float horizontalAccelerationPercent = 0.5f;
        [Range(0, 1f)] [SerializeField] public float horizontalVelocityLimitPercent = 0.5f;
        [Range(0, 1f)] [SerializeField] public float initialJumpThrustPercent = 0.65f;
        [Range(0, 1f)] [SerializeField] public float jumpThrustOverTimePercent = 0.5f;
        [Range(0, 1f)] [SerializeField] public float jumpThrustLimitPercent = 0.238f;
        [Range(0, 1f)] [SerializeField] public float gravityOnCharacterPercent = 0.793f;
        [Range(0, 1f)] [SerializeField] public float corneringTimeRequiredPercent = 0.263f; // time it takes in seconds to begin to crawl up a wall

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
        #endregion unity properties

        #region fields not set in unity
        // publically exposed read-only values
        public CharacterContacts characterContactsCurrentFrame { get; private set; }
        public UserInputCollection userInput { get {return _userInput; } }
        public CharacterOrienter characterOrienter { get; private set; }
        public float currentJumpThrust { get; protected set; }
        public CharacterAnimationController characterAnimationController { get; private set; }
        public float jumpThrustLimit { get; private set; }  // this is calculated every frame. we make it a member var because the state controller needs access
        public float corneringTimeRequired { get; private set; }  // this is calculated every frame. we make it a member var because the state controller needs access

        // private and protected members
        protected CharacterMovementStateController _stateController;
        private const float _platformContactCheckRadius = .2f; // Radius of the overlap circle to determine if touching a platform
        protected UserInputCollection _userInput;
        protected Vector2 _forcesAccumulated;
        protected const float pushOffAmount = 0.5f;

        // min and max limiters on movement parameters
        const float maxHorizontalAcceleration = 50000f;
        const float minHorizontalAcceleration = 5000f;
        const float maxHorizontalVelocityLimit = 25f;
        const float minHorizontalVelocityLimit = 3f;
        const float maxInitialJumpThrust = 30f;
        const float minInitialJumpThrust = 5f;
        const float maxJumpThrustOverTime = 160f;
        const float minJumpThrustOverTime = 10f;
        const float maxJumpThrustLimit = 160f;
        const float minJumpThrustLimit = 10f;
        const float maxGravityOnCharacter = 100f;
        const float minGravityOnCharacter = 10f;        
        const float maxCorneringTimeRequired = 5f;
        const float minCorneringTimeRequired = 0.5f;

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

            _stateController = new CharacterMovementStateController(this, MovementState.FLOATING);

            characterAnimationController = new CharacterAnimationController(animator, isDebugModeOn);

            _forcesAccumulated = Vector2.zero;
        }
        protected virtual void FixedUpdate()
        {
            // apply the accumulation of physical forces
            rigidBody2D.velocity += _forcesAccumulated;
            
            // constrain velocities to speed limit
            float horizontalVelocityLimit = minHorizontalVelocityLimit +
                (horizontalVelocityLimitPercent * (maxHorizontalVelocityLimit - minHorizontalVelocityLimit));

            if (rigidBody2D.velocity.x > horizontalVelocityLimit)
            {
                rigidBody2D.velocity = new Vector2(horizontalVelocityLimit, rigidBody2D.velocity.y);
            }
            if (rigidBody2D.velocity.x < (horizontalVelocityLimit * - 1))
            {
                rigidBody2D.velocity = new Vector2((horizontalVelocityLimit * -1), rigidBody2D.velocity.y);
            }
            if (_stateController.currentMovementState == MovementState.GROUNDED)
            {
                // also constrain the vertical
                if (rigidBody2D.velocity.y > horizontalVelocityLimit)
                {
                    rigidBody2D.velocity = new Vector2(rigidBody2D.velocity.x, horizontalVelocityLimit);
                }
                if (rigidBody2D.velocity.y < (horizontalVelocityLimit * -1))
                {
                    rigidBody2D.velocity = new Vector2(rigidBody2D.velocity.x, (horizontalVelocityLimit * -1));
                }
            }
            // reset the accumulation
            _forcesAccumulated = Vector2.zero;
        }
        protected virtual void Update()
        {
            if (isDebugModeOn) LoggerCustom.SetFrameCount(Time.frameCount);
            CheckUserInput();
            PopulatePlatformCollisionVars();
            
            // calculate jump thrust limit every frame in case it updated outside of the script
            jumpThrustLimit = minJumpThrustLimit +
                (jumpThrustLimitPercent * (maxJumpThrustLimit - minJumpThrustLimit));
            
            // calculate cornering time required every frame in case it updated outside of the script
            corneringTimeRequired = minCorneringTimeRequired +
                (corneringTimeRequiredPercent * (maxCorneringTimeRequired - minCorneringTimeRequired));

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
            if (_stateController.currentMovementState == MovementState.GROUNDED)
            {
                AddDirectionalMovementForceV();
            }
            

            // respond to jump button held down
            if(_stateController.currentMovementState == MovementState.JUMP_ACCELERATING)
            {
                if (_userInput.isJumpHeldDown)
                {
                    float jumpThrustLimit = minJumpThrustLimit +
                        (jumpThrustLimitPercent * (maxJumpThrustLimit - minJumpThrustLimit));
                    if (currentJumpThrust < jumpThrustLimit)
                    {
                        float jumpThrustOverTime = minJumpThrustOverTime +
                            (jumpThrustOverTimePercent * (maxJumpThrustOverTime - minJumpThrustOverTime));
                        AddJumpForce((jumpThrustOverTime * Time.deltaTime));
                    }
                }
            }

            

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

            float gravityOnCharacter = minGravityOnCharacter +
                (gravityOnCharacterPercent * (maxGravityOnCharacter - minGravityOnCharacter));

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
                float acceleration = minHorizontalAcceleration +
                    (horizontalAccelerationPercent * (maxHorizontalAcceleration - minHorizontalAcceleration));
                MoveH(_userInput.moveHPressure * acceleration * Time.deltaTime);
            }
        }
        private void AddDirectionalMovementForceV()
        {
            if (characterOrienter.headingDirection == FacingDirection.UP
                || characterOrienter.headingDirection == FacingDirection.DOWN)
            {
                float acceleration = minHorizontalAcceleration +
                    (horizontalAccelerationPercent * (maxHorizontalAcceleration - minHorizontalAcceleration));
                MoveV(_userInput.moveVPressure * acceleration * Time.deltaTime);
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
        internal virtual bool HandleTrigger(MovementTrigger t)
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
        private bool IsGroundedRayCast()
        {
            // the physics engine isn't always reliable. sometimes
            // using platform collision makes it seem like we're
            // not actually thouching and forces a transition
            // to a floating state. Instead, we'll use raycasts 
            // to see if we're close enough to the ground to be
            // considered grounded

            characterOrienter.GetOppositeDirection(characterOrienter.thrustingDirection);

            float distanceToCheck = _platformContactCheckRadius * 2f;

            float distanceWidth = Vector2.Distance(transform.position, forwardCheckTransform.position);

            Vector2 firePoint1 = Vector2.zero;
            Vector2 targetPoint1 = Vector2.zero;
            Vector2 firePoint2 = Vector2.zero;
            Vector2 targetPoint2 = Vector2.zero;
            

            if (characterOrienter.thrustingDirection == FacingDirection.UP)
            {
                firePoint1 = bellyCheckTransform.position;
                targetPoint1 = firePoint1 + new Vector2(0, -distanceToCheck);

                if(characterOrienter.headingDirection == FacingDirection.RIGHT)
                {
                    // aft is left
                    firePoint2 = firePoint1 + new Vector2(-distanceWidth, 0);
                    targetPoint2 = firePoint2 + new Vector2(-distanceToCheck, -distanceToCheck);

                }
                if (characterOrienter.headingDirection == FacingDirection.LEFT)
                {
                    // aft is right
                    firePoint2 = firePoint1 + new Vector2(distanceWidth, 0);
                    targetPoint2 = firePoint2 + new Vector2(distanceToCheck, -distanceToCheck);

                }
            }
            if (characterOrienter.thrustingDirection == FacingDirection.DOWN)
            {
                firePoint1 = bellyCheckTransform.position;
                targetPoint1 = firePoint1 + new Vector2(0, distanceToCheck);

                if (characterOrienter.headingDirection == FacingDirection.RIGHT)
                {
                    // aft is left
                    firePoint2 = firePoint1 + new Vector2(-distanceWidth, 0);
                    targetPoint2 = firePoint2 + new Vector2(-distanceToCheck, distanceToCheck);

                }
                if (characterOrienter.headingDirection == FacingDirection.LEFT)
                {
                    // aft is right
                    firePoint2 = firePoint1 + new Vector2(distanceWidth, 0);
                    targetPoint2 = firePoint2 + new Vector2(distanceToCheck, distanceToCheck);

                }
            }
            if (characterOrienter.thrustingDirection == FacingDirection.RIGHT)
            {
                firePoint1 = bellyCheckTransform.position;
                targetPoint1 = firePoint1 + new Vector2(-distanceToCheck,0);

                if (characterOrienter.headingDirection == FacingDirection.UP)
                {
                    // aft is down
                    firePoint2 = firePoint1 + new Vector2(0, -distanceWidth);
                    targetPoint2 = firePoint2 + new Vector2(-distanceToCheck, -distanceToCheck);

                }
                if (characterOrienter.headingDirection == FacingDirection.DOWN)
                {
                    // aft is up
                    firePoint2 = firePoint1 + new Vector2(0, distanceWidth);
                    targetPoint2 = firePoint2 + new Vector2(-distanceToCheck, distanceToCheck);

                }
            }
            if (characterOrienter.thrustingDirection == FacingDirection.LEFT)
            {
                firePoint1 = bellyCheckTransform.position;
                targetPoint1 = firePoint1 + new Vector2(distanceToCheck, 0);

                if (characterOrienter.headingDirection == FacingDirection.UP)
                {
                    // aft is down
                    firePoint2 = firePoint1 + new Vector2(0, -distanceWidth);
                    targetPoint2 = firePoint2 + new Vector2(distanceToCheck, -distanceToCheck);

                }
                if (characterOrienter.headingDirection == FacingDirection.DOWN)
                {
                    // aft is up
                    firePoint2 = firePoint1 + new Vector2(0, distanceWidth);
                    targetPoint2 = firePoint2 + new Vector2(distanceToCheck, distanceToCheck);

                }
            }

            RaycastHit2D hitInfo1 = Raycaster.FireAtTargetPoint(firePoint1, targetPoint1, distanceToCheck, whatIsPlatform);
            RaycastHit2D hitInfo2 = Raycaster.FireAtTargetPoint(firePoint2, targetPoint2, distanceToCheck, whatIsPlatform);
            
            int hitCount = 0;
            if (hitInfo1) hitCount++;
            if (hitInfo2) hitCount++;

            if (hitCount >= 1) return true;
            return false;
        }
        private void LandFloor()
        {
            LoggerCustom.DEBUG("LandH");
            
            SetFacingDirections(characterOrienter.headingDirection, FacingDirection.UP);
            currentJumpThrust = 0f;
        }
        private void LandWall()
        {
            LoggerCustom.DEBUG("LandV");

            PushTowardRotation();


            if (characterOrienter.headingDirection == FacingDirection.RIGHT)
            {
                SetFacingDirections(FacingDirection.UP, FacingDirection.LEFT);
            }
            else if (characterOrienter.headingDirection == FacingDirection.LEFT)
            {
                SetFacingDirections(FacingDirection.UP, FacingDirection.RIGHT);
            }

            currentJumpThrust = 0f;
        }
        private void LandCeiling()
        {
            LoggerCustom.DEBUG("LandCeiling");

            SetFacingDirections(characterOrienter.headingDirection, FacingDirection.DOWN);
            currentJumpThrust = 0f;
        }
        private void MoveH(float movementValue)
        {
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
            characterContactsCurrentFrame.isTouchingPlatformWithBase = IsGroundedRayCast();
            //if (IsCollidingWithPlatform(bellyCheckTransform))
            //{
            //    characterContactsCurrentFrame.isTouchingPlatformWithBase = true;
            //}
            //else
            //{
            //    characterContactsCurrentFrame.isTouchingPlatformWithBase = false;
            //}
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
        protected virtual void PushOff()
        {
            // move the character away from the ground after
            // the jump a little to keep collision detection
            // from thinking that we struck ground. don't
            // do this with physics as sometimes the next
            // check happens while we're still too close to the
            // ground
            
            Vector3 positionAddition = Vector3.zero;
            if (characterContactsCurrentFrame.isTouchingPlatformWithBase)
            {
                // move in the direction of thrust
                if (characterOrienter.thrustingDirection == FacingDirection.UP)
                    positionAddition.y = pushOffAmount;
                if (characterOrienter.thrustingDirection == FacingDirection.DOWN)
                    positionAddition.y = -pushOffAmount;
                if (characterOrienter.thrustingDirection == FacingDirection.RIGHT)
                    positionAddition.x = pushOffAmount;
                if (characterOrienter.thrustingDirection == FacingDirection.LEFT)
                    positionAddition.x = -pushOffAmount;
            }
            if (characterContactsCurrentFrame.isTouchingPlatformForward)
            {
                // move away from the heading direction
                if (characterOrienter.headingDirection == FacingDirection.UP)
                    positionAddition.y = -pushOffAmount;
                if (characterOrienter.headingDirection == FacingDirection.DOWN)
                    positionAddition.y = pushOffAmount;
                if (characterOrienter.headingDirection == FacingDirection.RIGHT)
                    positionAddition.x = -pushOffAmount;
                if (characterOrienter.headingDirection == FacingDirection.LEFT)
                    positionAddition.x = pushOffAmount;
            }
            transform.position += positionAddition;
        }
        private void PushTowardRotation()
        {
            float distance = Vector2.Distance(ceilingCheckTransform.position, transform.position);
            Vector3 movement = Vector3.zero;
            if (characterOrienter.headingDirection == FacingDirection.RIGHT)
                movement.x = distance;
            else if (characterOrienter.headingDirection == FacingDirection.LEFT)
                movement.x = -distance;
            if (characterOrienter.headingDirection == FacingDirection.UP)
                movement.y = distance;
            else if (characterOrienter.headingDirection == FacingDirection.DOWN)
                movement.y = -distance;
            transform.position += movement;
        }
        protected void SetFacingDirections(FacingDirection heading, FacingDirection thrusting)
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
            transform.localRotation = Quaternion.Euler(rotator.targetRotation);

            characterOrienter.SetHeadingDirection(heading);
            characterOrienter.SetThrustingDirection(thrusting);
        }
        private bool TriggerCorner()
        {
            LoggerCustom.DEBUG("Begin cornering");

            // move the game object toward the wall so that
            // rotation doesn't cause all the collision check
            // objects to be touching nothing
            PushTowardRotation();

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
            return true;
        }
        private bool TriggerJump()
        {
            LoggerCustom.DEBUG("Begin jump");
            currentJumpThrust = 0;
            float initialJumpThrust = minInitialJumpThrust +
                (initialJumpThrustPercent * (maxInitialJumpThrust - minInitialJumpThrust));
            AddJumpForce(initialJumpThrust);
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
        
        
    }
}

using Assets.Scripts.Animation;
using Assets.Scripts.Utility;
using System;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.CharacterControl
{
    public class ControllableCharacter : MonoBehaviour
    {
        #region unity properties
        [Header("Debug")]
        [Space(10)]
        
        [SerializeField] public string debugLogFileDirectory = string.Empty; // C:\Users\Dan\AppData\LocalLow\SpiderController\
        [Header("Movement parameters")]
        [Space(10)]
        [Range(0, 1f)] [SerializeField] public float horizontalAccelerationPercent = 0.5f;
        [Range(0, 1f)] [SerializeField] public float horizontalVelocityLimitPercent = 0.5f;
        [Range(0, 1f)] [SerializeField] public float breaksPressurePercent = 0.5f;
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
        [SerializeField] public bool canFly = false;
        [SerializeField] public bool canHighJump = false;

        [Header("Movement references")]
        [Space(10)]
        
        [SerializeField] public Rigidbody2D rigidBody2D;
        [SerializeField] public Transform bellyCheckTransform;
        [SerializeField] public Transform forwardCheckTransform;
        [SerializeField] public Transform ceilingCheckTransform;
        [SerializeField] public LayerMask whatIsPlatform;  // A mask determining what is ground to the character 
        [SerializeField] public Transform targetingReticuleTransform;

        [Header("Animation")]
        [Space(10)]
        [SerializeField] public Animator animator;
        [SerializeField] public string idleAnimationName;
        [SerializeField] public string movingAnimationName;
        [SerializeField] public string jumpingAnimationName;
        [SerializeField] public string fallingAnimationName;
        [SerializeField] public string damageAnimationName;
        [SerializeField] public string deathAnimationName;

        [Header("Combat Properties")]
        [Space(10)]
        [SerializeField] public float maxHP = 100;
        [SerializeField] public float currentHP = 100;
        [SerializeField] public float armorClass = 10;  // the higher, the more protection
        [SerializeField] public float contactDamageDealt = 20;
        [SerializeField] public float invincibilityDurationInSeconds = 1.5f;
        [Range(0, 1f)] [SerializeField] public float knockBackForcePercent = 0.5f;
        #endregion unity properties

        #region fields not set in unity
        // publically exposed read-only values
        public CharacterContacts characterContactsCurrentFrame { get; private set; }
        public UserInputCollection userInput { get { return _userInput; } }
        public CharacterOrienter characterOrienter { get; private set; }
        public float currentJumpThrust { get; protected set; }
        public CharacterAnimationController characterAnimationController { get; private set; }
        public float jumpThrustLimit { get; private set; }  // this is calculated every frame. we make it a member var because the state controller needs access
        public float corneringTimeRequired { get; private set; }  // this is calculated every frame. we make it a member var because the state controller needs access
        public bool isTakingDamage { get; private set; }    // used to control the animator
        public bool isAlive { get; protected set; } = true;

        // private and protected members
        protected CharacterMovementStateController _stateController;
        private const float _platformContactCheckRadius = .2f; // Radius of the overlap circle to determine if touching a platform
        protected UserInputCollection _userInput;
        protected Vector2 _floatingForcesAccumulated;
        protected Vector2 _groundedForcesAccumulated;
        protected Vector2 _knockBackForcesAccumulated;
        protected const float _pushOffAmount = 0.5f;
        private const float _breaksThreshold = 0.1f; // kick in the breaks when velocity is greater than this and no move pressure
        protected string _gameVersion;
        protected Vector2 _groundCheckRay1FirePoint = Vector2.zero;
        protected Vector2 _groundCheckRay1TargetPoint = Vector2.zero;
        protected Vector2 _groundCheckRay2FirePoint = Vector2.zero;
        protected Vector2 _groundCheckRay2TargetPoint = Vector2.zero;
        private Transform _groundedTransform;    // the game object we're grounded to if grounded
        private Vector2 _groundedStrikePoint;    // the vector 2 where we struck the grounded transform
        protected float _highJumpMultiplier = 1.4f;



        protected bool _isInvincible;
        
        // min and max limiters on movement parameters
        protected const float _maxHorizontalAcceleration = 12f;
        protected const float _minHorizontalAcceleration = 0.5f;
        protected const float _maxHorizontalVelocityLimit = 25f;
        protected const float _minHorizontalVelocityLimit = 3f;
        protected const float _maxInitialJumpThrust = 60f; //3000f;
        protected const float _minInitialJumpThrust = 0f; //500f;
        protected const float _maxJumpThrustOverTime = 32f; //1600f;
        protected const float _minJumpThrustOverTime = 0f; //100f;
        protected const float _maxJumpThrustLimit = 120f; //3200f;
        protected const float _minJumpThrustLimit = 0f; //200f;
        protected const float _maxGravityOnCharacter = 100f;
        protected const float _minGravityOnCharacter = 0f;
        protected const float _maxCorneringTimeRequired = 5f;
        protected const float _minCorneringTimeRequired = 0.5f;
        protected const float _maxBreaksPressure = 60.0f;
        protected const float _minBreaksPressure = 0.0f;
        protected const float _minKnockBackForce = 0.0f;
        protected const float _maxKnockBackForce = 30.0f;


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

            characterOrienter = new CharacterOrienter();

            _stateController = new CharacterMovementStateController(this, MovementState.FLOATING);

            characterAnimationController = new CharacterAnimationController(this, animator);

            _floatingForcesAccumulated = Vector2.zero;
            isTakingDamage = false;
            _isInvincible = false;
    }
        
        protected virtual void FixedUpdate()
        {
            //Update(); // todo: determine if Update() call here is necessary

            if (_stateController.currentMovementState == MovementState.GROUNDED)
            {
                ApplyForcesGrounded();
            }
            else ApplyForcesFloating();
            _knockBackForcesAccumulated = Vector2.zero;

            _stateController.ResetClutch();
            
        }
        protected virtual void Update()
        {
            LoggerCustom.SetFrameCount(Time.frameCount);

            CheckUserInput();
            PopulatePlatformCollisionVars();

            // calculate jump thrust limit every frame in case it updated outside of the script
            jumpThrustLimit = _minJumpThrustLimit +
                (jumpThrustLimitPercent * (_maxJumpThrustLimit - _minJumpThrustLimit));

            if (canHighJump)
            {
                jumpThrustLimit = jumpThrustLimit * _highJumpMultiplier;
            }

            // calculate cornering time required every frame in case it updated outside of the script
            corneringTimeRequired = _minCorneringTimeRequired +
                (corneringTimeRequiredPercent * (_maxCorneringTimeRequired - _minCorneringTimeRequired));

            _stateController.UpdateCurrentState();
            characterAnimationController.UpdateCurrentState();

            // respond to H and V movement inputs
            if (_stateController.currentMovementState == MovementState.GROUNDED)
            {
                AddDirectionalMovementForceHGrounded();
                AddDirectionalMovementForceVGrounded();
            }
            if(_stateController.currentMovementState == MovementState.TETHERED
                || (_stateController.currentMovementState == MovementState.FLOATING && isHorizontalMovementInAirAllowed)
                || (_stateController.currentMovementState == MovementState.JUMP_ACCELERATING && isHorizontalMovementInAirAllowed)
                )
            {
                AddDirectionalMovementForceHFloating();
                if (canFly) AddDirectionalMovementForceVFloating();
            }
            


            // respond to jump button held down
            if (_stateController.currentMovementState == MovementState.JUMP_ACCELERATING)
            {
                if (_userInput.isJumpHeldDown)
                {
                    float jumpThrustLimit = _minJumpThrustLimit +
                        (jumpThrustLimitPercent * (_maxJumpThrustLimit - _minJumpThrustLimit));

                    if (canHighJump)
                    {
                        jumpThrustLimit = jumpThrustLimit * _highJumpMultiplier;
                    }

                    if (currentJumpThrust < jumpThrustLimit)
                    {
                        float jumpThrustOverTime = _minJumpThrustOverTime +
                            (jumpThrustOverTimePercent * (_maxJumpThrustOverTime - _minJumpThrustOverTime));
                        if (canHighJump)
                        {
                            jumpThrustOverTime = jumpThrustOverTime * _highJumpMultiplier;
                        }
                        AddJumpForce(jumpThrustOverTime * Time.deltaTime);
                    }
                }
            }



            // apply artifical gravity based on which way we're facing
            AddArtificalGravity();
        }
        #endregion

        #region public methods
        public MovementState GetCurrentState()
        {
            return _stateController.currentMovementState;
        }
        internal virtual bool HandleTrigger(MovementTrigger t)
        {
            bool success = false;
            switch (t)
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
        #endregion

        #region private / protected methods
        protected virtual void ApplyForcesFloating()
        {
            // apply the accumulation of physical forces
            rigidBody2D.velocity += _floatingForcesAccumulated;
            // add knockback to forces. we add it here instead of 
            // just adding to the _floatingForces in the update
            // because input of zero zeroes out the _groundedForces.
            // it's just easier to do the same for floating than
            // to inject the knockback at different points
            rigidBody2D.velocity += _knockBackForcesAccumulated;

            // constrain velocities to speed limit
            float horizontalVelocityLimit = _minHorizontalVelocityLimit +
                (horizontalVelocityLimitPercent * (_maxHorizontalVelocityLimit - _minHorizontalVelocityLimit));

            if (rigidBody2D.velocity.x > horizontalVelocityLimit)
            {
                rigidBody2D.velocity = new Vector2(horizontalVelocityLimit, rigidBody2D.velocity.y);
            }
            if (rigidBody2D.velocity.x < (horizontalVelocityLimit * -1))
            {
                rigidBody2D.velocity = new Vector2((horizontalVelocityLimit * -1), rigidBody2D.velocity.y);
            }

            // also constrain the vertical, unless jump accelerating
            // this means we constrain fall velocity but not jump
            if (_stateController.currentMovementState != MovementState.JUMP_ACCELERATING)
            {
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
            _floatingForcesAccumulated = Vector2.zero;

        }
        private void ApplyForcesGrounded()
        {
            // zero out velocity as we want to turn off
            // the physics engine while grounded
            // unless the character is a flying character. 
            // then they'll need to float off the ground.
            if(!canFly) rigidBody2D.velocity = Vector2.zero;

            // add knockback to forces. we add it here instead of 
            // just adding to the _groundedForces in the update
            // because input of zero zeroes out the _groundedForces
            _groundedForcesAccumulated += _knockBackForcesAccumulated;

            // add accumulated forces as movement
            // but first check that this doesn't move you 
            // into a platform
            Vector2 targetMove = new Vector2(transform.position.x + _groundedForcesAccumulated.x,
                transform.position.y + _groundedForcesAccumulated.y);
            RaycastHit2D hitInfo = Raycaster.FireAtTargetPoint(transform.position, targetMove, 1, whatIsPlatform);
            if(!hitInfo)
            {
                // all clear
                transform.position = targetMove;
            }
            else
            {
                float distanceToTargetMove = Vector2.Distance(transform.position, targetMove);
                float distanceToStrikePoint = Vector2.Distance(transform.position, hitInfo.point);
                float distanceToForwardCollider = Vector2.Distance(transform.position, forwardCheckTransform.position);
                
                if (distanceToStrikePoint > distanceToTargetMove)
                {
                    // still good, strike point was further out
                    transform.position = targetMove;
                }
                else //if (distanceToStrikePoint < distanceToForwardCollider)
                {
                    // we're inside of a wall already
                    string burp = "true";
                    float distanceBetweenMiddleAndForwardCollider = Vector2.Distance(transform.position, hitInfo.point);

                    if (characterOrienter.headingDirection == FacingDirection.LEFT)
                    {
                        transform.position = new Vector2(
                            hitInfo.point.x + distanceBetweenMiddleAndForwardCollider,
                            transform.position.y
                            );
                    }
                    if (characterOrienter.headingDirection == FacingDirection.RIGHT)
                    {
                        transform.position = new Vector2(
                            hitInfo.point.x - distanceBetweenMiddleAndForwardCollider,
                            transform.position.y
                            );
                    }
                    if (characterOrienter.headingDirection == FacingDirection.DOWN)
                    {
                        transform.position = new Vector2(
                            transform.position.x,
                            hitInfo.point.y + distanceBetweenMiddleAndForwardCollider
                            );
                    }
                    if (characterOrienter.headingDirection == FacingDirection.UP)
                    {
                        transform.position = new Vector2(
                            transform.position.x,
                            hitInfo.point.y - distanceBetweenMiddleAndForwardCollider
                            );
                    }
                }
                //else
                //{
                //    // moving here would put us inside a wall
                //    // don't do it
                //    string burp = "true";
                //}
            }



            //Vector3.SmoothDamp(rigidBody2D.velocity, targetVelocity, ref _velocity, movementSmoothing);

            // and move the player character to be at the strike point
            if (_groundedTransform != null)
            {
                if (characterOrienter.headingDirection == FacingDirection.LEFT ||
                characterOrienter.headingDirection == FacingDirection.RIGHT)
                {
                    if (characterOrienter.thrustingDirection == FacingDirection.UP)
                    {
                        // belly transform Y = 11 
                        // strike point = 9
                        // move y by -2
                        float distanceY = bellyCheckTransform.position.y - _groundedStrikePoint.y;
                        transform.position = new Vector2(transform.position.x, transform.position.y - distanceY);

                    }
                    else if (characterOrienter.thrustingDirection == FacingDirection.DOWN)
                    {
                        // belly transform Y = 9 
                        // strike point = 11
                        // move y by +2
                        float distanceY = bellyCheckTransform.position.y - _groundedStrikePoint.y;
                        transform.position = new Vector2(transform.position.x, transform.position.y - distanceY);

                    }
                }
            }

            // reset the accumulation
            _groundedForcesAccumulated = Vector2.zero;
            


            // stop velocity when it's very low to prevent jittering
            if ((rigidBody2D.velocity.x > 0 && rigidBody2D.velocity.x < _breaksThreshold)
                || (rigidBody2D.velocity.x < 0 && rigidBody2D.velocity.x > -_breaksThreshold))
            {
                rigidBody2D.velocity = new Vector2(0, rigidBody2D.velocity.y);
            }
            if ((rigidBody2D.velocity.y > 0 && rigidBody2D.velocity.y < _breaksThreshold)
                || (rigidBody2D.velocity.y < 0 && rigidBody2D.velocity.y > -_breaksThreshold))
            {
                rigidBody2D.velocity = new Vector2(rigidBody2D.velocity.x, 0);
            }


        }
        protected virtual void CheckUserInput()
        {

        }
        protected virtual void AddArtificalGravity()
        {
            // turn off gravity when grounded to avoid jittery behavior in the y axis
            //if (_stateController.currentMovementState == MovementState.GROUNDED) return;

            FacingDirection formerGravityDirection = characterOrienter.gravityDirection;

            float gravityOnCharacter = _minGravityOnCharacter +
                (gravityOnCharacterPercent * (_maxGravityOnCharacter - _minGravityOnCharacter));

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
                _floatingForcesAccumulated += new Vector2(0, thrustMultiplier);
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
                _floatingForcesAccumulated += new Vector2(thrustMultiplier, 0);
            }
        }
        private void AddDirectionalMovementForceHFloating()
        {
            _floatingForcesAccumulated = MoveH(_floatingForcesAccumulated);
        }
        private void AddDirectionalMovementForceHGrounded()
        {
            if (_userInput.moveHPressure == 0) ApplyBreaks();
            else
            {
                _groundedForcesAccumulated = MoveH(_groundedForcesAccumulated);

            }

        }
        private void AddDirectionalMovementForceVFloating()
        {
            _floatingForcesAccumulated = MoveV(_floatingForcesAccumulated);
        }
        private void AddDirectionalMovementForceVGrounded()
        {
            if (_userInput.moveVPressure == 0) ApplyBreaks();
            else
            {
                _groundedForcesAccumulated = MoveV(_groundedForcesAccumulated);
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
            _floatingForcesAccumulated += thrust;
        }
        private void ApplyBreaks()
        {
            // if the character has velocity in the X
            // axis, add force in the opposite direction
            // to make stopping more forceful

            // todo: fix this velocity
            //float breaksPressure = minBreaksPressure +
            //        (breaksPressurePercent * (maxBreaksPressure - minBreaksPressure));

            //float pressureToApply = breaksPressure * Time.deltaTime;
            //if (rigidBody2D.velocity.x > breaksThreshold)
            //{
            //    if (pressureToApply < rigidBody2D.velocity.x)
            //        _floatingForcesAccumulated += new Vector2(-pressureToApply, 0);
            //}
            //else if (rigidBody2D.velocity.x < -breaksThreshold)
            //{
            //    if (pressureToApply < (rigidBody2D.velocity.x * -1))
            //        _floatingForcesAccumulated += new Vector2(pressureToApply, 0);
            //}
            //else return;
        }
        protected virtual void Die()
        {
            isAlive = false;
            // todo: trigger death timer before disappearing
            Destroy(gameObject);
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
        protected virtual bool IsGroundedRayCast()
        {
            // the physics engine isn't always reliable. sometimes
            // using platform collision makes it seem like we're
            // not actually thouching and forces a transition
            // to a floating state. Instead, we'll use raycasts 
            // to see if we're close enough to the ground to be
            // considered grounded

            
            float distanceToCheck = _platformContactCheckRadius * 2f;

            float distanceWidth = Vector2.Distance(transform.position, forwardCheckTransform.position);

            // reset the grounded transform every time so that it'll be null if 
            // we don't strike ground
            _groundedTransform = null;
            _groundedStrikePoint = Vector2.zero;




            if (characterOrienter.thrustingDirection == FacingDirection.UP)
            {
                _groundCheckRay1FirePoint = bellyCheckTransform.position;
                _groundCheckRay1TargetPoint = _groundCheckRay1FirePoint + new Vector2(0, -distanceToCheck);

                if (characterOrienter.headingDirection == FacingDirection.RIGHT)
                {
                    // aft is left
                    _groundCheckRay2FirePoint = _groundCheckRay1FirePoint + new Vector2(-distanceWidth, 0);
                    _groundCheckRay2TargetPoint = _groundCheckRay2FirePoint + new Vector2(-distanceToCheck, -distanceToCheck);

                }
                if (characterOrienter.headingDirection == FacingDirection.LEFT)
                {
                    // aft is right
                    _groundCheckRay2FirePoint = _groundCheckRay1FirePoint + new Vector2(distanceWidth, 0);
                    _groundCheckRay2TargetPoint = _groundCheckRay2FirePoint + new Vector2(distanceToCheck, -distanceToCheck);

                }
            }
            if (characterOrienter.thrustingDirection == FacingDirection.DOWN)
            {
                _groundCheckRay1FirePoint = bellyCheckTransform.position;
                _groundCheckRay1TargetPoint = _groundCheckRay1FirePoint + new Vector2(0, distanceToCheck);

                if (characterOrienter.headingDirection == FacingDirection.RIGHT)
                {
                    // aft is left
                    _groundCheckRay2FirePoint = _groundCheckRay1FirePoint + new Vector2(-distanceWidth, 0);
                    _groundCheckRay2TargetPoint = _groundCheckRay2FirePoint + new Vector2(-distanceToCheck, distanceToCheck);

                }
                if (characterOrienter.headingDirection == FacingDirection.LEFT)
                {
                    // aft is right
                    _groundCheckRay2FirePoint = _groundCheckRay1FirePoint + new Vector2(distanceWidth, 0);
                    _groundCheckRay2TargetPoint = _groundCheckRay2FirePoint + new Vector2(distanceToCheck, distanceToCheck);

                }
            }
            if (characterOrienter.thrustingDirection == FacingDirection.RIGHT)
            {
                _groundCheckRay1FirePoint = bellyCheckTransform.position;
                _groundCheckRay1TargetPoint = _groundCheckRay1FirePoint + new Vector2(-distanceToCheck, 0);

                if (characterOrienter.headingDirection == FacingDirection.UP)
                {
                    // aft is down
                    _groundCheckRay2FirePoint = _groundCheckRay1FirePoint + new Vector2(0, -distanceWidth);
                    _groundCheckRay2TargetPoint = _groundCheckRay2FirePoint + new Vector2(-distanceToCheck, -distanceToCheck);

                }
                if (characterOrienter.headingDirection == FacingDirection.DOWN)
                {
                    // aft is up
                    _groundCheckRay2FirePoint = _groundCheckRay1FirePoint + new Vector2(0, distanceWidth);
                    _groundCheckRay2TargetPoint = _groundCheckRay2FirePoint + new Vector2(-distanceToCheck, distanceToCheck);

                }
            }
            if (characterOrienter.thrustingDirection == FacingDirection.LEFT)
            {
                _groundCheckRay1FirePoint = bellyCheckTransform.position;
                _groundCheckRay1TargetPoint = _groundCheckRay1FirePoint + new Vector2(distanceToCheck, 0);

                if (characterOrienter.headingDirection == FacingDirection.UP)
                {
                    // aft is down
                    _groundCheckRay2FirePoint = _groundCheckRay1FirePoint + new Vector2(0, -distanceWidth);
                    _groundCheckRay2TargetPoint = _groundCheckRay2FirePoint + new Vector2(distanceToCheck, -distanceToCheck);

                }
                if (characterOrienter.headingDirection == FacingDirection.DOWN)
                {
                    // aft is up
                    _groundCheckRay2FirePoint = _groundCheckRay1FirePoint + new Vector2(0, distanceWidth);
                    _groundCheckRay2TargetPoint = _groundCheckRay2FirePoint + new Vector2(distanceToCheck, distanceToCheck);

                }
            }

            int hitCount = 0;
            RaycastHit2D hitInfo1 = Raycaster.FireAtTargetPoint(_groundCheckRay1FirePoint, _groundCheckRay1TargetPoint, distanceToCheck, whatIsPlatform);
            if (hitInfo1)
            {
                hitCount++;
                if (hitInfo1.collider.transform != null)
                {
                    _groundedTransform = hitInfo1.collider.transform;
                    _groundedStrikePoint = hitInfo1.point;
                }
            }

            
            
            if (_stateController.currentMovementState == MovementState.GROUNDED)
            {
                // only check the aft state if we're grounded already
                // use this for coyote time
                RaycastHit2D hitInfo2 = Raycaster.FireAtTargetPoint(_groundCheckRay2FirePoint, _groundCheckRay2TargetPoint, distanceToCheck, whatIsPlatform);
                if (hitInfo2)
                {
                    hitCount++;
                    if (_groundedTransform == null && hitInfo2.collider.transform != null)
                    {
                        // only update the grounded transform if we didn't do so with target 1
                        // if target 1 and 2 both hit, then taget 1's is the one we want to keep
                        _groundedTransform = hitInfo2.collider.transform;
                        _groundedStrikePoint = hitInfo2.point;
                    }
                }
            }

            if (hitCount >= 1) return true;
            return false;
        }
        private void KnockBack()
        {
            FacingDirection direction = characterOrienter.GetOppositeDirection(characterOrienter.headingDirection);
            StartCoroutine(KnockBackOverTime(direction));
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
        private Vector2 MoveH(Vector2 forceAccumulation)
        {
            if (characterOrienter.headingDirection == FacingDirection.RIGHT
                || characterOrienter.headingDirection == FacingDirection.LEFT)
            {
                float acceleration = _minHorizontalAcceleration +
                    (horizontalAccelerationPercent * (_maxHorizontalAcceleration - _minHorizontalAcceleration));

                float movementValue = _userInput.moveHPressure * acceleration;
                if(_stateController.currentMovementState != MovementState.GROUNDED)
                {
                    movementValue *= 10;
                }

                // Move the character by finding the target velocity
                Vector2 targetVelocity = new Vector2(movementValue, 0);
                // And then smoothing it out and applying it to the character
                forceAccumulation += targetVelocity * Time.deltaTime;

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
            return forceAccumulation;
        }
        private Vector2 MoveV(Vector2 forceAccumulation)
        {
            if (characterOrienter.headingDirection == FacingDirection.UP
                || characterOrienter.headingDirection == FacingDirection.DOWN
                || canFly)
            {
                float acceleration = _minHorizontalAcceleration +
                    (horizontalAccelerationPercent * (_maxHorizontalAcceleration - _minHorizontalAcceleration));
                float movementValue = _userInput.moveVPressure * acceleration;

                // Move the character by finding the target velocity
                Vector2 targetVelocity = new Vector2(0, movementValue);
                forceAccumulation += targetVelocity * Time.deltaTime;

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
            return forceAccumulation;
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
                    positionAddition.y = _pushOffAmount;
                if (characterOrienter.thrustingDirection == FacingDirection.DOWN)
                    positionAddition.y = -_pushOffAmount;
                if (characterOrienter.thrustingDirection == FacingDirection.RIGHT)
                    positionAddition.x = _pushOffAmount;
                if (characterOrienter.thrustingDirection == FacingDirection.LEFT)
                    positionAddition.x = -_pushOffAmount;
            }
            if (characterContactsCurrentFrame.isTouchingPlatformForward)
            {
                // move away from the heading direction
                if (characterOrienter.headingDirection == FacingDirection.UP)
                    positionAddition.y = -_pushOffAmount;
                if (characterOrienter.headingDirection == FacingDirection.DOWN)
                    positionAddition.y = _pushOffAmount;
                if (characterOrienter.headingDirection == FacingDirection.RIGHT)
                    positionAddition.x = -_pushOffAmount;
                if (characterOrienter.headingDirection == FacingDirection.LEFT)
                    positionAddition.x = _pushOffAmount;
            }
            transform.position += positionAddition;
        }
        private void PushTowardRotation()
        {
            float distance = 0.75f;// Vector2.Distance(ceilingCheckTransform.position, transform.position);
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
        protected virtual void ReactToDamageDealt(bool noIFrames = false)
        {
            // use this method for i-frames or triggering animations
            if(!noIFrames) StartCoroutine(InvokeInvincibility());
            KnockBack();
        }
        private void ReduceHealth(float amount)
        {
            currentHP -= amount;
            if(currentHP <= 0)
            {
                Die();
            }
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
        internal virtual void TakeDamage(float damageAmount)
        {
            TakeDamage(damageAmount, false);
        }
        internal virtual void TakeDamage(float damageAmount, bool noIFrames)
        {
            // using noIFrames passes instruction to the reaction
            // method not to invoke iframes. use for light, 
            // frequent attacks

            if (_isInvincible) return;

            // deduct armor class
            float damageDone = damageAmount - armorClass;
            if (damageDone > 0)
            {
                ReduceHealth(damageDone);
                ReactToDamageDealt(noIFrames);
            }
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
            float initialJumpThrust = _minInitialJumpThrust +
                (initialJumpThrustPercent * (_maxInitialJumpThrust - _minInitialJumpThrust));
            if (canHighJump)
            {
                initialJumpThrust = initialJumpThrust * _highJumpMultiplier;
            }
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
        #endregion

        #region coroutines
        IEnumerator InvokeInvincibility()
        {
            isTakingDamage = true;
            _isInvincible = true;
            float timeElapsed = 0;

            while (timeElapsed < invincibilityDurationInSeconds)
            {
                timeElapsed += Time.deltaTime;
                yield return null;
            }
            isTakingDamage = false;
            _isInvincible = false;

        }
        private IEnumerator KnockBackOverTime(FacingDirection direction)
        {
            float timer = 0f;
            const float duration = 0.25f;

            float knockBackForce = _minKnockBackForce +
                (knockBackForcePercent * (_maxKnockBackForce - _minKnockBackForce));

            while (timer < duration)
            {
                switch (direction)
                {
                    case FacingDirection.RIGHT:
                        _knockBackForcesAccumulated += new Vector2(knockBackForce * Time.deltaTime, 0);
                        break;
                    case FacingDirection.LEFT:
                        _knockBackForcesAccumulated += new Vector2(knockBackForce * Time.deltaTime * -1, 0);
                        break;
                    case FacingDirection.UP:
                        _knockBackForcesAccumulated += new Vector2(0, knockBackForce * Time.deltaTime);
                        break;
                    case FacingDirection.DOWN:
                        _knockBackForcesAccumulated += new Vector2(0, knockBackForce * Time.deltaTime * -1);
                        break;
                }

                timer += Time.deltaTime;
                yield return 0;
            }
        }
        #endregion
    }
}

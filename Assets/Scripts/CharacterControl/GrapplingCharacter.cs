using Assets.Scripts.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.CharacterControl
{
    internal class GrapplingCharacter  : ControllableCharacter
    {
        [Header("Grapple control")]
        [Space(10)]
        [SerializeField] public LineRenderer grappleBeamLineRenderer;
        [SerializeField] public DistanceJoint2D grappleBeamJoint;
        [Range(0, 1f)] [SerializeField] public float grappleBeamForcePercent = 0.5f;
        [Range(0, 1f)] [SerializeField] public float grappleBeamMaxDistancePercent = 0.5f;

        // min and max limiters on grapple parameters
        const float maxGrappleBeamForce = 60;
        const float minGrappleBeamForce = 5f;
        const float maxGrappleBeamMaxDistance = 12f;
        const float minGrappleBeamMaxDistance = 1f;

        // private members
        private bool _shouldStopReelingIn;
        private float _missLineCountdown;
        private const float _missLineDuration = 0.25f;  // measured in seconds

        #region Unity implementation methods
        protected override void Awake()
        {
            DisableGrapple();
            base.Awake();
        }
        protected override void Update()
        {
            base.Update();
            if (_stateController.currentMovementState == MovementState.TETHERED)
            {
                if (_userInput.moveVPressure > 0.1f || _userInput.moveVPressure < -0.1f)
                {
                    Climb(_userInput.moveVPressure);
                }
            }
            // make sure the grapple beam's position zero stays with the character
            if (_stateController.currentMovementState == MovementState.TETHERED)
                UpdateGrappleBeam();
            // fade out the grapple beam line, but keep it anchored, if we missed
            if (_stateController.currentMovementState != MovementState.TETHERED
                && grappleBeamLineRenderer.enabled)
            {
                UpdateGrappleBeam();
                _missLineCountdown -= Time.deltaTime;
                if(_missLineCountdown < 0.01f)
                {
                    DisableGrapple();
                }
            }
        }
        #endregion

        internal override bool HandleTrigger(MovementTrigger t)
        {
            bool success = false;
            switch (t)
            {
                case MovementTrigger.TRIGGER_GRAPPLE_ATTEMPT:
                    success = TriggerGrappleAttempt();
                    break;
                case MovementTrigger.TRIGGER_GRAPPLE_SUCCESS:
                    success = true; // nothing to do here as action was already done in the attempt
                    break;
                case MovementTrigger.TRIGGER_GRAPPLE_RELEASE:
                    success = TriggerGrappleRelease();
                    break;
                case MovementTrigger.TRIGGER_GRAPPLE_STRUCK_GROUND:
                    success = TriggerGrappleStruckGround();
                    break;
                default:
                    success = base.HandleTrigger(t);
                    break;
            }
            return success;
        }

        #region private methods
        private void Climb(float moveVPressure)
        {
            // stop the shortening of the distance because we're now manually doin' it
            _shouldStopReelingIn = true;

            float grappleBeamForce = minGrappleBeamForce +
                    (grappleBeamForcePercent * (maxGrappleBeamForce - minGrappleBeamForce));


            if (moveVPressure > 0)
            {
                // climbing up, shorten the distance
                grappleBeamJoint.distance -= (grappleBeamForce * Time.deltaTime);
            }
            else
            {
                // climbing down, lengthen the distance
                grappleBeamJoint.distance += (grappleBeamForce * Time.deltaTime);
            }
        }
        private void DisableGrapple()
        {
            grappleBeamLineRenderer.enabled = false;
            grappleBeamJoint.enabled = false;
            _shouldStopReelingIn = false;
        }
        private void EnableGrapple()
        {
            grappleBeamLineRenderer.enabled = true;
            grappleBeamJoint.enabled = true;
            _shouldStopReelingIn = false;
        }
        private void PushOff()
        {
            // move the character away from the ground and 
            // shorten the distance a little to keep collision 
            // detection to think that we struck ground. don't
            // do this with physics as sometimes the next
            // check happens while we're still too close to the
            // ground
            const float pushOffAmount = 0.5f;
            Vector3 positionAddition = Vector3.zero;
            if(characterContactsCurrentFrame.isTouchingPlatformWithBase)
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
            LoggerCustom.DEBUG(string.Format("Position before push-off: {0}, {1}.", transform.position.x, transform.position.y));
            transform.position += positionAddition;
            grappleBeamJoint.distance -= pushOffAmount;
            LoggerCustom.DEBUG(string.Format("Position after push-off: {0}, {1}.", transform.position.x, transform.position.y));

        }
        private bool TriggerGrappleAttempt()
        {
            LoggerCustom.DEBUG("Begin grapple beam attempt");
            currentJumpThrust = 0;


            // how to raycast https://www.youtube.com/watch?v=wkKsl1Mfp5M
            // raycasting starts at 13:38 

            // grapple tutorial:
            // part 1: https://www.youtube.com/watch?v=sHhzWlrTgJo
            // part 2: https://www.youtube.com/watch?v=DTFgQIs5iMY

            // set up the ray cast
            Vector2 firePoint = ceilingCheckTransform.position;
            Vector2 targetDirection = new Vector2(targetingReticuleTransform.position.x,
                targetingReticuleTransform.position.y)
                - firePoint;
            float distanceBetween = targetDirection.magnitude;
            Vector2 normalizedDirection = targetDirection / distanceBetween;

            // calculate our max distance
            float grappleBeamMaxDistance = minGrappleBeamMaxDistance +
                (grappleBeamMaxDistancePercent * (maxGrappleBeamMaxDistance - minGrappleBeamMaxDistance));

            // fire it and see what it hit
            RaycastHit2D hitInfo = Physics2D.Raycast(firePoint, normalizedDirection,
                grappleBeamMaxDistance, whatIsPlatform.value);

            bool didHit = false;
            if (hitInfo)
            {
                Rigidbody2D rigidBodyToAnchor = hitInfo.collider.gameObject.GetComponent<Rigidbody2D>();
                if (rigidBodyToAnchor != null)
                {
                    LoggerCustom.DEBUG(string.Format("Grapple hit {0} at {1}, {2}", hitInfo.transform.name,
                        hitInfo.transform.position.x, hitInfo.transform.position.y));

                    EnableGrapple();
                    grappleBeamJoint.connectedBody = rigidBodyToAnchor;
                    // don't anchor to the center of the object
                    // anchor to where we hit, which is the hitpoint 
                    // minus the center of the object
                    grappleBeamJoint.connectedAnchor = hitInfo.point - new Vector2(
                        hitInfo.collider.transform.position.x, hitInfo.collider.transform.position.y);
                    grappleBeamJoint.distance = Vector2.Distance(transform.position, hitInfo.point);

                    // push off the ground so we don't retrigger a landing
                    PushOff();

                    didHit = true;

                    // define our line
                    grappleBeamLineRenderer.SetPosition(0, transform.position);
                    grappleBeamLineRenderer.SetPosition(1, hitInfo.point);


                }


            }
            if (!didHit)
            {
                // draw the "miss" line
                grappleBeamLineRenderer.enabled = true;
                Vector3 finalDirection = normalizedDirection * grappleBeamMaxDistance;
                Vector3 targetPositionForPoint1 = transform.position + finalDirection;

                grappleBeamLineRenderer.SetPosition(0, transform.position);
                grappleBeamLineRenderer.SetPosition(1, targetPositionForPoint1);

                _missLineCountdown = _missLineDuration;
            }

            return didHit;
        }
        private bool TriggerGrappleReachedAnchor()
        {
            DisableGrapple();
            return true;
        }
        private bool TriggerGrappleRelease()
        {
            DisableGrapple();
            return true;
        }
        private bool TriggerGrappleStruckGround()
        {
            // you can strike ground in a few ways
            if (characterContactsCurrentFrame.isTouchingPlatformForward
                && characterOrienter.headingDirection == FacingDirection.RIGHT)
            {
                SetFacingDirections(FacingDirection.UP, FacingDirection.LEFT);
            }
            else if (characterContactsCurrentFrame.isTouchingPlatformForward
                && characterOrienter.headingDirection == FacingDirection.LEFT)
            {
                SetFacingDirections(FacingDirection.UP, FacingDirection.RIGHT);
            }
            else if (characterContactsCurrentFrame.isTouchingPlatformForward
                && characterOrienter.headingDirection == FacingDirection.UP)
            {
                SetFacingDirections(characterOrienter.thrustingDirection, FacingDirection.DOWN);
            }
            if (characterContactsCurrentFrame.isTouchingPlatformWithTop)
            {
                SetFacingDirections(characterOrienter.headingDirection, FacingDirection.DOWN);
            }
            if (characterContactsCurrentFrame.isTouchingPlatformWithBase)
            {
                SetFacingDirections(characterOrienter.headingDirection, FacingDirection.UP);
            }

            return TriggerGrappleReachedAnchor();
        }
        private void UpdateGrappleBeam()
        {
            // if the distance between character and anchor > 2.5f
            // then retract the beam a little
            // todo: create a trigger to change _shouldStopReelingIn to true and allow the character to move up and down the grapple
            const float maxScroll = 2.5f;
            if (!_shouldStopReelingIn && grappleBeamJoint.distance > maxScroll)
            {
                float grappleBeamForce = minGrappleBeamForce +
                    (grappleBeamForcePercent * (maxGrappleBeamForce - minGrappleBeamForce));
                grappleBeamJoint.distance -= (grappleBeamForce * Time.deltaTime);
            }
            if(!_shouldStopReelingIn && grappleBeamJoint.distance < maxScroll)
            {
                _shouldStopReelingIn = true;
            }
            // regardless of how far we are, update the line's 
            // position 0 to keep up with swinging and such
            grappleBeamLineRenderer.SetPosition(0, transform.position);
        } 
        #endregion


    }
}

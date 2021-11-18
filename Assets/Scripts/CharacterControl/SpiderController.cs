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
    public class SpiderController : ControllableCharacter
    {
        #region Vars set in Unity
        public UnityEngine.UI.Text debugTextBox;

        #endregion

        #region implementation of unity methods
        protected override void Awake()
        {
            base.Awake();
            // hide scene elements that shouldn't be there at the beginning
            grappleBeam.enabled = false;

            LoggerCustom.Init(isDebugModeOn, debugLogFileDirectory, Time.frameCount);


            LoggerCustom.INFO("**********************************************************************************");
            LoggerCustom.INFO("BEGINNING NEW GAME");
            LoggerCustom.INFO("**********************************************************************************");
            LogAllVarsState();
        }
        protected override void FixedUpdate()
        {
            base.FixedUpdate();
            TrackTargetingReticlueToMousePosition();
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
        
        protected override void CheckUserInput()
        {
            // every frame check whether user has changed input
            _userInput.moveHPressure = Input.GetAxisRaw("Horizontal");
            _userInput.moveVPressure = Input.GetAxisRaw("Vertical");
            _userInput.isJumpPressed = Input.GetButtonDown("Jump");
            _userInput.isJumpReleased = Input.GetButtonUp("Jump");
            _userInput.isJumpHeldDown = Input.GetButton("Jump");
            _userInput.isGrappleButtonPressed = Input.GetButtonDown("Fire3");
            _userInput.isGrappleButtonReleased = Input.GetButtonUp("Fire3");
            _userInput.isGrappleButtonHeldDown = Input.GetButton("Fire3");

            if (_userInput.isJumpPressed)
            {
                LoggerCustom.DEBUG("Pressed jump");
            }
            if (_userInput.isJumpReleased)
            {
                LoggerCustom.DEBUG("Released jump");
            }
            if (_userInput.isGrappleButtonPressed)
            {
                LoggerCustom.DEBUG(string.Format("Pressed hookshot button at {0}, {1}", 
                    targetingReticuleTransform.position.x, targetingReticuleTransform.position.y));
            }
            
        }
        private void TrackTargetingReticlueToMousePosition()
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = 0;
            targetingReticuleTransform.position = mousePosition;
        }
        #endregion

        #region utilities
        private void LogAllVarsState()
        {
            LoggerCustom.DEBUG("Logging state of all controller variables");
            LoggerCustom.INFO("**********************************************************************************");
            LoggerCustom.DEBUG(string.Format("{0}|{1}", "isDebugModeOn", isDebugModeOn));
            LoggerCustom.DEBUG(string.Format("{0}|{1}", "debugLogFileDirectory", debugLogFileDirectory));
            LoggerCustom.DEBUG(string.Format("{0}|{1}", "runSpeed", horizontalAcceleration));
            LoggerCustom.DEBUG(string.Format("{0}|{1}", "jumpThrustPerSecond", jumpThrustPerSecond));
            LoggerCustom.DEBUG(string.Format("{0}|{1}", "maxJumpThrust", maxJumpThrust));
            LoggerCustom.DEBUG(string.Format("{0}|{1}", "initialJumpThrust", initialJumpThrust));
            LoggerCustom.DEBUG(string.Format("{0}|{1}", "rotationDegreesPerSecond", rotationDegreesPerSecond));
            LoggerCustom.DEBUG(string.Format("{0}|{1}", "movementSmoothing", movementSmoothing));
            LoggerCustom.DEBUG(string.Format("{0}|{1}", "gravityOnPlayer", gravityOnCharacter));
            LoggerCustom.DEBUG(string.Format("{0}|{1}", "isHorizontalMovementInAirAllowed", isHorizontalMovementInAirAllowed));
            LoggerCustom.DEBUG(string.Format("{0}|{1}", "canWallCrall", canWallCrawl));
            LoggerCustom.DEBUG(string.Format("{0}|{1}", "canCeilingCrawl", canCeilingCrawl));
            LoggerCustom.DEBUG(string.Format("{0}|{1}", "canGrapple", canGrapple));
            LoggerCustom.DEBUG(string.Format("{0}|{1}", "isHorizontalMovementInAirAllowed", isHorizontalMovementInAirAllowed));
            LoggerCustom.INFO("**********************************************************************************");
        }
        private void WriteDebugInfoToUi()
        {
            StringBuilder sb = new StringBuilder();
            //sb.AppendLine(string.Format("Heading direction: {0}", characterOrienter.headingDirection));
            //sb.AppendLine(string.Format("Thrusting direction: {0}", characterOrienter.thrustingDirection));
            //sb.AppendLine(string.Format("Gravity direction: {0}", characterOrienter.gravityDirection));
            //sb.AppendLine(string.Format("Current jump thrust: {0}", currentJumpThrust));
            sb.AppendLine(string.Format("Current state: {0}", _stateController.currentMovementState));
            sb.AppendLine(string.Format("Animation: {0}", characterAnimationController.currentState));
            sb.AppendLine(string.Format("velocity: {0}", rigidBody2D.velocity));

            debugTextBox.text = sb.ToString();
        }
        #endregion
    }
}

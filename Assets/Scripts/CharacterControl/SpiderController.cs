using Assets.Scripts;
using Assets.Scripts.Animation;
using Assets.Scripts.Camera;
using Assets.Scripts.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using UnityEngine;
using Assets.Scripts.WorldBuilder;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.CharacterControl
{
    public class SpiderController : GrapplingCharacter
    {
        #region Vars set in Unity
        [SerializeField] public string replayFile = string.Empty; // E:\Unity Projects\SpiderPocGit\Logs\CustomLogger\spiderReplay-2021-11-21.14.42.14.490.json
        [SerializeField] public GameObject builder;
        public UnityEngine.UI.Text debugTextBox;

        public int currentRoom { get; private set; }

        private CameraControl cameraControl;
        private WorldBuilder.WorldBuilder worldBuilder;

        private bool hasFarted = false;


        #endregion

        #region implementation of unity methods
        protected override void Awake()
        {
            _gameVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            if (isDebugModeOn)
            {
                Replay.InitializeReplay(_gameVersion, debugLogFileDirectory);
            }

            base.Awake();
            if (replayFile != string.Empty)
            {
                Replay.DeSerializeFromReplayFile(replayFile);
            }



            LoggerCustom.Init(isDebugModeOn, debugLogFileDirectory, Time.frameCount);


            LoggerCustom.INFO("**********************************************************************************");
            LoggerCustom.INFO("BEGINNING NEW GAME");
            LoggerCustom.INFO("**********************************************************************************");
            LogAllVarsState();

            cameraControl = UnityEngine.Camera.main.GetComponent<CameraControl>();

            worldBuilder = builder.GetComponent<WorldBuilder.WorldBuilder>();
            currentRoom = 0;
        }
        
        protected override void FixedUpdate()
        {
            base.FixedUpdate();
            TrackTargetingReticlueToMousePosition();
            if (isDebugModeOn) WriteDebugInfoToUi();
        }
        protected override void Update()
        {
            base.Update();
            if (isDebugModeOn)
            {
                Replay.AddInputForFrame(Time.frameCount, _userInput);
            }

            if (!hasFarted)
            {
                // todo: limit camera constraint to room transitions
                const float camMargin = 3.0f;
                (Vector2 upperLeft, Vector2 lowerRight) roomDimensions = worldBuilder.GetRoomDimensions(currentRoom);
                cameraControl.ActivateLimits(
                    roomDimensions.upperLeft.x + camMargin, roomDimensions.lowerRight.x - camMargin,
                    roomDimensions.lowerRight.y + camMargin, roomDimensions.upperLeft.y - camMargin);

                hasFarted = true;
            }


        }
        void OnDestroy()
        {
            if (isDebugModeOn)
            {
                Replay.WriteReplayFile();
            }
            if (isDebugModeOn)
            {
                LoggerCustom.INFO("**********************************************************************************");
                LoggerCustom.INFO("GAME OVER");
                LoggerCustom.INFO("**********************************************************************************");
                LogAllVarsState();

                // write out the log to a log file
                LoggerCustom.WriteLogToFile();

            }
        }
        #endregion

        
        #region controller methods
        
        protected override void CheckUserInput()
        {
            if(replayFile != string.Empty)
            {
                _userInput = Replay.GetInputForFrame(Time.frameCount);
                return;
            }
            // every frame check whether user has changed input
            _userInput.moveHPressure = Input.GetAxisRaw("Horizontal");
            _userInput.moveVPressure = Input.GetAxisRaw("Vertical");
            _userInput.isJumpPressed = Input.GetButtonDown("Jump");
            _userInput.isJumpReleased = Input.GetButtonUp("Jump");
            _userInput.isJumpHeldDown = Input.GetButton("Jump");
            _userInput.isGrappleButtonPressed = Input.GetButtonDown("Fire3");
            _userInput.isGrappleButtonReleased = Input.GetButtonUp("Fire3");
            _userInput.isGrappleButtonHeldDown = Input.GetButton("Fire3");
            _userInput.mouseX = Input.mousePosition.x;
            _userInput.mouseY = Input.mousePosition.y;


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
            Vector3 mousePosition = UnityEngine.Camera.main.ScreenToWorldPoint(    // had to specify UnityEngine to deconflict w/ my Camera namespace
                new Vector3(_userInput.mouseX, _userInput.mouseY, 0));
            mousePosition.z = 0;
            targetingReticuleTransform.position = mousePosition;
        }
        #endregion

        #region utilities
        private void LogAllVarsState()
        {
            LoggerCustom.INFO("**********************************************************************************");
            LoggerCustom.INFO(string.Format("{0}|{1}", "_gameVersion", _gameVersion));
            LoggerCustom.INFO("**********************************************************************************");
            LoggerCustom.DEBUG("Movement parameters");
            LoggerCustom.INFO("**********************************************************************************");
            LoggerCustom.DEBUG(string.Format("{0}|{1}", "horizontalAccelerationPercent", horizontalAccelerationPercent));
            LoggerCustom.DEBUG(string.Format("{0}|{1}", "horizontalVelocityLimitPercent", horizontalVelocityLimitPercent));
            LoggerCustom.DEBUG(string.Format("{0}|{1}", "breaksPressurePercent", breaksPressurePercent));
            LoggerCustom.DEBUG(string.Format("{0}|{1}", "initialJumpThrustPercent", initialJumpThrustPercent));
            LoggerCustom.DEBUG(string.Format("{0}|{1}", "jumpThrustOverTimePercent", jumpThrustOverTimePercent));
            LoggerCustom.DEBUG(string.Format("{0}|{1}", "jumpThrustLimitPercent", jumpThrustLimitPercent));
            LoggerCustom.DEBUG(string.Format("{0}|{1}", "gravityOnCharacterPercent", gravityOnCharacterPercent));
            LoggerCustom.DEBUG(string.Format("{0}|{1}", "grappleBeamForcePercent", grappleBeamForcePercent));
            LoggerCustom.DEBUG(string.Format("{0}|{1}", "grappleBeamMaxDistancePercent", grappleBeamMaxDistancePercent));
            LoggerCustom.DEBUG(string.Format("{0}|{1}", "corneringTimeRequiredPercent", corneringTimeRequiredPercent));

            // log camera control
            if (UnityEngine.Camera.main != null)
            {
                 
                if (cameraControl != null)
                {
                    cameraControl.isDebugModeOn = isDebugModeOn;
                    cameraControl.LogAllVarsState();
                }
            }

            LoggerCustom.INFO("**********************************************************************************");
            LoggerCustom.DEBUG("Movement abilities");
            LoggerCustom.INFO("**********************************************************************************");
            LoggerCustom.DEBUG(string.Format("{0}|{1}", "isHorizontalMovementInAirAllowed", isHorizontalMovementInAirAllowed));
            LoggerCustom.DEBUG(string.Format("{0}|{1}", "canWallCrall", canWallCrawl));
            LoggerCustom.DEBUG(string.Format("{0}|{1}", "canCeilingCrawl", canCeilingCrawl));
            LoggerCustom.DEBUG(string.Format("{0}|{1}", "canGrapple", canGrapple));
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
            sb.AppendLine(string.Format("Move pressure H: {0}", userInput.moveHPressure));

            debugTextBox.text = sb.ToString();
        }
        #endregion
    }
}

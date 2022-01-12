using UnityEngine;
using System.Collections;
using Assets.Scripts.CharacterControl;
using System;
using Assets.Scripts.Utility;
using Assets.Scripts.WorldBuilder;

namespace Assets.Scripts.Camera
{
    class CameraControl : MonoBehaviour
    {
        
        [SerializeField] public float idealDistanceInFrontOfPlayer = 3f;
        [SerializeField] public float moveSpeed = 50f; // the full velocity, not when lazing back to the player while idle
        [SerializeField] public GameObject player;
        [SerializeField] public float roomPadBottom = 1.0f; // how much you want to see beyond the room border
        [SerializeField] public float roomPadTop = 1.0f;
        [SerializeField] public float roomPadLeft = 1.0f;
        [SerializeField] public float roomPadRight = 1.0f;
        private Vector2 _camPositionWithoutZ;
        private Vector2 _playerPositionWithoutZ;
        private Vector2 _playerPositionWithoutZPrior;
        private Vector2 _roomUpLeft;
        private Vector2 _roomDownRight;
        private float _camZ;
        private Vector2 _camTargetPosition;
        //private float _idleTimer;
        private float _moveSpeedCurrent;
        private UnityEngine.Camera _gameCamera;
        //private Vector2 _momentum;


        public void UpdateRoomDimensions(Vector2 upLeft, Vector2 downRight)
        {
            _roomUpLeft = upLeft;
            _roomDownRight = downRight;
        }
        private void FixedUpdate()
        {
            MoveTowardTarget();
        }
        private void Update()
        {
            // because FixedUpdate is not called while game is paused
            // if the game is paused, move the camera during Update()
            if(GamePauser.isPaused) MoveTowardTarget();
        }

        private void LateUpdate()
        {
            // why put cam control in LateUpdate? 
            // https://forum.unity.com/threads/fixedupdate-or-lateupdate-for-camera-movement.735764/

            _camPositionWithoutZ = new Vector2(transform.position.x, transform.position.y);
            _playerPositionWithoutZ = new Vector2(player.transform.position.x, player.transform.position.y);

            if (_playerPositionWithoutZ != _playerPositionWithoutZPrior
                && Vector2.Distance(_playerPositionWithoutZ, _playerPositionWithoutZPrior) > 0.01) 
                // the distance check keeps it from freaking out during cornering
            {
                // we want to stay a specified distance in front of the player
                // so first get the normalized direction and then multiply
                // that direction by the distance we want to stay out front
                // but only do this if the player moved. If the player didn't
                // move then the playerMovementDirection below becomes a
                // Vector2 with NAN as X and Y
                Vector2 playerMovementDirection = Raycaster.GetNormalizedDirectionBetweenPoints(
                    _playerPositionWithoutZPrior, _playerPositionWithoutZ);

                _camTargetPosition = _playerPositionWithoutZ + (playerMovementDirection * idealDistanceInFrontOfPlayer);

                //_idleTimer = 0;
                

            }
            //else
            //{
            //    const float idleThreashold = 0.25f;
            //    _idleTimer += Time.deltaTime;
            //    if (_idleTimer > idleThreashold)
            //    {
            //        // player hasn't moved. Drift the cam back
            //        // to the player
            //        _camTargetPosition = _playerPositionWithoutZ;
            //        _moveSpeedCurrent = moveSpeed * 0.05f;
            //    }
            //}
            
            // now constrain the dimensions
            ClampCamTargetPosition();

            // update prior position for movement determination
            _playerPositionWithoutZPrior = _playerPositionWithoutZ;
        }
        private void Start()
        {
            _camZ = transform.position.z;
            _playerPositionWithoutZPrior = new Vector2(player.transform.position.x, player.transform.position.y);
            _camTargetPosition = _playerPositionWithoutZ;
            _camPositionWithoutZ = _playerPositionWithoutZ;
            //_idleTimer = 0;
            _gameCamera = transform.gameObject.GetComponent<UnityEngine.Camera>();
            _moveSpeedCurrent = moveSpeed;
        }
        private void ClampCamTargetPosition()
        {
            float distanceFromCenterY = _gameCamera.orthographicSize;
            float distanceFromCenterX = _gameCamera.orthographicSize * _gameCamera.aspect;

            float minX = _roomUpLeft.x + distanceFromCenterX - (roomPadLeft +1);    // added an extra 1 to the side pads because it's not showing right on wider monitors
            float maxX = _roomDownRight.x - distanceFromCenterX + (roomPadRight + 1);
            float minY = _roomDownRight.y + distanceFromCenterY - roomPadBottom;
            float maxY = _roomUpLeft.y - distanceFromCenterY + roomPadTop;

            // clamp to room dimensions
            _camTargetPosition.x = Mathf.Clamp(
                _camTargetPosition.x,
                minX,
                maxX);
            _camTargetPosition.y = Mathf.Clamp(
                _camTargetPosition.y,
                minY,
                maxY);

        }
        private void MoveTowardTarget()
        {
            const float snapDistance = 0.1f;

            if (_camTargetPosition == null || Vector2.Distance(_camTargetPosition, _camPositionWithoutZ) < snapDistance)
                return;

            UpdateMovementSpeed();

            // use Time.unscaledDeltaTime so that the camera can still move 
            // when the engine is paused. (Doorway transitions)

            transform.position = Vector3.MoveTowards(
                new Vector3(_camPositionWithoutZ.x, _camPositionWithoutZ.y, _camZ),
                new Vector3(_camTargetPosition.x, _camTargetPosition.y, _camZ),
                _moveSpeedCurrent * Time.unscaledDeltaTime);
        }
        private void UpdateMovementSpeed()
        {
            float minSpeed = 0;
            float maxSpeed = moveSpeed * 10f;
            float distanceAtMaxSpeed = 10f;
            float distance = Vector2.Distance(_camTargetPosition, _camPositionWithoutZ);
            float percentOfDistanceAtMaxSpeed = distance / distanceAtMaxSpeed;
            float targetSpeed = minSpeed + ((maxSpeed - minSpeed) * percentOfDistanceAtMaxSpeed);
            if (_moveSpeedCurrent < targetSpeed)
            {
                float diff = targetSpeed - _moveSpeedCurrent;
                _moveSpeedCurrent += diff * 0.25f;
            }
            else if (_moveSpeedCurrent > targetSpeed + 1) _moveSpeedCurrent--;
            else _moveSpeedCurrent = targetSpeed;
            //if (distance > 5)
            //{
            //    _moveSpeedCurrent += 20f;
            //}
            //else _moveSpeedCurrent = moveSpeed;
        }
        
        
    }



}
using UnityEngine;
using System.Collections;
using Assets.Scripts.CharacterControl;
using System;

namespace Assets.Scripts.Camera
{
    // inspired by https://gist.github.com/Jellybit/9f6182c25bceac06db31
    // and then simplified

    class CameraControl : MonoBehaviour
    {
        [SerializeField] public float maxDistanceFromPlayer = 4f;
        [SerializeField] public float moveSpeed = 20f;
        [SerializeField] public GameObject player;
        [SerializeField] public float scrollMultiplier = 1.8f;
        [SerializeField] public float roomPadBottom = 6.0f;
        [SerializeField] public float roomPadTop = 6.0f;
        [SerializeField] public float roomPadLeft = 8.0f;
        [SerializeField] public float roomPadRight = 8.0f;

        private Vector2 _camPositionWithoutZ;
        private Vector2 _playerPositionWithoutZ;
        private Vector2 _playerPositionWithoutZPrior;
        private Vector2 _roomUpLeft;
        private Vector2 _roomDownRight;
        private float _camZ;
        private bool _isClutchEngaged;  // when on, prevent the camera from tracking the player
        private Vector2 _camTargetPosition;


        void LateUpdate()
        {
            _camPositionWithoutZ = new Vector2(transform.position.x, transform.position.y);
            _playerPositionWithoutZ = new Vector2(player.transform.position.x, player.transform.position.y);

            if (!_isClutchEngaged)
            {
                // get how far the player moved
                Vector3 playerMoveDistance = _playerPositionWithoutZ - _playerPositionWithoutZPrior;

                //Move the camera this direction, but faster than the player moved.
                Vector2 multipliedDifference = playerMoveDistance * scrollMultiplier;

                // assign the new cam position value
                _camPositionWithoutZ += multipliedDifference;

                // now constrain the dimensions
                ClampCamPosition();

                SetTransformToCapPosition();
            }

            _playerPositionWithoutZPrior = _playerPositionWithoutZ;

        }
        private void Start()
        {
            _camZ = transform.position.z;
            _playerPositionWithoutZPrior = new Vector2(player.transform.position.x, player.transform.position.y);
            transform.position = new Vector3(_playerPositionWithoutZ.x, _playerPositionWithoutZ.y, _camZ);
            _isClutchEngaged = false;
        }

        public Vector2 GetCameraPosition()
        {
            return _camPositionWithoutZ;
        }
        public void MoveCamera(Vector3 targetPosition)
        {
            StartCoroutine(MoveToPosition(targetPosition));
        }
        public void MoveCameraToConstraint()
        {
            // gently move toward the constrained position
            // rather than snap to it
            // this assumes only moving in 1 dimension
            // at a time
            
            float minLeft = _roomUpLeft.x + roomPadLeft;
            float maxRight = _roomDownRight.x - roomPadRight;
            float minBottom = _roomDownRight.y + roomPadBottom;
            float maxTop = _roomUpLeft.y - roomPadTop;
            if (_camPositionWithoutZ.x < minLeft)
            {
                StartCoroutine(MoveToPosition(new Vector2(minLeft, _camPositionWithoutZ.y)));
            }
            if (_camPositionWithoutZ.x > maxRight)
            {
                StartCoroutine(MoveToPosition(new Vector2(maxRight, _camPositionWithoutZ.y)));
            }
            if (_camPositionWithoutZ.y < minBottom)
            {
                StartCoroutine(MoveToPosition(new Vector2(_camPositionWithoutZ.x, minBottom)));
            }
            if (_camPositionWithoutZ.y > maxTop)
            {
                StartCoroutine(MoveToPosition(new Vector2(_camPositionWithoutZ.x, maxTop)));
            }
        }

        public void UpdateRoomDimensions(Vector2 upLeft, Vector2 downRight)
        {
            _roomUpLeft = upLeft;
            _roomDownRight = downRight;
        }

        private void ClampCamPosition()
        {
            float minX = _camPositionWithoutZ.x;
            float maxX = _camPositionWithoutZ.x;
            float minY = _camPositionWithoutZ.y;
            float maxY = _camPositionWithoutZ.y;

            // first keep it from getting too far in front of the player
            // Get the distance of the player from the camera.
            float distance = Vector2.Distance(_camPositionWithoutZ, _playerPositionWithoutZ);
            if (distance > maxDistanceFromPlayer)
            {
                _camPositionWithoutZ.x = Mathf.Clamp(
                    _camPositionWithoutZ.x,
                    _playerPositionWithoutZ.x - maxDistanceFromPlayer,
                    _playerPositionWithoutZ.x + maxDistanceFromPlayer);
                _camPositionWithoutZ.y = Mathf.Clamp(
                    _camPositionWithoutZ.y,
                    _playerPositionWithoutZ.y - maxDistanceFromPlayer,
                    _playerPositionWithoutZ.y + maxDistanceFromPlayer);
            }


            // clamp to room dimensions
            _camPositionWithoutZ.x = Mathf.Clamp(
                _camPositionWithoutZ.x,
                _roomUpLeft.x + roomPadLeft,
                _roomDownRight.x - roomPadRight);
            _camPositionWithoutZ.y = Mathf.Clamp(
                _camPositionWithoutZ.y,
                _roomDownRight.y + roomPadBottom,
                _roomUpLeft.y - roomPadTop);

        }
        private void SetTransformToCapPosition()
        {
            transform.position = new Vector3(_camPositionWithoutZ.x, _camPositionWithoutZ.y, _camZ);
        }
        IEnumerator MoveToPosition(Vector2 targetPosition)
        {
            _camTargetPosition = targetPosition; // keep the target a class variable so we can update it mid-move
            _isClutchEngaged = true;

            const float snapDistance = 0.1f;


            while (_camPositionWithoutZ != _camTargetPosition)
            {
                _camPositionWithoutZ = Vector3.MoveTowards(transform.position, _camTargetPosition, moveSpeed * Time.deltaTime);
                if (Vector2.Distance(_camPositionWithoutZ, _camTargetPosition) < snapDistance)
                {
                    _camPositionWithoutZ = _camTargetPosition;
                }
                SetTransformToCapPosition();
                yield return 0;
            }

            _isClutchEngaged = false;
        }
    }



}
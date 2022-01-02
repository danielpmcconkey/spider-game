using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Camera
{
    public class ConstantBackgroundPositioner : MonoBehaviour
    {
        [SerializeField] public UnityEngine.Camera mainCamera;
        private SpriteRenderer _spriteRenderer;

        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }


        private void LateUpdate()
        {
            // scale the X and Y to match the camera view box
            SetScaleToMatchCamera();

            // get position of camera
            Vector2 camPosition = mainCamera.transform.position;
            
            // set the position of the BG sprite
            transform.position = camPosition;
        }

        private void SetScaleToMatchCamera()
        {
            // note: all measurements below are in unity meters

            // Calculate orthographic camera bounds
            float camViewHeight = 1f * mainCamera.orthographicSize;
            float camViewWidth = camViewHeight * mainCamera.aspect;

            // get current dimensions
            float currentWidthOfSprite = _spriteRenderer.size.x;
            float currentHeightOfSprite = _spriteRenderer.size.y;

            // determine scaling needs
            float scaleX = camViewWidth / currentWidthOfSprite;
            float scaleY = camViewHeight / currentHeightOfSprite;

            // now scale the sprite
            transform.localScale = new Vector2(scaleX, scaleY);
        }
    }
}

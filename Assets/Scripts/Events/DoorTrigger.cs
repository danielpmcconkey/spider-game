using Assets.Scripts.Events;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Events
{
    public class DoorTrigger: MonoBehaviour
    {
        private Vector2 _originalPosition;
        private const float _doorOpenTimeInSeconds = 4.0f;
        [SerializeField] public GameObject doorSlider;
        
        [SerializeField] public float doorGravity = 91f;
        [SerializeField] public float doorHeight = 3f;
        private bool _isSliding;

        private void Start()
        {
            _originalPosition = doorSlider.transform.position;
            _isSliding = false;
        }
        private void Update()
        {
            
        }
        private void FixedUpdate()
        {
            if (doorSlider.transform.position.y > _originalPosition.y && !_isSliding)
            {
                doorSlider.transform.position = new Vector2(
                    _originalPosition.x, doorSlider.transform.position.y + (doorGravity * Time.deltaTime * -1));
            }
            if (doorSlider.transform.position.y < _originalPosition.y)
            {
                doorSlider.transform.position = _originalPosition;
            }
        }

        private void OnTriggerEnter2D(Collider2D collider)
        {
            if(collider.CompareTag("Player"))
            {
                StartCoroutine(PushDoorUp(_doorOpenTimeInSeconds));
            }
        }
        
        IEnumerator PushDoorUp(float duration)
        {
            // use Time.unscaledDeltaTime to allow doors
            // to move during doorway transitions

            _isSliding = true;
            float timeElapsed = 0;

            while (timeElapsed < duration)
            {
                float targetY = doorSlider.transform.position.y + doorGravity * Time.unscaledDeltaTime;
                if(targetY > _originalPosition.y + doorHeight) targetY = _originalPosition.y + doorHeight;

                doorSlider.transform.position = new Vector2(
                    _originalPosition.x, targetY
                    );
                timeElapsed += Time.unscaledDeltaTime;

                yield return null;
            }
            _isSliding = false;            
        }
    }
}

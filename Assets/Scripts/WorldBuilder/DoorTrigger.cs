using Assets.Scripts.Events;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.WorldBuilder
{
    public class DoorTrigger: MonoBehaviour
    {
        private Vector2 _originalPosition;
        private Vector2 _upPosition;
        private const float _doorCloseTimeInSeconds = 2.0f;
        [SerializeField] public GameObject doorSlider;
        private IEnumerator _coroutineOpen;
        private IEnumerator _coroutineClose;

        private void Start()
        {
            _originalPosition = doorSlider.transform.position;
            _upPosition = new Vector2(_originalPosition.x, _originalPosition.y + Globals.doorHeightInTiles);

            _coroutineOpen = SlideToward(_originalPosition, _upPosition, 4.0f);
            _coroutineClose = SlideToward(_upPosition, _originalPosition, 2.0f);
        }
        private void Update()
        {
            
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if(collision.CompareTag("Player"))
            {
                GameEvents.current.DoorwayTriggerEnter();

                StopCoroutine(_coroutineOpen);
                StopCoroutine(_coroutineClose);
                StartCoroutine(_coroutineOpen);
            }
        }
        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {
                GameEvents.current.DoorwayTriggerExit();
                StopCoroutine(_coroutineOpen);
                StopCoroutine(_coroutineClose);
                StartCoroutine(_coroutineClose);
            }
        }
        IEnumerator SlideToward(Vector2 start, Vector2 target, float lerpDuration)
        {
            float timeElapsed = 0;
            while (timeElapsed < lerpDuration)
            {
                doorSlider.transform.position = Vector2.Lerp(start, target, timeElapsed / lerpDuration);
                timeElapsed += Time.deltaTime;

                yield return null;
            }
            // finally, snap it into desired position
            doorSlider.transform.position = target;
        }
    }
}

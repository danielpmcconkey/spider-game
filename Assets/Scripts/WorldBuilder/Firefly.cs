using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Assets.Scripts.WorldBuilder
{
    public class Firefly : MonoBehaviour
    {
        [SerializeField] public GameObject swarmParent;
        [SerializeField] public Light2D thisLight;
        [SerializeField] public float maxDistanceFromCenter = 3.0f;
        [SerializeField] public float moveSpeed = 10.0f;
        [SerializeField] public float intensityChange = 2.0f;
        private Vector2 _swarmCenter;
        private Vector2 _currentPathTarget;
        private int _oddsOfChanging = 100; // once every this mary frames, change direction
        private float _moveSpeedCurrent;
        private float _targetIntensity;
        private float _originalIntensity;
        private float _maxColorChange = 0.1f;


        private void Start()
        {
            _swarmCenter = swarmParent.transform.position;
            _moveSpeedCurrent = moveSpeed * Utility.RNG.GetRandomPercent();
            _originalIntensity = thisLight.intensity;
            ChangePathTarget();
            ChangeTargetIntensity();
            ChangeColor();
            
        }
        private void Update()
        {
            if(Utility.RNG.GetRandomInt(0, _oddsOfChanging) == 0)
            {
                ChangePathTarget();
            }
            if (Utility.RNG.GetRandomInt(0, _oddsOfChanging) == 0)
            {
                ChangeTargetIntensity();
            }
            MoveTowardTarget();
            MoveIntensity();
        }

        private void MoveTowardTarget()
        {
            transform.position = Vector3.Lerp(
                transform.position,
                _currentPathTarget,
                _moveSpeedCurrent * Time.deltaTime);
        }
        private void MoveIntensity()
        {
            float currentIntensity = thisLight.intensity;
            if (currentIntensity < _targetIntensity) thisLight.intensity += (_moveSpeedCurrent * Time.deltaTime);
            else if (currentIntensity > _targetIntensity) thisLight.intensity -= (_moveSpeedCurrent * Time.deltaTime);
        }
        private void ChangePathTarget()
        {
            float minX = _swarmCenter.x - maxDistanceFromCenter;
            float maxX = _swarmCenter.x + maxDistanceFromCenter;
            float minY = _swarmCenter.y - maxDistanceFromCenter;
            float maxY = _swarmCenter.y + maxDistanceFromCenter;

            float x = minX + (Utility.RNG.GetRandomPercent() * (maxX - minX));
            float y = minY + (Utility.RNG.GetRandomPercent() * (maxY - minY));

            _currentPathTarget = new Vector2(x, y);
        }
        private void ChangeTargetIntensity()
        {
            float minIntensity = _originalIntensity - intensityChange;
            float maxIntensity = _originalIntensity + intensityChange;
            _targetIntensity = minIntensity + (Utility.RNG.GetRandomPercent() * (maxIntensity - minIntensity));
        }
        private void ChangeColor()
        {
            float red = (thisLight.color.r - _maxColorChange) +
                (Utility.RNG.GetRandomPercent() *
                ((thisLight.color.r + _maxColorChange) - (thisLight.color.r - _maxColorChange)));
            float green = (thisLight.color.g - _maxColorChange) +
                (Utility.RNG.GetRandomPercent() *
                ((thisLight.color.g + _maxColorChange) - (thisLight.color.g - _maxColorChange)));
            float blue = (thisLight.color.b - _maxColorChange) +
                (Utility.RNG.GetRandomPercent() *
                ((thisLight.color.b + _maxColorChange) - (thisLight.color.b - _maxColorChange)));
            thisLight.color = new Color(red, green, blue);
        }
    }
}

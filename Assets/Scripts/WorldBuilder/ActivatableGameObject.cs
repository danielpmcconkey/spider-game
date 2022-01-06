using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.WorldBuilder
{
    public class ActivatableGameObject
    {
        protected GameObject _gameObject;
        public void ActivateSelf()
        {
            if (!_gameObject.activeSelf)
            {
                _gameObject.SetActive(true);
            }
        }
        public void DeActivateSelf()
        {
            if (_gameObject.activeSelf)
            {
                _gameObject.SetActive(false);
            }
        }
        public void SwapGameObject(GameObject gameObject)
        {
            // todo: get rid of SwapGameObject as it kills encapsulation
            _gameObject = gameObject;
        }
    }
}

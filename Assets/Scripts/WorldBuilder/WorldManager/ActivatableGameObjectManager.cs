using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.WorldBuilder.WorldManager
{
    public class ActivatableGameObjectManager
    {
        protected GameObject _gameObject;

        public ActivatableGameObjectManager(GameObject gameObject)
        {
            _gameObject = gameObject;
        }

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
    }
}

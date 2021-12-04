using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.CharacterControl
{
    public class AiControlledCharacter : ControllableCharacter
    {
        [SerializeField] public GameObject targetCharacter;
        [SerializeField] public float agroDistance = 10;

        protected bool isAgroed = false;

        protected override void Awake()
        {
            base.Awake();
            if(targetCharacter == null)
            {
                targetCharacter = GameObject.Find("PlayerCharacter");
            }
        }
        protected override void FixedUpdate()
        {
            base.FixedUpdate();
        }
        protected override void CheckUserInput()
        {
            if (!_isAlive) return; // no movement for deadies

            // every frame check whether user has changed input
            _userInput = GetNextMove();
        }
        protected virtual UserInputCollection GetNextMove()
        {
            UserInputCollection userInput = new UserInputCollection();

            Vector2 playerPos = targetCharacter.transform.position;
            Vector2 enemyPos = transform.position;
            if(Vector2.Distance(playerPos, enemyPos) < agroDistance)
            {
                isAgroed = true;
                Vector2 direction = playerPos - enemyPos;
                if (direction.x > 0) userInput.moveHPressure = 1;
                if (direction.x < 0) userInput.moveHPressure = -1;
                if (direction.y > 0) userInput.moveVPressure = 1;
                if (direction.y < 0) userInput.moveVPressure = -1;

            }
            else
            {
                isAgroed = false;
                userInput.moveHPressure = 0;
                userInput.moveVPressure = 0;
                Stop();
            }
            return userInput;
        }
        protected virtual void Stop()
        {
            _groundedForcesAccumulated = Vector2.zero;
            if (canFly) _floatingForcesAccumulated = Vector2.zero;
        }
    }
}

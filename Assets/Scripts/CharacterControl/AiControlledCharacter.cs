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
        [SerializeField] public GameObject player;
        [SerializeField] public float agroDistance = 10;
        protected override void CheckUserInput()
        {
            
            // every frame check whether user has changed input
            
            _userInput = GetNextMove();




        }
        protected override void AddArtificalGravity()
        {
            base.AddArtificalGravity();
        }
        protected virtual UserInputCollection GetNextMove()
        {
            UserInputCollection userInput = new UserInputCollection();

            Vector2 playerPos = player.transform.position;
            Vector2 enemyPos = transform.position;
            if(Vector2.Distance(playerPos, enemyPos) < agroDistance)
            {
                Vector2 direction = playerPos - enemyPos;
                if (direction.x > 0) userInput.moveHPressure = 1;
                if (direction.x < 0) userInput.moveHPressure = -1;
                if (direction.y > 0) userInput.moveVPressure = 1;
                if (direction.y < 0) userInput.moveVPressure = -1;

            }



            //userInput.moveHPressure = 0f;
            //userInput.moveVPressure = 0f;
            //userInput.isJumpPressed = false;
            //userInput.isJumpReleased = false;
            //userInput.isJumpHeldDown = false;
            //userInput.isGrappleButtonPressed = false;
            //userInput.isGrappleButtonReleased = false;
            //userInput.isGrappleButtonHeldDown = false;
            //userInput.mouseX = 0f;
            //userInput.mouseY = 0f;
            return userInput;
        }
    }
}

using Assets.Scripts.Configurations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{    
    public class MovementScript 
    {
        private MovementConfig movement;

        public MovementScript(MovementConfig movementConfig)
        {
            if(!movementConfig.IsValidConfig())
            {
                Debug.Log("invalid movement config");
            }
            else
            {
                movement = movementConfig;
            }
        }
        
        public void GetInput(PlayerScript player)
        {
            if(movement is null)
            {

            }

            if (player.stats.IsDead) return;

            if (movement.IsValidConfig())
            {
                if (Input.GetKeyDown(movement.RightKey))
                {
                    player.Move(Vector3.right);
                }
                else if (Input.GetKeyDown(movement.LeftKey))
                {
                    player.Move(Vector3.left);
                }
                else if (Input.GetKeyDown(movement.UpKey))
                {
                    player.Move(Vector3.up);
                }
                else if (Input.GetKeyDown(movement.DownKey))
                {
                    player.Move(Vector3.down);
                }
                else if (Input.GetKeyDown(movement.BombKey))
                {
                    player.CreateBomb();
                }
            }            
        }        
    }
}

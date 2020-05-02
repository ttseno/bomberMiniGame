using Assets.Scripts.Configurations;
using UnityEngine;

namespace Assets.Scripts
{    
    public class MovementScript : MonoBehaviour
    {
        [SerializeField]
        private MovementConfig movementConfig;


        private PlayerScript player;

        void Start()
        {
            player = gameObject.GetComponent<PlayerScript>();
        }

        void Update()
        {
            GetInput();
        }
        
        private void GetInput()
        {
            if (movementConfig.IsValidConfig())
            {
                if (Input.GetKeyDown(movementConfig.RightKey))
                {
                    player.Move(Vector3.right);
                }
                else if (Input.GetKeyDown(movementConfig.LeftKey))
                {
                    player.Move(Vector3.left);
                }
                else if (Input.GetKeyDown(movementConfig.UpKey))
                {
                    player.Move(Vector3.up);
                }
                else if (Input.GetKeyDown(movementConfig.DownKey))
                {
                    player.Move(Vector3.down);
                }
                else if (Input.GetKeyDown(movementConfig.BombKey))
                {
                    player.CreateBomb();
                }
            }            
        }        
    }
}

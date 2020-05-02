using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

namespace Assets.Scripts
{

    [Serializable]
    public class InformBombEvent : UnityEvent<BombScript>
    {

    }
    public class AIMovementScript : MonoBehaviour
    {
        [SerializeField] private float speed;

        private PlayerScript player;

        private List<Vector3> dangerZone = new List<Vector3>();

        private float nextTime;
        private List<Vector3> directions = new List<Vector3>()
        {
            Vector3.left,
            Vector3.up,
            Vector3.down,
            Vector3.right
        };

        void Start()
        {
            player = gameObject.GetComponent<PlayerScript>();
            nextTime = Time.time;
        }

        void Update()
        {
            if (nextTime > Time.time)
                return;

            nextTime = Time.time + speed;

            if (dangerZone.Contains(transform.position))
            {
                MoveAwayFromTheBomb();
            }
            else if (HasBlockAround() && player.stats.HasAvailableBomb())
                player.CreateBomb();
            else
            {
                var nextPosition = AnyDirection();
                if (!dangerZone.Contains(player.transform.position + nextPosition))
                    player.Move(nextPosition);
            }
        }

        private bool HasBlockAround()
        {
            return directions.Any(direction =>
            {
                var position = transform.position + direction;
                return Physics2D
                    .RaycastAll(position, Vector2.zero)
                    .FirstOrDefault(hit => hit.collider?.name.Contains("Block") ?? false);
            });
        }

        private bool HasExit(Vector3 nextPosition)
        {
            return directions.Any(direction =>
            {
                var position = nextPosition + direction;
                var rayCast = Physics2D
                    .RaycastAll(position, Vector2.zero);
                
                var any = rayCast.Any();

                return !any;
            });
        }

        private void MoveAwayFromTheBomb()
        {
            var position = transform.position;

            foreach (var direction in directions)
            {
                var nextPosition= transform.position + direction;
                if (HasExit(nextPosition) || !dangerZone.Contains(nextPosition))
                    player.Move(direction);

                if (transform.position != position)
                    break;

            }
        }

        public Vector3 AnyDirection()
        {
            return directions[Random.Range(0, directions.Count)];
        }

        public void AddToDangerZone(BombScript bomb)
        {
            print("Adding to danger zone");

            dangerZone.Add(bomb.transform.position);

            foreach (var direction in directions)
            {
                for (int i = 1; i <= bomb.bombSize + 1; i++)
                {
                    dangerZone.Add(bomb.transform.position + direction * i);
                }
            }
            print("Danger zone total tiles: " + dangerZone.Count);
        }

        public void RemoveFromDangerZone(BombScript bomb)
        {
            print("Removing from danger zone");

            dangerZone.Remove(bomb.transform.position);

            foreach (var direction in directions)
            {
                for (int i = 1; i <= bomb.bombSize + 1; i++)
                {
                    var zone = dangerZone.First(x => x == bomb.transform.position + direction * i);
                    dangerZone.Remove(zone);
                }
            }

            print("Danger zone total tiles: " + dangerZone.Count);
        }
    }
}

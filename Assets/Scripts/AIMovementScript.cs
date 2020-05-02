using System;
using System.Collections.Generic;
using System.Linq;
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
        private Vector3 lastDirection = Vector3.zero;
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
            {
                lastDirection = Vector3.zero;
                player.CreateBomb();
            }
            else
            {
                lastDirection = Vector3.zero;
                var directionToPower = DirectionToPowerAround();
                var nextPosition = directionToPower == Vector3.zero ? AnyDirection() : directionToPower;
                if (!dangerZone.Contains(player.transform.position + nextPosition))
                    player.Move(nextPosition);
            }
        }

        private Vector3 DirectionToPowerAround()
        {
            return directions.FirstOrDefault(direction =>
            {
                var position = transform.position + direction;
                return Physics2D
                    .RaycastAll(position, Vector2.zero)
                    .Any(hit => hit.collider?.name.Contains("Power") ?? false);
            });
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

            foreach (var direction in directions.Where(x => x != -lastDirection))
            {
                var nextPosition = transform.position + direction;
                if (HasExit(nextPosition) || !dangerZone.Contains(nextPosition))
                    player.Move(direction);

                if (transform.position != position)
                {
                    lastDirection = direction;
                    break;
                }

            }
        }

        public Vector3 AnyDirection() => directions[Random.Range(0, directions.Count)];

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

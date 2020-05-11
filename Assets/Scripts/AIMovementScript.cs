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
        private static List<Vector3> directions = new List<Vector3>()
        {
            Vector3.left,
            Vector3.up,
            Vector3.down,
            Vector3.right
        };
        
        private static Dictionary<string, float> tileWeights = new Dictionary<string, float>()
        {
            { "Explosion", -1},
            { "Bomb", -1},
            { "Wall", 1},
            { "Blocks", 30},
            { "Player", 50},
            { "Powers", 70},
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
            else
            {
                var lastPosition = transform.position;
                lastDirection = Vector3.zero;
                var nextDirection = MostValuableDirection();
                if (nextDirection != Vector3.zero && !dangerZone.Contains(player.transform.position + nextDirection))
                    player.Move(nextDirection);

                //if (HasBlockAround() && player.stats.HasAvailableBomb())
                if(transform.position == lastPosition)
                    CreateBomb();

            }
        }

        private void CreateBomb()
        {
            var newBombRange = BombDestructionRange(transform.position, player.stats.BombSize);
            if(!dangerZone.Any(x => newBombRange.Any(y => y.y == x.y && y.x == x.x)))
                player.CreateBomb();
        }


        private Vector3 MostValuableDirection()
        {
            RaycastHit2D RaycastHit2D(Vector3 direction)
            {
                var hitList = Physics2D
                    .RaycastAll(transform.position, direction);
                var hit2 = hitList.FirstOrDefault(x => x.collider.name != transform.name);
                return hit2;
            }

            var hits = directions.Select(x => new
            {
                Direction = x,
                Hit = RaycastHit2D(x)
            });

            var mostValuableHit = hits.Aggregate((act, next) => PathWeight(act.Hit) > PathWeight(next.Hit) ? act : next);
            
            return mostValuableHit.Hit.collider.gameObject.tag == "Wall" ? Vector3.zero : mostValuableHit.Direction;
            
            float PathWeight(RaycastHit2D hit)
            {
                var distance = hit.distance;

                if (!tileWeights.TryGetValue(hit.collider.gameObject.tag, out var rewardWeight))
                    return 0;

                return rewardWeight / distance;
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

            foreach (var direction in directions.Where(x => x != -lastDirection).OrderBy(x => Random.value))
            {
                var nextPosition = transform.position + direction;

                var rayCast = Physics2D
                    .RaycastAll(nextPosition, Vector2.zero);
                if(rayCast.Any(x => x.collider.gameObject.tag == "Explosion"))
                    continue;

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

        private List<Vector3> BombDestructionRange(Vector3 bombPosition, int bombSize)
        {
            var destructionRange = new List<Vector3>()
            {
                bombPosition
            };

            foreach (var direction in directions)
            {
                for (int i = 1; i <= bombSize; i++)
                {
                    destructionRange.Add(bombPosition + direction * i);
                }
            }

            return destructionRange;
        }

        public void AddToDangerZone(BombScript bomb)
        {
            var bombDestructionRange = BombDestructionRange(bomb.transform.position, bomb.bombSize);
            dangerZone.AddRange(bombDestructionRange);
        }

        public void RemoveFromDangerZone(BombScript bomb)
        {
            print("Removing from danger zone");

            dangerZone.Remove(bomb.transform.position);

            foreach (var direction in directions)
            {
                for (int i = 1; i <= bomb.bombSize; i++)
                {
                    var zone = dangerZone.First(x => x == bomb.transform.position + direction * i);
                    dangerZone.Remove(zone);
                }
            }

            print("Danger zone total tiles: " + dangerZone.Count);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace Assets.Scripts
{
    public class BombScript : MonoBehaviour
    {
        #region Serializeble fields

        [SerializeField]
        private Sprite bombSprite;
        [SerializeField]
        private Sprite explodedBombSprite;
        [SerializeField]
        private Sprite explosionTrailSprite;
        [SerializeField]
        private Sprite explosionTrailEndSprite;
        [SerializeField]
        private int timer = 5;

        public InformBombEvent CreatedBombEvent;
        public InformBombEvent ExplodedBombEvent;
        #endregion

        private PlayerScript player;

        private int explosionTimer = 1;
        public  int bombSize;

        List<TrailDirection> trailDirections = new List<TrailDirection> {
        new TrailDirection { Direction = Vector3.right, Rotation = Vector3.forward * 0 },
        new TrailDirection { Direction = Vector3.up, Rotation = Vector3.forward * 90 },
        new TrailDirection { Direction = Vector3.left, Rotation = Vector3.forward * 180 },
        new TrailDirection { Direction = Vector3.down, Rotation = Vector3.forward * 270 }
    };
        bool isDestroying = false;


        public void CreateBomb(PlayerScript player)
        {
            var bomb = GameObjectHelper.CreateGameObject(
                   "Bomb",
                   player.transform.position,
                   Vector3.zero,
                   bombSprite);

            var bombScript = bomb.AddComponent<BombScript>();
            bombScript.player = player;
            bombScript.bombSize = player.stats.BombSize;
            bombScript.explodedBombSprite = explodedBombSprite;
            bombScript.explosionTrailEndSprite = explosionTrailEndSprite;
            bombScript.explosionTrailSprite = explosionTrailSprite;
            bombScript.explosionTimer = explosionTimer;
            bombScript.timer = timer;
            bombScript.CreatedBombEvent = CreatedBombEvent;
            bombScript.ExplodedBombEvent = ExplodedBombEvent;

            bombScript.StartCountDown();

            CreatedBombEvent.Invoke(bombScript);
        }
        
        public void StartCountDown()
        {
            StartCoroutine(DestroyBombRoutine());
        }

        IEnumerator DestroyBombRoutine()
        {
            yield return new WaitForSeconds(timer);
            DestroyBomb();
        }

        private void CreateDestructionTrail(Vector3 position, int explosionSize)
        {
            var hitPlayer = Physics2D
                .RaycastAll(position, Vector2.zero)
                .FirstOrDefault(hit => hit.collider?.name.Contains("Player") ?? false);

            if (!(hitPlayer.collider is null))
            {
                var playerCollided = hitPlayer.collider.gameObject.GetComponent<PlayerScript>();
                playerCollided.Kill();
            }

            foreach (var trail in trailDirections)
            {
                var location = position + trail.Direction;
                var hit = Physics2D.Raycast(location, Vector2.zero);

                for (int i = 1; i < explosionSize && hit.collider is null; i++)
                {
                    var bombTrail = GameObjectHelper.CreateGameObject(
                        "Bomb_trail",
                        location,
                        trail.Rotation,
                        explosionTrailSprite,
                        "Explosion");

                    Destroy(bombTrail, explosionTimer);

                    location += trail.Direction;
                    hit = Physics2D.Raycast(location, Vector2.zero);
                }

                if (!(hit.collider is null))
                {
                    switch (hit.collider.name)
                    {
                        case "Player - Enemy":
                        case "Player":
                            {
                                var playerCollided = hit.collider.gameObject.GetComponent<PlayerScript>();
                                playerCollided.Kill();

                                break;
                            }
                        case "WallTileMap":
                            {
                                continue;
                            }
                        case string name when name.Contains("Block"):
                            {
                                var blockCollided = hit.collider.gameObject.GetComponent<BlockScript>();
                                blockCollided.Destroy();

                                break;
                            }
                        case string name when name.Contains("Power"):
                            {
                                var powerCollided = hit.collider.gameObject;
                                Destroy(powerCollided);

                                break;
                            }
                        case string name when name.Contains("Bomb_trail"):
                            {
                                break;
                            }
                        case string name when name.Contains("Bomb"):
                            {
                                var collidedBomb = hit.collider.gameObject;
                                var bombScript = collidedBomb.GetComponent<BombScript>();

                                bombScript.DestroyBomb();

                                break;
                            }
                        default:
                            {
                                Debug.Log($"Bomb got {hit.collider.name}");
                                break;
                            }
                    }
                }

                var bombTrailEnd = GameObjectHelper.CreateGameObject(
                    "Bomb_trail",
                    location,
                    trail.Rotation,
                    explosionTrailEndSprite,
                    "Explosion");

                Destroy(bombTrailEnd, explosionTimer);

            }
        }

        private void DestroyBomb()
        {
            if (isDestroying) return;
            isDestroying = true;

            var position = transform.position;
            name = "Bomb_trail" + position;

            var bombRenderer = GetComponent<SpriteRenderer>();
            bombRenderer.sprite = explodedBombSprite;
            bombRenderer.sortingLayerName = "Explosion";

            CreateDestructionTrail(position, this.bombSize);

            StartCoroutine(DesactivateBomb());
        }

        IEnumerator DesactivateBomb()
        {
            yield return new WaitForSeconds(explosionTimer);
                
            Destroy(gameObject);
            player.DeactivateBomb();
            ExplodedBombEvent.Invoke(this);
        }
              
        public class TrailDirection
        {
            public Vector3 Direction { get; set; }
            public Vector3 Rotation { get; set; }
        }
    }
}

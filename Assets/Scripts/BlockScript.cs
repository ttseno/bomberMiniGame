using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Assets.Scripts
{
    public class BlockScript : MonoBehaviour
    {
        [SerializeField]
        private Sprite blockSprite;
        [SerializeField]
        private Sprite sizeUpBombPowerSprite;
        [SerializeField]
        private Sprite addBombPowerSprite;
        [SerializeField]
        private Tilemap groundTileMap;

        private float blockProbability = 0.85f;

        private float addBombProbability = 0.3f;
        private float sizeUpBombProbability = 0.2f;

        private static readonly List<Vector2> directions = new List<Vector2>()
        {
            Vector2.left,
            Vector2.down,
            Vector2.up,
            Vector2.right
        };


        // Use this for initialization
        void Start()
        {
            CreateBlocks();
        }

        private void CreateBlocks()
        {
            foreach (var pos in groundTileMap.cellBounds.allPositionsWithin)
            {
                Vector3 location = groundTileMap.CellToWorld(pos);

                if (groundTileMap.HasTile(pos))
                {
                    var isPositionAvailable = directions
                        .Select(x => Physics2D.Raycast(location, x, 1))
                        .All(hit => hit.collider is null || !hit.collider.name.Contains("Player"));

                    if (isPositionAvailable && ShouldBuild(blockProbability))
                    {
                        var block = GameObjectHelper.CreateGameObject(
                           "Block",
                           location,
                           Vector3.zero,
                           blockSprite,
                           "Blocks");
                        var blockScript = block.AddComponent<BlockScript>();
                        blockScript.addBombPowerSprite = this.addBombPowerSprite;
                        blockScript.sizeUpBombPowerSprite = this.sizeUpBombPowerSprite;
                    }
                }
            }
        }

        private bool ShouldBuild(float probability)
        {
            return Random.value < probability;
        }

        private (string name, Sprite sprite) ShufflePower()
        {
            switch (Random.value)
            {
                case float value when value < addBombProbability:
                    return ("AddBombPower", addBombPowerSprite);
                case float value when value < addBombProbability + sizeUpBombProbability:
                    return ("SizeUpBombPower", sizeUpBombPowerSprite);
                default:
                    return ("", null);
            }
        }

        public void Destroy()
        {
            Debug.Log("Block destroyed");
            (var name, var sprite) = ShufflePower();

            Destroy(gameObject);
            if (sprite is null) return;

            var block = GameObjectHelper.CreateGameObject(
                   name,
                   transform.position,
                   Vector3.zero,
                   sprite,
                   "Powers");
        }
    }
}
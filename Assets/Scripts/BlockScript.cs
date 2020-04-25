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
        private Tilemap groundTileMap;

        // Use this for initialization
        void Start()
        {
            foreach (var pos in groundTileMap.cellBounds.allPositionsWithin)
            {
                Vector3Int localPlace = new Vector3Int(pos.x, pos.y, pos.z);
                Vector3 location = groundTileMap.CellToWorld(localPlace);
                if (groundTileMap.HasTile(localPlace))
                {
                    var directions = new List<Vector2>(){
                        Vector2.left,
                        Vector2.down,
                        Vector2.up,
                        Vector2.right
                    };

                    var rays = directions
                        .Select(x => Physics2D.Raycast(location, x, 1))
                        .All(hit=> hit.collider is null || !hit.collider.name.Contains("Player"));

                    if (rays)
                    {
                        GameObjectHelper.CreateGameObject(
                           "Block",
                           location,
                           Vector3.zero,
                           blockSprite,
                           "Blocks");
                    }
                }
            }

        }
        
        
    }
}
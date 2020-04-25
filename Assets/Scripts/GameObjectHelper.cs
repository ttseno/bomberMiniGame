using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    public static class GameObjectHelper
    {
        public static GameObject CreateGameObject(string name, Vector3 location, Vector3 rotation, Sprite sprite, string sortingLayerName = "Powers")
        {
            var gameObjectName = name + location;
            var gameObject = new GameObject(gameObjectName);
            gameObject.SetActive(true);
            var gameObjectRenderer = gameObject.AddComponent<SpriteRenderer>();
            gameObjectRenderer.sortingLayerName = sortingLayerName;
            gameObjectRenderer.sprite = sprite;
            gameObjectRenderer.sortingOrder = 3;
            gameObject.transform.position = location;
            gameObject.transform.Rotate(rotation);
            var rigidBody = gameObject.AddComponent<Rigidbody2D>();
            rigidBody.bodyType = RigidbodyType2D.Static;
            var boxCollider = gameObject.AddComponent<BoxCollider2D>();
            boxCollider.size = Vector2.one;

            return gameObject;
        }
    }
}

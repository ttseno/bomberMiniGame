﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


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

    #endregion

    private int explosionTimer = 1;

    List<TrailDirection> trailDirections = new List<TrailDirection> {
        new TrailDirection { Direction = Vector3.right, Rotation = Vector3.forward * 0 },
        new TrailDirection { Direction = Vector3.up, Rotation = Vector3.forward * 90 },
        new TrailDirection { Direction = Vector3.left, Rotation = Vector3.forward * 180 },
        new TrailDirection { Direction = Vector3.down, Rotation = Vector3.forward * 270 }
    };

    public void CreateBomb(PlayerScript player)
    {
        var bomb = CreateGameObject(
               "Bomb",
               player.transform.position,
               Vector3.zero,
               bombSprite);

        StartCoroutine(DestroyBombRoutine(bomb, player));
    }

    IEnumerator DestroyBombRoutine(GameObject bomb, PlayerScript player)
    {
        var bombRenderer = bomb.GetComponent<SpriteRenderer>();
        yield return new WaitForSeconds(timer);
        bombRenderer.sprite = explodedBombSprite;
        bombRenderer.sortingLayerName = "Explosion";

        CreateDestructionTrail(bomb.transform.position, player.bombSize);

        Destroy(bomb, explosionTimer);
        player.DeactivateBomb();
    }

    private void CreateDestructionTrail(Vector3 position, int explosionSize)
    {
        foreach (var trail in trailDirections)
        {
            var location = position + trail.Direction;
            var hit = Physics2D.Raycast(location, Vector2.zero);

            for (int i = 1; i < explosionSize && hit.collider is null; i++)
            {
                var bombTrail = CreateGameObject(
                    "Bomb_trail",
                    location,
                    trail.Rotation,
                    explosionTrailSprite,
                    "Explosion");

                Destroy(bombTrail, explosionTimer);

                location += trail.Direction;
                hit = Physics2D.Raycast(location, Vector2.zero);
            }

            if (!(hit.collider is null) && hit.collider.name == "WallTileMap")
                continue;

            var bombTrailEnd = CreateGameObject(
                "Bomb_trail",
                location,
                trail.Rotation,
                explosionTrailEndSprite,
                "Explosion");

            Destroy(bombTrailEnd, explosionTimer);
            
        }
    }

    private GameObject CreateGameObject(string name, Vector3 location, Vector3 rotation, Sprite sprite, string sortingLayerName = "Powers")
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

    public class TrailDirection
    {
        public Vector3 Direction { get; set; }
        public Vector3 Rotation { get; set; }
    }
}

using Assets.Scripts;
using Assets.Scripts.Configurations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


[Serializable]
public class BombEvent : UnityEvent<PlayerScript>
{

}

public class PlayerScript : MonoBehaviour
{
    [SerializeField]
    private PlayerSkinConfig skinConfig;
    [SerializeField]
    public PlayerStats stats;
    
    public BombEvent bombEvent;
      
    private float deadTimer = 3f;

    private Dictionary<Vector3, Sprite> SpriteToDirection;

    // Start is called before the first frame update
    void Start()
    {
        SpriteToDirection = new Dictionary<Vector3, Sprite>()
        {
            {Vector3.right, skinConfig.RightSprite},
            {Vector3.left, skinConfig.LeftSprite},
            {Vector3.up, skinConfig.UpSprite},
            {Vector3.down, skinConfig.DownSprite},
        };

        stats.ActiveBombs = 0;
    }


    void OnTriggerEnter2D(Collider2D collider)
    {
        Debug.Log($"collision {collider.name}");
        if (collider.name.Contains("Bomb_trail"))
            Kill();
    }

    public void Kill()
    {
        ChangeSprite(skinConfig.DeadSprite);
        stats.IsDead = true;
        stats.Lifes--;

        StartCoroutine(StageDeath());
    }

    IEnumerator StageDeath()
    {
        yield return new WaitForSeconds(deadTimer);

        if (stats.Lifes > 0)
        {
            ChangeSprite(skinConfig.DownSprite);
            stats.IsDead = false;
        }
        else if (gameObject.name.Contains("Enemy"))
            Destroy(gameObject);
        else
            EndGame();        
    }
    
    public void DeactivateBomb()
    {
        stats.ActiveBombs--;
    }

    public void ChangeSprite(Sprite sprite)
    {
        var spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = sprite;
    }

    public void Move(Vector3 direction)
    {
        if (stats.IsDead) return;

        ChangeSprite(SpriteToDirection[direction]);

        var newPosition = transform.position + direction;

        var hit = Physics2D.Raycast(newPosition, Vector2.zero);
        if (!(hit.collider is null))
        {
            switch (hit.collider.name)
            {
                case string name when name.Contains("AddBombPower"):
                    {
                        stats.AllowedBombs++;
                        var powerObject = hit.collider.gameObject;
                        Destroy(powerObject);

                        break;
                    }
                case string name when name.Contains("SizeUpBombPower"):
                    {
                        stats.BombSize++;
                        var powerObject = hit.collider.gameObject;
                        Destroy(powerObject);

                        break;
                    }
                case string name when name.Contains("AddLifeBombPower"):
                    {
                        stats.Lifes++;
                        var powerObject = hit.collider.gameObject;
                        Destroy(powerObject);

                        break;
                    }
                case string name when name.Contains("Bomb_trail"):
                    break;
                default:
                    return;
            }
        }
        
        transform.position = newPosition;
    }
       
    public void CreateBomb()
    {
        if (stats.IsDead) return;
        if (stats.HasAvailableBomb())
        {
            bombEvent.Invoke(this);
            stats.ActiveBombs++;
        }
    }

    private void EndGame()
    {
        FindObjectOfType<InGameManager>().EndGame();
    }
    
}

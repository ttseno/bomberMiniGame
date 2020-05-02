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
    private MovementConfig movementConfig;
    [SerializeField]
    public PlayerStats stats;
    
    public BombEvent bombEvent;
      
    private float deadTimer = 3f;

    public MovementScript movementScript;
    public Dictionary<Vector3, Sprite> SpriteToDirection;

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

        //transform.position = new Vector3(-4.5f, 3.5f, 0f);
        stats.ActiveBombs = 0;
        stats.BombSize = 0;

        movementScript = new MovementScript(movementConfig);
    }

    // Update is called once per frame
    void Update()
    {
        movementScript.GetInput(this);
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
        else
            Destroy(gameObject);        
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
        
        Debug.Log("moving");
        transform.position = newPosition;
    }
       
    public void CreateBomb()
    {
        if (stats.ActiveBombs < stats.AllowedBombs)
        {
            bombEvent.Invoke(this);
            stats.ActiveBombs++;
        }
    }
    
}

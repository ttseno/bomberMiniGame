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
    private Sprite rightNinjaSprite;
    [SerializeField]
    private Sprite leftNinjaSprite;
    [SerializeField]
    private Sprite upNinjaSprite;
    [SerializeField]
    private Sprite downNinjaSprite;
    
    [SerializeField]
    private Sprite deadSprite;

    [SerializeField]
    private int allowedBombs = 1;
    private int activeBombs;

    public BombEvent bombEvent;

    public int bombSize;

    private Dictionary<Vector3, Sprite> SpriteToDirection;

    private bool isDead = false;
    private int lifes = 1;
    private float deadTimer = 3f;

    // Start is called before the first frame update
    void Start()
    {
        SpriteToDirection = new Dictionary<Vector3, Sprite>()
        {
            {Vector3.right, rightNinjaSprite},
            {Vector3.left, leftNinjaSprite},
            {Vector3.up, upNinjaSprite},
            {Vector3.down, downNinjaSprite},
        };

        transform.position = new Vector3(-4.5f, 3.5f, 0f);
        activeBombs = 0;
        bombSize = 0;
    }

    // Update is called once per frame
    void Update()
    {
        GetInput();
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        Debug.Log($"collision {collider.name}");
        if (collider.name.Contains("Bomb_trail"))
            Kill();
    }

    public void Kill()
    {
        ChangeSprite(deadSprite);
        isDead = true;
        lifes--;

        StartCoroutine(StageDead());
    }

    IEnumerator StageDead()
    {
        yield return new WaitForSeconds(deadTimer);

        if (lifes > 0)
        {
            ChangeSprite(downNinjaSprite);
            isDead = false;
        }
        else
            Destroy(gameObject);
        
    }
    
    public void DeactivateBomb()
    {
        activeBombs--;
    }

    private void ChangeSprite(Sprite sprite)
    {
        var spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = sprite;
    }

    private void Move(Vector3 direction)
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
                        allowedBombs++;
                        var powerObject = hit.collider.gameObject;
                        Destroy(powerObject);

                        break;
                    }
                case string name when name.Contains("SizeUpBombPower"):
                    {
                        bombSize++;
                        var powerObject = hit.collider.gameObject;
                        Destroy(powerObject);

                        break;
                    }
                case string name when name.Contains("AddLifeBombPower"):
                    {
                        lifes++;
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

    private void GetInput()
    {
        if (isDead) return;

        if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            Move(Vector3.right);
        }
        else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            Move(Vector3.left);
        }
        else if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            Move(Vector3.up);
        }
        else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            Move(Vector3.down);
        }
        else if (Input.GetKeyDown(KeyCode.Space))
        {
            if (activeBombs < allowedBombs)
            {
                bombEvent.Invoke(this);
                activeBombs++;
            }
        }
    }

}

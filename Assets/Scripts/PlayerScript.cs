using System;
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
    private int allowedBombs = 1;
    private int activeBombs;

    private bool collided = false;

    private Vector3 lastPosition;
    private Vector3 direction;

    SpriteRenderer spriteRenderer;

    public BombEvent bombEvent;

    public int bombSize;

    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        activeBombs = 0;
        bombSize = 3;
    }

    // Update is called once per frame
    void Update()
    {
        if (!collided)
        {
            GetInput();
        }
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        Debug.Log("collision");
        collided = true;
        transform.position = lastPosition;
    }

    void OnTriggerExit2D(Collider2D other)
    {
        collided = false;
    }

    public void DeactivateBomb()
    {
        activeBombs--;
    }

    private void Move()
    { 
         Debug.Log("moving");
        transform.position += direction;      
    }

    private void GetInput()
    {
        if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            direction = Vector3.right;
            lastPosition = transform.position;
            spriteRenderer.sprite = rightNinjaSprite;
            Move();
        }
        else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            direction = Vector3.left;
            lastPosition = transform.position;
            spriteRenderer.sprite = leftNinjaSprite;
            Move();
        }
        else if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            direction = Vector3.up;
            lastPosition = transform.position;
            spriteRenderer.sprite = upNinjaSprite;
            Move();
        }
        else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            direction = Vector3.down;
            lastPosition = transform.position;
            spriteRenderer.sprite = downNinjaSprite;
            Move();
        }
        else if (Input.GetKeyDown(KeyCode.Space))
        {
            if(activeBombs < allowedBombs)
            {
                bombEvent.Invoke(this);
                activeBombs++;
            }
        }            
    }
      
}

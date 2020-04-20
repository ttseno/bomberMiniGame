using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombScript : MonoBehaviour
{
    [SerializeField]
    private Sprite bombSprite;
    [SerializeField]
    private Sprite explodedBombSprite;
    [SerializeField]
    private int timer = 5;
       

    public void CreateBomb(Vector3 location)
    {
        var bombName = "Bomb" + location;
        var bomb = new GameObject(bombName);
        bomb.SetActive(true);
        var bombRenderer = bomb.AddComponent<SpriteRenderer>();
        bombRenderer.sprite = bombSprite;
        bombRenderer.sortingOrder = 3;
        bomb.transform.position = location;

        StartCoroutine(DestroyBombRoutine(bomb, bombRenderer));
    }

    IEnumerator DestroyBombRoutine(GameObject bomb, SpriteRenderer bombRenderer)
    {
        yield return new WaitForSeconds(timer);        
        bombRenderer.sprite = explodedBombSprite;
        Destroy(bomb, 1);        
    }
     
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }



}

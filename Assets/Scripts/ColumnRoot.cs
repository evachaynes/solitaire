using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColumnRoot : MonoBehaviour
{
    public Sprite rootSprite;
    public int currentColumn;

    // Start is called before the first frame update
    void Start()
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = rootSprite;
        BoxCollider2D collider = GetComponent<BoxCollider2D>();
        collider.size = spriteRenderer.bounds.size;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

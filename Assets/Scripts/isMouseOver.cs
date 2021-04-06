using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class isMouseOver : MonoBehaviour
{
    public bool over = false;
    public Vector2 cursor_point;
    private Collider2D tilemap;
    private Camera cam;

    void Start()
    {
        tilemap = gameObject.GetComponent<Collider2D>();
        cam = Camera.main;
    }

    void Update()
    {
        if (tilemap != null)
        {
            Vector3 mouse = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0f);
            cursor_point = Camera.main.ScreenToWorldPoint(mouse);
            over = tilemap.OverlapPoint(cursor_point);
        }
    }

    void OnMouseOver()
    {
        over = true;
    }

    void OnMouseExit()
    {
        over = false;
    }
}

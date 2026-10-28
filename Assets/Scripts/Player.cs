using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class Player : MonoBehaviour
{
    
    private Vector2 movement;
    private Animator animator;
    private SpriteRenderer sr;

    public float speed;
    
    public Sprite idleUp;
    public Sprite idleDown; 
    public Sprite idleLeft; 
    public Sprite idleRight; 

    public Tilemap floor;
    public Tilemap walls;

    private Boolean isMoving;

    void Start()
    {
        animator = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
        sr.sprite = idleDown;
        transform.position = floor.GetCellCenterWorld(floor.WorldToCell(transform.position));
        transform.position = floor.GetCellCenterWorld(new Vector3Int(0,0,0));
        Debug.Log(floor.WorldToCell(transform.position));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void onMovement(Vector2 input)
    {
        movement = input;
    }

    private void moveToSquare(Vector3Int cellB)
    {
        isMoving = true;
        Vector3 cellBLocation = floor.GetCellCenterWorld(cellB);
        while(Vector3.Distance(transform.position,cellB) > 0.00001f)
        {
            transform.position = Vector3.MoveTowards(transform.position, cellB, speed*Time.deltaTime);
        }
    }

    private bool isWall(Vector3Int cell)
    {
        return true;
    }
}

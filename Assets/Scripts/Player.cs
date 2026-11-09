using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

public class Player : MonoBehaviour
{
    
    private Vector2Int movement;
    private Animator animator;
    private SpriteRenderer sr;

    public float speed;
    public string thisScene;
    public bool hasBombs;
    public Sprite[] explosionAnimation;
    
    public Sprite idleUp;
    public Sprite idleDown; 
    public Sprite idleLeft; 
    public Sprite idleRight; 

    public Tilemap floor;
    public Tilemap walls;
    public Tilemap targets;
    public Tilemap bombs;
    public GameObject box;
    public Light2D globalLight;
    public AudioSource footstepAudio;
    public AudioSource slidingAudio;
    public AudioSource SFXAudio;
    public AudioClip powerOn;
    public AudioClip arming;
    public AudioClip explosion;

    private bool isMoving;
    private bool isBoxMoving;
    private bool endingScene = false;
    private bool bombing = false;


    void Start()
    {
        animator = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
        sr.sprite = idleDown;
        transform.position = floor.GetCellCenterWorld(floor.WorldToCell(transform.position));
        box.transform.position = floor.GetCellCenterWorld(floor.WorldToCell(box.transform.position));
        animator.enabled = false;
        sr.sprite = idleDown;
        footstepAudio.Stop();
    }

    private float idleTime;
    private float boxIdleTime;
    void Update()
    {
        if (endingScene || (hasBombs && bombing))
        {
            return;
        }

        idleTime = isMoving ? 0f : idleTime+Time.deltaTime;
        if (idleTime > 0.02f && footstepAudio.isPlaying)
        {
            footstepAudio.Stop();
        }
        boxIdleTime = isBoxMoving ? 0f : boxIdleTime + Time.deltaTime;
        if(boxIdleTime > 0.02f && slidingAudio.isPlaying)
        {
            slidingAudio.Stop();
        }

        if (hasBombs && bombs.HasTile(floor.WorldToCell(box.transform.position)))
        {
            bombing = true;
            StartCoroutine(Boom(box));
        }

        if (hasBombs && bombs.HasTile(floor.WorldToCell(transform.position)))
        {
            bombing = true;
            StartCoroutine(Boom(gameObject));
        }

        if (targets.HasTile(floor.WorldToCell(box.transform.position)))
        {
            Debug.Log("box on target");
            StartCoroutine(EndLevel());
            endingScene = true;
        }
        if(isMoving == false && isBoxMoving == false &&(movement.x!=0 || movement.y != 0))
        {
            Vector3Int position = floor.WorldToCell(transform.position);
            Vector3Int boxPosition = floor.WorldToCell(box.transform.position);
            Vector3Int newTarget = new Vector3Int(
                position.x+movement.x,
                position.y+(Math.Abs(movement.x)>0 ? 0 : movement.y),
                position.z
            );
            Vector3Int boxTarget = new Vector3Int(
                position.x+movement.x+movement.x,
                position.y+ 2*(Math.Abs(movement.x)>0 ? 0 : movement.y),
                position.z
            );
            if (newTarget == boxPosition)
            {
                if (!walls.HasTile(boxTarget))
                {
                    StartCoroutine(MoveToSquare(newTarget, movement));
                    StartCoroutine(MoveBoxToSquare(boxTarget));
                }
                else
                {
                    return;
                }
            }

            if(!walls.HasTile(newTarget)){ 

                StartCoroutine(MoveToSquare(newTarget, movement));
            }
        
        }
    }

    void OnMovement(InputValue value)
    {
        Vector2 input = value.Get<Vector2>();
        movement = new Vector2Int((int)Math.Round(input.x),(int)Math.Round(input.y));
    }

    void OnResetLevel()
    {
        Debug.Log("Resetting Scene");
        SceneManager.LoadScene(thisScene);
    }

    IEnumerator EndLevel()
    {
        footstepAudio.Stop();
        slidingAudio.Stop();
        yield return new WaitForSeconds(1f);
        globalLight.intensity = 1;
        SFXAudio.PlayOneShot(powerOn);
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(
            thisScene switch
            {
                "Scenes/Level1" => "Scenes/Level2",
                "Scenes/Level2" => "Scenes/Level3",
                _ => "Scenes/End"
            }
        );
    }

    IEnumerator Boom(GameObject explodee)
    {
        footstepAudio.Stop();
        slidingAudio.Stop();
        SFXAudio.PlayOneShot(arming);
        yield return new WaitForSeconds(1f);
        SpriteRenderer esr =  explodee.GetComponent<SpriteRenderer>();
        SFXAudio.PlayOneShot(explosion);
        foreach (Sprite s in explosionAnimation)
        {
            esr.sprite = s;
            yield return null;
        }
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(thisScene);
    }

    IEnumerator MoveBoxToSquare(Vector3Int cellB)
    {
        if(!slidingAudio.isPlaying) slidingAudio.Play();
        isBoxMoving = true;
        Vector3 target = floor.GetCellCenterWorld(cellB);
        while(Vector3.Distance(box.transform.position,target) > 0.00001f)
        {
            box.transform.position = Vector3.MoveTowards(box.transform.position, target, speed*Time.deltaTime);
            yield return null; //goes 1 frame
        }
        isBoxMoving = false;
    }
    IEnumerator MoveToSquare(Vector3Int cellB, Vector2Int initMovement)
    {
        if(!footstepAudio.isPlaying) footstepAudio.Play();
        Vector3 target = floor.GetCellCenterWorld(cellB);
        isMoving = true;
        animator.enabled = true;
        if (initMovement.x == 1)
        {
            animator.SetBool("MoveRight", true);
        }
        else if (initMovement.x == -1)
        {
            animator.SetBool("MoveLeft", true);
        }
        else if (initMovement.y == 1)
        {
            animator.SetBool("MoveUp", true);
        }
        else
        {
            animator.SetBool("MoveDown", true);
        }



        while(Vector3.Distance(transform.position,target) > 0.00001f)
        {
            transform.position = Vector3.MoveTowards(transform.position, target, speed*Time.deltaTime);
            yield return null;
        }


        isMoving = false;
        animator.SetBool("MoveRight", false);
        animator.SetBool("MoveLeft", false);
        animator.SetBool("MoveDown", false);
        animator.SetBool("MoveUp", false);
        animator.enabled = false;
        if (initMovement.x == 1)
        {
            sr.sprite = idleRight;
        }
        else if (initMovement.x == -1)
        {
            sr.sprite = idleLeft;
        }
        else if (initMovement.y == 1)
        {
            sr.sprite = idleUp;
        }
        else
        {
            sr.sprite = idleDown;
        }
    }

}

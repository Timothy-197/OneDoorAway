// this script is for basic move of the player
// the script should be attached to the Player of the player perfab
// 1. move right & left
// 2. jump
// 3. ground detect & gravity: you can change the direction of the gravity, support slope
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicMove : MonoBehaviour
{
    public float jumpSpeed;
    public float MoveSpeed;
    public float MaxSpeed;
    public float gravity;
    public float headBounceSpeed;
    public LayerMask groundLayer;

    //private CharacterController ch;
    private Transform tr;
    private Animator ani;
    private float horizontalInput; // horizontal direction
    private Vector3 horizontalDir; // horizontal direction
    private Transform foot; // the foot position
    private Transform head;
    private Vector3 gravityDir; // gravity direction
    public float hitDistance; // the distance which can be treated as hit
    private float verticalVelo; // vertical velocity (to simulate gravity) -> relative to the World space
    private float horizontalVelo; // horizontal velocity -> relative to the player

    private bool isgrounded;
    private bool shouldJump;
    private bool isJumping;
    private bool justJumped;

    private float jumpSpeedBalance; // a float to balance the jump speed

    public static BasicMove Instance;

    private void Start()
    {
        Instance = this; // player singleton

        //ch = this.GetComponent<CharacterController>();
        tr = this.transform;
        ani = tr.GetChild(0).gameObject.GetComponent<Animator>();
        foot = tr.GetChild(1).gameObject.transform;
        head = tr.GetChild(2).gameObject.transform;
        gravityDir = new Vector3(0, -1f, 0);
        hitDistance = 0.02f;
        isgrounded = false;
        shouldJump = false;
        isJumping = false;
        jumpSpeedBalance = 1f;
        justJumped = false;
    }

    private void Update()
    {
        // horizontal move input
        horizontalInput = Input.GetAxis("Horizontal");
        ani.SetFloat("RunSpeed", Mathf.Abs(horizontalInput));

        // check head hit
        if (isHeadGroundCheck()) verticalVelo = headBounceSpeed; // reset verical velocity

        // check ground hit, simlulate gravity
        isgrounded = isGroundedCheck();
        if (isgrounded) {
            if (justJumped)
            {
                justJumped = false;
                verticalVelo = 0; // clear the vertical velocity when just jumped
            }
            if (verticalVelo > 0) verticalVelo = 0;
            if (horizontalInput != 0) 
                horizontalVelo = MoveSpeed * Time.deltaTime * horizontalInput;
            else horizontalVelo = 0;
            ani.SetBool("JumpUp", false);
            ani.SetBool("JumpDown", false);
        }
        else
        {
            // adjust the player tilt angle
            tr.up = -gravityDir;
            //ch.transform.up = -gravityDir;

            // do not change horiaontal velocity
            // control animations in the air
            ani.SetFloat("RunSpeed", 0);
            if (verticalVelo < MaxSpeed) verticalVelo += gravity * Time.deltaTime;
            else verticalVelo = MaxSpeed;
            // jump animation
            if (verticalVelo < 0) // jump up
            {
                ani.SetBool("JumpDown", false);
                ani.SetBool("JumpUp", true);
            }
            else { // jump down
                if (isJumping)
                {
                    ani.SetBool("UpToDown", true);
                    isJumping = false;
                }
                else {
                    ani.SetBool("UpToDown", false);
                    ani.SetBool("JumpUp", false);
                    ani.SetBool("JumpDown", true);
                }
            }
        }


        // jump input
        if ((Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) && isgrounded) { // jump when on the ground
            shouldJump = true;
        }

        Debug.Log("the horizontal velocity is: " + horizontalVelo);
        Debug.Log("the vertical velocity is: " + verticalVelo);
    }

    private void FixedUpdate()
    {
        // perform jump
        if (shouldJump) { 
            verticalVelo = -jumpSpeed * jumpSpeedBalance;
            shouldJump = false;
            isJumping = true;
            justJumped = true;
        }

        // turn direction
        if (horizontalInput > 0)
        {
            tr.GetChild(0).localEulerAngles = new Vector3(0, 0, 0);
        }
        else if (horizontalInput < 0) {
            tr.GetChild(0).localEulerAngles = new Vector3(0, 180, 0);
        }

        // vertical movement
        //ch.Move(gravityDir * verticalVelo); // move direction is determined by the grivaty direction
        tr.Translate(gravityDir * verticalVelo * Time.deltaTime, Space.World);

        // horizontal move
        horizontalDir = Vector3.Cross(tr.up, tr.forward);
        //ch.Move(horizopntalDir * horizontalVelo);
        tr.Translate(horizontalDir * horizontalVelo * Time.deltaTime, Space.World);
    }

    // the functions:
    // 1. perform isGrounded check / check if head hits the ground
    // 2. adjust the player's transform to the normal of the ground
    private bool isGroundedCheck()
    {
        RaycastHit2D hit = Physics2D.Raycast(foot.position, -tr.up, 1f, groundLayer);
        if (hit.collider != null) {
            if (hit.distance < hitDistance) {
                // ajust noraml to the ground
                //Debug.Log("on ground, distance is: " + hit.distance);
                if (Vector2.Dot(Vector2.up, hit.normal) > (Mathf.Sqrt(2) / 2))
                {
                    tr.up = hit.normal.normalized;
                }
                return true;
            }
        }
        return false;
    }
    private bool isHeadGroundCheck() // check the ground upwards
    {
        RaycastHit2D hit = Physics2D.Raycast(head.position, tr.up, hitDistance, groundLayer);
        if (hit.collider != null)
        {
            return true;
        }
        return false;
    }

    /*
     * Acquire the speed of the player
     * ---------------------------------
     * return: scalar value of the player's velocity
     */
    public float GetSpeed()
    {
        return (Vector3.Cross(tr.up, tr.forward) * horizontalVelo + Vector3.up * verticalVelo).magnitude;
    }

    /*
     * Set new speed for the player
     * ------------------------------------------
     * parameter: (Vector3) the velocity set to the player
     * for this is a 2d game, you can also input 2d vector
     * x: right is positive
     * y: up is positive
     */
    public void SetVelocity(Vector3 newVelo)
    {
        horizontalVelo = newVelo.x;
        verticalVelo = -newVelo.y;
    }
    public void SetVelocity(Vector2 newVelo)
    {
        horizontalVelo = newVelo.x;
        verticalVelo = -newVelo.y;
    }

}

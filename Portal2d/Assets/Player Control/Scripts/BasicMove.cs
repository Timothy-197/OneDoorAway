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
    public LayerMask groundLayer;
    public float gravity;
    
    private CharacterController ch;
    private Transform tr;
    private Animator ani;
    private float horizontalInput; // horizontal direction
    private Vector3 horizopntalDir; // horizontal direction
    private Transform foot; // the foot position
    private Vector3 gravityDir; // gravity direction
    private float hitDistance; // the distance which can be treated as hit
    private float verticalVelo; // vertical velocity (to simulate gravity)

    private bool isgrounded;
    private bool shouldJump;
    private bool isJumping;

    private float jumpSpeedBalance; // a float to balance the jump speed

    private void Start()
    {
        ch = this.GetComponent<CharacterController>();
        tr = this.transform;
        ani = tr.GetChild(0).gameObject.GetComponent<Animator>();
        foot = tr.GetChild(1).gameObject.transform;
        gravityDir = new Vector3(0, -1f, 0);
        hitDistance = 0.3f;
        isgrounded = false;
        shouldJump = false;
        isJumping = false;
        jumpSpeedBalance = 0.05f;
    }

    private void Update()
    {
        // horizontal move input
        horizontalInput = Input.GetAxis("Horizontal");
        ani.SetFloat("RunSpeed", Mathf.Abs(horizontalInput));

        // check ground hit, simlulate gravity
        isgrounded = isGroundedCheck();
        if (isgrounded) {
            ani.SetBool("JumpUp", false);
            ani.SetBool("JumpDown", false);
            verticalVelo = 0; 
        }
        else
        {
            // control animations in the air
            ani.SetFloat("RunSpeed", 0);
            if (verticalVelo < MaxSpeed) verticalVelo += gravity * Time.deltaTime;
            else verticalVelo = MaxSpeed;
            // jump animation
            if (verticalVelo < 0)
            {
                ani.SetBool("JumpDown", false);
                ani.SetBool("JumpUp", true);
            }
            else {
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
        if (Input.GetKeyDown(KeyCode.Space) && isgrounded) { // jump when on the ground
            shouldJump = true;
        }
    }

    private void FixedUpdate()
    {
        // perform jump
        if (shouldJump) { 
            verticalVelo = -jumpSpeed * jumpSpeedBalance;
            shouldJump = false;
            isJumping = true;
        }

        // simulate gravity
        ch.Move(gravityDir * verticalVelo); // move direction is determined by the grivaty direction

        // turn direction
        if (horizontalInput > 0)
        {
            tr.GetChild(0).localEulerAngles = new Vector3(0, 0, 0);
        }
        else if (horizontalInput < 0) {
            tr.GetChild(0).localEulerAngles = new Vector3(0, 180, 0);
        }

        // horizontal move

        horizopntalDir = Vector3.Cross(tr.up, tr.forward) * horizontalInput;
        ch.Move(horizopntalDir*MoveSpeed*Time.deltaTime);
    }

    // the function:
    // 1. perform isGpunded checkk
    // 2. adjust the player's transform to the normal of the ground
    private bool isGroundedCheck()
    {
        RaycastHit hit;
        if (Physics.Raycast(new Ray(foot.position, -tr.up), out hit, 2f, groundLayer)) {
            if (hit.distance < hitDistance) {
                if (Vector3.Dot(Vector3.up, hit.normal) > (Mathf.Sqrt(2) / 2)) {
                    tr.up = hit.normal.normalized;
                    ch.transform.up = hit.normal.normalized;
                }
                //Debug.Log("stay on the ground!");
                return true;
            }
        }
        return false;
    }
}

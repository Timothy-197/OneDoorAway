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
    private Rigidbody2D rb;

    public float jumpSpeed;
    public float MoveSpeed;
    public float jumpBalance;
    public float MaxSpeed;
    public float gravity;
    public float headBounceSpeed;
    public float inAirBalance;
    public float hitDistance; // the distance which can be treated as hit
    [Tooltip("the layer of the ground objects")]
    public LayerMask groundLayer;
    [Tooltip("the layer of the portal objects")]
    public LayerMask objectLayer;
    [Tooltip("the max distance that player can pick up an object")]
    public float overlapR;
    //[Tooltip("max horizontal speed in the air")]
    //public float inAirMaxHoriSpeed;
    //[Tooltip("horizontal speed change in the air")]
    //public float inAirDeltaSpeed;

    //private CharacterController ch;
    private Transform tr;
    private Animator ani;
    private float horizontalInput; // horizontal direction
    private Vector3 horizontalDir; // horizontal direction
    private Transform foot; // the foot position
    private Transform head;
    private Vector3 gravityDir; // gravity direction
    private float verticalVelo; // vertical velocity (to simulate gravity) -> relative to the World space
    private float horizontalVelo; // horizontal velocity -> relative to the player

    private bool isgrounded;
    private bool shouldJump;
    private bool isJumping;
    private bool justJumped; // whether the player is jumping
    //private bool justOnGround;

    private bool isCarrying; // indicates whether the player is carrying an object
    private bool isObjDetected; // whether an object is detected by the player
    private GameObject carryObj; // object to carray

    private float jumpSpeedBalance; // a float to balance the jump speed

    public static BasicMove Instance;

    private void Start()
    {
        Instance = this; // player singleton
        PlayerEvents.current.onEnterPortal += SetToInertia;
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
        isCarrying = false;
        isObjDetected = false;
        //justOnGround = false;
        carryObj = null;

        rb = GetComponent<Rigidbody2D>();
    }

    private void OnDestroy()
    {
        PlayerEvents.current.onEnterPortal -= SetToInertia;
    }

    private void Update()
    {
        // set the normal ajusted to the gravity
        if (gravity >= 0) gravityDir = new Vector3(0, -1f, 0);
        else gravityDir = new Vector3(0, 1f, 0);

        // constrain rigidbody
        if (rb.velocity.magnitude > 0)
            rb.velocity = Vector2.zero;

        // check if there is object to carry + carry the object
        if (isCarrying) {
            carryObj.transform.position = tr.GetChild(3).transform.position;
            carryObj.transform.eulerAngles = new Vector3(0, 0, 0);
        }
        DetectBlock();

        // check if place the object
        if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow)) {
            if (isCarrying)
            { // when carrying, place down the object
                isCarrying = false;
                // release the object
                carryObj.GetComponent<Rigidbody2D>().AddForce(new Vector2(0, 0.2f), ForceMode2D.Impulse);
            }
            else { // if not carrying, pick up the object if an object is detected
                if (isObjDetected) CarryObj();
            }
        }

        // horizontal move input
        horizontalInput = Input.GetAxis("Horizontal");
        ani.SetFloat("RunSpeed", Mathf.Abs(horizontalInput));

        // check head hit
        if (isHeadGroundCheck()) {
            verticalVelo = headBounceSpeed; // reset verical velocity                                                   
        }

        // check ground hit, simlulate gravity
        isgrounded = isGroundedCheck();
        if (isgrounded) {
            //justOnGround = true;
            if (justJumped)
            {
                justJumped = false;
                verticalVelo = 0; // clear the vertical velocity when just jumped
            }
            if (verticalVelo > 0) verticalVelo = 0;
            if (horizontalInput != 0) 
                horizontalVelo = MoveSpeed * horizontalInput;
            else horizontalVelo = 0;
            ani.SetBool("JumpUp", false);
            ani.SetBool("JumpDown", false);
        }
        else
        {
            if (justJumped) { // can only change horizontal while jumping
                horizontalVelo = horizontalInput * jumpBalance * MoveSpeed;
            }

            // adjust the player tilt angle
            tr.up = -gravityDir;

            // control animations in the air
            ani.SetFloat("RunSpeed", 0);
            if (Mathf.Abs(verticalVelo) < MaxSpeed) verticalVelo += gravity * Time.deltaTime;
            else verticalVelo = - MaxSpeed * gravityDir.y;
            // jump animation
            if (verticalVelo * (-gravityDir.y) < 1) // jump up
            {
                ani.SetBool("JumpDown", false);
                if (justJumped) ani.SetBool("JumpUp", true);
                else ani.SetBool("JumpUp", false);
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
    }

    private void FixedUpdate()
    {
        // perform jump
        if (shouldJump) { 
            verticalVelo = jumpSpeed * jumpSpeedBalance * gravityDir.y;
            //horizontalVelo *= jumpBalance;
            shouldJump = false;
            isJumping = true;
            justJumped = true;
        }

        // turn direction
        if (horizontalInput > 0) // child 0: the sprite, child 3: the player hook that carries tools
        {
            tr.GetChild(0).localEulerAngles = new Vector3(0, 0, 0);
            tr.GetChild(3).localPosition = new Vector3(0.6f, -0.1f, 0);
        }
        else if (horizontalInput < 0) {
            tr.GetChild(0).localEulerAngles = new Vector3(0, 180, 0);
            tr.GetChild(3).localPosition = new Vector3(-0.6f, -0.1f, 0);
        }

        // vertical movement
        //ch.Move(gravityDir * verticalVelo); // move direction is determined by the grivaty direction
        tr.Translate(new Vector3(0, -1f, 0) * verticalVelo * Time.fixedDeltaTime, Space.World);

        // horizontal move
        horizontalDir = Vector3.Cross(tr.up, tr.forward);
        //ch.Move(horizopntalDir * horizontalVelo);
        tr.Translate(horizontalDir * horizontalVelo * Time.fixedDeltaTime, Space.World);
    }

    // Ground check functions
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

    // set the player driven by the intertia driven intertia after entering the portal
    private void SetToInertia()
    {
        justJumped = false;
    }

    // movable objects detect function:
    // detect the closest movable objects around and trigger the UI that prompts player to pick
    // return value: the collider of detected object
    private void DetectBlock() 
    {
        var hits = Physics2D.OverlapCircleAll(tr.position, overlapR, objectLayer);
        if (hits.Length == 0) {
            PlayerEvents.current.CarryObject();
            return; 
        }
        else
        {
            //Debug.Log("detect block!");
            if (!isCarrying)
            {
                isObjDetected = true;
                PlayerEvents.current.DetectObject();
            } // detect the object if the player does not carry an object
        }
    }
    public void CarryObj() // call this to pick up the object
    {
        PlayerEvents.current.CarryObject(); // hide ui
        var hits = Physics2D.OverlapCircleAll(tr.position, overlapR, objectLayer);
        int result = 0;
        float minD = Vector2.Distance(tr.position, hits[0].transform.position);
        for (int i = 1; i < hits.Length; i++)
        {
            if (Vector2.Distance(tr.position, hits[i].transform.position) < minD) result = i;
        }
        // set indicators an the carrying object
        isCarrying = true;
        isObjDetected = false;
        carryObj = hits[result].gameObject;
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
    public Vector3 GetSpeed_Vector()
    {
        return (Vector3.Cross(tr.up, tr.forward) * horizontalVelo + Vector3.up * verticalVelo);
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

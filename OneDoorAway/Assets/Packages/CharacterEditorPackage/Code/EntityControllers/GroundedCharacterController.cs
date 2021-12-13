using UnityEngine;
using System.Collections;
//--------------------------------------------------------------------
//GroundedCharacterController is an CharacterControllerBase which implements the core of a platforming character.
//It is the CharacterControllerBase used for all the character controllers
//It provides functionality for jumping and moving (in the air or on the ground).
//Any values it uses can be accessed by AbilityModule implementation in case they need to use or override them.
//
//--------------------------------------------------------------------
public class GroundedCharacterController : CharacterControllerBase
{
    [SerializeField] float m_WalkForce = 0.0f;
    [SerializeField] float m_WalkForceApplyLimit = 0.0f;
    [SerializeField] float m_StoppingForce = 0.0f;
    [SerializeField] bool m_ApplyStoppingForceWhenActivelyBraking = false;
    [SerializeField] float m_AirControl = 0.0f;
    [SerializeField] float m_AirForceApplyLimit = 0.0f;
    [SerializeField] float m_DragConstant = 0.0f;
    [SerializeField] float m_Gravity = 0.0f;
    [SerializeField] bool m_ApplyGravityOnGround = false;
    [SerializeField] bool m_ApplyGravityIntoGroundNormal = false;
    [SerializeField] float m_FrictionConstant = 0.0f;
    [SerializeField] bool m_AlignRotationToGroundedNormal = false;
//Jumping values
    [SerializeField] float m_JumpVelocity = 0.0f;
    [SerializeField] float m_JumpCutVelocity = 0.0f;
    [SerializeField] float m_MinAllowedJumpCutVelocity = 0.0f;
    [SerializeField] float m_GroundedToleranceTime = 0.0f;
    [SerializeField] float m_JumpCacheTime = 0.0f;
    [SerializeField] float m_JumpAlignedToGroundFactor = 0.0f;
    [SerializeField] float m_HorizontalJumpBoostFactor = 0.0f;
    [SerializeField] bool m_ResetVerticalSpeedOnJumpIfMovingDown = false;
    float m_LastJumpPressedTime;
    bool m_JumpInputIsCached;
    bool m_JumpCutPossible;
    float m_LastJumpTime;
    float m_LastGroundedTime;
    float m_LastTouchingSurfaceTime;

    Vector2 m_LastGroundedNormal;
//Jump event (for other scripts to use when the jump is triggered)
    public delegate void OnJumpEvent();
    public event OnJumpEvent OnJump;

    protected ButtonInput m_JumpInput;

    //Called by Unity upon adding a new component to an object, or when Reset is selected in the context menu. Used here to provide default values.
    //Also used when fixing up components using the CharacterFixEditor button
    void Reset()
    {
        m_WalkForce = 90.0f;
        m_WalkForceApplyLimit = 18.0f;
        m_StoppingForce = 100.0f;
        m_ApplyStoppingForceWhenActivelyBraking = true;
        m_AirControl = 0.6f;
        m_AirForceApplyLimit = 18.0f;
        m_DragConstant = 0.0f;
        m_Gravity = 50.0f;
        m_ApplyGravityOnGround = true;
        m_ApplyGravityIntoGroundNormal = true;
        m_FrictionConstant = 8.0f;
        m_AlignRotationToGroundedNormal = false;
        m_JumpVelocity = 32.0f;
        m_JumpCutVelocity = 0.0f;
        m_MinAllowedJumpCutVelocity = 30.0f;
        m_GroundedToleranceTime = 0.1f;
        m_JumpCacheTime = 0.1f;
        m_JumpAlignedToGroundFactor = 0.0f;
        m_HorizontalJumpBoostFactor = 0.0f;
        m_ResetVerticalSpeedOnJumpIfMovingDown = true;
    }

    //This is called every update to update some controller state not directly related to movement
    //Jump input is cached to allow for tolerances (jumping being recognized as valid just before/after touching a jumpable surface
    protected override void UpdateController()
    {
        bool isGrounded = m_ControlledCollider.IsGrounded();
        if (isGrounded)
        {
            m_LastGroundedTime = Time.fixedTime;
            m_LastGroundedNormal = m_ControlledCollider.GetGroundedInfo().GetNormal();
        }

        if (m_ControlledCollider.GetSideCastInfo().m_HasHitSide)
        { 
            m_LastTouchingSurfaceTime = Time.fixedTime;
        }
        if (m_JumpInput != null)
        { 
            if (m_JumpInput.m_WasJustPressed)
            {
                m_JumpInput.m_WasJustPressed = false;
                m_LastJumpPressedTime = Time.fixedTime;
                m_JumpInputIsCached = true;
            }
            //Default jump update (not jumping)
            if (m_JumpInputIsCached)
            {
                //Jump has not been started in time; jump cancelled
                if (Time.fixedTime - m_LastJumpPressedTime >= m_JumpCacheTime)
                {
                    m_JumpInputIsCached = false;
                }
            }
        }
    }

    //Default update, used when no movement abilities are valid
    //Combines input and other forces to update the velocity, then moves the collider using that velocity
    protected override void DefaultUpdateMovement()
    {
        //Jump cut can also be honored by other movement modules, but that is their decision
        UpdateJumpCut();

        if (TryDefaultJump())
        {
            m_ControlledCollider.UpdateWithVelocity(m_ControlledCollider.GetVelocity());
            return;
        }
        Vector2 currentVel = m_ControlledCollider.GetVelocity();
        Vector2 fInput = GetDirectedInputMovement() * GetInputForce();
        fInput = ClampInputVelocity(fInput, currentVel, GetInputForceApplyLimit());

        Vector2 fGravity = GetGravity();

        Vector2 fDrag = -0.5f * (currentVel.sqrMagnitude) * m_DragConstant * currentVel.normalized;

        Vector2 summedF = fInput + fGravity + fDrag;

        Vector2 newVel = currentVel + summedF * Time.fixedDeltaTime;

        if (m_ControlledCollider.IsGrounded())
        {
            newVel += GetStoppingForce(newVel, m_StoppingForce);
            Vector2 friction = GetFriction(newVel, summedF, m_FrictionConstant);
            newVel += friction;
        }

        m_ControlledCollider.UpdateWithVelocity(newVel);
        TryAligningWithGround();
    }
    //Default jump using this controller's jump values.
    public bool TryDefaultJump()
    {
        if (m_JumpInputIsCached)
        {
            //Character was grounded or is grounded; jump occurs
            if ((m_ControlledCollider.IsGrounded() || Time.fixedTime - m_LastGroundedTime <= m_GroundedToleranceTime) && !DidJustJump())
            {
                Vector2 currentVelocity = m_ControlledCollider.GetVelocity();
                if (m_ResetVerticalSpeedOnJumpIfMovingDown)
                {
                    currentVelocity.y = Mathf.Max(0.0f, currentVelocity.y);
                }

                Vector2 jumpDirection = Vector2.Lerp(Vector2.up, m_LastGroundedNormal, m_JumpAlignedToGroundFactor).normalized;
                Vector2 currentWalkDirection = m_ControlledCollider.GetGroundedInfo().GetWalkDirection(currentVelocity);
                float speedDot = Vector2.Dot(currentVelocity, currentWalkDirection.normalized);
                Vector2 jumpVel = m_JumpVelocity * jumpDirection + currentWalkDirection.normalized * speedDot * m_HorizontalJumpBoostFactor;

                Vector2 newVelocity = currentVelocity + jumpVel;
                Jump(newVelocity);
                return true;
            }
        }
        return false;
    }
    //See if jump height has to be cut short when the jump button is released
    public void UpdateJumpCut()
    {
        Vector2 currentVel = m_ControlledCollider.GetVelocity();
        //When below jump cut velocity, disable jump cuts.
        if (currentVel.y <= m_JumpCutVelocity && m_ControlledCollider.GetPreviousVelocity().y > m_JumpCutVelocity)
        {
            m_JumpCutPossible = false;
        }
        //After releasing the jump button, if jump can be cut short, do so
        if (m_JumpCutPossible && !m_JumpInput.m_IsPressed && currentVel.y <= m_MinAllowedJumpCutVelocity)
        {
            m_JumpCutPossible = false;
            if (currentVel.y > m_JumpCutVelocity)
            {
                currentVel.y = m_JumpCutVelocity;
            }
        }
        m_ControlledCollider.SetVelocity(currentVel);
    }
    public void StopJumpCut()
    {
        m_JumpCutPossible = false;
    }

    public void Jump(Vector2 a_Velocity, bool a_OverridePreviousVelocity = true, bool a_AllowLowJumps = true, bool a_ConsumeJumpInput = true)
    {
        if (a_AllowLowJumps)
        {
            m_JumpCutPossible = true;
        }
        if (a_ConsumeJumpInput)
        {
            m_JumpInputIsCached = false;
        }
        m_LastJumpTime = Time.fixedTime;
        LaunchCharacter(a_Velocity, a_OverridePreviousVelocity);
        if (OnJump != null)
        {
            OnJump();
        }
    }
    
    public void LaunchCharacter(Vector2 a_LaunchVelocity, bool a_OverridePreviousVelocity = true)
    {
        Vector2 newVelocity = m_ControlledCollider.GetVelocity();
        if (a_OverridePreviousVelocity)
        {
            newVelocity = Vector2.zero;
        }
        newVelocity += a_LaunchVelocity;
        m_ControlledCollider.SetVelocity(newVelocity);
    }

    public void TryAligningWithGround()
    {
        if (m_AlignRotationToGroundedNormal)
        {
            if (m_ControlledCollider.IsGrounded())
            {
                m_ControlledCollider.RotateToAlignWithNormal(m_ControlledCollider.GetGroundedInfo().GetNormal());
            }
            else
            {
                m_ControlledCollider.RotateToAlignWithNormal(Vector3.up);
            }
        }
        else
        {
            m_ControlledCollider.RotateToAlignWithNormal(Vector3.up);
        }
    }
    //Set player input
    //Set inputs (by PlayerInput)
    public override void SetPlayerInput(PlayerInput a_PlayerInput)
    {
        base.SetPlayerInput(a_PlayerInput);
        if (a_PlayerInput.GetButton("Jump") != null)
        { 
            m_JumpInput = a_PlayerInput.GetButton("Jump");
        }
        else
        {
            Debug.LogError("Jump input not set up in character input");
        }
    }

    public ButtonInput GetJumpInput()
    {
        return m_JumpInput;
    }

    //Get information about current controller state
    public bool DidJustJump()
    {
        return (Time.fixedTime - m_LastJumpTime <= 0.02f + m_GroundedToleranceTime);
    }

    public bool GetJumpIsCached()
    {
        return m_JumpInputIsCached;
    }

    public float GetGroundedToleranceTime()
    {
        return m_GroundedToleranceTime;
    }

    public float GetLastGroundedTime()
    {
        return m_LastGroundedTime;
    }

    public bool WasJustGrounded()
    {
        return (Time.fixedTime - m_LastGroundedTime <= 0.02f);
    }

    public float GetLastTouchingSurfaceTime()
    {
        return m_LastTouchingSurfaceTime;
    }

    public float GetInputForce()
    {
        float modifier = m_WalkForce;
        if (!m_ControlledCollider.IsGrounded())
        {
            modifier *= m_AirControl;
        }
        return modifier;
    }

    public float GetInputForceApplyLimit()
    { 
        if (m_ControlledCollider.IsGrounded())
        {
            return m_WalkForceApplyLimit;
        }
        else
        {
           return m_AirForceApplyLimit;
        }
    }

    public Vector2 GetStoppingForce(Vector2 a_Velocity, float a_StoppingForce)
    {
        Vector2 inputDirection = GetDirectedInputMovement();
        float dot = Vector2.Dot(inputDirection, a_Velocity);
        if (dot > 0 || (!m_ApplyStoppingForceWhenActivelyBraking && inputDirection.magnitude >= 0.05f))
        {
            return Vector2.zero;
        }

        Vector2 direction = -m_ControlledCollider.GetGroundedInfo().GetWalkDirection(a_Velocity); //Opposite direction of velocity

        Vector2 maxForceSpeedChange = direction * a_StoppingForce * Time.fixedDeltaTime;

        Vector2 velInDirection = Mathf.Abs(Vector2.Dot(a_Velocity, direction)) * direction;

        if (velInDirection.magnitude > maxForceSpeedChange.magnitude)
        {
            return maxForceSpeedChange;
        }
        else
        {
            return velInDirection;
        }
    }

    public Vector2 GetFriction(Vector2 a_Velocity, Vector2 a_CurrentForce, float a_FrictionConstant)
    {
        if (m_ControlledCollider.IsGrounded())
        {
            CGroundedInfo groundedInfo = m_ControlledCollider.GetGroundedInfo();
            
            Vector2 direction = -groundedInfo.GetWalkDirection(a_Velocity); //Opposite direction of velocity
            Vector2 maxFrictionSpeedChange = direction * a_FrictionConstant * Time.fixedDeltaTime;

            Vector2 velInDirection = Mathf.Abs(Vector2.Dot(a_Velocity, direction)) * direction;
            if (velInDirection.magnitude > maxFrictionSpeedChange.magnitude)
            {
                return maxFrictionSpeedChange;
            }
            else
            {
                return velInDirection;
            }
        }
        return Vector2.zero;
    }

    public Vector2 ClampInputVelocity(Vector2 a_InputMovement, Vector2 a_Velocity, float a_Limit)
    {
        float dot = Vector2.Dot(a_InputMovement.normalized, a_Velocity);
        if (dot > a_Limit)
        {
            a_InputMovement = Vector2.zero;
        }
        return a_InputMovement;
    }

    public Vector2 GetDirectedInputMovement()
    {
        CGroundedInfo groundedInfo = m_ControlledCollider.GetGroundedInfo();
        if (groundedInfo.m_IsGrounded)
        {
            Vector2 input = groundedInfo.GetWalkDirection(new Vector2(transform.right.x, transform.right.y) * GetInputMovement().x);
            return Vector2.ClampMagnitude(input, Vector2.Dot(input.normalized, GetInputMovement()));
        }
        return new Vector2(GetInputMovement().x, 0);
    }

    //Get constants of this controller
    public float GetWalkForce()
    {
        return m_WalkForce;
    }
    public float GetStoppingForceConstant()
    {
        return m_StoppingForce;
    }
    public float GetDragConstant()
    {
        return m_DragConstant;
    }
    public Vector2 GetGravity()
    {
        Vector2 fGravity = Vector2.down * m_Gravity;
        if (m_ApplyGravityIntoGroundNormal)
        {
            fGravity = -m_ControlledCollider.GetGroundedInfo().GetNormal() * m_Gravity;
        }
        if (m_ControlledCollider.IsGrounded() && !m_ApplyGravityOnGround)
        {
            fGravity = Vector2.zero;
        }
        return fGravity;
    }
    public float GetFrictionConstant()
    {
        return m_FrictionConstant;
    }

    public bool GetAlignsToGround()
    {
        return m_AlignRotationToGroundedNormal;
    }

    public float GetJumpVelocity()
    {
        return m_JumpVelocity;
    }

    protected override string GetCurrentSpriteStateForDefault()
    {
        if (m_ControlledCollider.IsGrounded())
        {
            if (Mathf.Abs(GetDirectedInputMovement().x) >= 0.05f)
            {
                return "Run";
            }
            else
            {
                if (m_ControlledCollider.GetGroundedInfo().IsDangling())
                {
                    return "Dangling";
                }
                else
                { 
                    return "Idle";
                }
            }
        }
        else
        {
            if (m_ControlledCollider.GetVelocity().y > 0)
            {
                if (DidJustJump())
                { 
                    if (Mathf.Abs(m_ControlledCollider.GetVelocity().x) < 0.0001f)
                    {
                        return "JumpStraight";
                    }
                    else
                    {
                        return "JumpSide";
                    }
                }
                else
                {
                    if (Mathf.Abs(m_ControlledCollider.GetVelocity().x) < 0.0001f)
                    {
                        return "FallUpStraight";
                    }
                    else
                    {
                        return "FallUpSide";
                    }
                }
            }
            else
            {
                if (Mathf.Abs(m_ControlledCollider.GetVelocity().x) < 0.0001f)
                {
                    return "FallStraight";
                }
                else
                {
                    return "FallSide"; 
                }
            }
        }
    }
}

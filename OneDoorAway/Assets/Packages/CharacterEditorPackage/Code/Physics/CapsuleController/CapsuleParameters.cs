using UnityEngine;
using System.Collections;
//Secondary class definition for ControlledCapsuleCollider. The reason for this source file is to keep the capsule parameters (and their accessors) separate from the main file. This prevents bloat.
//--------------------------------------------------------------------
//ControlledCapsuleCollider is a class which stores information about the capsule shape, transform and state.
//This includes context information (via CCState) and collision information
//--------------------------------------------------------------------

partial class ControlledCapsuleCollider
{
    [Header("General capsule settings")]
    [Tooltip("Minimum velocity to still try to move with")]
    [SerializeField]
    float m_MinimumViableVelocity = 0.0f;
    [Tooltip("The solver for moving colliders")]
    [SerializeField]
    CapsuleMovingColliderSolver m_CapsuleMovingColliderSolver = null;

    [Tooltip("Length of the capsule when starting, length is from center of upper hemisphere to center of lower hemisphere, not including radius")]
    [SerializeField]
    float m_Length = 0.0f;
    //Since this is a non-live value, use a previous value to see if the capsule size needs to be updated.
    float m_PrevLength;
    [Tooltip("Radius of the collider")]
    [SerializeField]
    float m_Radius = 0.0f;

    //[Header("Checks for allowing movement states")]
    [Tooltip("The maximum angle a character can still walk on (from straight up)")]
    [SerializeField]
    float m_MaximumGroundedAngle = 0.0f;
    [Tooltip("The maximum angle a character can still align to a wall with (from straight up)")]
    [SerializeField]
    float m_MaximumWallAngle = 0.0f;
    [Tooltip("The maximum angle a character can still grab onto edge with (from straight up)")]
    [SerializeField]
    float m_MaxGrabAngle = 0.0f;
    [Tooltip("The maximum angle a character can align with when hanging onto edge (from straight up)")]
    [SerializeField]
    float m_MaxEdgeAlignAngle = 0.0f;

    [Header("Distances for casting")]
    [Tooltip("Distance the grounded check will check for surfaces that could ground the collider")]
    [SerializeField]
    float m_GroundedCheckDistance = 0.0f;
    [Tooltip("Distance the sidecast will check for hitting any surface on the side")]
    [SerializeField]
    float m_SideCastDistance = 0.0f;
    [Tooltip("Distance the wallcasts will check for surfaces that would be walls")]
    [SerializeField]
    float m_WallCastDistance = 0.0f;
    [Tooltip("Distance the edgecast will start horizontally from the collider to check for colliders to grab")]
    [SerializeField]
    float m_EdgeCastHorizontalDistance = 0.0f;
    [Tooltip("Distance the edgecast will go down to check for colliders to grab")]
    [SerializeField]
    float m_EdgeCastVerticalDistance = 0.0f;
    [Tooltip("Distance the controller will cast to check for surface to align with")]
    [SerializeField]
    float m_EdgeAlignProbeDistance = 0.0f;

    [Header("Margins for casting")]
    [Tooltip("Margin the capsule moves back before casting in movement direction (prevents missing colliders already touching this)")]
    [SerializeField]
    float m_MovementCapsuleCastMargin = 0.0f;
    [Tooltip("Margin for the grounded check to start higher (prevents missing colliders already touching this)")]
    [SerializeField]
    float m_GroundedMargin = 0.0f;
    [Tooltip("Margin the capsule radius shrinks by when casting it (prevents zero distance collisions)")]
    [SerializeField]
    float m_SideCastMargin = 0.0f;
    [Tooltip("Margin the wallcasts start from within the collider (prevents missing colliders already touching this)")]
    [SerializeField]
    float m_WallCastMargin = 0.0f;
    [Tooltip("Margin the edgecasts starts up from the cast point (prevents missing colliders already touching the grab point)")]
    [SerializeField]
    float m_EdgeCastVerticalMargin = 0.0f;

    [Header("Margins for collider shape")]
    [Tooltip("Margin to shrink the capsulecast by when checking if rotation is possible")]
    [SerializeField]
    float m_RotateCastMargin = 0.0f;
    [Tooltip("Margin to shrink the capsulecast by when checking if resizing is possible")]
    [SerializeField]
    float m_ResizeCastMargin = 0.0f;
    [Tooltip("Margin to shrink the capsulecast by when checking if moving is possible (for non-essential movement)")]
    [SerializeField]
    float m_OptionalMoveCastMargin = 0.0f;


    //Called by Unity upon adding a new component to an object, or when Reset is selected in the context menu. Used here to provide default values.
    //Also used when fixing up components using the CharacterFixEditor button
    void Reset()
    {
        if (gameObject.layer == LayerMask.NameToLayer("Default"))
        {
            gameObject.layer = 31;
        }
        //if ((1 << gameObject.layer & m_LayerMask.value) != 0)
        {
            m_LayerMask = ~(1 << gameObject.layer);
        }
        m_MinimumViableVelocity = 0.01f;
        m_Length = 1.5f;
        m_Radius = 0.25f;

        m_MaximumGroundedAngle = 45.5f;
        m_MaximumWallAngle = 105.5f;
        m_MaxGrabAngle = 45.5f;
        m_MaxEdgeAlignAngle = 105.5f;

        m_GroundedCheckDistance = 0.05f;
        m_SideCastDistance = 0.05f;
        m_WallCastDistance = 0.05f;
        m_EdgeCastHorizontalDistance = 0.07f;
        m_EdgeCastVerticalDistance = 0.4f;
        m_EdgeAlignProbeDistance = 1.0f;

        m_MovementCapsuleCastMargin = 0.5f;
        m_GroundedMargin = 0.05f;
        m_SideCastMargin = 0.05f;
        m_WallCastMargin = 0.25f;
        m_EdgeCastVerticalMargin = 0.05f;

        m_RotateCastMargin = 0.02f;
        m_ResizeCastMargin = 0.02f;
        m_OptionalMoveCastMargin = 0.04f; 
    }

    //Get capsule information 
    public float GetDefaultLength()
    {
        return m_Length;
    }
    public float GetRadius()
    {
        return m_Radius;
    }

    public float GetMaxGroundedAngle()
    {
        return m_MaximumGroundedAngle;
    }
    public float GetMaxWallAngle()
    {
        return m_MaximumWallAngle;
    }
    public float GetMaxGrabAngle()
    {
        return m_MaxGrabAngle;
    }
    public float GetMaxEdgeAlignAngle()
    {
        return m_MaxEdgeAlignAngle;
    }

    public float GetWallCastDistance()
    {
        return m_WallCastDistance;
    }

    public float GetEdgeCastHorizontalDistance()
    {
        return m_EdgeCastHorizontalDistance;
    }
    public float GetEdgeCastVerticalDistance()
    {
        return m_EdgeCastVerticalDistance;
    }
    public float GetEdgeAlignProbeDistance()
    {
        return m_EdgeAlignProbeDistance;
    }

    public float GetMovementCapsuleCastMargin()
    {
        return m_MovementCapsuleCastMargin;
    }
    public float GetGroundedMargin()
    {
        return m_GroundedMargin;
    }
    public float GetSideCastMargin()
    {
        return m_SideCastMargin;
    }
    public float GetWallCastMargin()
    {
        return m_WallCastMargin;
    }
    public float GetEdgeCastVerticalMargin()
    {
        return m_EdgeCastVerticalMargin;
    }

    public float GetRotateCastMargin()
    {
        return m_RotateCastMargin;
    }
    public float GetResizeCastMargin()
    {
        return m_ResizeCastMargin;
    }
    public float GetOptionalMoveCastMargin()
    {
        return m_OptionalMoveCastMargin;
    }
}

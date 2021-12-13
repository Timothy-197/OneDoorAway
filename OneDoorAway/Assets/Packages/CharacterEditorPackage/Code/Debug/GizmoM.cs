using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;
using System.Collections.Generic;
//--------------------------------------------------------------------
//GizmoM is a custom debugging tool which uses Unity's OnGizmo function to draw lines and basic geometry.
//Advantage of using this over OnGizmo is that gizmos can be created for a certain time more easily
//--------------------------------------------------------------------
public class GizmoM : MonoBehaviour 
{
#if UNITY_EDITOR
    public class GizmoRequest
    {
        public enum Type
        {
            Line,
            Box,
            Sphere,
            Capsule
        }
        public Type m_Type;
        public Vector3 m_StartPosition;
        public Vector3 m_EndPosition;
        public Vector3 m_Size;
        public Quaternion m_Rotation;
        public float m_Radius;
        public float m_Length;
        public bool m_Solid;
        public Color m_Color;
        public float m_StartTime;
        public float m_TimeOut;
    }
    List<GizmoRequest> m_GizmoRequests = new List<GizmoRequest>();
    List<GizmoRequest> m_OldGizmoRequests = new List<GizmoRequest>();
    bool m_GizmosAreActive;
    float m_LastGizmoActiveTime;
    static GizmoM g_GizmoM;
    static GizmoM Get()
    {
        if (g_GizmoM == null)
        {
            g_GizmoM = Object.FindObjectOfType<GizmoM>();
        }
        return g_GizmoM;
    }
#endif
	public static void AddLine(Vector3 a_StartPosition, Vector3 a_EndPosition, Color a_Color, float a_TimeOut = 0)
    {
#if UNITY_EDITOR
        if (!Get())
        {
            return;
        }
        if (!Get().m_GizmosAreActive)
        {
            return;
        }
        GizmoRequest t_NewRequest = new GizmoRequest();
        t_NewRequest.m_Type = GizmoRequest.Type.Line;
        t_NewRequest.m_StartPosition = a_StartPosition;
        t_NewRequest.m_EndPosition = a_EndPosition;
        t_NewRequest.m_Color = a_Color;
        t_NewRequest.m_StartTime = Time.time;
        t_NewRequest.m_TimeOut = a_TimeOut;       
        Get().m_GizmoRequests.Add(t_NewRequest);
#endif
    }

	public static void AddBox(Vector3 a_Position, Quaternion a_Rotation, Vector3 a_Size, Color a_Color, bool a_IsSolid = false, float a_TimeOut = 0)
    {
#if UNITY_EDITOR
        if (!Get())
        {
            return;
        }
        if (!Get().m_GizmosAreActive)
        {
            return;
        }
        GizmoRequest t_NewRequest = new GizmoRequest();
        t_NewRequest.m_Type = GizmoRequest.Type.Box;
        t_NewRequest.m_StartPosition = a_Position;
        t_NewRequest.m_Rotation = a_Rotation;
        t_NewRequest.m_Size = a_Size;
        t_NewRequest.m_Color = a_Color;
        t_NewRequest.m_Solid = a_IsSolid;
        t_NewRequest.m_StartTime = Time.time;
        t_NewRequest.m_TimeOut = a_TimeOut;
        Get().m_GizmoRequests.Add(t_NewRequest);
#endif
    }

	public static void AddSphere(Vector3 a_Position, float a_Radius, Color a_Color, bool a_IsSolid = false, float a_TimeOut = 0)
    {
#if UNITY_EDITOR
        if (!Get())
        {
            return;
        }
        if (!Get().m_GizmosAreActive)
        {
            return;
        }
        GizmoRequest t_NewRequest = new GizmoRequest();
        t_NewRequest.m_Type = GizmoRequest.Type.Sphere;
        t_NewRequest.m_StartPosition = a_Position;
        t_NewRequest.m_Radius = a_Radius;
        t_NewRequest.m_Color = a_Color;
        t_NewRequest.m_Solid = a_IsSolid;
        t_NewRequest.m_StartTime = Time.time;
        t_NewRequest.m_TimeOut = a_TimeOut;
        Get().m_GizmoRequests.Add(t_NewRequest);
#endif
    }

	public static void AddCapsule(Vector3 a_Position, Quaternion a_Rotation, float a_Radius, float a_Length, Color a_Color, bool a_IsSolid = false, float a_TimeOut = 0)
    {
#if UNITY_EDITOR
        if (!Get())
        {
            return;
        }
        if (!Get().m_GizmosAreActive)
        {
            return;
        }
        GizmoRequest t_NewRequest = new GizmoRequest();
        t_NewRequest.m_Type = GizmoRequest.Type.Capsule;
        t_NewRequest.m_StartPosition = a_Position;
        t_NewRequest.m_Rotation = a_Rotation;
        t_NewRequest.m_Radius = a_Radius;
        t_NewRequest.m_Length = a_Length;
        t_NewRequest.m_Color = a_Color;
        t_NewRequest.m_StartTime = Time.time;
        t_NewRequest.m_TimeOut = a_TimeOut;
        Get().m_GizmoRequests.Add(t_NewRequest);
#endif
    }

    void Update()//If gizmos are not active, return;
    {
#if UNITY_EDITOR
        if (Time.time - m_LastGizmoActiveTime >= 1.0f)
        {
            m_GizmoRequests.Clear();
            m_GizmosAreActive = false;
        }
#endif
    }

    void FixedUpdate()
    {
#if UNITY_EDITOR
        if (!EditorApplication.isPaused)
        {
            m_OldGizmoRequests = new List<GizmoRequest>(m_GizmoRequests);

            for (int i = 0; i < m_GizmoRequests.Count;)
            {
				if (m_GizmoRequests[i].m_TimeOut == 0)
				{
					m_GizmoRequests.RemoveAt(i);
				}
				else if (m_GizmoRequests[i].m_TimeOut > 0 && Time.time - m_GizmoRequests[i].m_StartTime >= m_GizmoRequests[i].m_TimeOut)
                {
                    m_GizmoRequests.RemoveAt(i);
                }
                else
                {
                    i++;
                }
            }
        }
#endif
    }

    void OnDrawGizmos()
    {
#if UNITY_EDITOR
        m_GizmosAreActive = true;
        m_LastGizmoActiveTime = Time.time;
        List<GizmoRequest> t_Requests = m_GizmoRequests;
        if (EditorApplication.isPaused)
        {
            t_Requests = m_OldGizmoRequests;
        }
        
		for (int i = 0; i < t_Requests.Count; i++)
        {
            Gizmos.color = t_Requests[i].m_Color;
            switch (t_Requests[i].m_Type)
            {
                case GizmoRequest.Type.Line:
                    DrawLine(t_Requests[i]);
                break;
                case GizmoRequest.Type.Box:
                    DrawBox(t_Requests[i]);
                break;
                case GizmoRequest.Type.Sphere:
                    DrawSphere(t_Requests[i]);
                break;
                case GizmoRequest.Type.Capsule:
                    DrawCapsule(t_Requests[i]);
                break;
            }
        }
        if (!Application.isPlaying)
        {
            m_GizmoRequests.Clear();
        }
#endif
    }

#if UNITY_EDITOR
    void DrawLine(GizmoRequest a_Request)
    {
        Gizmos.DrawLine(a_Request.m_StartPosition, a_Request.m_EndPosition);
    }

    void DrawBox(GizmoRequest a_Request)
    {
        Gizmos.matrix = Matrix4x4.TRS(a_Request.m_StartPosition, a_Request.m_Rotation, Vector3.one);
        if (a_Request.m_Solid)
        { 
            Gizmos.DrawCube(Vector3.zero, a_Request.m_Size);
        }
        else
        {
            Gizmos.DrawWireCube(Vector3.zero, a_Request.m_Size);
        }
        Gizmos.matrix = Matrix4x4.identity;
    }

    void DrawSphere(GizmoRequest a_Request)
    {
        if (a_Request.m_Solid)
        {
            Gizmos.DrawSphere(a_Request.m_StartPosition, a_Request.m_Radius);
        }
        else
        {
            Gizmos.DrawWireSphere(a_Request.m_StartPosition, a_Request.m_Radius);
        }
    }
    
    void DrawCapsule(GizmoRequest a_Request)
    {
        Gizmos.matrix = Matrix4x4.TRS(a_Request.m_StartPosition, a_Request.m_Rotation, Vector3.one);
        Vector3 m_XSide = Vector3.right * a_Request.m_Radius;
        Vector3 m_ZSide = Vector3.forward * a_Request.m_Radius;
        Vector3 m_YLength = Vector3.up * a_Request.m_Length * 0.5f;
        Gizmos.DrawWireSphere(   m_YLength, a_Request.m_Radius);
        Gizmos.DrawWireSphere( - m_YLength, a_Request.m_Radius);

        Gizmos.DrawLine(  m_XSide + m_YLength,   m_XSide - m_YLength);
        Gizmos.DrawLine(- m_XSide + m_YLength, - m_XSide - m_YLength);
        Gizmos.DrawLine(  m_ZSide + m_YLength,   m_ZSide - m_YLength);
        Gizmos.DrawLine(- m_ZSide + m_YLength, - m_ZSide - m_YLength);
        Gizmos.matrix = Matrix4x4.identity;
    }
#endif
}

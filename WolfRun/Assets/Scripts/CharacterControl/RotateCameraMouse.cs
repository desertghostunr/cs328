using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateCameraMouse : MonoBehaviour
{

    public float sensitivity = 2.0f;

    public float MinimumX = -80.0f;
    public float MaximumX = 80.0f;

    private Camera m_camera;
    private Quaternion m_targetRot;

    // Use this for initialization
    void Start ()
    {
        m_camera = GetComponent<Camera>( );
        m_targetRot = m_camera.transform.localRotation;
    }
	
	// Update is called once per frame
	void Update ()
    {
        m_targetRot *= Quaternion.Euler( -sensitivity * Input.GetAxis( "Mouse Y" ), 0, 0 );

        m_targetRot = ClampRotationAroundXAxis( m_targetRot );

        m_camera.transform.localRotation = m_targetRot;        
    }

    //from MouseLook.cs
    Quaternion ClampRotationAroundXAxis( Quaternion q )
    {
        q.x /= q.w;
        q.y /= q.w;
        q.z /= q.w;
        q.w = 1.0f;

        float angleX = 2.0f * Mathf.Rad2Deg * Mathf.Atan (q.x);

        angleX = Mathf.Clamp( angleX, MinimumX, MaximumX );

        q.x = Mathf.Tan( 0.5f * Mathf.Deg2Rad * angleX );

        return q;
    }
}

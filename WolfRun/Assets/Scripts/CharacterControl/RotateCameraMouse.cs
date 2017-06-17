using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateCameraMouse : MonoBehaviour
{

    public float sensitivity = 2.0f;

    public float MinimumX = -80.0f;
    public float MaximumX = 80.0f;

    public bool useMouseLook = true;

    public bool rotateAlongX = false;

    public Transform player;

    public Vector3 m_cameraPointingOffset = Vector3.zero;

    private Camera m_camera;
    private Quaternion m_targetRot;

    private Vector3 m_localFollowPos;

    // Use this for initialization
    void Start ()
    {
        m_camera = GetComponentInChildren<Camera>( );
        m_targetRot = m_camera.transform.localRotation;

        m_localFollowPos = transform.localPosition;
    }
	

	void FixedUpdate ()
    {
        if( useMouseLook )
        {
            m_targetRot *= Quaternion.Euler( -sensitivity * Input.GetAxis( "Mouse Y" ), 0, 0 );

            m_targetRot = ClampRotationAroundXAxis( m_targetRot );

            m_camera.transform.localRotation = m_targetRot;
        }

        if( rotateAlongX )
        {
            RotateAroundX( );
        }
        else if( Vector3.Distance( transform.localPosition, m_localFollowPos ) > 0.1f )
        {
            transform.localPosition = new Vector3( Mathf.Lerp( transform.localPosition.x,
                                                               m_localFollowPos.x,
                                                               6 * Time.fixedDeltaTime ),
                                                   Mathf.Lerp( transform.localPosition.y,
                                                               m_localFollowPos.y,
                                                               6 * Time.fixedDeltaTime ),
                                                   Mathf.Lerp( transform.localPosition.z,
                                                               m_localFollowPos.z,
                                                               6 * Time.fixedDeltaTime ) );

            transform.LookAt( player, Vector3.up );

        }
        else if ( transform.localPosition != m_localFollowPos )
        {
            transform.localPosition = m_localFollowPos;
        }
    }

    public void RotateAroundX( )
    {
        Vector3 offset = Quaternion.AngleAxis( Input.GetAxis( "Mouse X") * sensitivity, Vector3.up ) * transform.localPosition;

        transform.localPosition = offset;

        transform.LookAt( player.position );
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

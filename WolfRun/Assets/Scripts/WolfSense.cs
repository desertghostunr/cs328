using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WolfSense : MonoBehaviour
{
    private Camera m_camera;
    private bool senseReady = true;

	// Use this for initialization
	void Start ()
    {
        m_camera = GetComponentInChildren<Camera>( );
        m_camera.gameObject.SetActive( false );
	}
	
	// Update is called once per frame
	void Update ()
    {
		if( Input.GetKey( KeyCode.F ) && senseReady )
        {

            StartCoroutine( StartSense( ) );
        }
	}

    IEnumerator StartSense( )
    {
        m_camera.gameObject.SetActive( true );
        senseReady = false;

        yield return new WaitForSeconds( 15.0f );

        m_camera.gameObject.SetActive( false );

        yield return new WaitForSeconds( 30.0f );

        senseReady = true;
    }
    
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WolfSense : MonoBehaviour
{
    private Camera m_camera;
    private bool senseReady = true;

    private GameObject[] smell;
    
	void Start ()
    {
        int index;

        m_camera = GetComponentInChildren<Camera>( );
        m_camera.gameObject.SetActive( false );

        smell = GameObject.FindGameObjectsWithTag( "Scent" );

        for ( index = 0; index < smell.Length; index++ )
        {
            smell[index].SetActive( false );
        }
    }

    public bool SenseOn( )
    {
        return !senseReady;
    }

    public void ActivateSense( )
    {
        if( senseReady )
        {
            StartCoroutine( StartSense( ) );
        }
    }

    IEnumerator StartSense( )
    {
        int index = 0;

        for( index = 0; index < smell.Length; index++ )
        {
            smell[index].SetActive( true );
        }

        m_camera.gameObject.SetActive( true );
        senseReady = false;

        yield return new WaitForSeconds( 15.0f );

        m_camera.gameObject.SetActive( false );

        yield return new WaitForSeconds( 15.0f );

        senseReady = true;
    }
    
}

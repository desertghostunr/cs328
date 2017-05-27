using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WolfSense : MonoBehaviour
{
    private Camera m_camera = null;
    private bool senseReady = true;
    private bool senseOn = false;

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
        return senseOn;
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
        int dIndex = 0;
        int index = 0;

        if(m_camera )
        {
            m_camera.gameObject.SetActive( true );
        }
        senseOn = true;
        
        senseReady = false;

        for( dIndex = 0; dIndex < 15; dIndex++ )
        {
            for ( index = 0; index < smell.Length; index++ )
            {
                smell[index].SetActive( true );
            }

            yield return new WaitForSeconds( 1.0f );
        }

        if ( m_camera )
        {
            m_camera.gameObject.SetActive( false );
        }

        senseOn = false;

        yield return new WaitForSeconds( 10.0f );

        senseReady = true;
    }
    
}

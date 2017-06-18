using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WolfSense : MonoBehaviour
{
    private Camera m_camera = null;
    private bool senseReady = true;
    private bool senseOn = false;

    public float senseCharge = 15.0f;

    private GameObject[] smell;


    private float m_chargeMax;

	void Start ()
    {
        int index;

        m_camera = GetComponentInChildren<Camera>( );

        if( m_camera )
        {
            m_camera.gameObject.SetActive( false );
        }
        

        smell = GameObject.FindGameObjectsWithTag( "Scent" );

        for ( index = 0; index < smell.Length; index++ )
        {
            smell[index].SetActive( false );
        }

        m_chargeMax = senseCharge;
    }

    public bool SenseOn( )
    {
        return senseOn;
    }

    public bool ActivateSense( )
    {
        if( senseReady )
        {
            StartCoroutine( StartSense( ) );
        }

        return senseReady;
    }

    public void DeactivateSwitch( )
    {
        senseOn = false;
    }

    IEnumerator StartSense( )
    {
        int index = 0;

        if(m_camera )
        {
            m_camera.gameObject.SetActive( true );
        }

        senseOn = true;
        
        senseReady = false;

        while( senseOn && senseCharge > 0.0f )
        {
            for ( index = 0; index < smell.Length; index++ )
            {
                smell[index].SetActive( true );
            }

            yield return new WaitForSeconds( 1.0f );

            senseCharge -= 1.0f;
        }

        if ( m_camera )
        {
            m_camera.gameObject.SetActive( false );
        }

        senseOn = false;

        for ( index = 0; index < smell.Length; index++ )
        {
            smell[index].SetActive( false );
        }

        StartCoroutine( RechargeSense( ) );

        yield return new WaitForSeconds( 10.0f );

        senseReady = true;
    }

    IEnumerator RechargeSense( )
    {
        while( !senseOn && senseCharge < m_chargeMax )
        {
            yield return new WaitForSeconds( 0.333f );

            senseCharge += 0.333f;
        }
    }
    
}

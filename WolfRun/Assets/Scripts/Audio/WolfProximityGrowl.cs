using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(1000)]
public class WolfProximityGrowl : MonoBehaviour
{
    public string targetTag = "Boy";

    public float growlDistance = 25.0f;

    public AudioClip audioClip;

    private AudioSource m_audioSource;
    private Transform m_target;

    // Use this for initialization
    void Start ()
    {
        m_audioSource = GetComponent<AudioSource>( );

        m_target = GameObject.FindGameObjectWithTag( targetTag ).transform;

        StartCoroutine( GrowlCoroutine( ) );
	}

    IEnumerator GrowlCoroutine( )
    {
        yield return new WaitUntil( ( ) => ( Vector3.Distance( m_target.position, transform.position ) <= growlDistance ) );
        
        m_audioSource.PlayOneShot( audioClip, 0.85f );

        StartCoroutine( DelayGrowlCoroutine( ) );
    }

    IEnumerator DelayGrowlCoroutine( )
    {
        yield return new WaitUntil( ( ) => ( Vector3.Distance( m_target.position, transform.position ) >= 1.5f * growlDistance ) );

        StartCoroutine( GrowlCoroutine( ) );
    }
}

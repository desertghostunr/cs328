using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent( typeof( AudioSource ) )]
public class WolfVictory : MonoBehaviour
{

    public AudioClip biteSound;    
    private AudioSource wAudio;
    private GameManager gameManager;

    private UniversalCharacterController m_universalCC;

    private SimpleNPCHunter m_simpleHunter;
    // Use this for initialization
    void Start ()
    {
        gameManager = FindObjectOfType<GameManager>( );
        wAudio = GetComponent<AudioSource>( );
        m_universalCC = GetComponent<UniversalCharacterController>( );
        m_simpleHunter = GetComponent<SimpleNPCHunter>( );
    }

    private void OnTriggerEnter( Collider other )
    {
        if ( other.tag == "Boy" )
        {
            gameManager.ToggleBlackScreen( );

            wAudio.volume = 1.0f;
            wAudio.clip = biteSound;
            wAudio.Play( );

            gameManager.SetWinner( name );

            m_universalCC.SetCanMove( false );

            if( m_simpleHunter )
            {
                m_simpleHunter.active = false;
            }

            other.GetComponent<UniversalCharacterController>( ).SetCanMove( false );
        }
    }
}

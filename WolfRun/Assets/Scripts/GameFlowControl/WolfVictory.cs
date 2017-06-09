using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent( typeof( AudioSource ) )]
public class WolfVictory : MonoBehaviour
{

    public AudioClip biteSound;    
    private AudioSource wAudio;
    private GameManager gameManager;

    // Use this for initialization
    void Start ()
    {
        gameManager = FindObjectOfType<GameManager>( );
        wAudio = GetComponent<AudioSource>( );
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
        }
    }
}

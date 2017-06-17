using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinScript : MonoBehaviour {

    private GameManager gm;

    private GameObject m_wolf;

    private PlayerCharacterController m_playerCC;

    private void Start( )
    {
        gm = FindObjectOfType<GameManager>( );
    }

    private void OnTriggerEnter( Collider other )
    {
        
        if (other.tag == "End" )
        {
            gm.SetWinner( name );

            m_wolf = GameObject.Find( "Wolf" );

            if( m_wolf )
            {
                m_wolf.GetComponent<UniversalCharacterController>( ).SetCanMove( false );
            }

            m_playerCC = GetComponent<PlayerCharacterController>( );

            if( m_playerCC )
            {
                m_playerCC.SetTurnWithMouse( false );
            }
        }
    }
}

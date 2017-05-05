using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinScript : MonoBehaviour {

    private GameManager gm;

    private void Start( )
    {
        gm = FindObjectOfType<GameManager>( );
    }

    private void OnTriggerEnter( Collider other )
    {
        if( other.tag == "End" )
        {
            gm.SetWinner( name );
        }
    }
}

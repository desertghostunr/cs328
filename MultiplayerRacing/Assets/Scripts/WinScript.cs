using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinScript : MonoBehaviour {

    public GameManager gm;

    private void OnTriggerEnter( Collider other )
    {
        bool winner = false;
        if( other.tag == "GameManager" )
        {
            winner = gm.SetWinner( name );
        }

        if( winner && GetComponent<Money>( ) )
        {
            GetComponent<Money>( ).AddMoney( 10 );
        }
    }
}

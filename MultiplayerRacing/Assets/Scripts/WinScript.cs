using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinScript : MonoBehaviour {

    public GameManager gm;

    private void OnTriggerEnter( Collider other )
    {
        if( other.tag == "GameManager" )
        {
            gm.SetWinner( name );
        }
    }
}

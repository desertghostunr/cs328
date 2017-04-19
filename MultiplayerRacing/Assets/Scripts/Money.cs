using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Money : MonoBehaviour
{

    public Text moneyTxt;

    public int moneyAmnt = 10;

	// Use this for initialization
	void Start ()
    {
        moneyTxt.text = "$" + moneyAmnt.ToString( );
    }

    public void AddMoney( int amount )
    {
        moneyAmnt += amount;
        moneyTxt.text = "$" + moneyAmnt.ToString( );
    }

    private void OnTriggerEnter( Collider other )
    {
        if( other == null )
        {
            return; 
        }

        if ( other.tag == "Money" )
        {
            moneyAmnt++;

            moneyTxt.text = "$" + moneyAmnt.ToString( );

            Destroy( other.gameObject );
        }
    }
}

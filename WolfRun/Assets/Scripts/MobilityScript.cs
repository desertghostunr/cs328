using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobilityScript : MonoBehaviour
{

    private GrassManager grassManager;
    private Vector3 lastGoodPosition;

	// Use this for initialization
	void Start ()
    {
        lastGoodPosition = transform.position;

        grassManager = GetComponent<GrassManager>( );
	}
	
	// Update is called once per frame
	void Update ()
    {
		if( grassManager.OnGrass( ) )
        {
            //Pushback( );
            transform.position = lastGoodPosition;
        }
        else
        {
            lastGoodPosition = transform.position;
        }
	}

    
}

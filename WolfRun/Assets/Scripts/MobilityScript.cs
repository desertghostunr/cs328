using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(GrassManager))]
public class MobilityScript : MonoBehaviour
{    
    private GrassManager grassManager;
    private Vector3 lastGoodPosition;
    
	void Start ()
    {
        lastGoodPosition = transform.position;

        grassManager = GetComponent<GrassManager>();
	}
	
	void Update ()
    {
		if( grassManager.OnGrass( ) )
        {
            transform.position = lastGoodPosition;
        }
        else
        {
            lastGoodPosition = transform.position;
        }
	}    
}

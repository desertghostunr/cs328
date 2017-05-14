using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathObject : MonoBehaviour
{
    private int trigger = 0;

	// Use this for initialization
	void Start ()
    {
        trigger = 0;
	}
	
    public void IncrementTrigger( )
    {
        trigger++;
    }
	

    public void DecrementTrigger( )
    {
        trigger--;
    }

    public int GetTrigger( )
    {
        return trigger;
    }

    public bool OnPath( )
    {
        return ( trigger > 0 );
    }
}

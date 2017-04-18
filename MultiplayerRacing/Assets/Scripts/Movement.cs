/********************************
 * 
 * Copy Right © Andrew Frost 2017, all rights reserved.
 * 
 *******************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public float movementSpeed = 50;
    public float rotSpeed = 30;

    public float accuracyThreshold = 10;

    private Vector3[ ] worldPath;

    public bool lastPointReached = false;

    public int currentPoint = 0;

    private float xRand, zRand;

    private int numberOfItsStuck = 0;

    private int sTrials = 0;

    private Vector3 adjustedPath;

    private CharacterController controller;

    private GameManager gameManager;

    private float initY;

    // Use this for initialization
	void Start ()
    {
        xRand = 0;
        zRand = 0;
        controller = GetComponent<CharacterController>( );

        initY = transform.position.y;

        movementSpeed += Random.Range( -3.0f, 3.0f );
	}
	
	// Update is called once per frame
	void Update ()
    {
        if(  gameManager.started && !lastPointReached )
        {
            GoToPoint( );
        }
	}


    void GoToPoint( )
    {
        
        Vector3 dir, movement, pos;

        if( currentPoint >= worldPath.Length 
            || Vector3.Distance( transform.position, worldPath[ worldPath.Length - 1 ] ) < accuracyThreshold )
        {
            lastPointReached = true;
            return;
        }

                
        if( numberOfItsStuck > 3 && sTrials == 0 )
        {
            xRand = Random.Range( -150, 150 );
            zRand = Random.Range( -150, 150 );
        }
        else if( sTrials >= 25 )
        {
            sTrials = 0;
        }
        else if( numberOfItsStuck > 3 )
        {
            sTrials++;
        }
        else
        {
            sTrials = 0;
        }

        adjustedPath = new Vector3( worldPath[ currentPoint ].x + xRand,
                                            transform.position.y,
                                            worldPath[ currentPoint ].z + zRand );

        transform.forward = Vector3.RotateTowards( transform.forward,
                                                   adjustedPath - transform.position,
                                                   rotSpeed * Time.deltaTime, 0.0f );


        transform.rotation = Quaternion.Euler( 0, transform.rotation.eulerAngles.y, 0 );        

        dir = adjustedPath - transform.position;

        movement = dir.normalized * movementSpeed * Time.deltaTime;

        if( movement.magnitude > dir.magnitude )
        {
            movement = dir;
        }

        //move the object
        controller.Move( movement );

        //lock y in place
        pos = transform.position;
        pos.y = initY;
        transform.position = pos;

        //test if stuck
        if( controller.velocity.magnitude <= ( movementSpeed / 2.0f ) )
        {
            numberOfItsStuck++;
        }
        else
        {
            if( numberOfItsStuck > 0 )
            {
                numberOfItsStuck--;
            }
        }

        if( Vector3.Distance( transform.position, adjustedPath ) <= accuracyThreshold )
        {
            currentPoint++;

            xRand = 0;
            zRand = 0;
        }

    }

    static int CompareKeys( KeyValuePair<float, Vector3> first, KeyValuePair<float, Vector3> second )
    {
        return first.Key.CompareTo( second.Key );
    }


    public void SetPath( Vector3[ ] path, GameManager gm )
    {
        worldPath = path;

        gameManager = gm;

    }


    IEnumerator DeathTimer( float time )
    {
        yield return new WaitForSeconds( time );
        Destroy( gameObject );
    }

}

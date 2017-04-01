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

    public GameObject healthBar;

    public int worth = 15;

    private float origHBxScale = 0.5f;
    public float health = 100.0f;

    private Vector3[ ] worldPath;

    public bool lastPointReached = false;

    public bool hasArtificat = false;

    public bool collided = false;

    public int currentPoint = 0;

    private float xRand, zRand;

    private int numberOfItsStuck = 0;

    private int sTrials = 0;

    private Vector3 adjustedPath;

    private CharacterController controller;

    private GameManager gameManager;

    public Animator mAnimator;

    private float initY;

    // Use this for initialization
	void Start ()
    {
        xRand = Random.Range( -8.0f, 8.0f );
        zRand = Random.Range( -8.0f, 8.0f );
        controller = GetComponent<CharacterController>( );

        mAnimator = GetComponentInChildren<Animator>( );

        initY = transform.position.y;

        movementSpeed += Random.Range( -10.0f, 10.0f );
	}
	
	// Update is called once per frame
	void Update ()
    {

        if( !lastPointReached )
        {
            if( !mAnimator.GetBool("Dead") )
            {
                GoToPoint( );
            }            
        }
        else if( lastPointReached && hasArtificat )
        {
            gameManager.AddGold( -10 );
            ReversePath( false );
            hasArtificat = false;
            lastPointReached = false;
            collided = false;
        }
        else
        {            
            gameManager.ArtifactStolen( );
            ReversePath( true );
            hasArtificat = true;
            lastPointReached = false;
        }
        
	}

    public void DecrementHealth( float amnt )
    {
        Vector3 healthBarScale;

        health -= amnt;

        healthBarScale = healthBar.transform.localScale;

        if( health <= 0.0f )
        {
            gameManager.AddGold( 15 );

            mAnimator.SetBool( "Dead", true );

            StartCoroutine( DeathTimer( 2.8f ) );

            tag = "Other";

            healthBarScale = Vector3.zero;
            
        }
        else
        {

            healthBarScale.x = ( health / 100.0f ) * origHBxScale;
        }

        healthBar.transform.localScale = healthBarScale;
    }

    void ReversePath( bool trim )
    {
        int index = 0;
        Vector3 tmp;

        if( trim )
        {
            currentPoint = 1;
        }
        else
        {
            currentPoint = 0;
        }
        

        for( index = 0; index < worldPath.Length/2; index++ )
        {
            //swap with opposite position
            tmp = worldPath[ index ];
            worldPath[ index ] = worldPath[ worldPath.Length - 1 - index ];
            worldPath[ worldPath.Length - 1 - index ] = tmp;

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
                                            worldPath[ currentPoint ].y + 10.0f,
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

            if( currentPoint == worldPath.Length - 1 ) /*if end*/
            {
                xRand = 0;
                zRand = 0;
            }
            else
            {
                xRand = Random.Range( -3.0f, 3.0f );
                zRand = Random.Range( -3.0f, 3.0f );
            }
            
        }

    }

    private void OnTriggerEnter( Collider other )
    {
        if( !collided && other.tag == "Artifact" )
        {
            lastPointReached = true;
            collided = true;
        }
    }

    private void OnControllerColliderHit( ControllerColliderHit hit )
    {
        if( !collided && hit.gameObject.tag == "Artifact" )
        {
            lastPointReached = true;
            collided = true;
        }        
    }

    static int CompareKeys( KeyValuePair<float, Vector3> first, KeyValuePair<float, Vector3> second )
    {
        return first.Key.CompareTo( second.Key );
    }


    public void SetPath( Vector3[ ] path, float threshold, Vector3 end, GameManager gm )
    {
        int index, sIndex;

        float testVal;
        bool removed;

        List<Vector3> buffer =  new List<Vector3>( );

        if( path.Length == 0 )
        {
            Debug.Log( "Error: path length zero!" );
            return;
        }

        gameManager = gm;

        //get distances

        for( index = 0; index < path.Length; index++ )
        {
            buffer.Add( path[ index ] );
        }

        //remove waypoints of overlapping paths with random selection
        index = 0;
        removed = false;

        while( index < buffer.Count - 1 )
        {
            sIndex = index + 1;

            if( Vector3.Distance( end, buffer[ index ] ) == 0 )
            {
                buffer.RemoveRange( sIndex, buffer.Count - sIndex );
                break;
            }

            if( Vector3.Distance( end, buffer[ sIndex ] ) > Vector3.Distance( end, buffer[ index ] ) )
            {               
                removed = false;

                while( sIndex < buffer.Count 
                       && Vector3.Distance( end, buffer[ sIndex ] ) > Vector3.Distance( end, buffer[ index ] ) )
                {
                    sIndex++;
                }

                if( sIndex - index > 2 ) //if a loop, then remove
                {
                    testVal = Random.Range( 0.0f, 100.0f );

                    if( ( int ) testVal > 20 )
                    {
                        buffer.RemoveRange( index, ( sIndex - index ) - 1 );

                        removed = true;
                    }
                }
            }

            if( !removed )
            {
                index++;
            }
            else
            {
                removed = false;
            }
            
        }
        //remove adjacent duplicates
        index = 0;

        while( index < buffer.Count - 1 )
        {
            if( buffer[ index ] == buffer[ index + 1 ] )
            {
                buffer.RemoveAt( index );
            }
            else
            {
                index++;
            }
        }

        worldPath = new Vector3[ buffer.Count ];

        for( index = 0; index < buffer.Count; index++ )
        {
            worldPath[ index ] = buffer[ index ];            
        }

        transform.position = new Vector3( worldPath[ 0 ].x + Random.Range( -threshold, threshold ), 
                                          worldPath[ 0 ].y + 10, 
                                          worldPath[ 0 ].z + Random.Range( -threshold, threshold ) );



    }


    IEnumerator DeathTimer( float time )
    {
        yield return new WaitForSeconds( time );
        Destroy( gameObject );
    }

}

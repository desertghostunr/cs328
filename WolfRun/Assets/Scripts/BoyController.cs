using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent( typeof( CharacterController ) )]
[RequireComponent( typeof( Animator ) )]
public class BoyController : MonoBehaviour
{
    public float moveSpeed = 5.5f;
    public float rotSpeed = 3.0f;

    public bool AI = false;

    public float aTime = 15.0f;

    public float dTime = 45.0f;

    private bool canEnterGrass = false;

    private bool abilityActive = false;

    private CharacterController charController;
    private Animator bAnimator;

    private GrassManager grassManager;

    private PathObject pathStatus;

    private Vector3 lastGoodPosition;

    private GameObject[ ] boySmell;

    private int zeroCount = 0;

    private float priorFM = 0.0f;

    // Use this for initialization
    void Start ()
    {
        charController = GetComponent<CharacterController>( );
        bAnimator = GetComponent<Animator>( );

        lastGoodPosition = transform.position;

        grassManager = GetComponent<GrassManager>( );

        pathStatus = GetComponent<PathObject>( );

        boySmell = GameObject.FindGameObjectsWithTag( "Scent" );
    }
	
	// Update is called once per frame
	void FixedUpdate ()
    {
        if( !AI )
        {
            Move( Input.GetAxis( "Vertical" ), Input.GetAxis( "Horizontal" ) );
            EnterGrass( Input.GetButton( "BoyAbility" ) );
        }

    }

    public void Move( float forwardMultiplier, float sideMultiplier )
    {
        Vector3 move;

        Vector3 forwardDir;

        float reverseMultiplier = 1.0f;

        if( zeroCount > 0 && forwardMultiplier == 0  && sideMultiplier == 0 )
        {
            zeroCount = 0;
            return;
        }
        else if( forwardMultiplier == 0 && sideMultiplier == 0 )
        {
            zeroCount++;
        }

        //Deactivate Sense
        if( grassManager.DeepInGrass( ) )
        {
            DeactivateSenseObjects( );
        }

        //check for obstacles
        if ( !canEnterGrass && !pathStatus.OnPath( ) )
        {
            transform.position = lastGoodPosition;            
        }
        else if ( canEnterGrass && !pathStatus.OnPath( ) )
        {
            forwardMultiplier = Mathf.SmoothStep( priorFM, forwardMultiplier / 3.0f, 0.25f );
            lastGoodPosition = transform.position;
        }
        else
        {
            lastGoodPosition = transform.position;
        }

        //movement /////////////////////////////////////////////////////////////

        if( forwardMultiplier < -0.05f )
        {
            reverseMultiplier /= 4.0f;
        }


        //calculate movement

        priorFM = forwardMultiplier;

        forwardDir = transform.TransformDirection( Vector3.forward );
        transform.Rotate( 0, sideMultiplier * rotSpeed * Time.deltaTime, 0 );

        //apply movement
        charController.SimpleMove( forwardDir * forwardMultiplier * moveSpeed * reverseMultiplier );

        if( forwardMultiplier >= 0.00f )
        {
            move = forwardMultiplier * Vector3.forward + sideMultiplier * Vector3.right;
        }
        else
        {
            move = forwardMultiplier * Vector3.back + sideMultiplier * Vector3.right;
        }

        // animation ////////////////////////////////
        bAnimator.SetFloat( "Forward", forwardMultiplier, 0.1f, Time.deltaTime );
        bAnimator.SetFloat( "Turn", Mathf.Atan2( move.x, move.z ), 0.1f, Time.deltaTime );        
    }

    public void EnterGrass( bool grassKey )
    {
        if( grassKey && !abilityActive )
        {
            StartCoroutine( GrassRoutine( ) );
        }
    }

    private void DeactivateSenseObjects( )
    {
        int index = 0;

        for ( index = 0; index < boySmell.Length; index++ )
        {
            boySmell[index].SetActive( false );
        }
    }

    IEnumerator GrassRoutine( )
    {
        

        abilityActive = true;

        canEnterGrass = true;              

        yield return new WaitForSeconds( aTime );

        canEnterGrass = false;

        yield return new WaitForSeconds( dTime );

        abilityActive = false;
    }
}

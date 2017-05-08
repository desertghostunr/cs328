using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent( typeof( CharacterController ) )]
[RequireComponent( typeof( Animator ) )]
public class BoyController : MonoBehaviour
{
    public float moveSpeed = 5.5f;
    public float rotSpeed = 3.0f;

    private CharacterController charController;
    private Animator bAnimator;
    // Use this for initialization
    void Start ()
    {
        charController = GetComponent<CharacterController>( );
        bAnimator = GetComponent<Animator>( );
    }
	
	// Update is called once per frame
	void Update ()
    {
        float forwardMultiplier, sideMultiplier;

        Vector3 move;

        Vector3 forwardDir;

        forwardMultiplier = Input.GetAxis( "Vertical" );

        sideMultiplier = Input.GetAxis( "Horizontal" );


        //movement /////////////////////////////////////////////////////////////
        //move forward when turning
        if ( Mathf.Abs( sideMultiplier ) > 0.05f )
        {
            forwardMultiplier = forwardMultiplier < 0.05f ? Mathf.Abs( sideMultiplier ) / 1.5f : forwardMultiplier;
        }


        //calculate movement

        forwardDir = transform.TransformDirection( Vector3.forward );
        transform.Rotate( 0, sideMultiplier * rotSpeed * Time.deltaTime, 0 );

        //apply movement
        charController.SimpleMove( forwardDir * forwardMultiplier * moveSpeed );

        move = forwardMultiplier * Vector3.forward + sideMultiplier * Vector3.right;

        // animation ////////////////////////////////
        if ( Mathf.Abs( sideMultiplier ) > 0.05f || forwardMultiplier > 0.05f )
        {
            bAnimator.SetFloat( "Forward", forwardMultiplier, 0.1f, Time.deltaTime );
            bAnimator.SetFloat( "Turn", Mathf.Atan2( move.x, move.z ), 0.1f, Time.deltaTime );
            Debug.Log( "Turn" );
        }

    }
}

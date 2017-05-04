using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent( typeof( CharacterController ) )]
[RequireComponent( typeof( Animator ) )]
public class WolfMovement : MonoBehaviour
{

    public float moveSpeed = 5.5f;
    public float rotSpeed = 3.0f;

    CharacterController charController;
    Animator wAnimator;

	// Use this for initialization
	void Start ()
    {
        charController = GetComponent<CharacterController>( );
        wAnimator = GetComponent<Animator>( );
	}
	
	// Update is called once per frame
	void Update ()
    {
        float forwardMultiplier, sideMultiplier;
        Vector3 forwardDir;

        float reverseMultiplier = 1.0f;

        forwardMultiplier = Input.GetAxis( "WolfFB" );

        sideMultiplier = Input.GetAxis( "WolfLR" );

        transform.Rotate( 0, sideMultiplier * rotSpeed * Time.deltaTime, 0 );

        if( Mathf.Abs( sideMultiplier ) > 0.05f && forwardMultiplier > -0.05f )
        {
            forwardMultiplier = Mathf.Max( forwardMultiplier, Mathf.Abs( sideMultiplier ) / 4.0f );
        }

        forwardDir = transform.TransformDirection( Vector3.forward );

        if( forwardMultiplier < -0.05f )
        {
            reverseMultiplier = Mathf.Abs( forwardMultiplier ) / 4.0f;
        }

        charController.SimpleMove( forwardDir * forwardMultiplier * moveSpeed * reverseMultiplier );

        if ( forwardMultiplier > 0.05f )
        {
            wAnimator.SetInteger( "FB_Controller", 1 );
        }
        else if ( forwardMultiplier < -0.05f )
        {
            wAnimator.SetInteger( "FB_Controller", -1 );
        }
        else
        {
            wAnimator.SetInteger( "FB_Controller", 0 );
        }

        if( !( Mathf.Abs( forwardMultiplier ) > 0.05f ) && sideMultiplier > 0.05f )
        {
            wAnimator.SetInteger( "RL_Controller", 1 );
        }
        else if ( !( Mathf.Abs( forwardMultiplier ) > 0.05f ) && sideMultiplier < -0.05f )
        {
            wAnimator.SetInteger( "RL_Controller", -1 );
        }
        else
        {
            wAnimator.SetInteger( "RL_Controller", 0 );
        }

    }
}

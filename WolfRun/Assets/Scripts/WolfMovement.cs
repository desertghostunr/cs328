using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent( typeof( CharacterController ) )]
[RequireComponent( typeof( Animator ) )]
[RequireComponent( typeof( GrassManager) )]
public class WolfMovement : MonoBehaviour
{

    public float moveSpeed = 5.5f;
    public float rotSpeed = 3.0f;

    private GrassManager grassManager;

    private CharacterController charController;
    private Animator wAnimator;

	// Use this for initialization
	void Start ()
    {
        charController = GetComponent<CharacterController>( );
        wAnimator = GetComponent<Animator>( );

        grassManager = GetComponent<GrassManager>( );
	}
	
	// Update is called once per frame
	void Update ()
    {
        float forwardMultiplier, sideMultiplier;
        Vector3 forwardDir;

        float reverseMultiplier = 1.0f;

        forwardMultiplier = Input.GetAxis( "WolfFB" );

        sideMultiplier = Input.GetAxis( "WolfLR" );

        //calculate movement
        transform.Rotate( 0, sideMultiplier * rotSpeed * Time.deltaTime, 0 );

        if ( Mathf.Abs( sideMultiplier ) > 0.05f && forwardMultiplier > -0.05f )
        {
            forwardMultiplier = Mathf.Max( forwardMultiplier, Mathf.Abs( sideMultiplier ) / 4.0f );
        }

        forwardDir = transform.TransformDirection( Vector3.forward );

        if ( forwardMultiplier < -0.05f )
        {
            reverseMultiplier = Mathf.Abs( forwardMultiplier ) / 4.0f;
        }

        //animation
        if ( forwardMultiplier > 0.05f )
        {
            wAnimator.SetInteger( "FB_Controller", 1 );

            if( grassManager.OnGrass( ) )
            {
                forwardMultiplier /= 3.0f;
            }
        }
        else if ( forwardMultiplier < -0.05f )
        {
            wAnimator.SetInteger( "FB_Controller", -1 );
        }
        else
        {
            wAnimator.SetInteger( "FB_Controller", 0 );
        }

        
        //apply movement
        charController.SimpleMove( forwardDir * forwardMultiplier * moveSpeed * reverseMultiplier );

    }
}

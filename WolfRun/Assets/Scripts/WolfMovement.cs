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

    public bool AI = false;

    private GrassManager grassManager;
    private GameManager gameManager;
    private WolfSense senseController;
    private CharacterController charController;
    private Animator wAnimator;

    private AudioSource wAudio;
    public AudioClip howlSound;

    private bool howlPlaying = false;

	// Use this for initialization
	void Start ()
    {
        charController = GetComponent<CharacterController>( );
        wAnimator = GetComponent<Animator>( );
        senseController = GetComponentInChildren<WolfSense>( );
        grassManager = GetComponent<GrassManager>( );
        gameManager = FindObjectOfType<GameManager>( );
        wAudio = GetComponent<AudioSource>( );
	}
	
	// Update is called once per frame
	void Update ()
    {
        if( !AI )
        {
            Move( Input.GetAxis( "WolfFB" ), Input.GetAxis( "WolfLR" ) );
        }

    }

    public void Move( float forwardMultiplier, float sideMultiplier )
    {
        Vector3 forwardDir;

        // animations //////////////////////////////////////////////////////////

        //handle sense input
        if ( Input.GetButton( "WolfSense" ) )
        {
            wAnimator.SetBool( "Howl", true );

        }

        if ( wAnimator.GetBool( "Howl" ) && wAnimator.GetCurrentAnimatorStateInfo( 0 ).IsName( "Howl_State" ) )
        {
            wAnimator.SetBool( "Howl", false );
            wAudio.clip = howlSound;
            wAudio.Play( );
            howlPlaying = true;
        }

        if ( howlPlaying && !wAnimator.GetCurrentAnimatorStateInfo( 0 ).IsName( "Howl_State" ) )
        {
            senseController.ActivateSense( );
            howlPlaying = false;
        }
        else if ( howlPlaying )
        {
            return; //don't move wolf if animation is playing
        }

        //determine speed based on input
        if ( Mathf.Abs( sideMultiplier ) > 0.05f && forwardMultiplier > -0.05f )
        {
            forwardMultiplier = Mathf.Max( forwardMultiplier, Mathf.Abs( sideMultiplier ) / 4.0f );
        }

        if ( forwardMultiplier < -0.05f )
        {
            forwardMultiplier /= 4.0f;
        }

        //slow movement based on grass
        if ( forwardMultiplier > 0.05f )
        {
            if ( senseController.SenseOn( ) )
            {
                forwardMultiplier /= 4.0f;
            }
            else if ( grassManager.OnGrass( ) )
            {
                forwardMultiplier /= 2.0f;
            }
        }



        //animation based on movement 
        if ( forwardMultiplier < 0.33f
             && ( Mathf.Abs( sideMultiplier ) > 0.05f || forwardMultiplier > 0.05f ) )
        {
            wAnimator.SetInteger( "RL_Controller", 1 );
            wAnimator.SetInteger( "FB_Controller", 0 );
        }
        else if ( forwardMultiplier > 0.05f )
        {
            wAnimator.SetInteger( "FB_Controller", 1 );
            wAnimator.SetInteger( "RL_Controller", 0 );
        }
        else if ( forwardMultiplier < -0.05f )
        {
            wAnimator.SetInteger( "FB_Controller", -1 );
            wAnimator.SetInteger( "RL_Controller", 0 );
        }
        else
        {
            wAnimator.SetInteger( "FB_Controller", 0 );
            wAnimator.SetInteger( "RL_Controller", 0 );
        }

        //movement /////////////////////////////////////////////////////////////

        //calculate movement
        forwardDir = transform.TransformDirection( Vector3.forward );

        transform.Rotate( 0, sideMultiplier * rotSpeed * Time.deltaTime, 0 );

        //apply movement
        charController.SimpleMove( forwardDir * forwardMultiplier * moveSpeed );
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Triggered by " + other.tag);
        if (other.tag == "Boy")
        {
            gameManager.SetWinner(name);
        }
    }
}

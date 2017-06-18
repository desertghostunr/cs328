/******************************************************
 * 
 * Copy Right © Andrew Frost 2017, all rights reserved.
 * 
 ******************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent( typeof( CharacterController ) )]
public class PlayerCharacterController : UniversalCharacterController
{
    public string forwardMotionInput;
    public string sideMotionInput;
    public string runMotionInput = "Sprint";
    public string crouchMotionInput = "Crouch";
    public string jumpMotionInput = "Jump";

    public bool turnWithMouse = false;
    public bool canModifyTurnWithMouse = false;
    public float mouseSensitivity = 2.0f;

    public bool toggleCrouch = false;

    protected CharacterController m_controller;

    protected bool m_jump = false;

    protected float m_walkSpeedInhibitor;

    protected bool m_crouchLock = false;

    // Use this for initialization
    protected override void Start ()
    {
        m_controller = GetComponent<CharacterController>( );

        base.Start( );
	}
	
	// Update is called once per frame
	private void Update ()
    {
        if ( m_canMove )
        {
            //get input
            ProcessInput( );            
        }
        else
        {
            m_forwardAxis = 0;
            m_turnAxis = 0;
            m_turnAxis = 0;
        }        

        //animation
        Animate( m_forwardAxis, m_turnAxis, m_sideAxis );
    }

    private void FixedUpdate( )
    {
        //move the character controller
        Move( ref m_forwardAxis, ref m_turnAxis, ref m_sideAxis );

        m_movementInhibitor = 1.0f;
    }

    //call in update
    public void ProcessInput( )
    {
        //walking speed
        m_walkSpeedInhibitor = Input.GetButton( runMotionInput ) ? 1.0f : walkSpeedInhibitor;

        //crouching
        if( canCrouch )
        {
            if ( !toggleCrouch )
            {
                m_isCrouched = Input.GetButton( crouchMotionInput );
            }
            else //handle toggle input
            {
                if( Input.GetButtonDown( crouchMotionInput ) )
                {
                    m_isCrouched = !m_isCrouched;
                }
            }
        }
        else
        {
            m_isCrouched = false;
        }       
        

        //jumping
        m_jump = canJump ? ( !m_isJumping && Input.GetButton( jumpMotionInput ) ) : false;

        if ( m_jump && m_isCrouched )
        {
            m_jump = m_isCrouched = false;
        }

        //movement
        m_forwardAxis = Input.GetAxis( forwardMotionInput ) * m_movementInhibitor * m_walkSpeedInhibitor;

        if ( !turnWithMouse )
        {
            m_turnAxis = Input.GetAxis( sideMotionInput ) * m_walkSpeedInhibitor;

            m_sideAxis = 0;
        }
        else
        {
            //calculate rotate based on mouse
            m_turnAxis = Input.GetAxis( "Mouse X" );
            m_sideAxis = Input.GetAxis( sideMotionInput ) * m_movementInhibitor * m_walkSpeedInhibitor;
        }
    }

    //call in fixed update
    public override void Move( ref float forward, ref float turn, ref float side )
    {
        Vector3 movement;
        float mouseMultiplier;

        if ( zeroCount > 0 && forward == 0 && turn == 0 && side == 0 )
        {
            m_moving = false;
            zeroCount = 0;
            return;
        }
        else if ( forward == 0 && turn == 0 && side == 0 )
        {
            zeroCount++;
        }
        else
        {
            m_moving = true;
            zeroCount = 0;
        }

        //preform rotation
        mouseMultiplier = turnWithMouse ? mouseSensitivity : 1.0f;
        transform.Rotate( 0, turn * mouseSensitivity * rotSpeed  * Time.deltaTime, 0 );

        //preform movement
        movement = GetForwardMovement( ref forward, ref turn ) //local z movement in world
                   + GetRightMovement( ref side ) //local x movement in world
                   + Jump( ); //y movement in world

        m_controller.Move( movement * Time.fixedDeltaTime );

        //information about the speed the player is moving
        m_movementSpeed = movement.magnitude;
    }

    private Vector3 Jump( )
    {
        //handle jumping
        if ( m_controller.isGrounded && m_jump ) //initiating jump
        {
            m_jumpVelocity = jumpSpeed * transform.TransformDirection( Vector3.up );
        }

        if ( !m_controller.isGrounded ) //set if falling
        {
            m_isJumping = true;
        }
        else
        {
            m_isJumping = false;
        }

        m_jumpVelocity += Physics.gravity * Time.fixedDeltaTime;

        return m_jumpVelocity;
    }

    private Vector3 GetForwardMovement( ref float forward, ref float turn )
    {
        Vector3 forwardDir;
        float reverseMultiplier;
        float crouchMultiplier;

        reverseMultiplier = forward < 0 ? reverseSpeedInhibitor : 1.0f;
        crouchMultiplier = ( m_isCrouched ? crouchMovementInhibitor : 1.0f );

        forward *= Mathf.Max( 0, 1.0f - Mathf.Abs( 0.5f * GetTurnValue( forward, turn ) ) );
        forwardDir = transform.TransformDirection( Vector3.forward );

        return ( forwardDir //direction
                 * forward //forward movement weight
                 * moveSpeed //forward speed 
                 * reverseMultiplier //reversal speed inhibitor
                 * crouchMultiplier ); //crouch speed inhibitor
    }

    private Vector3 GetRightMovement( ref float side )
    {
        Vector3 rightDir;
        float crouchMultiplier;

        crouchMultiplier = ( m_isCrouched ? crouchMovementInhibitor : 1.0f );
        rightDir = transform.TransformDirection( Vector3.right );

        return ( rightDir //direction
                 * side //side movement weight
                 * sidestepMoveSpeed //sidestep movement speed 
                 * crouchMultiplier ); //crouch speed inhibitor
    }

    private float GetTurnValue( float forward, float turn )
    {
        Vector3 move;

        if ( forward >= 0.00f )
        {
            move = forward * Vector3.forward + turn * Vector3.right;
        }
        else
        {
            move = forward * Vector3.back + turn * Vector3.right;
        }

        return Mathf.Atan2( move.x, move.z );
    }

    public void SetTurnWithMouse( bool val )
    {
        if( canModifyTurnWithMouse )
        {
            turnWithMouse = val;
        }
    }
}

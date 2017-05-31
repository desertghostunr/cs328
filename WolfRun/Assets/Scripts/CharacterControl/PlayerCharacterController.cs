using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent( typeof( CharacterController ) )]
public class PlayerCharacterController : UniversalCharacterController
{    
    public bool turnWithMouse = false;
    public float mouseSensitivity = 2.0f;

    protected CharacterController m_controller;

    // Use this for initialization
    protected override void Start ()
    {

        m_controller = GetComponent<CharacterController>( );

        base.Start( );
	}
	
	// Update is called once per frame
	void Update ()
    {
        if ( m_canMove )
        {
            m_isCrouched = canCrouch ? Input.GetButton( crouchMotionInput ) : false;

            m_forwardAxis = Input.GetAxis( forwardMotionInput ) * m_movementInhibitor;

            if ( !turnWithMouse )
            {
                m_turnAxis = Input.GetAxis( sideMotionInput );

                m_sideAxis = 0;
            }
            else
            {
                //calculate rotate based on mouse
                m_turnAxis = mouseSensitivity * Input.GetAxis( "Mouse X" );

                m_sideAxis = Input.GetAxis( sideMotionInput ) * m_movementInhibitor;
            }

            Move( m_forwardAxis, m_turnAxis, m_sideAxis );

        }
        else
        {
            m_forwardAxis = 0;
            m_turnAxis = 0;
            m_turnAxis = 0;
        }


        Animate( m_forwardAxis, m_turnAxis, m_sideAxis );
    }

    public override void Move( float forward, float turn, float side )
    {
        Vector3 forwardDir, rightDir;
        float reverseMultiplier;

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

        reverseMultiplier = forward < 0 ? reverseSpeedInhibitor : 1.0f;

        forwardDir = transform.TransformDirection( Vector3.forward );
        rightDir = transform.TransformDirection( Vector3.right );

        transform.Rotate( 0, turn * rotSpeed * Time.deltaTime, 0 );

        m_controller.SimpleMove( forwardDir 
                                 * forward 
                                 * moveSpeed 
                                 * reverseMultiplier
                                 * ( m_isCrouched ? crouchMovementInhibitor : 1.0f ) );

        m_controller.SimpleMove( rightDir 
                                 * side 
                                 * sidestepMoveSpeed
                                 * ( m_isCrouched ? crouchMovementInhibitor : 1.0f ) );
    }
}

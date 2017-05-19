using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(CharacterController))]
public class UniversalCharacterController : MonoBehaviour
{

    public string forwardMotionInput;
    public string sideMotionInput;

    public string forwardAnimationName = "Forward";
    public string turnAnimationName = "Turn";

    public float moveSpeed = 14.0f;
    public float rotSpeed = 80.0f;

    public float reverseSpeedInhibitor = 0.25f;

    public bool turnWithMouse = false;

    public bool controlledByUser = true;

    public float animationDampTime = 0.1f;

    private bool m_canMove = true;

    private float m_movementInhibitor = 1.0f;

    private float m_forwardAxis;
    private float m_turnAxis;
    private float m_sideAxis;

    private int zeroCount = 0;

    private Animator m_animator;

    private CharacterController m_controller;

    private bool m_moving = false;
    

    // Use this for initialization
    void Start ()
    {
        m_animator = GetComponent<Animator>( );
        m_controller = GetComponent<CharacterController>( );

        zeroCount = 0;
	}
	
	// Update is called once per frame
	void Update ()
    {
		if( controlledByUser && m_canMove )
        {
            m_forwardAxis = Input.GetAxis( forwardMotionInput ) * m_movementInhibitor;

            if( name == "Wolf" )Debug.Log( name + " " + m_forwardAxis );

            if( !turnWithMouse )
            {
                m_turnAxis = Input.GetAxis( sideMotionInput );

                m_sideAxis = 0;
            }
            else
            {
                //calculate rotate based on mouse
                m_turnAxis = 0;

                m_sideAxis = Input.GetAxis( sideMotionInput );
            }

            Move( m_forwardAxis, m_turnAxis, m_sideAxis );
            Animate( m_forwardAxis, m_turnAxis, m_sideAxis );
            
        }
	}

    public void Move( float forward, float turn, float side )
    {
        Vector3 forwardDir;
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
        transform.Rotate( 0, turn * rotSpeed * Time.deltaTime, 0 );

        m_controller.SimpleMove( forwardDir * forward * moveSpeed * reverseMultiplier );

    }

    public void Animate( float forward, float turn, float side )
    {
        Vector3 move;
        float animatorTurnValue;

        if ( forward >= 0.00f )
        {
            move = forward * Vector3.forward + turn * Vector3.right;
        }
        else
        {
            move = forward * Vector3.back + turn * Vector3.right;
        }

        animatorTurnValue = Mathf.Atan2( move.x, move.z );

        m_animator.SetFloat( forwardAnimationName, forward, animationDampTime, Time.deltaTime );
        m_animator.SetFloat( turnAnimationName, animatorTurnValue, animationDampTime, Time.deltaTime );
    }

    public void SetCanMove( bool canMove )
    {
        m_canMove = canMove;
    }

    public void SetMovementInhibitor( float movementInhibitor )
    {
        m_movementInhibitor = movementInhibitor;
    }

    public bool Moving( )
    {
        return m_moving;
    }
    
}

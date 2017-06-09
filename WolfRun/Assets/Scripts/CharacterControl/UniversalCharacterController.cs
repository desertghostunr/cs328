/******************************************************
 * 
 * Copy Right © Andrew Frost 2017, all rights reserved.
 * 
 ******************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class UniversalCharacterController : MonoBehaviour
{

    

    public string forwardAnimationName = "Forward";
    public string turnAnimationName = "Turn";
    public string crouchAnimationName = "Crouch";
    public string jumpAnimationName = "Jump";
    public string jumpLegAnimationName = "JumpLeg";
    public string jumpBoolAnimationName = "OnGround";

    public bool animate = true;

    public bool canCrouch = true;
    public float crouchMovementInhibitor = 0.25f;

    public bool canJump = true;
    public float jumpSpeed = 5.0f;

    public float moveSpeed = 14.0f;
    public float rotSpeed = 80.0f;

    public float sidestepMoveSpeed = 7.0f;

    public float sidestepAnimationMultiplier = 1.5f;

    public float reverseSpeedInhibitor = 0.25f;    

    public float animationDampTime = 0.1f;

    protected bool m_canMove = true;
    protected bool m_isCrouched = false;
    protected bool m_isJumping = false;
    protected bool m_previousJumpingValue = false;

    protected float m_movementInhibitor = 1.0f;

    protected Vector3 m_jumpVelocity = Vector3.zero;
    protected float m_jumpLeg = 0.0f;

    protected float m_forwardAxis;
    protected float m_turnAxis;
    protected float m_sideAxis;

    protected int zeroCount = 0;

    protected Animator m_animator;

    protected bool m_moving = false;
    

    // Use this for initialization
    protected virtual void Start ()
    {
        m_animator = GetComponent<Animator>( );

        zeroCount = 0;
	}

    public virtual void Move( )
    {
        Debug.Log( name + " is using a virtual function: Move( ). "
                    + "Calling this function does not move " + name + "." );
    }

    public virtual void Move( ref float forward, ref float turn, ref float side )
    {
        Debug.Log( name + " is using a virtual function: Move( float, float, float ). " 
                    + "Calling this function does not move " + name + "." );
    }

    public void Animate( float forward, float turn, float side )
    {   
        if( !animate )
        {
            return;
        }

        if ( canJump )
        {
            m_animator.SetBool( jumpBoolAnimationName, !m_isJumping );

            if( m_isJumping && !m_previousJumpingValue )
            {
                m_jumpLeg = forward > 0.0f ? Random.Range( -1.0f, 1.0f ) : 0.0f;
            }

            m_previousJumpingValue = m_isJumping;

            m_animator.SetFloat( jumpAnimationName, m_jumpVelocity.magnitude, animationDampTime, Time.deltaTime );
            m_animator.SetFloat( jumpLegAnimationName, m_jumpLeg, animationDampTime, Time.deltaTime );

        }

        m_animator.SetFloat( forwardAnimationName, forward, animationDampTime, Time.deltaTime );
        m_animator.SetFloat( turnAnimationName, GetAnimationTurnValue( forward, turn, side ), animationDampTime, Time.deltaTime ); 
        
        if( canCrouch )
        {
            m_animator.SetBool( crouchAnimationName, m_isCrouched );
        }

        
        
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

    private float GetAnimationTurnValue( float forward, float turn, float side )
    {
        Vector3 move;

        if ( forward >= 0.00f )
        {
            move = forward * Vector3.forward + ( turn + ( side * sidestepAnimationMultiplier ) ) * Vector3.right;
        }
        else
        {
            move = forward * Vector3.back + ( turn + ( side * sidestepAnimationMultiplier ) ) * Vector3.right;
        }

        return Mathf.Atan2( move.x, move.z );
    }
    
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class UniversalCharacterController : MonoBehaviour
{

    public string forwardMotionInput;
    public string sideMotionInput;
    public string crouchMotionInput = "Crouch";

    public string forwardAnimationName = "Forward";
    public string turnAnimationName = "Turn";
    public string crouchAnimationName = "Crouch";

    public bool canCrouch = true;

    public float crouchMovementInhibitor = 0.25f;


    public float moveSpeed = 14.0f;
    public float rotSpeed = 80.0f;

    public float sidestepMoveSpeed = 7.0f;

    public float sidestepAnimationMultiplier = 1.5f;

    public float reverseSpeedInhibitor = 0.25f;    

    public float animationDampTime = 0.1f;

    protected bool m_canMove = true;

    protected bool m_isCrouched = false;

    protected float m_movementInhibitor = 1.0f;

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

    public virtual void Move( float forward, float turn, float side )
    {
        Debug.Log( name + " is using a virtual function: Move( float, float, float ). " 
                    + "Calling this function does not move " + name + "." );
    }

    public void Animate( float forward, float turn, float side )
    {
        Vector3 move;
        float animatorTurnValue;

        if ( forward >= 0.00f )
        {
            move = forward * Vector3.forward + ( turn + side * sidestepAnimationMultiplier ) * Vector3.right;
        }
        else
        {
            move = forward * Vector3.back + ( turn + side * sidestepAnimationMultiplier ) * Vector3.right;
        }

        animatorTurnValue = Mathf.Atan2( move.x, move.z );

        m_animator.SetFloat( forwardAnimationName, forward, animationDampTime, Time.deltaTime );
        m_animator.SetFloat( turnAnimationName, animatorTurnValue, animationDampTime, Time.deltaTime ); 
        
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
    
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class NPCharacterController : UniversalCharacterController
{
    private NavMeshAgent m_agent;

    private NavMeshPath m_pausedPath = null;

	// Use this for initialization
	protected override void Start ()
    {
        m_agent = GetComponent<NavMeshAgent>( );
        m_agent.speed = moveSpeed;
        m_agent.angularSpeed = rotSpeed;

        base.Start( );
	}

    // Update is called once per frame
    void Update ()
    {
        if ( m_canMove )
        {
            Move( );
            m_movementInhibitor = 1.0f;
        }
        else
        {
            m_agent.velocity = Vector3.zero;
        }
        
        Animate( );

        if ( !m_agent.isOnNavMesh )
        {
            transform.position = GetNearestLocationOnNavMesh( transform.position );

            m_agent.enabled = false;
            m_agent.enabled = true;
        }
    }

    public override void Move(  )
    {
        m_agent.speed = m_movementInhibitor * moveSpeed;
        m_agent.angularSpeed = m_movementInhibitor * rotSpeed;

        if( m_agent.velocity.magnitude > 0 )
        {
            m_moving = true;
        }
        else
        {
            m_moving = false;
        }
    }

    public void Animate( )
    {
        Vector3 localMovement = m_agent.transform.InverseTransformDirection( m_agent.velocity ).normalized;

        Animate( localMovement.z, localMovement.x, 0.0f );
    }

    public bool SetDestination( Vector3 dest )
    {
        return IsActiveEnabledAndReady( ) ? m_agent.SetDestination( dest ) : false;
    }

    public void Resume( )
    {
        if( m_pausedPath != null )
        {
            m_agent.SetPath( m_pausedPath );
            m_pausedPath = null;
        }
        
    }

    public void Stop( )
    {
        if( m_agent.path != null )
        {
            m_pausedPath = m_agent.path;
        }

        m_agent.velocity = Vector3.zero;

        m_agent.ResetPath( );
    }

    public Vector3 GetNearestLocationOnNavMesh( Vector3 position )
    {
        NavMeshHit closestHit;

        if ( NavMesh.SamplePosition( position, out closestHit, 1000f, NavMesh.AllAreas ) )
        {
            return closestHit.position;
        }

        return transform.position;
    }

    public bool IsActiveEnabledAndReady( )
    {
        return ( m_agent && m_agent.isActiveAndEnabled && m_agent.isOnNavMesh );
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class NPCharacterController : UniversalCharacterController
{
    private NavMeshAgent m_agent;

	// Use this for initialization
	protected override void Start ()
    {
        m_agent = GetComponent<NavMeshAgent>( ); 
        

        base.Start( );
	}

    // Update is called once per frame
    void Update ()
    {
        if ( m_canMove )
        {
            Move( );
            Animate( );           
        }

        if ( !m_agent.isOnNavMesh )
        {
            transform.position = GetNearestLocationOnNavMesh( transform.position );

            m_agent.enabled = false;
            m_agent.enabled = true;
        }
    }

    public override void Move(  )
    {
        m_agent.speed = moveSpeed * m_movementInhibitor;
        m_agent.angularSpeed = rotSpeed;
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
        return ( m_agent.isActiveAndEnabled && m_agent.isOnNavMesh );
    }
}

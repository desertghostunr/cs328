using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleNPCIntelligence : MonoBehaviour
{
    public string targetTag = null;
    public string enemyTag = null;

    public float enemySeenProbability = 0.25f;

    public float fleeEnemyProbability = 0.8f;

    public float fleeDistance = 5.0f;

    public float sightDistance = 10.0f;

    public float sightRefreshRate = 0.75f;

    public float destinationDistanceRange = 0.5f;

    public float intelligence = 0.0f;

    protected NPCharacterController m_NPC;

    protected bool m_travelingToLoc = false;
    protected Vector3 m_destination;

    protected float m_timeSinceLastSightScan = 0.0f;

    protected List<GameObject> m_spottedEnemies = new List<GameObject>();

    // Use this for initialization
    protected virtual void Start ()
    {
        m_NPC = GetComponent<NPCharacterController>( );

        intelligence = Mathf.Max( 0.0f, Mathf.Min( 1.0f, intelligence ) );
    }

    protected virtual void FixedUpdate( )
    {
        if( m_timeSinceLastSightScan <= 0.0f )
        {
            LookForTarget( );
            m_timeSinceLastSightScan = sightRefreshRate;
        }
        else
        {
            m_timeSinceLastSightScan -= Time.deltaTime;
        }
    }

    public virtual bool PlanDestination( )
    {
        Debug.Log( name + "is calling a virtual function: PlanDestination( )." );
        return false;
    }

    public float GetCorrectDestinationProbability( )
    {
        return Random.Range( intelligence, 1.0f );
    }

    public virtual void LookForTarget( )
    {
        Collider[] colliders; 
        int index;
        float sightingProb = 0.0f, dotProd = 0.0f, distance;

        if( targetTag == null && enemyTag == null )
        {
            return;
        }

        colliders = Physics.OverlapSphere( transform.position, sightDistance );

        m_spottedEnemies.Clear( );

        for ( index = 0; index < colliders.Length; index++ )
        {
            if( targetTag != null && colliders[ index].tag == targetTag )
            {
                m_destination = colliders[index].transform.position;
                m_NPC.SetDestination( m_destination );
                break;
            }
            else if( enemyTag != null && colliders[index].tag == enemyTag )
            {
                dotProd = Vector3.Dot( transform.forward, ( transform.position - colliders[index].transform.position ) );

                sightingProb = Random.Range( 1.0f - intelligence, 1.0f );

                distance = Vector3.Distance( transform.position, colliders[index].transform.position );

                if ( sightingProb > dotProd 
                     && distance < ( sightDistance * Random.Range( 0.0f, enemySeenProbability ) ) )
                {
                    m_spottedEnemies.Add( colliders[index].gameObject );
                }
                
            }
        }

    }
}

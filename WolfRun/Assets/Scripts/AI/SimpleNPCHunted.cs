using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(NPCharacterController))]
public class SimpleNPCHunted : SimpleNPCIntelligence
{
    private List<Vector3> m_locList;

	// Use this for initialization
	protected override void Start ()
    {
        base.Start( );
	}
	
	// Update is called once per frame
	void Update ()
    {
        //planning
		if( !m_travelingToLoc )
        {
            m_travelingToLoc = PlanDestination( );            
        }

        if ( Vector3.Distance( m_destination, transform.position ) <= destinationDistanceRange )
        {
            m_travelingToLoc = false;
        }

        AvoidEnemies( );
	}

    public override bool PlanDestination( )
    {
        int destIndex;
        float correctDestProb = GetCorrectDestinationProbability( );

        if ( !m_NPC.IsActiveEnabledAndReady( ) )
        {
            return false;
        }

        destIndex = Mathf.Max( 0, 
                               Mathf.Min( ( int ) ( correctDestProb * m_locList.Count ), 
                                           m_locList.Count - 1 ) );        

        m_destination = m_locList[destIndex];

        m_locList.RemoveAt( destIndex );

        return m_NPC.SetDestination( m_destination );
    }

    public void AvoidEnemies( )
    {
        int index = 0;

        for( index = 0; index < m_spottedEnemies.Count; index++ )
        {
            if( fleeEnemyProbability > Random.value )
            {
                m_locList.Add( m_destination );

                m_destination = m_NPC.GetNearestLocationOnNavMesh( -1.0f * Random.Range(0.25f, fleeDistance) * transform.forward );
                
                m_travelingToLoc = m_NPC.SetDestination( m_destination );
            }
        }
    }

    public void SetLocationList( List<Vector3> list )
    {
        m_locList = list;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleNPCHunter : SimpleNPCIntelligence
{
    public string trackedObjectTag = "Scent";
    public float enemySmellAccuracyProbability = 0.9f;
    public float attackEnemyProbability = 0.95f;

    public float searchRange = 50.0f;

    private bool m_enemyScentDetected = false;
    private WolfHowl m_trackingAbility;
    private GameObject m_trackedObject;

    // Use this for initialization
    protected override void Start ()
    {
        base.Start( );

        m_trackingAbility = GetComponent<WolfHowl>( );
	}
	
	// Update is called once per frame
	void Update ()
    {
        if ( !m_travelingToLoc )
        {
            m_travelingToLoc = PlanDestination( );
        }

        if ( Vector3.Distance( m_destination, transform.position ) <= destinationDistanceRange )
        {
            m_travelingToLoc = false;
        }

        if( m_trackedObject && !m_trackedObject.activeSelf )
        {
            m_trackedObject = null;
        }

        if( m_trackedObject )
        {
            m_destination = m_trackedObject.transform.position;
            m_NPC.SetDestination( m_destination );
        }

        AttackEnemy( );
    }


    public override bool PlanDestination( )
    {

        float useAbilityProb;
        float pointOffset;
        Vector3 pointToGoTo;

        if ( !m_NPC.IsActiveEnabledAndReady( ) )
        {
            return false;
        }

        useAbilityProb = Random.Range( intelligence, 1.0f );

        if( useAbilityProb > enemySmellAccuracyProbability )
        {
            m_trackingAbility.ActivateAbility( );
            StartCoroutine( TrackEnemy( ) );
        }
        else
        {
            pointOffset = searchRange * intelligence;

            pointToGoTo = transform.position + new Vector3( Random.Range( -pointOffset, pointOffset ), 
                                                            0, 
                                                            Random.Range( -pointOffset, pointOffset ) );

            m_destination = m_NPC.GetNearestLocationOnNavMesh( pointToGoTo );
            m_NPC.SetDestination( m_destination );
        }

        return true;
    }

    IEnumerator TrackEnemy( )
    {
        GameObject[] scentGOs;

        yield return new WaitUntil( () => m_trackingAbility.SenseOn( ) );

        scentGOs = GameObject.FindGameObjectsWithTag( trackedObjectTag );

        if( scentGOs.Length > 0 )
        {
            m_trackedObject = scentGOs[Random.Range( 0, scentGOs.Length - 1 )];
        }
        else
        {
            m_travelingToLoc = false;
        }

        
    }

    void AttackEnemy( )
    {
        int index;       

        for( index = 0; index < m_spottedEnemies.Count; index++ )
        {
            if ( m_spottedEnemies.Count == 1 || attackEnemyProbability > Random.value )
            {
                m_destination = m_spottedEnemies[index].transform.position;

                m_travelingToLoc = m_NPC.SetDestination( m_destination );

                break;
            }
        }
    }
}

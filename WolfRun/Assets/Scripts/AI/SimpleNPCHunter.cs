using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleNPCHunter : SimpleNPCIntelligence
{
    public string trackedObjectTag = "Scent";
    public float enemySmellAccuracyProbability = 0.5f;
    public float attackEnemyProbability = 0.95f;

    public float searchRange = 50.0f;

    public AudioClip growlClip;
    public AudioClip barkingClip;

    public bool active = true;

    private bool m_enemyScentDetected = false;
    private WolfHowl m_trackingAbility;
    private GameObject m_trackedObject;

    private bool m_enemySighted = false;

    private bool m_intimidated = false;

    private AudioSource m_audioSouce;

    // Use this for initialization
    protected override void Start ()
    {
        base.Start( );

        m_trackingAbility = GetComponent<WolfHowl>( );

        m_audioSouce = GetComponent<AudioSource>( );
	}
	
	protected override void FixedUpdate ()
    {
        if( !active || !m_NPC.CanMove( ) )
        {
            return;
        }

        base.FixedUpdate( );

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
            FollowEnemy( );
        }

        if ( m_enemySighted && !m_intimidated && Vector3.Distance( transform.position, m_destination ) <= 7 )
        {
            StartCoroutine( IntimidationCoroutine( ) );
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

    public void FollowEnemy( )
    {
        float distance;
        float offset;
        Vector3 errorVector = Vector3.zero;

        distance = Vector3.Distance( transform.position, m_trackedObject.transform.position );

        offset = distance * Mathf.Clamp01( 1.0f - Mathf.Min( intelligence, 0.66f ) );

        errorVector.x = Random.Range( -1f * offset, offset );
        errorVector.z = Random.Range( -1f * offset, offset );

        m_destination = ( m_trackedObject.transform.position + errorVector );
        m_NPC.SetDestination( m_NPC.GetNearestLocationOnNavMesh( m_destination ) );
        m_enemySighted = true;
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

        if( m_trackedObject 
            && Vector3.Distance( m_trackedObject.transform.position, transform.position ) < sightDistance )
        {
            return;
        }

        for( index = 0; index < m_spottedEnemies.Count; index++ )
        {
            if ( m_spottedEnemies.Count == 1 || attackEnemyProbability > Random.value )
            {
                m_trackedObject = m_spottedEnemies[index];

                m_destination = m_trackedObject.transform.position;

                m_travelingToLoc = m_NPC.SetDestination( m_destination );

                m_enemySighted = true;

                m_intimidated = false;

                return;
            }
        }

        m_enemySighted = false;
        m_intimidated = false;
    }

    IEnumerator IntimidationCoroutine( )
    {
        float timeUsed = 0, delay;
        AudioClip clip;

        m_NPC.SetCanMove( false );
        m_NPC.Stop( );

        m_intimidated = true;

        delay = Random.Range( 1.5f, 3.5f );        
        

        while ( timeUsed < delay )
        {
            clip = Random.value < 0.5f ? growlClip : barkingClip;

            m_audioSouce.clip = clip;
            m_audioSouce.volume = 1;

            m_audioSouce.Play( );

            yield return new WaitForSeconds( clip.length );

            timeUsed += m_audioSouce.clip.length;
        }

        m_NPC.SetCanMove( true );
        m_NPC.Resume( );
    }
}

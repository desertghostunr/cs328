﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent( typeof( Animator ) )]
[RequireComponent( typeof( UniversalCharacterController ) )]
[RequireComponent( typeof( AudioSource ) )]
public class WolfHowl : MonoBehaviour
{
    public string abilityActivator;
    public string abilityAnimationName;
    public string abilityAnimationStateName;

    public bool playerControlled = true;

    public AudioClip howlSound;

    public float  movementInhibitor = 0.25f;

    private Animator wAnimator;

    private bool abilityInUse = false;

    private UniversalCharacterController m_universalCC;

    private WolfSense senseController;

    private AudioSource wAudio;


    // Use this for initialization
    void Start ()
    {
        wAnimator = GetComponent<Animator>( );
        senseController = GetComponentInChildren<WolfSense>( );
        wAudio = GetComponent<AudioSource>( );

        m_universalCC = GetComponent<UniversalCharacterController>( );
    }
	
	// Update is called once per frame
	void Update ()
    {
        if ( playerControlled && Input.GetButton( abilityActivator ) )
        {
            ActivateAbility( );
        }

        ProcessAbility( );
        ManageMovement( );
    }

    public void ActivateAbility( )
    {
        wAnimator.SetBool( abilityAnimationName, true );        
    }

    public void ProcessAbility( )
    {
        if ( wAnimator.GetBool( abilityAnimationName ) && wAnimator.GetCurrentAnimatorStateInfo( 0 ).IsName( abilityAnimationStateName ) )
        {
            wAnimator.SetBool( abilityAnimationName, false );
            wAudio.clip = howlSound;
            wAudio.Play( );
            abilityInUse = true;
        }

        if ( abilityInUse && !wAnimator.GetCurrentAnimatorStateInfo( 0 ).IsName( abilityAnimationStateName ) )
        {
            if( senseController )
            {
                senseController.ActivateSense( );
            }
            
            abilityInUse = false;
        }
        else if ( abilityInUse )
        {
            //don't move wolf if animation is playing
            m_universalCC.SetCanMove( false );
        }
        else if ( !abilityInUse )
        {
            m_universalCC.SetCanMove( true );
        }
    }

    public void ManageMovement( )
    {
        float localMovementInhibitor;

        if( senseController )
        {
            localMovementInhibitor = senseController.SenseOn( ) ? movementInhibitor : 1.0f;

            m_universalCC.SetMovementInhibitor( localMovementInhibitor );
        }
    }
}

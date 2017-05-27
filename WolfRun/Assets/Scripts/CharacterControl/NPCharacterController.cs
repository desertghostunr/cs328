using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NPCharacterController : UniversalCharacterController
{
    private NavMeshAgent m_agent;

	// Use this for initialization
	protected override void Start ()
    {
        m_agent = GetComponent<NavMeshAgent>( );
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    public override void Move( float forward, float turn, float side )
    {
        
    }
}

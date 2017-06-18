/******************************************************
 * 
 * Copy Right © Andrew Frost 2017, all rights reserved.
 * 
 ******************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(UniversalCharacterController))]
public class TerrainMotionController : MonoBehaviour
{
    public float[] detailWeights;
    public float[] textureWeights;

    public float steepnessWeight = 0.0f;

    public string terrainTag;

    private UniversalCharacterController m_universalCC = null;
    private TerrainInfo m_terrainInformation = null;

    private GameObject m_currentTerrainGO = null;

    public float m_currentWeight { get; private set; } 

	// Use this for initialization
	void Start ()
    {
        m_universalCC = GetComponent<UniversalCharacterController>( );

        m_terrainInformation = FindObjectOfType<TerrainInfo>( );

        m_currentWeight = 1.0f;
	}
	
	
	void Update ( )
    {

		if( !m_terrainInformation || !m_universalCC.Moving( ) )
        {
            return;
        }

        m_currentWeight = m_terrainInformation.getTerrainWeight( transform.position, 
                                                                 textureWeights, 
                                                                 detailWeights, 
                                                                 steepnessWeight );        

        m_universalCC.SetMovementInhibitor( Mathf.Max( 1.0f - m_currentWeight, 0.0f ) );
	}

    private void OnCollisionEnter( Collision collision )
    {
        if( collision.collider.tag == terrainTag )
        {
            if( collision.collider.gameObject != m_currentTerrainGO )
            {
                m_currentTerrainGO = collision.collider.gameObject;
                m_terrainInformation = m_currentTerrainGO.GetComponent<TerrainInfo>( );
            }
        }
    }

    private void OnControllerColliderHit( ControllerColliderHit hit )
    {
        if ( hit.gameObject.tag == terrainTag )
        {
            if ( hit.gameObject != m_currentTerrainGO )
            {
                m_currentTerrainGO = hit.gameObject;
                m_terrainInformation = m_currentTerrainGO.GetComponent<TerrainInfo>( );
            }
        }
    }
}

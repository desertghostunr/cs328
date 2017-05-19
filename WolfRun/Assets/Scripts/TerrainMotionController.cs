using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(UniversalCharacterController))]
public class TerrainMotionController : MonoBehaviour
{
    public float[] detailWeights;
    public float[] textureWeights;

    private UniversalCharacterController m_universalCC = null;
    private TerrainInfo m_terrainInformation = null;

	// Use this for initialization
	void Start ()
    {
        m_universalCC = GetComponent<UniversalCharacterController>( );

        m_terrainInformation = FindObjectOfType<TerrainInfo>( );
	}
	
	// Update is called once per frame
	void Update ()
    {
        float terrainWeight;

		if( !m_terrainInformation || !m_universalCC.Moving( ) )
        {
            return;
        }

        terrainWeight = m_terrainInformation.getTerrainWeight( transform.position, 
                                                               textureWeights, 
                                                               detailWeights );        

        m_universalCC.SetMovementInhibitor( Mathf.Max( terrainWeight, 0.0f ) );
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(UniversalCharacterController))]
[RequireComponent(typeof(TerrainMotionController))]
public class DetectionManager : MonoBehaviour
{
    [Range(0.0f, 1.0f)]
    public float standingCost = 0.5f;

    [Range(0.0f, 1.0f)]
    public float movingCost = 0.5f;

    [Range(0.0f, 1.0f)]
    public float crouchCost = 0.25f;

    //private variables
    private UniversalCharacterController m_universalCharacterController;
    private TerrainMotionController m_terrainMotionController;

	// Use this for initialization
	void Start( )
    {
        m_universalCharacterController = GetComponent<UniversalCharacterController>( );
        m_terrainMotionController = GetComponent<TerrainMotionController>( );
	}

    //returns a detetion weight between 0 and 1 based on stance, movement, and location
    public float DetectionLevel( )
    {
        float detectionLvl = 0.0f;

        //lower stance has a lower cost
        if( m_universalCharacterController.Crouching( ) )
        {
            detectionLvl += crouchCost;
        }
        else
        {
            detectionLvl += standingCost;
        }

        //slower movement has a lower cost
        if ( m_universalCharacterController.Moving( ) ) 
        {
            detectionLvl += movingCost * m_universalCharacterController.NormalizedMovementSpeed01( );
        }

        detectionLvl -= m_terrainMotionController.m_currentWeight;

        return Mathf.Clamp01( detectionLvl );
    }
}

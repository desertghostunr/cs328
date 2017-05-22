/******************************************************
 * 
 * Copy Right © Andrew Frost 2017, all rights reserved.
 * 
 ******************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

[RequireComponent( typeof(TerrainMotionController))]
public class AINavigation : MonoBehaviour
{
    public int mapDim = 51;
    public string terrainTag;

    public float obstacleRefreshTime = 1.0f;

    private TerrainInfo m_terrainInfo;
    private TerrainMotionController m_terrainMotionControl;
    private GameObject m_currentTerrainGO = null;

    private bool m_navMapBuilt = false;
    private bool m_destinationReached = false;
    private bool m_routePlanned = false;
    private bool m_refreshObstacles = true;

    private float[,] m_navigationMap;
    private float[,] m_tmpNavigationMap;

    private Vector3 m_destination;

    private Thread m_buildNavMapThread;

    

	// Use this for initialization
	private void Start ()
    {
        if( mapDim < 0 )
        {
            mapDim = 51;
        }

		if( ( mapDim % 2 ) == 0 )
        {
            mapDim += 1;
        }

        m_terrainInfo = FindObjectOfType<TerrainInfo>( );
        m_terrainMotionControl = GetComponent<TerrainMotionController>( );

        m_navigationMap = new float[ mapDim, mapDim ];
        m_tmpNavigationMap = new float[ mapDim, mapDim ];

        m_buildNavMapThread = new Thread( ( ) => BuildNavMap( ) );
	}
	
	// Update is called once per frame
	private void Update ()
    {
		if( !m_navMapBuilt )
        {
            //build nav map
            m_buildNavMapThread.Start( );            
        }
        else if ( m_navMapBuilt && !m_routePlanned )
        {
            //plan route
        }
        else if ( m_navMapBuilt && m_routePlanned && !m_destinationReached )
        {
            //go to destination
        }
        else if ( m_navMapBuilt && m_routePlanned && m_destinationReached )
        {
            //reset destination
            m_navMapBuilt = false;
            m_routePlanned = false;
            m_destinationReached = false;
        }

    }

    private void FixedUpdate( )
    {
        if ( m_navMapBuilt && m_refreshObstacles )
        {
            m_navigationMap = m_tmpNavigationMap; //clear obstacles
            AddObstaclesToNavMap( ); //add obstacles
        }
    }

    public void BuildNavMap( )
    {
        Vector3 position;
        int x, y;

        m_navMapBuilt = false;

        for( y = 0; y < m_tmpNavigationMap.GetLength( 0 ); y++ )
        {
            for ( x = 0; x < m_tmpNavigationMap.GetLength( 1 ); x++ )
            {
                position.x = x;
                position.y = 0;
                position.z = y;

                m_tmpNavigationMap[y, x] 
                    = m_terrainInfo.getTerrainWeight( position, 
                                                      m_terrainMotionControl.detailWeights, 
                                                      m_terrainMotionControl.textureWeights, 
                                                      m_terrainMotionControl.steepnessWeight );
            }
        }

        m_navigationMap = m_tmpNavigationMap;

        m_navMapBuilt = true;
    }

    public void AddObstaclesToNavMap( )
    {
        int index;
        Collider[] obstacles;

        StartCoroutine( RefreshTimer( ) );

        obstacles = Physics.OverlapSphere( transform.position, mapDim );

        for( index = 0; index < obstacles.Length; index++ )
        {
            SetMapWeight( ref m_navigationMap, obstacles[index].transform.position, 0 );
        }
    }

    public void PlanRoute( )
    {
        if( !m_navMapBuilt )
        {
            return;
        }


    }

    public IEnumerator RefreshTimer( )
    {
        m_refreshObstacles = false;

        yield return new WaitForSeconds( obstacleRefreshTime );

        m_refreshObstacles = true;
    }

    public static bool SetMapWeight( ref float[,] map, Vector3 position, float weight )
    {
        if( (int) position.x < 0 || (int) position.z < 0 
            || (int) position.x >= map.GetLength( 1 ) 
            || (int) position.z >= map.GetLength( 0 ) )
        {
            return false;
        }

        map[( int ) position.z, ( int ) position.x] = weight;

        return true;
    }

    private void OnCollisionEnter( Collision collision )
    {
        if ( collision.collider.tag == terrainTag )
        {
            if ( collision.collider.gameObject != m_currentTerrainGO )
            {
                m_currentTerrainGO = collision.collider.gameObject;
                m_terrainInfo = m_currentTerrainGO.GetComponent<TerrainInfo>( );
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
                m_terrainInfo = m_currentTerrainGO.GetComponent<TerrainInfo>( );
            }
        }
    }
}

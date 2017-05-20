using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Terrain))]
public class TerrainInfo : MonoBehaviour
{

    private Terrain m_terrain;
    private int[,,] m_detailMap;
    private float[,,] m_textureMap;

    // Use this for initialization
    void Start ()
    {
        int index;
        List<int[,]> tmpDetailList;

        m_terrain = GetComponent<Terrain>( );

        if( !m_terrain )
        {
            return;
        }

        tmpDetailList = new List<int[ , ]>( );

        //set detail layers
        for( index = 0; index < m_terrain.terrainData.detailPrototypes.Length; index++ )
        {
            tmpDetailList.Add( m_terrain.terrainData.GetDetailLayer( 0, 0, 
                                                                     m_terrain.terrainData.detailWidth, 
                                                                     m_terrain.terrainData.detailHeight, 
                                                                     index ) );

        }

        setDetailMap( tmpDetailList );

        tmpDetailList.Clear( );

        //set alpha textures
        m_textureMap = m_terrain.terrainData.GetAlphamaps( 0, 0, 
                                                           m_terrain.terrainData.alphamapWidth, 
                                                           m_terrain.terrainData.alphamapHeight );


	}

    public bool setDetailMap( List<int[,]> detailMapList, bool binary = true )
    {
        int y, x, inner;

        if( detailMapList.Count == 0 )
        {
            return false;
        }

        y = detailMapList[0].GetLength( 0 );
        x = detailMapList[0].GetLength( 1 );

        //check if able to convert to a multidimensional array
        for( inner = 0; inner < detailMapList.Count; inner++ )
        {
            if( detailMapList[ inner ].GetLength(0) != y 
                || detailMapList[ inner].GetLength(1) != x )
            {
                return false;
            }
        }

        m_detailMap = new int[detailMapList[0].GetLength( 0 ), 
                              detailMapList[1].GetLength( 1 ), 
                              detailMapList.Count];

        for( y = 0; y < m_detailMap.GetLength(0); y++ )
        {
            for( x = 0; x < m_detailMap.GetLength(1); x++ )
            {
                for( inner = 0; inner < m_detailMap.GetLength(2); inner++ )
                {
                    if( binary )
                    {
                        m_detailMap[y, x, inner] = ConvertBinary( detailMapList[inner][y, x], 0, 1 );
                    }
                    else
                    {
                        m_detailMap[y, x, inner] = detailMapList[inner][y, x];
                    }
                }
            }
        }

        return true;
    }

    public Type ConvertBinary<Type>( Type value, Type zero, Type one )
    {
        return value.Equals( zero ) ? zero : one;
    }

    public void setTextureMap( float[ , , ] alphaMap )
    {
        m_textureMap = alphaMap;
    }

    public float getTerrainWeight( Vector3 position, float[] textureWeights, float[] detailWeights, float steepnessWeight )
    {
        int x, y, index;

        float weightSum = 0.0f;
        float maxWeight = m_detailMap.GetLength( 2 )  + m_textureMap.GetLength( 2 ) + 1;
        float tmpWeight;

        if( !m_terrain )
        {
            return 0;
        }


        //detail weight
        x = GetDetailMapX( position );
        y = GetDetailMapY( position );

        if( y >= m_detailMap.GetLength(0) || x >= m_detailMap.GetLength(1) )
        {
            return 0;
        }

        for( index = 0; index < Mathf.Min( m_detailMap.GetLength( 2 ), detailWeights.Length ); index++ )
        {
            tmpWeight = GetInterpolatedValue( m_detailMap, x, y, index );

            weightSum += detailWeights[index] * maxWeight * tmpWeight;
        }


        //alpha weight
        x = GetAlphaMapX( position );
        y = GetAlphaMapY( position );

        if ( y >= m_textureMap.GetLength( 0 ) || x >= m_textureMap.GetLength( 1 ) )
        {
            return maxWeight == 0 ? 0 : 1.0f - Mathf.Min( 1.0f, Mathf.Max( weightSum / maxWeight, 0 ) );
        }

        for ( index = 0; index < Mathf.Min( m_textureMap.GetLength( 2 ), textureWeights.Length ); index++ )
        {
            tmpWeight = GetInterpolatedValue( m_textureMap, x, y, index );

            weightSum += textureWeights[index] * maxWeight * tmpWeight;
        }

        //terrain steepness
        weightSum += steepnessWeight * maxWeight * ( m_terrain.terrainData.GetSteepness( GetTerrainNormalizedX( position ), GetTerrainNormalizedZ( position ) ) / 90.0f );
        
        return maxWeight == 0 ? 0 : 1.0f - Mathf.Min( 1.0f, Mathf.Max( weightSum / maxWeight, 0 ) );
    }

    public float GetInterpolatedValue( float[,,] matrix, int x, int y, int layer)
    {
        int xIter, yIter;
        float sum = 0;
        float counter = 0;

        if( layer >= matrix.GetLength( 2 ) )
        {
            return 0;
        }

        for( yIter = Mathf.Max( y - 1, 0 ); 
             yIter < Mathf.Min( y + 1, matrix.GetLength( 0 ) - 1 ); 
             yIter++ )
        {
            for ( xIter = Mathf.Max( x - 1, 0 ); 
                  xIter < Mathf.Min( x + 1, matrix.GetLength( 1 ) - 1 ); 
                  xIter++ )
            {
                sum += matrix[yIter, xIter, layer];
                counter++;
            }
        }

        return ( sum / counter );
    }

    public float GetInterpolatedValue( int[ , , ] matrix, int x, int y, int layer )
    {
        int xIter, yIter;
        float sum = 0;
        float counter = 0;

        if ( layer >= matrix.GetLength( 2 ) )
        {
            return 0;
        }

        for ( yIter = Mathf.Max( y - 1, 0 );
             yIter < Mathf.Min( y + 1, matrix.GetLength( 0 ) - 1 );
             yIter++ )
        {
            for ( xIter = Mathf.Max( x - 1, 0 );
                  xIter < Mathf.Min( x + 1, matrix.GetLength( 1 ) - 1 );
                  xIter++ )
            {
                sum += matrix[yIter, xIter, layer];
                counter++;
            }
        }

        return ( sum / counter );
    }

    public float GetTerrainNormalizedX( Vector3 position )
    {
        return ( position.x - m_terrain.transform.position.x ) / m_terrain.terrainData.size.x;
    }

    public float GetTerrainNormalizedZ( Vector3 position )
    {
        return ( position.z - m_terrain.transform.position.z ) / m_terrain.terrainData.size.z;
    }

    public int GetDetailMapX( Vector3 position )
    {
        return ( int ) ( m_terrain.terrainData.detailWidth * ( ( 1.0f / m_terrain.terrainData.size.x ) * ( position.x - m_terrain.transform.position.x ) ) );
    }

    public int GetDetailMapY( Vector3 position )
    {
        return ( int ) ( m_terrain.terrainData.detailHeight * ( ( 1.0f / m_terrain.terrainData.size.z ) * ( position.z - m_terrain.transform.position.z ) ) );
    }

    public int GetAlphaMapX( Vector3 position )
    {
        return ( int ) ( m_terrain.terrainData.alphamapWidth * ( ( 1.0f / m_terrain.terrainData.size.x ) * ( position.x - m_terrain.transform.position.x ) ) );
    }

    public int GetAlphaMapY( Vector3 position )
    {
        return ( int ) ( m_terrain.terrainData.alphamapHeight * ( ( 1.0f / m_terrain.terrainData.size.z ) * ( position.z - m_terrain.transform.position.z ) ) );
    }

}

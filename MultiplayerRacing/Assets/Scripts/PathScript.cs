﻿/********************************
 * 
 * Copy Right © Andrew Frost 2017, all rights reserved.
 * 
 *******************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PathScript : MonoBehaviour
{

    public float xStartCenter = 50;
    public float yStartCenter = 150;

    public float xRange = 25;

    public float yRange = 100;

    public float baseHeight = 25.0f;

    public float heightDelta = 5.0f;

    public int maxIterations = 50;

    public float tolerance = 100.0f;

    public float stepDist = 20.0f;

    public float startAndEndMinDist = 150;

    public float roomToPathRatio = 3.0f;

    public GameObject raider = null;  

    private TerrainData tData;

    private Vector2 start, end;

    public float[ , ] heightMap;

    public Vector3 startWorld, endWorld;

    private Vector2[ ] points2D;

    public Vector3[ ] points3D;

    public float PerlinFactor = 0.01f;

    public float sigma = 3.0f;

    public float erosionSigma = 15.0f;

    public GameObject[] players;

    private GameManager gameManager;

    static private int ROW_ORIENTATION = 0;
    static private int COL_ORIENTATION = 1;

    // Use this for initialization
    void Start ()
    {
        int its = 0;        

        tData = GetComponent<Terrain>( ).terrainData;

        while( !GeneratePath( ) );
       

        tData.SetHeights( 0, 0, heightMap );

        //convert height map points to world
        startWorld = new Vector3( start.y, tData.GetHeight( (int)start.y, (int)start.x ), start.x );

        endWorld = new Vector3( end.y, tData.GetHeight( ( int ) end.y, ( int ) end.x ), end.x );

        points3D = new Vector3[ maxIterations ];

        for( its = 0; its < points2D.Length; its++ )
        {
            points3D[ its ] = ( new Vector3( points2D[ its ].y, 
                                tData.GetHeight( ( int ) points2D[ its ].y, 
                                                 ( int ) points2D[ its ].x ), 
                                points2D[ its ].x ) );
        }


        gameManager = FindObjectOfType<GameManager>( );

        if( gameManager == null )
        {
            Debug.Log( "Unable to acquire game manager!" );
        }

        for( its = 0; its < players.Length; its++)
        {
            players[its].transform.position = new Vector3(startWorld.x + Random.Range(-2.0f, 2.0f), players[its].transform.position.y, startWorld.z + Random.Range(-2.0f, 2.0f));
        }
    }
	
	// Update is called once per frame
	void Update ()
    {
        
	}

    bool GeneratePath( )
    {

        int rValue, lastSelect, its = 0;
        float rChange, slope, yIntersect;

        float minHeight;

        int currX, currY, index;

        Vector2 position, nextPosition/*, tmpPoint*/;
        //List<Vector2> tmpPoints;

        List<Vector2> pointsList = new List<Vector2>( );
        
        bool[ , ] visitedMap;

        bool[,] erosionMap;

        float[] mask;

        start.x = Random.Range( xStartCenter - xRange, xStartCenter + xRange );
        start.y = Random.Range( yStartCenter - yRange, yStartCenter + yRange );

        heightMap = new float[ tData.heightmapResolution, tData.heightmapResolution ];
        visitedMap = new bool[ tData.heightmapResolution, tData.heightmapResolution ];
        erosionMap = new bool[ tData.heightmapResolution, tData.heightmapResolution ];

        

        for ( currX = 0; currX < tData.heightmapResolution; currX++ )
        {
            for ( currY = 0; currY < tData.heightmapResolution; currY++ )
            {
                heightMap[ currX, currY ] = baseHeight 
                                           * Mathf.PerlinNoise( currX * PerlinFactor 
                                                                      + Random.Range( -PerlinFactor, PerlinFactor ), 
                                                                currY * PerlinFactor 
                                                                      + Random.Range( -PerlinFactor, PerlinFactor ) );
                visitedMap[ currX, currY ] = false;
            }
        }

        points2D = new Vector2[ maxIterations ];

        position = start;

        points2D[ 0 ] = start;
        

        lastSelect = 9;

        while (  its < maxIterations - 1 )
        {
            its++;


            rValue = ( int ) ( 8.0f * Random.Range( 0.0f, 1.0f ) );
            rChange = Random.Range( -stepDist / 2.0f, stepDist / 2.0f ) + stepDist;

            if ( rValue + 4 == lastSelect || rValue - 4 == lastSelect ) //if opposite
            {
                rValue = lastSelect;
            }

            nextPosition = SelectNextPosition( rValue, rChange, position );            

            if ( nextPosition.y + tolerance > tData.heightmapResolution - 75 
                 || nextPosition.x + tolerance > tData.heightmapResolution - 75
                 || nextPosition.y - tolerance < 75
                 || nextPosition.x - tolerance < 75 
                 || Vector2.Distance( start, nextPosition ) < ( Vector2.Distance(start, position ) -  ( rChange * 0.3f ) ) )
            {
                nextPosition = position;
                continue;
            }            

            if( rValue == 0 || rValue == 4 )
            {
                for( index = ( int ) position.x; index < ( int ) nextPosition.x + 1; index++ )
                {
                    pointsList.Add( new Vector2( index, position.y ) );
                }

                for( index = ( int ) nextPosition.x; index < ( int ) position.x + 1; index++ )
                {
                    pointsList.Add( new Vector2( index, nextPosition.y ) );
                }
            }
            else if( rValue == 2 || rValue == 6 )
            {
                for( index = ( int ) position.y; index < ( int ) nextPosition.y + 1; index++ )
                {
                    pointsList.Add( new Vector2( position.x, index ) );
                }

                for( index = ( int ) nextPosition.y; index < ( int ) position.y + 1; index++ )
                {
                    pointsList.Add( new Vector2( position.x, index ) );
                }
            }
            else
            {
                slope = ( position.y - nextPosition.y ) / ( position.x - nextPosition.x );
                yIntersect = position.y - slope * position.x;

                for( index = ( int ) position.x; index < ( int ) nextPosition.x + 1; index++ )
                {
                    pointsList.Add( new Vector2( index, slope * index + yIntersect ) );
                }

                for( index = ( int ) nextPosition.x; index < ( int ) position.x + 1; index++ )
                {
                    pointsList.Add( new Vector2( index, slope * index + yIntersect ) );
                }
            }

            lastSelect = rValue;

            points2D[ its ] = ( nextPosition );

            position = nextPosition;

        }

        end = position;

        points2D[ maxIterations - 1 ] = end;

        if( ( Vector2.Distance( start, end ) < startAndEndMinDist ) )
        {
            return false;
        }

        //"rasterize" the line ///////////////////
        //get the minimum point
        minHeight = Mathf.Infinity;

        
       for( currX = 0; currX < tData.heightmapResolution; currX++ )
       {
            for( currY = 0; currY < tData.heightmapResolution; currY++ )
            {
                if( minHeight > heightMap[ currX, currY ] )
                {
                    minHeight = heightMap[ currX, currY ];
                }
            }
        }

        //perform the rasterization
        for( index = 0; index < pointsList.Count; index++ )
        {
            rChange = Random.Range( -tolerance * 0.25f, tolerance * 0.5f );
            
            for( currX = ( int ) Mathf.Max( pointsList[ index ].x - roomToPathRatio * tolerance, 10 );
                 currX < ( int ) Mathf.Min( pointsList[ index ].x + roomToPathRatio * tolerance, tData.heightmapResolution - 10 );
                 currX++ )
            {
                for( currY = ( int ) Mathf.Max( pointsList[ index ].y - roomToPathRatio * tolerance, 10 );
                     currY < ( int ) Mathf.Min( pointsList[ index ].y + roomToPathRatio * tolerance, tData.heightmapResolution - 10 );
                     currY++ )
                {
                    if( !visitedMap[ currX, currY ] && ( tolerance + rChange > Mathf.Sqrt( ( currX - pointsList[ index ].x ) * ( currX - pointsList[ index ].x ) + ( currY - pointsList[ index ].y ) * ( currY - pointsList[ index ].y ) ) ) )
                    {
                        heightMap[ currX, currY ] = minHeight - heightDelta;
                        visitedMap[ currX, currY ] = true;
                    }
                    else if( tolerance + rChange + 5 > Mathf.Sqrt( ( currX - pointsList[ index ].x ) * ( currX - pointsList[ index ].x ) + ( currY - pointsList[ index ].y ) * ( currY - pointsList[ index ].y ) ) )
                    {
                        erosionMap[ currX, currY ] = true;
                    }
                }
            }
        }

        //carve areas around the start and end point

        for( currX = ( int ) Mathf.Max( start.x - roomToPathRatio * tolerance, 10 );
                 currX < ( int ) Mathf.Min( start.x + roomToPathRatio * tolerance, tData.heightmapResolution - 10 );
                 currX++ )
        {
            for( currY = ( int ) Mathf.Max( start.y - roomToPathRatio * tolerance, 10 );
                currY < ( int ) Mathf.Min( start.y + roomToPathRatio * tolerance, tData.heightmapResolution - 10 );
                currY++ )            
            {

                if( !visitedMap[ currX, currY ] && ( roomToPathRatio * tolerance > Mathf.Sqrt( ( currX - start.x ) * ( currX - start.x ) + ( currY - start.y ) * ( currY - start.y ) ) ) )
                {
                    heightMap[ currX, currY ] = minHeight - heightDelta;
                    visitedMap[ currX, currY ] = true;
                }
                else if( roomToPathRatio * tolerance + 5 > Mathf.Sqrt( ( currX - start.x ) * ( currX - start.x ) + ( currY - start.y ) * ( currY - start.y ) ) )
                {
                    erosionMap[ currX, currY ] = true;
                }
            }
        }
        for( currX = ( int ) Mathf.Max( end.x - roomToPathRatio * tolerance, 10 );
                 currX < ( int ) Mathf.Min( end.x + roomToPathRatio * tolerance, tData.heightmapResolution - 10 );
                 currX++ )
        {
            for( currY = ( int ) Mathf.Max( end.y - roomToPathRatio * tolerance, 10 );
                 currY < ( int ) Mathf.Min( end.y + roomToPathRatio * tolerance, tData.heightmapResolution - 10 );
                 currY++ )
            {
            
                if( !visitedMap[ currX, currY ] && ( roomToPathRatio * tolerance > Mathf.Sqrt( ( currX - end.x ) * ( currX - end.x ) + ( currY - end.y ) * ( currY - end.y ) ) ) )
                {
                    heightMap[ currX, currY ] = minHeight - heightDelta;
                    visitedMap[ currX, currY ] = true;
                }
                else if( roomToPathRatio * tolerance + 5 > Mathf.Sqrt( ( currX - end.x ) * ( currX - end.x ) + ( currY - end.y ) * ( currY - end.y ) ) )
                {
                    erosionMap[ currX, currY ] = true;
                }
            }
        }

        mask = new float[ ( int ) Mathf.Max( sigma * 5, 5 ) ];

        Generate1DGaussianMask( sigma, mask );

        Convolve1DMask( heightMap, mask, visitedMap );

        mask = new float[ ( int ) Mathf.Max( erosionSigma * 5, 5 ) ];

        Generate1DGaussianMask( sigma, mask );

        Convolve1DMask( heightMap, mask, visitedMap );

        return true;
    }

    static public Vector2 SelectNextPosition( int rValue, float rChange, Vector2 position )
    {
        Vector2 nextPosition;

        nextPosition = position;

        switch( rValue )
        {
            case 0: //+ 0                    
                nextPosition.x += rChange;
                break;

            case 1:// + + 
                nextPosition.x += rChange * Random.Range( 0.8f, 1.2f );
                nextPosition.y += rChange * Random.Range( 0.8f, 1.2f );
                break;

            case 2:// 0 +                     
                nextPosition.y += rChange;
                break;

            case 3:// - +  
                nextPosition.x -= rChange * Random.Range( 0.8f, 1.2f );
                nextPosition.y += rChange * Random.Range( 0.8f, 1.2f );
                break;

            case 4: //- 0                    
                nextPosition.x -= rChange;
                break;

            case 5: //- -                    
                nextPosition.x -= rChange * Random.Range( 0.8f, 1.2f );
                nextPosition.y -= rChange * Random.Range( 0.8f, 1.2f );
                break;

            case 6: // 0 -                    
                nextPosition.y -= rChange;
                break;

            case 7: // + - 
                nextPosition.x += rChange * Random.Range( 0.8f, 1.2f ); ;
                nextPosition.y -= rChange * Random.Range( 0.8f, 1.2f ); ;
                break;
            default:
                nextPosition.x += rChange;
                rValue = 0;
                break;
        }

        return nextPosition;
    }

    static public  void Generate1DGaussianMask( float sigma, float[] mask )
    {
        int index;

        float cst, tssq, x, sum;

        cst = 1.0f / ( sigma * Mathf.Sqrt( 2.0f * Mathf.PI ) );

        tssq = 1.0f / ( 2 * sigma * sigma );

        sum = 0.0f;

        for( index = 0; index < mask.Length; index++ )
        {
            x = ( float ) ( index - mask.Length / 2 );
            mask[ index ] = ( cst * Mathf.Exp( -( x * x * tssq ) ) );

            sum += mask[ index ];
        }       

        for( index = 0; index < mask.Length; index++ )
        {
            mask[ index ] /= sum;
        }

    }

    static public void Convolve1DMask( float[,] image, float[ ] mask, bool[,] applicationMask )
    {
        int x, y;

        float[,] tmp = new float[ image.GetLength( 0 ), image.GetLength( 1 ) ];

        for( x = 0; x < image.GetLength( 0 ); x++ )
        {
            for( y = 0; y < image.GetLength( 1 ); y++ )
            {
                if( applicationMask[ x, y ] )
                {
                    Apply1DMaskOnPoint( tmp, image, mask, x, y, ROW_ORIENTATION );
                }
                else
                {
                    tmp[ x, y ] = image[ x, y ];
                }
            }
        }

        for( x = 0; x < image.GetLength( 0 ); x++ )
        {
            for( y = 0; y < image.GetLength( 1 ); y++ )
            {
                if( applicationMask[ x, y ] )
                {
                    Apply1DMaskOnPoint( image, tmp, mask, x, y, COL_ORIENTATION );
                }
                else
                {
                    image[ x, y ] = tmp[ x, y ];
                }
            }
        }


    }

    static public void Apply1DMaskOnPoint( float[,] dImage, float[,] sImage, float[] mask, int x, int y, int orientation )
    {
        float acc = 0.0f;
        int maskCenter, tX, tY;
        int index, offset;

        maskCenter = mask.Length / 2;

        acc = 0.0f;

        if( orientation == ROW_ORIENTATION )
        {
            for( index = 0; index < mask.Length; index++ )
            {
                offset = index - maskCenter;

                tX = x;
                tY = y;

                tX += offset;

                tX = WrapPointX( sImage, tX );
                tY = WrapPointY( sImage, tY );

                acc += mask[ index ] * sImage[ tX, tY ];
            }
        }
        else
        {
            for( index = 0; index < mask.Length; index++ )
            {
                offset = index - maskCenter;

                tX = x;
                tY = y;

                tY += offset;

                tX = WrapPointX( sImage, tX );
                tY = WrapPointY( sImage, tY );

                acc += mask[ index ] * sImage[ tX, tY ];
            }
        }

        dImage[ x, y ] = acc;

    }

    static public int WrapPointX( float[,] image, int x)
    {
        return ModuloWrap( x, image.GetLength( 0 ) );
    }

    static public int WrapPointY( float[,] image, int y )
    {
        return ModuloWrap( y, image.GetLength( 1 ) );
    }

    static public int ModuloWrap( int val, int max )
    {
        int retVal;

        retVal = val % max;

        if( retVal < 0 )
        {
            retVal += max;
        }

        return retVal;
    }

    public void PlaceRaider( )
    {
        GameObject tmp;
        Movement mvment;

        tmp = Instantiate( raider );

        mvment = tmp.GetComponent<Movement>( );

        mvment.SetPath( points3D, tolerance, endWorld, gameManager );

        
    }
}

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

    public int minIterations = 500;

    public int maxIterations = 1000;

    public float tolerance = 100.0f;

    public float stepDist = 20.0f;

    public float startAndEndMinDist = 150;

    public float roomToPathRatio = 3.0f;

    public GameObject raider = null;  

    private TerrainData tData;

    private Terrain terrain;

    private Vector2 start, end;

    public float[ , ] heightMap;

    public Vector3 startWorld, endWorld;

    private Vector2[ ] points2D;

    public Vector3[ ] points3D;

    public float PerlinFactor = 0.01f;

    public float sigma = 3.0f;

    public float erosionSigma = 15.0f;

    public GameObject[] players;

    public GameObject tree;

    public int numberOfTrees = 100;

    private GameManager gameManager;

    private bool[ , ] visitedMap;

    private bool[,] erosionMap;

    static private int ROW_ORIENTATION = 0;
    static private int COL_ORIENTATION = 1;

    // Use this for initialization
    void Start ()
    {
        int its = 0;

        terrain = GetComponent<Terrain>( );

        tData = terrain.terrainData;        

        if( !GeneratePath( ) )
        {
            Debug.Log( "Resorting to default path" );
        }
       

        tData.SetHeights( 0, 0, heightMap );

        //convert height map points to world
        startWorld = new Vector3( start.x, tData.GetHeight( (int)start.x, (int)start.y ), start.y );

        endWorld = new Vector3( end.x, tData.GetHeight( ( int ) end.x, ( int ) end.y ), end.y );

        points3D = new Vector3[ maxIterations ];

        for( its = 0; its < points2D.Length; its++ )
        {
            points3D[ its ] = ( new Vector3( points2D[ its ].x, 
                                             tData.GetHeight( ( int ) points2D[ its ].x, 
                                                              ( int ) points2D[ its ].y ), 
                                             points2D[ its ].y ) );
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

        SpawnTrees( numberOfTrees );
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

        float shift;

        

        float[] mask;
        float[,] mask2D;

        start.x = Random.Range( xStartCenter - xRange, xStartCenter + xRange );
        start.y = Random.Range( yStartCenter - yRange, yStartCenter + yRange );

        if ( start.y + tolerance > tData.heightmapResolution - 75
             || start.x + tolerance > tData.heightmapResolution - 75
             || start.y - tolerance < 75
             || start.x - tolerance < 75 )
        {
            start.x = Mathf.Min( start.x, ( float ) tData.heightmapResolution - 75 - tolerance );
            start.x = Mathf.Max( start.x, ( float ) 75 + tolerance );
            start.y = Mathf.Min( start.y, ( float ) tData.heightmapResolution - 75 - tolerance );
            start.y = Mathf.Max( start.y, ( float ) 75 + tolerance );
        }


        heightMap = new float[ tData.heightmapResolution, tData.heightmapResolution ];
        visitedMap = new bool[ tData.heightmapResolution, tData.heightmapResolution ];
        erosionMap = new bool[ tData.heightmapResolution, tData.heightmapResolution ];

        shift = Random.Range( 0, 10000 );

        
        for ( currY = 0; currY < tData.heightmapResolution; currY++ )
        {
            for ( currX = 0; currX < tData.heightmapResolution; currX++ )
            {
                heightMap[currY, currX] = baseHeight 
                                           * Mathf.PerlinNoise( currX * PerlinFactor + shift
                                                                      + Random.Range( -PerlinFactor, PerlinFactor ), 
                                                                currY * PerlinFactor + shift 
                                                                      + Random.Range( -PerlinFactor, PerlinFactor ) );
                visitedMap[currY, currX] = false;
                erosionMap[ currY, currX] = false;
            }
        }

        points2D = new Vector2[ maxIterations ];

        position = start;

        points2D[ 0 ] = start;
        

        lastSelect = 9;

        its = 0;

        while ( its < maxIterations - 1 )
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
                 || nextPosition.x - tolerance < 75 )
            {
                nextPosition.x = Mathf.Min( nextPosition.x, ( float ) tData.heightmapResolution - 75 - tolerance );
                nextPosition.x = Mathf.Max( nextPosition.x, ( float ) 75 + tolerance );
                nextPosition.y = Mathf.Min( nextPosition.y, ( float ) tData.heightmapResolution - 75 - tolerance );
                nextPosition.y = Mathf.Max( nextPosition.y, ( float ) 75 + tolerance );
            }            

            if( Vector2.Distance( start, nextPosition ) < ( Vector2.Distance( start, position ) - ( rChange * 0.3f ) ) )
            {
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

            if( ( its > minIterations ) 
                && ( Vector2.Distance( start, nextPosition ) >= startAndEndMinDist ) )
            {
                break;
            }
        }

        end = position;

        points2D[ maxIterations - 1 ] = end;

        if( ( Vector2.Distance( start, end ) < startAndEndMinDist ) )
        {
            Debug.Log( "Dist: " + Vector2.Distance( start, end ) );
            return false;
        }

        //"rasterize" the line ///////////////////
        //get the minimum point
        minHeight = Mathf.Infinity;

        
       
        for ( currY = 0; currY < tData.heightmapResolution; currY++ )
        {
            for ( currX = 0; currX < tData.heightmapResolution; currX++ )
            {
                if( minHeight > heightMap[currY, currX] )
                {
                    minHeight = heightMap[currY, currX];
                }
            }
        }

        
        for ( currY = 0; currY < tData.heightmapResolution; currY++ )
        {
            for ( currX = 0; currX < tData.heightmapResolution; currX++ )
            {
                if ( heightMap[currY, currX] < minHeight + heightDelta )
                {
                    heightMap[currY, currX] = minHeight + heightDelta;
                }
            }
        }

        minHeight = Mathf.Max( 0.0f, minHeight );

        //perform the rasterization
        for ( index = 0; index < pointsList.Count; index++ )
        {
            rChange = Random.Range( -tolerance * 0.25f, tolerance * 0.5f );

            for ( currY = ( int ) Mathf.Max( pointsList[index].y - roomToPathRatio * 2 * tolerance, 10 );
                  currY < ( int ) Mathf.Min( pointsList[index].y + roomToPathRatio * 2 * tolerance, tData.heightmapResolution - 10 );
                  currY++ )
            {
                for ( currX = ( int ) Mathf.Max( pointsList[ index ].x - roomToPathRatio * 2 * tolerance, 10 );
                      currX < ( int ) Mathf.Min( pointsList[ index ].x + roomToPathRatio * 2 * tolerance, tData.heightmapResolution - 10 );
                      currX++ )
                {                
                    if( ( tolerance + rChange > Mathf.Sqrt( ( currX - pointsList[ index ].x ) * ( currX - pointsList[ index ].x ) + ( currY - pointsList[ index ].y ) * ( currY - pointsList[ index ].y ) ) ) )
                    {
                        heightMap[currY, currX] = Mathf.SmoothStep( minHeight, heightMap[currY, currX], Vector2.Distance( pointsList[index], new Vector2( currX, currY ) ) / ( 1.5f * tolerance ) );
                        visitedMap[currY, currX] = true;
                        erosionMap[currY, currX] = true;
                    }
                    else if( tolerance + rChange + 20 > Mathf.Sqrt( ( currX - pointsList[ index ].x ) * ( currX - pointsList[ index ].x ) + ( currY - pointsList[ index ].y ) * ( currY - pointsList[ index ].y ) ) )
                    {
                        erosionMap[currY, currX] = true;
                    }
                }
            }
        }

        //carve areas around the start and end point

        for ( currY = ( int ) Mathf.Max( start.y - roomToPathRatio * 2 * tolerance, 10 );
              currY < ( int ) Mathf.Min( start.y + roomToPathRatio * 2 * tolerance, tData.heightmapResolution - 10 );
              currY++ )
        {
            for ( currX = ( int ) Mathf.Max( start.x - roomToPathRatio * 2 * tolerance, 10 );
                  currX < ( int ) Mathf.Min( start.x + roomToPathRatio * 2 * tolerance, tData.heightmapResolution - 10 );
                  currX++ )
            {
                if( ( roomToPathRatio * tolerance > Mathf.Sqrt( ( currX - start.x ) * ( currX - start.x ) + ( currY - start.y ) * ( currY - start.y ) ) ) )
                {
                    heightMap[currY, currX] = Mathf.SmoothStep( minHeight, heightMap[currY, currX], Vector2.Distance( start, new Vector2( currX, currY ) ) / ( 1.5f * roomToPathRatio * tolerance ) );
                    visitedMap[currY, currX] = true;
                    erosionMap[currY, currX] = true;
                }
                else if( roomToPathRatio * tolerance + 20 > Mathf.Sqrt( ( currX - start.x ) * ( currX - start.x ) + ( currY - start.y ) * ( currY - start.y ) ) )
                {
                    erosionMap[currY, currX] = true;
                }
            }
        }

        for ( currY = ( int ) Mathf.Max( end.y - roomToPathRatio * 2 * tolerance, 10 );
              currY < ( int ) Mathf.Min( end.y + roomToPathRatio * 2 * tolerance, tData.heightmapResolution - 10 );
              currY++ )
        {
            for ( currX = ( int ) Mathf.Max( end.x - roomToPathRatio * 2 * tolerance, 10 );
                  currX < ( int ) Mathf.Min( end.x + roomToPathRatio * 2 * tolerance, tData.heightmapResolution - 10 );
                  currX++ )
            {
            
                if( ( roomToPathRatio * tolerance > Mathf.Sqrt( ( currX - end.x ) * ( currX - end.x ) + ( currY - end.y ) * ( currY - end.y ) ) ) )
                {
                    heightMap[currY, currX] = Mathf.SmoothStep( minHeight, heightMap[currY, currX], Vector2.Distance( end, new Vector2( currX, currY ) ) / ( 1.5f * roomToPathRatio * tolerance ) );
                    visitedMap[currY, currX] = true;
                    erosionMap[currY, currX] = true;
                }
                else if( roomToPathRatio * tolerance + 20 > Mathf.Sqrt( ( currX - end.x ) * ( currX - end.x ) + ( currY - end.y ) * ( currY - end.y ) ) )
                {
                    erosionMap[currY, currX] = true;
                }
            }
        }

        mask2D = new float[( int ) Mathf.Max( erosionSigma * 5, 5 ), ( int ) Mathf.Max( erosionSigma * 5, 5 )];

        Generate2DGaussian( mask2D, erosionSigma, ( int ) Mathf.Max( erosionSigma * 5, 5 ) );

        ConvolveErosiveMask( heightMap, mask2D, erosionMap );

        mask = new float[ ( int ) Mathf.Max( sigma * 5, 5 ) ];

        Generate1DGaussianMask( sigma, mask );

        Convolve1DMask( heightMap, mask, visitedMap );

        Convolve1DMask( heightMap, mask, erosionMap );

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

        for( y = 0; y < image.GetLength( 0 ); y++ )
        {
            for( x = 0; x < image.GetLength( 1 ); x++ )
            {
                if( applicationMask[ y, x ] )
                {
                    Apply1DMaskOnPoint( tmp, image, mask, x, y, ROW_ORIENTATION );
                }
                else
                {
                    tmp[ y, x ] = image[ y, x ];
                }
            }
        }

        for( y = 0; y < image.GetLength( 0 ); y++ )
        {
            for( x = 0; x < image.GetLength( 1 ); x++ )
            {
                if( applicationMask[ y, x ] )
                {
                    Apply1DMaskOnPoint( image, tmp, mask, x, y, COL_ORIENTATION );
                }
                else
                {
                    image[ y, x ] = tmp[ y, x ];
                }
            }
        }


    }

    static public void ConvolveErosiveMask( float[ , ] image, float[ , ] mask, bool[ , ] applicationMask )
    {
        int x, y;

        float[ , ] tmpImage = image;

        for ( y = 0; y < image.GetLength( 0 ); y++ )
        {
            for ( x = 0; x < image.GetLength( 1 ); x++ )
            {
                if ( applicationMask[y, x] )
                {
                    Apply2DErosionMaskOnPoint( image, tmpImage, mask, x, y );
                }
            }
        }
    }

    static public void ConvolveOrientedMask( float[ , ] image, float[ , ] mask, bool[ , ] applicationMask )
    {
        int x, y, mX, mY;

        float dx, dy;

        float[,] sobelX, sobelY;

        float[,] tMask;

        

        sobelX = new float[ 3, 3 ] { { -1, 0, 1 }, 
                                     { -2, 0, 2 }, 
                                     { -1, 0, 1 } };

        sobelY = new float[ 3, 3 ] { { -1, -2, -1 },
                                     {  0,  0,  0 },
                                     {  1,  2,  1 } }; ;

        for ( y= 0; y < image.GetLength( 0 ); y++ )
        {
            for( x = 0; x < image.GetLength( 1 ); x++ )
            {
                if ( applicationMask[y, x] )
                {
                    //get gradient at the pixel
                    dx = Apply2DMaskOnPoint( image, sobelX, x, y );
                    dy = Apply2DMaskOnPoint( image, sobelY, x, y );

                    tMask = mask;

                    
                }
            }
        }
    }

    static public void Generate2DGaussian( float[, ] mask, float sigma, int size )
    {
        int row, col;
        float cst, tssq, x, y, sum;

        cst = 1.0f / ( sigma * Mathf.Sqrt( 2.0f * Mathf.PI ) );

        tssq = 1.0f / ( 2 * sigma * sigma );

        sum = 0.0f;

        for( row = 0; row < mask.GetLength( 0 ); row++ )
        {
            y = ( row - size / 2 );

            for( col = 0; col < mask.GetLength( 1 ); col++ )
            {
                x = ( col - size / 2 );

                mask[row, col] = cst * Mathf.Exp( -( ( x * x + y * y ) * tssq ) );

                sum += mask[row, col];
            }
        }

        for ( row = 0; row < mask.GetLength( 0 ); row++ )
        {

            for ( col = 0; col < mask.GetLength( 1 ); col++ )
            {
                mask[row, col] /= sum;
            }
        }
    }

    static public void Apply2DErosionMaskOnPoint( float[ , ] dImage, float[ , ] sImage, float[ , ] mask, int x, int y )
    {
        int xC, yC, xO, yO, xT, yT;
        int rIndex, cIndex, distributionCount;
        float acc;
        float sampleHeight;

        float [ , ] tMask = mask;
        bool [ , ] marked = new bool[ mask.GetLength( 0 ), mask.GetLength( 1 ) ];

        yC = mask.GetLength( 0 ) / 2;
        xC = mask.GetLength( 1 ) / 2;

        acc = 0.0f;
        distributionCount = 0;

        sampleHeight = sImage[y, x];

        for ( rIndex = 0; rIndex < mask.GetLength( 0 ); rIndex++ )
        {
            yO = rIndex - yC;

            yT = WrapPointY( sImage, yO + y );

            for ( cIndex = 0; cIndex < mask.GetLength( 1 ); cIndex++ )
            {
                xO = cIndex - xC;

                xT = WrapPointX( sImage, xO + x );

                if( sampleHeight < sImage[yT, xT] )
                {
                    acc += tMask[rIndex, cIndex] * 0.2f;
                    tMask[rIndex, cIndex] *= 0.8f;
                    marked[rIndex, cIndex] = true;
                    distributionCount++;
                }
                else
                {
                    marked[rIndex, cIndex] = false;
                }

            }
        }

        distributionCount = ( mask.GetLength( 1 ) * mask.GetLength( 0 ) ) - distributionCount;

        if( distributionCount != 0 )
        {
            acc /= distributionCount;
        }
        else
        {
            acc = 0.0f;
        }


        for ( rIndex = 0; rIndex < mask.GetLength( 0 ); rIndex++ )
        {

            for ( cIndex = 0; cIndex < mask.GetLength( 1 ); cIndex++ )
            {

                if ( !marked[ rIndex, cIndex ] )
                {
                    tMask[rIndex, cIndex] += acc;
                }

            }
        }

        acc = 0.0f;

        for ( rIndex = 0; rIndex < mask.GetLength( 0 ); rIndex++ )
        {
            yO = rIndex - yC;

            yT = WrapPointY( sImage, yO + y );

            for ( cIndex = 0; cIndex < mask.GetLength( 1 ); cIndex++ )
            {
                xO = cIndex - xC;

                xT = WrapPointX( sImage, xO + x );

                acc += tMask[rIndex, cIndex] * sImage[yT, xT];

            }
        }

        dImage[y, x] = acc;
    }

    static public void Apply2DMaskOnPoint( float[,] dImage, float[,] sImage, float[,] mask, int x, int y )
    {
        int xC, yC, xO, yO, xT, yT;
        int rIndex, cIndex;
        float acc;

        yC = mask.GetLength(0) / 2;
        xC = mask.GetLength(1) / 2;

        acc = 0.0f;

        for( rIndex = 0; rIndex < mask.GetLength( 0 ); rIndex++ )
        {
            yO = rIndex - yC;

            yT = WrapPointY( sImage, yO + y );

            for ( cIndex = 0; cIndex < mask.GetLength(1); cIndex++ )
            {
                xO = cIndex - xC;

                xT = WrapPointX( sImage, xO + x );

                acc += mask[rIndex, cIndex] * sImage[yT, xT];

            }
        }

        dImage[y, x] = acc;
    }

    static public float Apply2DMaskOnPoint( float[ , ] sImage, float[ , ] mask, int x, int y )
    {
        int xC, yC, xO, yO, xT, yT;
        int rIndex, cIndex;
        float acc;

        yC = mask.GetLength( 0 ) / 2;
        xC = mask.GetLength( 1 ) / 2;

        acc = 0.0f;

        for ( rIndex = 0; rIndex < mask.GetLength( 0 ); rIndex++ )
        {
            yO = rIndex - yC;

            yT = WrapPointY( sImage, yO + y );

            for ( cIndex = 0; cIndex < mask.GetLength( 1 ); cIndex++ )
            {
                xO = cIndex - xC;

                xT = WrapPointX( sImage, xO + x );

                acc += mask[rIndex, cIndex] * sImage[yT, xT];

            }
        }

        return acc;
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
        return ModuloWrap( x, image.GetLength( 1 ) );
    }

    static public int WrapPointY( float[,] image, int y )
    {
        return ModuloWrap( y, image.GetLength( 0 ) );
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

    public void SpawnTrees( int nT )
    {
        int index;
        GameObject nTree;
        Vector3 position;
        Vector3 angle;

        //spawn trees
        if ( tree != null && terrain != null )
        {
            for ( index = 0; index < nT; index++ )
            {
                position.x = Random.Range( 60.0f,
                                           tData.heightmapResolution - 60.0f );

                position.z = Random.Range(  60.0f,
                                           tData.heightmapResolution - 60.0f );

                if( visitedMap[ (int) position.z, (int) position.x ] )
                {
                    continue;
                }

                position.y = 0;

                position.y = terrain.SampleHeight( position );

                angle = Vector3.zero;

                angle.y = Random.Range( 0, 180 );

                nTree = Instantiate( tree, position, Quaternion.Euler( angle ) );

                nTree.transform.localScale = new Vector3( nTree.transform.localScale.x, Random.Range( 0.87f, 2.25f ), nTree.transform.localScale.z );

            }
        }

    }
}

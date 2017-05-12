﻿/********************************
 * 
 * Copy Right © Andrew Frost 2020, all rights reserved.
 * 
 *******************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

public class PathScript : MonoBehaviour
{

    public float xStartCenter = 50;
    public float yStartCenter = 150;

    public float xRange = 25;

    public float yRange = 100;

    public int minIterations = 500;

    public int maxIterations = 1000;

    public float tolerance = 100.0f;

    public float stepDist = 20.0f;

    public float startAndEndMinDist = 150;

    public float roomToPathRatio = 3.0f;

    public float rasterizationAreaMultiplier = 2.0f;

    private TerrainData tData;

    private Terrain terrain;

    private Vector2 start, end;

    public int[ , ] detailMap;

    public int numberOfTrees = 100;

    public float treeProb = 0.20f;

    public Vector3 startWorld, endWorld;

    private Vector2[ ] points2D;

   // public Vector3[ ] points3D;

    public GameObject boy;

    public GameObject wolf;

    public GameObject fire;

    public int cornOffset = 10;

    public float cullProb = 0.8f;
    
    void Start ()
    {
        int its = 0;
        int x, y;

        GrassManager boyGrass = boy.GetComponent<GrassManager>( );
        GrassManager wolfGrass = wolf.GetComponent<GrassManager>( );

        terrain = GetComponent<Terrain>( );

        tData = terrain.terrainData;        

        if( !GeneratePath( ref its ) )
        {
            Debug.Log( "ERROR!" );
        }

        //set details
        tData.SetDetailLayer( 0, 0, 0, detailMap );

        if (wolfGrass)
        {
            wolfGrass.size = tData.size;
            wolfGrass.detailHeight = tData.detailHeight;
            wolfGrass.detailWidth = tData.detailWidth;
            wolfGrass.map = (int[,])detailMap.Clone();
        }

        //add corn
        ChangeResolution( ref detailMap, 1, 0 );
        RandomCulling( ref detailMap, 0, cullProb );

        tData.SetDetailLayer( 0, 0, 1, detailMap );

        //slow down or stop players based on grass

        if ( boyGrass )
        {
            boyGrass.size = tData.size;
            boyGrass.detailHeight = tData.detailHeight;
            boyGrass.detailWidth = tData.detailWidth;
            boyGrass.map = ( int[ , ] ) detailMap.Clone( );
        }

        

        x = (int)((start.x / tData.detailHeight) * tData.size.x);
        y = (int)((start.y / tData.detailHeight) * tData.size.z);

        //convert 2D points to world
        startWorld = new Vector3(x, tData.GetHeight( x, y ), y );

        x = (int)((end.x / tData.detailHeight) * tData.size.x);
        y = (int)((end.y / tData.detailHeight) * tData.size.z);

        endWorld = new Vector3(x, tData.GetHeight(x, y), y);

        boy.transform.position = new Vector3( startWorld.x, boy.transform.position.y, startWorld.z );        

        fire.transform.position = new Vector3( endWorld.x, fire.transform.position.y, endWorld.z );

        //gameManager = FindObjectOfType<GameManager>( );

        /*if( gameManager == null )
        {
            Debug.Log( "Unable to acquire game manager!" );
        }*/
    }
	
	// Update is called once per frame
	void Update ()
    {
        
	}

    bool GeneratePath( ref int iterationsRan )
    {
        List<Vector2> pointList;
        List<Thread> threadList;

        int rValue, lastSelect, its = 0;
        float rChange;

        int currX, currY, index;

        int detailRes;

        Vector2 position, nextPosition;

        TreeInstance cornPlant;
        Vector3 cornPosition;

        List<TreeInstance> trees;

        int numTrees = 0;


        detailRes = tData.detailResolution;

        start.x = Random.Range( xStartCenter - xRange, xStartCenter + xRange );
        start.y = Random.Range( yStartCenter - yRange, yStartCenter + yRange );

        if ( start.y + tolerance > tData.detailResolution - 150
             || start.x + tolerance > tData.detailResolution - 150
             || start.y - tolerance < 150
             || start.x - tolerance < 150 )
        {
            start.x = Mathf.Min( start.x, ( float ) detailRes - 150 - tolerance );
            start.x = Mathf.Max( start.x, ( float ) 150 + tolerance );
            start.y = Mathf.Min( start.y, ( float ) detailRes - 150 - tolerance );
            start.y = Mathf.Max( start.y, ( float ) 150 + tolerance );
        }


        detailMap = new int[detailRes, detailRes];

        
        //initialize detail map
        for( currY = 0; currY < detailRes; currY++ )
        {
            for( currX = 0; currX < detailRes; currX++ )
            {
                detailMap[currY, currX] = 3;           
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

            if ( rValue + 4 == lastSelect || rValue - 4 == lastSelect ) //if opposite
            {
                rValue = lastSelect;
            }

            nextPosition = SelectNextPosition( rValue, stepDist, position );            

            if ( nextPosition.y + tolerance > detailRes - 150
                 || nextPosition.x + tolerance > detailRes - 150
                 || nextPosition.y - tolerance < 150
                 || nextPosition.x - tolerance < 150 )
            {
                nextPosition = points2D[its - 1]; //back track
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

        iterationsRan = its + 1;

        if( ( Vector2.Distance( start, end ) < startAndEndMinDist ) )
        {
            Debug.Log( "Dist: " + Vector2.Distance( start, end ) );
            return false;
        }

        //generate list of points to rasterize
        pointList = new List<Vector2>( );

        //copy points
        for( index = 0; index < iterationsRan; index++ )
        {
            pointList.Add( points2D[index] ); //push back the point
        }

        //"rasterize" the line ///////////////////    

        InsertIntermediatePoints( ref pointList, tolerance / 2.0f );


        //initialize threads

        threadList = new List<Thread>( );

        for( index = 0; index < SystemInfo.processorCount; index++ )
        {
            its = pointList.Count / ( SystemInfo.processorCount );

            its = its + ( pointList.Count - ( its * ( SystemInfo.processorCount ) ) );

            threadList.Add( new Thread( ( ) => RasterizeMaze( pointList, 
                                                              its * index, 
                                                              Mathf.Max( its * index + its, 
                                                                         pointList.Count ),
                                                              detailRes ) ) );

            

        }

        //perform the rasterization on the terrain

        for ( index = 0; index < SystemInfo.processorCount; index++ )
        {
            threadList[index].Start( );
        }

        RasterizeEndPoint( start );
        RasterizeEndPoint( end );

        for ( index = 0; index < SystemInfo.processorCount; index++ )
        {
            threadList[index].Join( );
        }

        //place corn plants
        trees = new List<TreeInstance>( );

        cornPosition = Vector3.zero;

        numTrees = 0;

        for ( currY = 50; currY < tData.size.z - 50; currY += cornOffset )
        {
            for ( currX = 50; currX < tData.size.x - 50; currX += cornOffset )
            {
                cornPlant = new TreeInstance( );                

                cornPlant.prototypeIndex = 0;
                cornPlant.heightScale = 1;
                cornPlant.widthScale = 1;

                cornPosition.x = ( 1.0f / tData.size.x ) * currX;
                cornPosition.z = ( 1.0f / tData.size.z ) * currY;
                cornPosition.y = tData.GetInterpolatedHeight( cornPosition.x, cornPosition.z );

                cornPlant.position = cornPosition;


                if ( detailMap[ ( int ) ( tData.detailHeight * cornPosition.z ), ( int ) ( tData.detailWidth * cornPosition.x ) ] == 0 )
                {
                    continue;
                }

                rChange = Random.Range( 0.0f, 1.0f );

                if ( rChange < treeProb && numTrees < numberOfTrees )
                {
                    rChange = Random.Range( 0.0f, 1.0f );
                    numTrees++;
                    cornPlant.prototypeIndex = rChange <= 0.5f ? 1 : 2;
                }

                trees.Add( cornPlant );
            }
        }

        tData.treeInstances = trees.ToArray( ); 
        terrain.Flush( );
        trees.Clear( );

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


    public static void InsertIntermediatePoints( ref List<Vector2> list, float distanceBtwn )
    {
        int index;
        List<Vector2> newList = new List<Vector2>();

        for( index = 0; index < list.Count - 1; index++ )
        {
            

            newList.AddRange( GenerateListBetween2Points( list[ index ], list[ index + 1 ], distanceBtwn ) );

            newList.Add( list[index + 1] );
        }

        

        list = newList;
    }

    public static List<Vector2> GenerateListBetween2Points( Vector2 pointA, Vector2 pointB, float distanceBtwn )
    {
        int count;

        List<Vector2> retList = new List<Vector2>();

        count = ( int ) ( Vector2.Distance( pointA, pointB ) / distanceBtwn );

        retList = GetListFromMidPoints( pointA, pointB, count );

        return retList;
    }

    public static List<Vector2> GetListFromMidPoints( Vector2 pointA, Vector2 pointB, int iterations )
    {
        List<Vector2> retList = new List<Vector2>( );

        if( iterations > 0 )
        {
            Vector2 midPoint = MidPoint( pointA, pointB );
            retList.AddRange( GetListFromMidPoints( pointA, midPoint, iterations / 2 ) );
            retList.AddRange( GetListFromMidPoints( midPoint, pointB, iterations / 2 ) );
        }
        else
        {
            retList.Add( pointA );
            retList.Add( pointB );
        }

        return retList;
    }
    
    public static Vector2 MidPoint( Vector2 pointA, Vector2 pointB )
    {
        return ( pointA + pointB ) / 2.0f;
    }

    public static void ChangeResolution( ref int[,] map, int nRes, int delim )
    {
        int x, y;

        for( y = 0; y < map.GetLength( 0 ); y++ )
        {
            for( x = 0; x < map.GetLength(0); x++ )
            {
                if( map[y, x] != delim )
                {
                    map[y, x] = nRes;
                }
            }
        }
    }

    public static void RandomCulling( ref int[ , ] map, int delim, float prob )
    {
        float rand;

        int x, y;

        for ( y = 0; y < map.GetLength( 0 ); y++ )
        {
            for ( x = 0; x < map.GetLength( 0 ); x++ )
            {
                if ( map[y, x] != delim )
                {
                    rand = Random.Range( 0.0f, 1.0f );

                    map[y, x] = rand <= prob ? 0 : map[y,x];
                }
            }
        }
    }


    private void RasterizeMaze( List<Vector2> pointList, int start, int end, int detailRes )
    {
        int currX, currY, index;

        for ( index = 0; index < pointList.Count; index++ )
        {

            for ( currY = ( int ) Mathf.Max( pointList[index].y - rasterizationAreaMultiplier * tolerance, 10 );
                  currY < ( int ) Mathf.Min( pointList[index].y + rasterizationAreaMultiplier * tolerance, detailRes - 10 );
                  currY++ )
            {
                for ( currX = ( int ) Mathf.Max( pointList[index].x - rasterizationAreaMultiplier * tolerance, 10 );
                      currX < ( int ) Mathf.Min( pointList[index].x + rasterizationAreaMultiplier * tolerance, detailRes - 10 );
                      currX++ )
                {
                    if ( ( ( tolerance ) > Mathf.Sqrt( ( currX - pointList[index].x ) * ( currX - pointList[index].x ) + ( currY - pointList[index].y ) * ( currY - pointList[index].y ) ) ) )
                    {
                        detailMap[currY, currX] = 0;
                    }
                }
            }
        }

    }

    private void RasterizeEndPoint( Vector2 point )
    {
        int currX, currY;

        for ( currY = ( int ) Mathf.Max( point.y - roomToPathRatio * tolerance, 10 );
              currY < ( int ) Mathf.Min( point.y + roomToPathRatio * tolerance, tData.detailResolution - 10 );
              currY++ )
        {
            for ( currX = ( int ) Mathf.Max( point.x - roomToPathRatio * tolerance, 10 );
                  currX < ( int ) Mathf.Min( point.x + roomToPathRatio * tolerance, tData.detailResolution - 10 );
                  currX++ )
            {
                if ( ( roomToPathRatio * tolerance > Mathf.Sqrt( ( currX - point.x ) * ( currX - point.x ) + ( currY - point.y ) * ( currY - point.y ) ) ) )
                {
                    detailMap[currY, currX] = 0;
                }
            }
        }
    }

}

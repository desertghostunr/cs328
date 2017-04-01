/********************************
 * 
 * Copy © Andrew Frost 2017, all rights reserved.
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

    public int maxIterations = 12;

    public float tolerance = 100.0f;

    public float stepDist = 50.0f;

    public float startAndEndMinDist = 150;

    public float roomToPathRatio = 3.0f;

    public GameObject artifact = null;

    public GameObject spawnCover = null;

    public GameObject raider = null;  

    private TerrainData tData;

    private Vector2 start, end;

    public float[ , ] heightMap;

    public Vector3 startWorld, endWorld;

    private Vector2[ ] points2D;

    public Vector3[ ] points3D;


    private GameManager gameManager;

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

        PlaceArtifact( );

        if( spawnCover != null )
        {
            Instantiate( spawnCover, new Vector3( startWorld.x, startWorld.y + 19, startWorld.z ), Quaternion.identity );
        }
    }
	
	// Update is called once per frame
	void Update ()
    {
        
	}

    public void PlaceArtifact( )
    {
        GameObject tmp = null;
        Quaternion newRotDir;

        if( artifact != null  )
        {
            newRotDir = Quaternion.FromToRotation( Vector3.forward, startWorld - endWorld );

            tmp = GameObject.FindGameObjectWithTag( "Artifact" );

            if( tmp == null )
            {
                Instantiate( artifact, new Vector3( endWorld.x, endWorld.y + 7, endWorld.z ), newRotDir );
            }
            else
            {
                tmp.transform.position = new Vector3( endWorld.x, endWorld.y + 7, endWorld.z );
                tmp.transform.rotation = newRotDir;
            }

            
            
        }
    }

    bool GeneratePath( )
    {

        int rValue, lastSelect, its = 0;
        float rChange, slope, yIntersect;

        int currX, currY, index;

        Vector2 position, nextPosition/*, tmpPoint*/;
        //List<Vector2> tmpPoints;

        List<Vector2> pointsList = new List<Vector2>( );
        
        bool[ , ] visitedMap;

        start.x = Random.Range( xStartCenter - xRange, xStartCenter + xRange );
        start.y = Random.Range( yStartCenter - yRange, yStartCenter + yRange );

        heightMap = new float[ tData.heightmapResolution, tData.heightmapResolution ];
        visitedMap = new bool[ tData.heightmapResolution, tData.heightmapResolution ];

        for ( currX = 0; currX < tData.heightmapResolution; currX++ )
        {
            for ( currY = 0; currY < tData.heightmapResolution; currY++ )
            {
                heightMap[ currX, currY ] = baseHeight;
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

            nextPosition = position;

            switch ( rValue )
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

            if ( nextPosition.y + tolerance > tData.heightmapResolution - 75 
                 || nextPosition.x + tolerance > tData.heightmapResolution - 75
                 || nextPosition.y - tolerance < 75
                 || nextPosition.x - tolerance < 75 )
            {
                nextPosition = position;
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

        //"rasterize" the line
        for( index = 0; index < pointsList.Count; index++ )
        {
            for( currY = ( int ) Mathf.Max( pointsList[ index ].y - tolerance, 10 );
                 currY < ( int ) Mathf.Min( pointsList[ index ].y + tolerance, tData.heightmapResolution - 10 );
                 currY++ )
            {
                for( currX = ( int ) Mathf.Max( pointsList[ index ].x - tolerance, 10 );
                     currX < ( int ) Mathf.Min( pointsList[ index ].x + tolerance, tData.heightmapResolution - 10 );
                     currX++ )
                {
                    if( !visitedMap[ currX, currY ] && ( tolerance > Mathf.Sqrt( ( currX - pointsList[ index ].x ) * ( currX - pointsList[ index ].x ) + ( currY - pointsList[ index ].y ) * ( currY - pointsList[ index ].y ) ) ) )
                    {
                        heightMap[ currX, currY ] -= heightDelta;
                        visitedMap[ currX, currY ] = true;
                    }                   
                }
            }
        }
        
        //carve areas around the start and end point
        for( currY = ( int ) Mathf.Max( start.y - roomToPathRatio * tolerance, 10 );
             currY < ( int ) Mathf.Min( start.y + roomToPathRatio * tolerance, tData.heightmapResolution - 10 );
             currY++ )            
        {
            for( currX = ( int ) Mathf.Max( start.x - roomToPathRatio * tolerance, 10 );
                 currX < ( int ) Mathf.Min( start.x + roomToPathRatio * tolerance, tData.heightmapResolution - 10 );
                 currX++ )
            {
                if( !visitedMap[ currX, currY ] && ( roomToPathRatio * tolerance > Mathf.Sqrt( ( currX - start.x ) * ( currX - start.x ) + ( currY - start.y ) * ( currY - start.y ) ) ) )
                {
                    heightMap[ currX, currY ] -= heightDelta;
                    visitedMap[ currX, currY ] = true;
                }                
            }
        }

        for( currY = ( int ) Mathf.Max( end.y - roomToPathRatio * tolerance, 10 );
             currY < ( int ) Mathf.Min( end.y + roomToPathRatio * tolerance, tData.heightmapResolution - 10 );
             currY++ )
        {
            for( currX = ( int ) Mathf.Max( end.x - roomToPathRatio * tolerance, 10 );
                 currX < ( int ) Mathf.Min( end.x + roomToPathRatio * tolerance, tData.heightmapResolution - 10 );
                 currX++ )                
            {
                if( !visitedMap[ currX, currY ] && ( roomToPathRatio * tolerance > Mathf.Sqrt( ( currX - end.x ) * ( currX - end.x ) + ( currY - end.y ) * ( currY - end.y ) ) ) )
                {
                    heightMap[ currX, currY ] -= heightDelta;
                    visitedMap[ currX, currY ] = true;
                }               
            }
        }

        return true;
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

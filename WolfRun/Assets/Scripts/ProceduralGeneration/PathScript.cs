/********************************
 * 
 * Copy Right © Andrew Frost 2020, all rights reserved.
 * 
 *******************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using UnityEngine.SceneManagement;

public class PathScript : MonoBehaviour
{
    public float heightVariation = .005f;
    public float perlinMultiplier = 0.001f;
    public float perlinExtremes = 25f;

    public bool hunterAI = false;
    public bool huntedAI = false;
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

    public bool useMultiThreaded = true;

    private GameObject boy;

    private GameObject wolf;

    public GameObject fire;

    public int cornOffset = 10;

    public float cullProb = 0.8f;

    public int cornTreePrototypeIndex= 0;

    public int shortGrassPrototypeIndex = 0;
    public int cornGrassPrototypeIndex = 1;

    public int grassTerrainTextureLayer = 1;

    public int pathTerrainTextureLayer = 0;

    public int pathExtremes = 150;

    public int treeExtremes = 50;

    public bool plantCorn = true;
    
    void Start ()
    {
        System.DateTime t1 =  System.DateTime.Now;
        float[,,] alphaMap = null;
        float x, y;
        Vector3 tmpPosition;

        boy = GameObject.Find( "Boy" );
        wolf = GameObject.Find( "Wolf" );

        terrain = GetComponent<Terrain>( );

        tData = terrain.terrainData;

        Fill2DArray( ref detailMap, tData.detailResolution, tData.detailResolution, 2 );

        InitializeAlphaMapToLayer( ref alphaMap, grassTerrainTextureLayer );

        GenerateTerrain( );

        if( !InitializePath( ) )
        {
            SceneManager.LoadScene( "Menu" );
        }

        PaintAlphaBasedOnDetails( ref alphaMap, pathTerrainTextureLayer );

        tData.SetAlphamaps( 0, 0, alphaMap );

        //set details
        tData.SetDetailLayer( 0, 0, shortGrassPrototypeIndex, detailMap );

        //add corn
        ChangeResolution( ref detailMap, 1, 0 );
        RandomCulling( ref detailMap, 0, cullProb );

        tData.SetDetailLayer( 0, 0, cornGrassPrototypeIndex, detailMap );

        //build a mesh path for hunted
        if( huntedAI )
        {
            BuildMeshPath( );
        }

        if( hunterAI )
        {
            gameObject.AddComponent<NavMeshSourceTag>( );
        }
        

        x = ((start.x / tData.detailWidth) * tData.size.x);
        y = ((start.y / tData.detailHeight) * tData.size.z);

        //convert 2D points to world
        startWorld = new Vector3(x, tData.GetInterpolatedHeight( x / tData.size.x, y / tData.size.z ), y );

        startWorld += transform.position;

        x = ((end.x / tData.detailWidth) * tData.size.x);
        y = ((end.y / tData.detailHeight) * tData.size.z);

        endWorld = new Vector3(x, tData.GetInterpolatedHeight( x / tData.size.x, y / tData.size.z ), y);

        endWorld += transform.position;

        boy.transform.position = new Vector3( startWorld.x, terrain.SampleHeight( startWorld ) + 0.25f , startWorld.z );        
        
        fire.transform.position = new Vector3( endWorld.x, terrain.SampleHeight( endWorld ), endWorld.z );

        tmpPosition = tData.size - startWorld;

        wolf.transform.position = new Vector3( tmpPosition.x, terrain.SampleHeight( tmpPosition ) + 0.5f, tmpPosition.z );

        //gameManager = FindObjectOfType<GameManager>( );

        /*if( gameManager == null )
        {
            Debug.Log( "Unable to acquire game manager!" );
        }*/

        System.DateTime t2 = System.DateTime.Now;

        Debug.Log( "Path Generation Time:" + " " + ( t2.Subtract( t1 ).TotalSeconds ) );
    }

    public void GenerateTerrain( )
    {
        float x, y;

        float xOffset, yOffset;

        float xPerlin, yPerlin;

        float[,] heightMap = terrain.terrainData.GetHeights( 0, 0, tData.heightmapWidth, 
                                                                   tData.heightmapHeight);

        xOffset = Random.Range( 0, 1000 );
        yOffset = Random.Range( 0, 1000 );

        for( y = perlinExtremes; y < tData.heightmapHeight - perlinExtremes; y++ )
        {
            yPerlin = ( y + yOffset ) * perlinMultiplier;

            for( x = perlinExtremes; x < tData.heightmapWidth - perlinExtremes; x++ )
            {
                xPerlin = ( x + xOffset ) * perlinMultiplier;

                heightMap[(int)y, (int)x] = heightVariation * Mathf.PerlinNoise( xPerlin, yPerlin );
            }
        }

        tData.SetHeights( 0, 0, heightMap );
    }

    public List<Vector2> GeneratePath( )
    {
        List<Vector2> pointList;

        int rValue, lastSelect, its = 0;        

        int detailRes;

        Vector2 position, nextPosition;

        detailRes = tData.detailResolution;

        start.x = Random.Range( xStartCenter - xRange, xStartCenter + xRange );
        start.y = Random.Range( yStartCenter - yRange, yStartCenter + yRange );

        if ( start.y + tolerance > tData.detailResolution - pathExtremes
             || start.x + tolerance > tData.detailResolution - pathExtremes
             || start.y - tolerance < pathExtremes
             || start.x - tolerance < pathExtremes )
        {
            start.x = Mathf.Min( start.x, ( float ) detailRes - pathExtremes - tolerance );
            start.x = Mathf.Max( start.x, pathExtremes + tolerance );
            start.y = Mathf.Min( start.y, ( float ) detailRes - pathExtremes - tolerance );
            start.y = Mathf.Max( start.y, pathExtremes + tolerance );
        }

        //points2D = new Vector2[ maxIterations ];

        pointList = new List<Vector2>( );

        position = start;

        pointList.Add( start );
        

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

            if ( nextPosition.y + tolerance > detailRes - pathExtremes
                 || nextPosition.x + tolerance > detailRes - pathExtremes
                 || nextPosition.y - tolerance < pathExtremes
                 || nextPosition.x - tolerance < pathExtremes )
            {
                nextPosition = pointList[its - 1]; //back track
            }

            lastSelect = rValue;

            pointList.Add( nextPosition );

            position = nextPosition;

            if( ( its > minIterations ) 
                && ( Vector2.Distance( start, position ) >= startAndEndMinDist ) 
                && CloseEnoughToEdge( position, 10f ) )
            {
                break;
            }
            else if( ( its > minIterations )
                       && ( Vector2.Distance( start, position ) >= startAndEndMinDist ) )
            {
                position.x = ( tData.detailResolution - pathExtremes - tolerance );

                break;
            }
        }

        end = position;

        pointList.Add( end );

        return pointList;
    }

    public bool CloseEnoughToEdge( Vector2 point, float minDistToEdge )
    {
        return ( Mathf.Abs( point.x + tolerance - ( tData.detailResolution - pathExtremes ) ) <= minDistToEdge
                 || Mathf.Abs( point.y + tolerance - ( tData.detailResolution - pathExtremes ) ) <= minDistToEdge
                 || Mathf.Abs( point.x - tolerance - pathExtremes ) <= minDistToEdge
                 || Mathf.Abs( point.y - tolerance - pathExtremes ) <= minDistToEdge );
    }

    public bool InitializePath( )
    {
        List<Vector2> pointList;
        List<Vector3> worldPath;

        //path generation
        pointList = GeneratePath();

        if( pointList.Count == 0 )
        {
            return false;
        }

        //perform the rasterization on the terrain

        RasterizeList( pointList, tData.detailResolution );

        RasterizeEndPoint( start );
        RasterizeEndPoint( end );

        PlantTrees( plantCorn );

        if( huntedAI )
        {
            worldPath = ConvertPointListToWorldCoordinates( pointList );

            FindObjectOfType<SimpleNPCHunted>( ).SetLocationList( worldPath );
        }

        

        return true;
    }

    public bool BuildMeshPath( )
    {
        int[,] map = new int[ (int)tData.size.z, (int)tData.size.x ];
        float[,,] alphaMap = tData.GetAlphamaps(0, 0, tData.alphamapWidth, tData.alphamapHeight);
        float val;
        int x, y;

        MeshGenerator mGen = FindObjectOfType<MeshGenerator>( );        

        if ( !mGen )
        {
            return false;
        }

        for( y = 0; y < map.GetLength(0); y++ )
        {
            for( x = 0; x < map.GetLength(1); x++ )
            {
                val = alphaMap[( int ) ( tData.alphamapWidth * ( ( 1.0f / tData.size.x ) * ( x - transform.position.x ) ) ),
                               ( int ) ( tData.alphamapHeight * ( ( 1.0f / tData.size.z ) * ( y - transform.position.z ) ) ), pathTerrainTextureLayer];
                map[y, x] = val > 0 ? 1 : 0;
            }
        }

        mGen.GenerateMesh( map, 1.0f, terrain );


        return true;
    }

    private void RasterizeList( List<Vector2> pointList, int detailRes )
    {
        int its;

        InsertIntermediatePoints( ref pointList, tolerance / 2.0f );

        its = pointList.Count / ( SystemInfo.processorCount );

        its = its + ( pointList.Count - ( its * ( SystemInfo.processorCount ) ) );

        if ( its > 0 && useMultiThreaded )
        {
            MultiThreadedRasterization( pointList, detailRes, its );
        }
        else
        {
            RasterizeMaze( pointList, 0, pointList.Count, detailRes );
        }
    }

    private void PlantTrees( bool addCornPlants = true, bool cornPlantsExist = true )
    {

        float rChange;

        int currX, currY;

        TerrainCollider tCollider = terrain.GetComponent<TerrainCollider>( );

        TreeInstance tree;
        Vector3 treePosition;

        List<TreeInstance> trees;

        int numTrees = 0, its, numPrototypes;

        //place far distance corn plants and trees
        trees = new List<TreeInstance>( );

        treePosition = Vector3.zero;

        numTrees = 0;

        numPrototypes = tData.treePrototypes.Length;

        if( numPrototypes == 0 )
        {
            return;
        }

        its = 0;

        for ( currY = treeExtremes; currY < tData.size.z - treeExtremes; currY += cornOffset )
        {
            for ( currX = treeExtremes; currX < tData.size.x - treeExtremes; currX += cornOffset )
            {
                tree = new TreeInstance( );

                tree.prototypeIndex = 0;
                tree.heightScale = 1;
                tree.widthScale = 1;                

                treePosition.x = ( 1.0f / tData.size.x ) * currX;
                treePosition.z = ( 1.0f / tData.size.z ) * currY;
                treePosition.y = tData.GetInterpolatedHeight( treePosition.x, treePosition.z ) / tData.size.y;

                tree.position = treePosition;
                

                if ( detailMap[( int ) ( tData.detailHeight * treePosition.z ), ( int ) ( tData.detailWidth * treePosition.x )] == 0 )
                {
                    its++;
                    continue;
                }

                rChange = Random.Range( 0.0f, 1.0f );

                if ( rChange < treeProb && numTrees < numberOfTrees )
                {
                    tree.prototypeIndex = Random.Range( 0, numPrototypes );

                    if( cornPlantsExist && tree.prototypeIndex == cornTreePrototypeIndex )
                    {
                        tree.prototypeIndex = ( tree.prototypeIndex + 1 ) % numPrototypes; 
                    }
                    
                    numTrees++;
                }

                if( ( addCornPlants || cornPlantsExist ) 
                      && ( !addCornPlants && tree.prototypeIndex != cornTreePrototypeIndex ) )
                {
                    trees.Add( tree );
                }
                else if( !cornPlantsExist )
                {
                    trees.Add( tree );
                }
            }
        }

        tData.treeInstances = trees.ToArray( );

        terrain.Flush( );

        tCollider.enabled = false;
        tCollider.enabled = true;

        trees.Clear( );
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

    public void Fill2DArray<Type>( ref Type[,] array, int rSize, int cSize, Type value )
    {
        int x, y;

        array = new Type[rSize, cSize];

        for( y = 0; y < array.GetLength( 0 ); y++ )
        {
            for( x = 0; x < array.GetLength( 1 ); x++ )
            {
                array[y, x] = value;
            }
        }
    }

    //textures the terrain with one layer
    private void InitializeAlphaMapToLayer( ref float[ , , ] alphaMap, int layer )
    {
        int x, y, inner;

        alphaMap = tData.GetAlphamaps( 0, 0, tData.alphamapWidth, tData.alphamapHeight );

        for( y = 0; y < alphaMap.GetLength( 0 ); y++ )
        {
            for( x = 0; x < alphaMap.GetLength( 1 ); x++ )
            {
                for( inner = 0; inner < alphaMap.GetLength( 2 ); inner++ )
                {
                    alphaMap[y, x, inner] = ( inner == layer ) ? 1 : 0;
                }
            }
        }
    }

    private void PaintAlphaBasedOnDetails( ref float[ ,, ] alphaMap, int layer )
    {
        int aX, aY, dX, dY, inner;

        for ( dY = 0; dY < detailMap.GetLength( 0 ); dY++ )
        {
            for ( dX = 0; dX < detailMap.GetLength( 1 ); dX++ )
            {
                if( detailMap[ dY, dX ] == 0 )
                {

                    aY = ( int ) (  ( ( float ) dY / tData.detailHeight ) * tData.alphamapHeight );
                    aX = ( int ) (  ( ( float ) dX / tData.detailWidth ) * tData.alphamapWidth );

                    for ( inner = 0; inner < alphaMap.GetLength( 2 ); inner++ )
                    {
                        alphaMap[aY, aX, inner] = ( inner == layer ) ? 1.0f : 0.0f;
                    }
                }

                

            }
        }
    }

    public List<Vector3> ConvertPointListToWorldCoordinates( List<Vector2> points )
    {
        Vector3 pointA;
        int index;

        List<Vector3> worldPoints = new List<Vector3>();

        for( index = 0; index < points.Count; index++ )
        {
            pointA.x = ( ( points[index].x / tData.detailWidth ) * tData.size.x );
            pointA.y = 0;
            pointA.z = ( ( points[index].y / tData.detailHeight ) * tData.size.z );

            pointA += transform.position;

            pointA.y = terrain.SampleHeight( pointA ) + transform.position.y;            

            worldPoints.Add( pointA );
        }

        return worldPoints;
    }

    /*private void AddCollidersToPath( List<Vector2> pointList, bool trigger = true )
    {
        GameObject startGO, endGO;
        List<GameObject> gOList;

        int index;

        startGO = GenerateSphereAtPoint( start, tData.detailWidth, tData.detailHeight, tData.size, ( tolerance * roomToPathRatio ) / 2.0f, trigger );

        gOList = GenerateBoxColliderListOnPath( pointList, 0, pointList.Count, tData.detailWidth, tData.detailHeight, tData.size, 10, tolerance, trigger );

        endGO = GenerateSphereAtPoint( end, tData.detailWidth, tData.detailHeight, tData.size, ( tolerance * roomToPathRatio ) / 2.0f, trigger );


        startGO.transform.position = new Vector3( startGO.transform.position.x,
                                                  terrain.SampleHeight( startGO.transform.position ),
                                                  startGO.transform.position.z );


        for ( index = 0; index < gOList.Count; index++ )
        {
            gOList[index].transform.position = new Vector3( gOList[index].transform.position.x, 
                                                            terrain.SampleHeight( gOList[index].transform.position ), 
                                                            gOList[index].transform.position.z );
        }

        endGO.transform.position = new Vector3( endGO.transform.position.x,
                                                terrain.SampleHeight( endGO.transform.position ),
                                                endGO.transform.position.z );

    }*/

    /*private GameObject GenerateSphereAtPoint
    ( 
        Vector2 point, 
        int dWidth, 
        int dHeight, 
        Vector3 dMSize, 
        float radius,  
        bool trigger = true 
    )
    {
        GameObject tmpGO;
        SphereCollider tmpSC;
        Vector2 pointA;

        //calculate local position and bounds
        pointA.x = ( ( point.x / dWidth ) * dMSize.x );
        pointA.y = ( ( point.y / dHeight ) * dMSize.z );

        //create collider and add it to list
        tmpGO = new GameObject( );
        tmpGO.name = trigger ? "p_trigger" : "p_collider";
        tmpSC = tmpGO.AddComponent<SphereCollider>( );

        //define collider size and status
        tmpSC.radius = radius;
        tmpSC.isTrigger = trigger;

        //position for x and y
        tmpGO.transform.transform.position = new Vector3( pointA.x, transform.position.y, pointA.y );

        //parent the GameObject
        if( pathManager )
        {
            tmpGO.transform.SetParent( pathManager.transform );
        }
        

        return tmpGO;

    }*/


    /*private List<GameObject> GenerateBoxColliderListOnPath
    (
        List<Vector2> points,
        int start,
        int end,
        int dWidth,
        int dHeight,
        Vector3 dMSize,
        float colliderHeight,
        float colliderWidth,
        bool trigger = true
    )
    {
        List<GameObject> colliderList;
        GameObject tmpGO;
        BoxCollider tmpBC;
        Vector2 pointA, pointB, pointC;
        int index;

        float angle, dist;

        colliderList = new List<GameObject>( );

        if( ( end - start < 1 ) && ( end - start >= 0 ) )
        {
            colliderList.Add( GenerateSphereAtPoint( points[ 0 ], dWidth, dHeight, dMSize, colliderWidth / 2.0f, trigger ) );

            return colliderList;
        }

        for( index = Mathf.Max( start, 0 ); index < Mathf.Min( end - 1, points.Count - 1 ); index++ )
        {

            //calculate local position and bounds for each set of points
            pointA.x = ( ( points[index].x / dWidth ) * dMSize.x );
            pointA.y = ( ( points[index].y / dHeight ) * dMSize.z );

            pointB.x = ( ( points[index + 1].x / dWidth ) * dMSize.x );
            pointB.y = ( ( points[index + 1].y / dHeight ) * dMSize.z );

            dist = Vector2.Distance( pointA, pointB );

            if ( dist == 0 )
            {
                continue;
            }            
            
            pointC = MidPoint( pointA, pointB );            

            //create collider and add it to list
            tmpGO = new GameObject( );
            tmpGO.name = trigger ? "p_trigger" : "p_collider";
            tmpBC = tmpGO.AddComponent<BoxCollider>( );

            //define collider size and status
            tmpBC.size = new Vector3( colliderWidth, colliderHeight, dist + colliderWidth );
            tmpBC.isTrigger = trigger;

            //position for x and y
            tmpGO.transform.transform.position = new Vector3( pointC.x, transform.position.y, pointC.y );

            //rotate the game object
            angle = Vector2.Angle( Vector2.up, ( pointB - pointA ).normalized );

            angle *= Mathf.Sign( Vector2.Dot( Vector2.right, ( pointB - pointA ).normalized ) );

            tmpGO.transform.Rotate( Vector3.up, angle );

            //parent the GameObject
            if( pathManager )
            {
                tmpGO.transform.SetParent( pathManager.transform );
            }            

            colliderList.Add( tmpGO );
        }

        return colliderList;
    }*/

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

    public static void ConvertToBinaryImage( ref int[ , ] map, int startRow, int endRow, bool invert = false )
    {
        int x, y;

        for ( y = Mathf.Max( startRow, 0 ); y < Mathf.Min( endRow, map.GetLength( 0 ) ); y++ )
        {
            for ( x = 0; x < map.GetLength( 1 ); x++ )
            {
                if ( invert )
                {
                    map[y, x] = map[y, x] <= 0 ? 1 : 0;
                }
                else
                {
                    map[y, x] = map[y, x] <= 0 ? 0 : 1;
                }
            }
        }
    }

    //requires a binary image with 0 and edgeValue as the only two values
    public static void ThinToBinaryEdges( ref int[ , ] map, int startRow, int endRow, int edgeValue )
    {
        int x, y;

        for( y = Mathf.Max( startRow, 0 ); y < Mathf.Min( endRow, map.GetLength( 0 ) ); y++ )
        {
            for( x = 0; x < map.GetLength( 1 ); x++ )
            {
                if( !DifferentNeighbor( map, x, y, edgeValue ) )
                {
                    map[y, x] = 0;
                }
            }
        }       
    }


    public static bool DifferentNeighbor( int[ , ] map, int x, int y, int value )
    {
        int iX, iY;

        for( iY = Mathf.Max( y - 1, 0 ); iY < Mathf.Min( y + 1, map.GetLength( 0 ) ); iY++ )
        {
            for ( iX = Mathf.Max( x - 1, 0 ); iX < Mathf.Min( x + 1, map.GetLength( 1 ) ); iX++ )
            {
                if( map[ iY, iX ] != value )
                {
                    return true;
                }
            }
        }

        return false;
    }

    private void MultiThreadedRasterization( List<Vector2> pointList, int detailRes, int its )
    {
        List<Thread> threadList;
        int index;

        //initialize threads
        threadList = new List<Thread>( );   


        //perform the rasterization on the terrain

        for ( index = 0; index < SystemInfo.processorCount; index++ )
        {
            threadList.Add( new Thread( ( ) => RasterizeMaze( pointList,
                                                              its * index,
                                                              Mathf.Max( its * index + its,
                                                                         pointList.Count ),
                                                              detailRes ) ) );


            threadList[index].Start( );

        }

        for ( index = 0; index < SystemInfo.processorCount; index++ )
        {
            threadList[index].Join( );
        }
    }


    private void MakeMesh( )
    {
        int[,] meshMap;
        int index;
        int rows;

        List<Thread> threadList = new List<Thread>();


        //create a map of points to use in mesh
        meshMap = ( int[ , ] ) detailMap.Clone( );

        rows = meshMap.GetLength( 0 ) / SystemInfo.processorCount;

        rows = rows + ( meshMap.GetLength( 0 ) - ( rows * SystemInfo.processorCount ) );

        for ( index = 0; index < SystemInfo.processorCount; index++ )
        {
            threadList.Add( new Thread( ( ) => ConvertToBinaryImage( ref meshMap, rows * index, rows * index + rows, true ) ) );
            threadList[index].Start( );
        }


        for ( index = 0; index < SystemInfo.processorCount; index++ )
        {
            threadList[index].Join( );
        }

        threadList.Clear( );

        for ( index = 0; index < SystemInfo.processorCount; index++ )
        {
            threadList.Add( new Thread( ( ) => ThinToBinaryEdges( ref meshMap, rows * index, rows * index + rows, 1 ) ) );
            threadList[index].Start( );
        }


        for ( index = 0; index < SystemInfo.processorCount; index++ )
        {
            threadList[index].Join( );
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

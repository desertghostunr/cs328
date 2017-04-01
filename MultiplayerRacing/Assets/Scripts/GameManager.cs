/********************************
 * 
 * Copy Right © Andrew Frost 2017, all rights reserved.
 * 
 *******************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class GameManager : MonoBehaviour
{
    public Text gameOverTxt;

    public Text goldAmntTxt;

    public Text livesTxt;

    public Text timeTxt;

    public Text waveNumberTxt;

    public PathScript path;

    public GameObject[] towers;

    public int[] tCost;

    public GameObject dummyTower;

    public Camera mCamera;

    public GameObject cameraHolder;

    public Terrain terrain;

    public float timeToWave = 10;

    private int goldAmnt = 100;

    private int lives = 3;

    private int wave = 0;

    private UnityStandardAssets.Characters.FirstPerson.MouseLook mLook;

    private bool rightMouseDown = false;

    private GameObject tdummy;

    private bool leftMouseDown = false;

    private int enemyCount = 0;

    private int towerCount = 0;

    public int levelNumber = 0;

    public bool Save( )
    {
        GameObject[] sTowers;
        GameObject[] baddies;

        BinaryFormatter binForm;
        FileStream fStream;

        SaveManager saver;

        int index;

        binForm = new BinaryFormatter( );
        fStream = File.Create( Application.dataPath + "/sceneFile" + levelNumber.ToString( ) + ".dat" );

        saver = new SaveManager( );

        //get towers and baddies
        sTowers = GameObject.FindGameObjectsWithTag( "Tower" );
        baddies = GameObject.FindGameObjectsWithTag( "Raider" );

        //save map
        saver.hm = path.heightMap;

        saver.pX  =new float[ path.points3D.Length ];
        saver.pY = new float[ path.points3D.Length ];
        saver.pZ = new float[ path.points3D.Length ];

        for( index = 0; index < path.points3D.Length; index++ )
        {
            saver.pX[ index ] = path.points3D[ index ].x;
            saver.pY[ index ] = path.points3D[ index ].y;
            saver.pZ[ index ] = path.points3D[ index ].z;
        }

        //save towers
        saver.tV = new int[ sTowers.Length ];
        saver.tX = new float[ sTowers.Length ];
        saver.tY = new float[ sTowers.Length ];
        saver.tZ = new float[ sTowers.Length ];

        saver.trW = new float[ sTowers.Length ];
        saver.trX = new float[ sTowers.Length ];
        saver.trY = new float[ sTowers.Length ];
        saver.trZ = new float[ sTowers.Length ];

        for( index = 0; index < sTowers.Length; index++ )
        {
            saver.tX[ index ] = sTowers[ index ].transform.position.x;
            saver.tY[ index ] = sTowers[ index ].transform.position.y;
            saver.tZ[ index ] = sTowers[ index ].transform.position.z;

            saver.trW[ index ] = sTowers[ index ].transform.rotation.w;
            saver.trX[ index ] = sTowers[ index ].transform.rotation.x;
            saver.trY[ index ] = sTowers[ index ].transform.rotation.y;
            saver.trZ[ index ] = sTowers[ index ].transform.rotation.z;

            //saver.tV[ index ] = sTowers[ index ].GetComponent<Tower>( ).version;
        }

        saver.startX = path.startWorld.x;
        saver.startY = path.startWorld.y;
        saver.startZ = path.startWorld.z;

        saver.endX = path.endWorld.x;
        saver.endY = path.endWorld.y;
        saver.endZ = path.endWorld.z;

        //save game facts
        saver.timeRemaining = timeToWave;
        saver.gold = goldAmnt;
        saver.baddyCount = enemyCount;
        saver.waves = wave;
        saver.lives = lives;

        //save baddies
        saver.bH = new float[ baddies.Length ];
        saver.bX = new float[ baddies.Length ];
        saver.bY = new float[ baddies.Length ];
        saver.bZ = new float[ baddies.Length ];
        saver.bS = new float[ baddies.Length ];

        saver.brW = new float[ baddies.Length ];
        saver.brX = new float[ baddies.Length ];
        saver.brY = new float[ baddies.Length ];
        saver.brZ = new float[ baddies.Length ];
        saver.bD = new bool[ baddies.Length ];

        saver.bC = new bool[ baddies.Length ];
        saver.bhA = new bool[ baddies.Length ];
        saver.blPR = new bool[ baddies.Length ];
        saver.bcP = new int[ baddies.Length ];

        for( index = 0; index < baddies.Length; index++ )
        {
            saver.bX[ index ] = baddies[ index ].transform.position.x;
            saver.bY[ index ] = baddies[ index ].transform.position.y;
            saver.bZ[ index ] = baddies[ index ].transform.position.z;

            saver.brW[ index ] = baddies[ index ].transform.rotation.w;
            saver.brX[ index ] = baddies[ index ].transform.rotation.x;
            saver.brY[ index ] = baddies[ index ].transform.rotation.y;
            saver.brZ[ index ] = baddies[ index ].transform.rotation.z;

            saver.bS[ index ] = baddies[ index ].GetComponent<Movement>( ).movementSpeed;

            saver.bD[ index ] = baddies[ index ].GetComponent<Movement>( ).mAnimator.GetBool( "Dead" );

            saver.bH[ index ] = baddies[ index ].GetComponent<Movement>( ).health;

            saver.bC[ index ] = baddies[ index ].GetComponent<Movement>( ).collided;
            saver.bhA[ index ] = baddies[ index ].GetComponent<Movement>( ).hasArtificat;
            saver.blPR[ index ] = baddies[ index ].GetComponent<Movement>( ).lastPointReached;
            saver.bcP[ index ] = baddies[ index ].GetComponent<Movement>( ).currentPoint;
        }

        //save the data
        binForm.Serialize( fStream, saver );
        fStream.Close( );

        return true;
    }

    public bool Load( int number )
    {
        GameObject[] sTowers;
        GameObject[] baddies;

        GameObject tmpGO;

        BinaryFormatter binForm;
        FileStream fStream;

        SaveManager loader;

        Movement tMv;

        int index;        

        if( !File.Exists( Application.dataPath + "/sceneFile" + number.ToString( ) + ".dat" ) )
        {
            return false;
        }

        levelNumber = number;
        binForm = new BinaryFormatter( );
        fStream = File.Open( Application.dataPath + "/sceneFile" +number.ToString( ) + ".dat", FileMode.Open );

        loader = ( SaveManager ) binForm.Deserialize( fStream );
        fStream.Close( );

        //get towers and baddies in scene and delete them
        sTowers = GameObject.FindGameObjectsWithTag( "Tower" );
        baddies = GameObject.FindGameObjectsWithTag( "Raider" );

        for( index = 0; index < sTowers.Length; index++ )
        {
            Destroy( sTowers[ index ] );
        }

        for( index = 0; index < baddies.Length; index++ )
        {
            Destroy( baddies[ index ] );
        }

        //load map
        path.heightMap = loader.hm;
        terrain.terrainData.SetHeights( 0, 0, path.heightMap );

        path.points3D = new Vector3[ loader.pX.Length ];

        for( index = 0; index < loader.pX.Length; index++ )
        {
            path.points3D[ index ] = new Vector3( loader.pX[ index ], loader.pY[ index ], loader.pZ[ index ] );
        }

        path.startWorld = new Vector3( loader.startX, loader.startY, loader.startZ );
        path.endWorld = new Vector3( loader.endX, loader.endY, loader.endZ );

        towerCount = 0;

        //load towers "assume valid"
        for( index = 0; index < loader.tX.Length; index++ )
        {
            tmpGO = Instantiate( towers[ loader.tV[ index ] ], 
                                 new Vector3( loader.tX[ index ], loader.tY[ index ], loader.tZ[ index ] ), 
                                 new Quaternion( loader.trX[ index ], loader.trY[ index ], loader.trZ[ index ], loader.trW[ index ] ) );

            //tmpGO.GetComponent<Tower>( ).end = path.endWorld;

            towerCount++;
        }

        //load baddies
        for( index = 0; index < loader.bS.Length; index++ )
        {
            path.PlaceRaider( );
        }

        baddies = GameObject.FindGameObjectsWithTag( "Raider" );

        for( index = 0; index < baddies.Length; index++ )
        {
            tMv = baddies[ index ].GetComponent<Movement>( );

            tMv.health = loader.bH[ index ];
            tMv.mAnimator.SetBool( "Dead", loader.bD[ index ] );
            tMv.movementSpeed = loader.bS[ index ];

            tMv.DecrementHealth( 0 );

            tMv.currentPoint = loader.bcP[ index ];
            tMv.hasArtificat = loader.bhA[ index ];
            tMv.collided = loader.bC[ index ];
            tMv.lastPointReached = loader.blPR[ index ];

            baddies[ index ].transform.position = new Vector3( loader.bX[ index ], loader.bY[ index ], loader.bZ[ index ] );
            baddies[ index ].transform.rotation = new Quaternion( loader.brX[ index ], loader.brY[ index ], loader.brZ[ index ], loader.brW[ index ] );
        }

        //game facts
        timeToWave = loader.timeRemaining;
        goldAmnt = loader.gold;
        enemyCount = loader.baddyCount;
        wave = loader.waves;
        lives = loader.lives;

        gameOverTxt.text = "";
        goldAmntTxt.text = "Gold: " + goldAmnt.ToString( );

        livesTxt.text = "Lives: " + lives.ToString( );

        timeTxt.text = "Next Wave: " + timeToWave.ToString( ) + " s";

        waveNumberTxt.text = "Wave: " + wave.ToString( );


        //artifact
        path.PlaceArtifact( );

        return true;
    }


    // Use this for initialization
    void Start ()
    {
        gameOverTxt.text = "";
        goldAmntTxt.text = "Gold: " + goldAmnt.ToString( );

        livesTxt.text = "Lives: " + lives.ToString( );

        timeTxt.text = "Next Wave: " + timeToWave.ToString( ) + " s";

        waveNumberTxt.text = "Wave: " + wave.ToString( );

        mLook = new UnityStandardAssets.Characters.FirstPerson.MouseLook( );

        mLook.Init( cameraHolder.transform, mCamera.transform );

        mLook.SetCursorLock( false );
	}
	
	// Update is called once per frame
	void Update ()
    {
        Vector3 tmpVec3;

        timeToWave -= Time.deltaTime;

        if( timeToWave < 0 )
        {
            timeToWave = 0;
        }

        timeTxt.text = "Next Wave: " + timeToWave.ToString( "F0" ) + " s";

        if( timeToWave <= 0 && wave < 10 )
        {
            timeToWave = Random.Range( 15.0f, 30.0f );
            SpawnWave( );
            wave++;
            waveNumberTxt.text = "Wave: " + wave.ToString( );
        }

        if( Input.GetKeyDown( KeyCode.Escape ) )
        {
            Application.Quit( );
        }

        if( wave >= 10 && enemyCount == 0 )
        {
            NextLevel( );
            
        }

        if( Input.GetButtonDown( "Fire1" ) )
        {

            leftMouseDown = true;
            SpawnDummy( );
        }

        if( leftMouseDown )
        {

            if( tdummy != null )
            {
                MoveDummy( );
            }
            else
            {
                SpawnDummy( );
            }
        }
            

        if( Input.GetButtonUp( "Fire1" ) )
        {
            leftMouseDown = false;

            PlaceTower( );

            if( tdummy != null )
            {
                Destroy( tdummy );
                tdummy = null;
            }
            
        }

        if( Input.GetAxis( "Vertical" ) > 0.0f )
        {
            tmpVec3 = cameraHolder.transform.position + mCamera.transform.forward * 100 * Time.deltaTime;

            if( tmpVec3.y < 100 )
            {
                tmpVec3.y = 100;
            }

            cameraHolder.transform.position = tmpVec3;
        }

        if( Input.GetAxis( "Vertical" ) < 0.0f )
        {
            tmpVec3 = cameraHolder.transform.position - mCamera.transform.forward * 100 * Time.deltaTime;

            if( tmpVec3.y < 100 )
            {
                tmpVec3.y = 100;
            }

            cameraHolder.transform.position = tmpVec3;
        }

        if( Input.GetAxis( "Horizontal" ) > 0.0f )
        {
            cameraHolder.transform.position += cameraHolder.transform.right * 100 * Time.deltaTime;
        }

        if( Input.GetAxis( "Horizontal" ) < 0.0f )
        {
            cameraHolder.transform.position += cameraHolder.transform.right * -100 * Time.deltaTime;
        }

        if( Input.GetButtonDown( "Fire2") )
        {
            rightMouseDown = true;
        }

        if( Input.GetButtonUp( "Fire2" ) )
        {
            rightMouseDown = false;
        }

        if( rightMouseDown )
        {
            mLook.LookRotation( cameraHolder.transform, mCamera.transform );

            tmpVec3 = cameraHolder.transform.localRotation.eulerAngles;

            tmpVec3.z = 0;

            cameraHolder.transform.localRotation = Quaternion.Euler( tmpVec3 );
        }

    }

    public void AddGold( int amnt )
    {
        if( amnt > 0 )
        {
            enemyCount--;
        }

        goldAmnt += amnt;
        goldAmntTxt.text = "Gold: " + goldAmnt.ToString( );
    }

    private void SpawnWave( )
    {
        int baddyNumber = ( int ) ( Random.Range( 0.0f, (float)( wave ) ) + 1 ) * 3;

        int index;

        for( index = 0; index < baddyNumber; index++ )
        {
            path.PlaceRaider( );
            enemyCount++;
        }

    }

    void SpawnDummy( )
    {
        Vector3 mPos;

        Ray ray;
        RaycastHit hit;        

        if( goldAmnt < tCost[ 0 ] )
        {
            return;
        }

        mPos = Input.mousePosition;
        ray = mCamera.ScreenPointToRay( mPos );

        if( Physics.Raycast( ray, out hit, 1000.0f ) )
        {
            if( hit.collider.tag != "Tower" )
            {
                if( dummyTower != null && tdummy == null )
                {
                    tdummy = Instantiate( dummyTower );
                }
            }
        }
    }

    void MoveDummy( )
    {
        Vector3 mPos;
        Vector3 wPos;

        Ray ray;
        RaycastHit hit;
        Quaternion newRotDir;

        Vector3 normal;

        mPos = Input.mousePosition;
        ray = mCamera.ScreenPointToRay( mPos );

        if( Physics.Raycast( ray, out hit, 1000.0f ) )
        {
            wPos = hit.point;

            wPos.y = terrain.SampleHeight( wPos );

            normal = hit.normal;

            newRotDir = Quaternion.FromToRotation( Vector3.up, normal );

            //wPos += towers[ 0 ].GetComponent<Tower>( ).spawnHeightOffset * normal;

            tdummy.transform.position = wPos;
            tdummy.transform.rotation = newRotDir;
        }      


    }

    void PlaceTower( )
    {
        Vector3 mPos; 
        Vector3 wPos;

        Ray ray;
        RaycastHit hit;
        Quaternion newRotDir;

        Vector3 normal;

        GameObject tmp;

        int selection = 0;



        if( goldAmnt < tCost[ selection ] )
        {            
            return;
        }


        mPos = Input.mousePosition;
        ray = mCamera.ScreenPointToRay( mPos );

        if( Physics.Raycast( ray, out hit, 1000.0f) )
        {
            if( hit.collider.tag != "Terrain" && hit.collider.tag != "Tower" )
            {
                return;
            }

            if( hit.collider.gameObject.tag != "Tower" )
            {
                wPos = hit.point;

                wPos.y = terrain.SampleHeight( wPos );

                normal = hit.normal;

                newRotDir = Quaternion.FromToRotation( Vector3.up, normal );

                if( wPos.y == terrain.SampleHeight( path.startWorld ) )
                {
                    return;
                }

                if( towerCount < 12 )
                {
                    towerCount++;
                }
                else
                {
                    StartCoroutine( TowerLimit( ) );
                    return;
                }
            }
            else
            {
                normal = hit.collider.gameObject.transform.up;

                //selection = hit.collider.gameObject.GetComponent<Tower>( ).version + 1;

                if( selection >= towers.Length )
                {
                    return;
                }

                if( goldAmnt < tCost[ selection ] )
                {
                    return;
                }

                wPos = hit.collider.gameObject.transform.position;

                //wPos -= hit.collider.gameObject.GetComponent<Tower>( ).spawnHeightOffset * normal;

                newRotDir = hit.collider.gameObject.transform.rotation;

                Destroy( hit.collider.gameObject );
            }

            //wPos += towers[ selection ].GetComponent<Tower>( ).spawnHeightOffset * normal;

            tmp = Instantiate( towers[ selection ], wPos, newRotDir );

            //tmp.GetComponent<Tower>( ).end = path.endWorld;

            goldAmnt -= tCost[ selection ];

            goldAmntTxt.text = "Gold: " + goldAmnt.ToString( );
        }

    }

    public void ArtifactStolen( )
    {
        if( SubtractLife( ) )
        {
            StartCoroutine( ArtifactStolenRoutine( ) );
        }
              
    }

    IEnumerator ArtifactStolenRoutine( )
    {
        gameOverTxt.text = "Artifact stolen!";
        yield return new WaitForSeconds( 1.25f );
        gameOverTxt.text = "";
    }

    public bool SubtractLife( )
    {
        lives -= 1;        

        if( lives <= 0 )
        {
            lives = 0;
            EndGame( );
        }

        livesTxt.text = "Lives: " + lives.ToString( );

        return ( lives != 0 );
    }

    public void EndGame( )
    {
        gameOverTxt.text = "GAME OVER";

        StartCoroutine( EndGameTimer( ) );
    }

    public void NextLevel( )
    {
        gameOverTxt.text = "Level Complete";

        StartCoroutine( NextLevelTimer( ) );
    }

    IEnumerator NextLevelTimer( )
    {
        //SceneChanger[] sceneChanger;

        yield return new WaitForSeconds( 2.5f );

        //sceneChanger = FindObjectsOfType<SceneChanger>( );

        //if( sceneChanger.Length > 0 )
        {
            //sceneChanger[ 0 ].ChangeScene( true );
        }
    }

    IEnumerator EndGameTimer( )
    {
        yield return new WaitForSeconds( 2.5f );

        SceneManager.LoadScene( "scene0" );
    }


    IEnumerator TowerLimit( )
    {
        gameOverTxt.text = "Tower limit reached!";

        yield return new WaitForSeconds( 0.5f );

        gameOverTxt.text = "";
    }
}


[System.Serializable]
class SaveManager
{ 
    public float[] tX;
    public float[] tY;
    public float[] tZ;
    public int[] tV;

    public float[] trW;
    public float[] trX;
    public float[] trY;
    public float[] trZ;


    public int gold;
    public int lives;
    public float timeRemaining;
    public int waves;


    public float[,] hm;

    public float[] pX;
    public float[] pY;
    public float[] pZ;

    public float startX, startY, startZ;
    public float endX, endY, endZ;

    public float[] bX;
    public float[] bY;
    public float[] bZ;
    public float[] bH;
    public float[] bS;

    public float[] brW;
    public float[] brX;
    public float[] brY;
    public float[] brZ;
    public bool[] bD;
    public bool[] bC;
    public bool[] bhA;
    public bool[] blPR;
    public int[] bcP;
 
    public int baddyCount;

};

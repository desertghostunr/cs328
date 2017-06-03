using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder( -500 )]
public class ModeController : MonoBehaviour
{
    [System.Flags]
    public enum MODE
    {
        none = 0,
        singleplayer = 1,
        multiplayer = 2,
        hunter = 4,
        hunted = 8,
        hotseat = 12, //same as hotseat
        all = 15 //will default to hotseat
    }

    public int mode = 0;
    public float aiIntelligence = 0.0f;
    
    public GameObject boyPrefab = null;
    public GameObject wolfPrefab = null;
    public GameObject boyHSPrefab = null;
    public GameObject wolfHSPrefab = null;
    public GameObject boyAIPrefab = null;
    public GameObject wolfAIPrefab = null;

    public Vector3 defaultBoyLocation =  new Vector3( 254.65f, 0.19f, 41.6f );
    public Vector3 defaultWolfLocation = new Vector3( 165.1f, 0.94f, 62.2f );

    private PathScript pathCreator;

    public static bool IsThere( int value, int flag )
    {
        return ( value & flag ) == flag;
    }

    // Use this for initialization
    void Start( )
    {
        GameObject hunter, hunted;

        SimpleNPCIntelligence ai;

        /*if ( mode == ( int ) MODE.none || mode == ( int ) MODE.all )
        {
            mode = ( int ) MODE.hotseat;
        }*/

        pathCreator = FindObjectOfType<PathScript>( );

        pathCreator.huntedAI = false;
        pathCreator.hunterAI = false;

        if ( IsThere( mode, ( int ) MODE.singleplayer )
            || IsThere( mode, ( int ) MODE.multiplayer ) )
        {
            if ( IsThere( mode, ( int ) MODE.hunter ) )
            {
                pathCreator.huntedAI = true;
                pathCreator.hunterAI = false;

                hunted = boyAIPrefab == null ? Resources.Load( "Prefabs/AI/Boy", typeof( GameObject ) ) as GameObject : boyAIPrefab;
                hunter = wolfPrefab == null ? Resources.Load( "Prefabs/Single-multi-player/Wolf", typeof( GameObject ) ) as GameObject : wolfPrefab;
            }
            else if ( IsThere( mode, ( int ) MODE.hunted ) )
            {
                pathCreator.hunterAI = true;
                pathCreator.huntedAI = false;

                hunted = boyPrefab == null ? Resources.Load( "Prefabs/Single-multi-player/Boy", typeof( GameObject ) ) as GameObject : boyPrefab;
                hunter = wolfAIPrefab == null ? Resources.Load( "Prefabs/AI/Wolf", typeof( GameObject ) ) as GameObject : wolfAIPrefab;
            }
            else
            {
                Debug.Log( "Invalid code given defaulting to hotseat mode!" );

                hunted = boyHSPrefab == null ? Resources.Load( "Prefabs/Hotseat/Boy", typeof( GameObject ) ) as GameObject : boyHSPrefab;
                hunter = wolfHSPrefab == null ? Resources.Load( "Prefabs/Hotseat/Wolf", typeof( GameObject ) ) as GameObject : wolfHSPrefab;
            }
        }
        else
        {
            hunted = boyHSPrefab == null ? Resources.Load( "Prefabs/Hotseat/Boy", typeof( GameObject ) ) as GameObject : boyHSPrefab;
            hunter = wolfHSPrefab == null ? Resources.Load( "Prefabs/Hotseat/Wolf", typeof( GameObject ) ) as GameObject : wolfHSPrefab;
        }

        hunted = Instantiate( hunted, defaultBoyLocation, Quaternion.identity );
        hunter = Instantiate( hunter, defaultWolfLocation, Quaternion.identity );

        hunted.name = "Boy";
        hunter.name = "Wolf";

        hunted.SetActive( false );
        hunted.SetActive( true );
        hunter.SetActive( false );
        hunter.SetActive( true );

        //set ai difficultly
        ai = hunted.GetComponent<SimpleNPCIntelligence>( );
        
        if( ai )
        {
            ai.intelligence = aiIntelligence;
        }

        ai = hunter.GetComponent<SimpleNPCIntelligence>( );

        if ( ai )
        {
            ai.intelligence = aiIntelligence;
        }

        //remove prefabs as they are no longer needed
        boyPrefab = null;
        wolfPrefab = null;
        boyHSPrefab = null;
        wolfHSPrefab = null;
        boyAIPrefab = null;
        wolfAIPrefab = null;
    }
	
	// Update is called once per frame
	void Update ()
    {
		
	}
}

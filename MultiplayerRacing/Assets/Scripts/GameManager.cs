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

public class GameManager : MonoBehaviour
{
    public Text gameTxt;

    public bool started = false;

    private bool gameOver = false;

    public float delay = 5.0f;

    private string winner;

    public PathScript path;

    public Rigidbody[ ] players;

    private void Start( )
    {
        StartCoroutine( StartDelay( ) );        
    }

    private void Update( )
    {
        int index; 

        if( !started )
        {
            for(index = 0; index < players.Length; index++ )
            {
                players[index].velocity = Vector3.zero;
                players[index].angularVelocity = Vector3.zero;
            }
        }

        if ( Input.GetKey( KeyCode.Escape ) )
        {
            SceneManager.LoadScene( "scene0" );
        }
    }

    public void SetWinner( string name )
    {
        if( !gameOver )
        {
            winner = name;

            gameOver = true;
            NextLevel( );
        }
    }


    public void NextLevel( )
    {
        gameTxt.text = winner + " wins! Loading next level...";

        StartCoroutine( NextLevelTimer( ) );
    }

    IEnumerator NextLevelTimer( )
    {

        yield return new WaitForSeconds( 2.5f );

        SceneManager.LoadScene( "scene1" );
    }

    IEnumerator StartDelay( )
    {

        gameTxt.text = "Starting in " + delay.ToString( "F0" );

        yield return new WaitForSeconds( 1.0f );

        delay -= 1.0f;

        if ( delay > 0 )
        {
            StartCoroutine( StartDelay( ) );
            
        }
        else
        {
            started = true;
            gameTxt.text = "GO!";

            transform.position = path.endWorld;

            yield return new WaitForSeconds( 0.5f );

            gameTxt.text = "";
        }



    }
}

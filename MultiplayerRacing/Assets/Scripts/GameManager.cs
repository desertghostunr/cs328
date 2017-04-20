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

    public GameObject panel;

    private string winner;

    private int winCounter = 0;

    public PathScript path;

    public Rigidbody[ ] players;

    public Money[ ] playerMoney;

    public PlayerController[ ] playerController;

    public PersistentDataBase pDB;

    private void Start( )
    {
        panel.SetActive( false );
        Cursor.visible = false;
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
            SceneManager.LoadScene( "Menu" );
        }
    }

    public bool SetWinner( string name )
    {
        bool first = false;

        if( !gameOver )
        {
            winner = name;

            gameOver = true;

            gameTxt.text = winner + " wins!";
            first = true;
        }

        winCounter++;

        if( winCounter >= 2 )
        {
            Cursor.visible = true;
            panel.SetActive( true );
        }

        return first;
    }

    public void IncreaseSpeed( int player )
    {
        if( player > playerMoney.Length || player > playerController.Length )
        {
            return;
        }

        if( playerMoney[ player ].moneyAmnt < 5 )
        {
            return;
        }

        playerMoney[player].AddMoney( -5 );
        playerController[player].speedMultiplier += 2.0f;
    }

    public void IncreaseRotationSpeed( int player )
    {
        if ( player > playerMoney.Length || player > playerController.Length )
        {
            return;
        }

        if ( playerMoney[player].moneyAmnt < 5 )
        {
            return;
        }

        playerMoney[player].AddMoney( -5 );
        playerController[player].rotateMultiplier += 2.0f;
    }


    public void NextLevel( )
    {
        if( pDB != null )
        {
            pDB.p1Data.Speed = playerController[0].speedMultiplier;
            pDB.p1Data.RotSpeed = playerController[0].rotateMultiplier;
            pDB.p1Data.money = playerMoney[0].moneyAmnt;

            pDB.p2Data.Speed = playerController[1].speedMultiplier;
            pDB.p2Data.RotSpeed = playerController[1].rotateMultiplier;
            pDB.p2Data.money = playerMoney[1].moneyAmnt;
        }
        gameTxt.text = "Loading!";
        panel.SetActive( false );
        Cursor.visible = false;
        SceneManager.LoadScene( "scene1" );
    }

    public void GotoMenu()
    {
        SceneManager.LoadScene("Menu");
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

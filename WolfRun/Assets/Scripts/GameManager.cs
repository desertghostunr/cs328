using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // Public variables


    // Private variables
    private bool isPaused = false;
    private GameObject pausePanel;

    private Text victoryText;
    private string winner;
    private bool gameGoing;

    private void Start()
    {
        pausePanel = GameObject.Find("Pause Panel");
        pausePanel.SetActive(false);

        victoryText = GameObject.FindGameObjectWithTag( "VictoryText" ).GetComponent<Text>( );

        if( victoryText != null )
        {
            victoryText.text = "";
        }

        gameGoing = true;
    }

    private void Update ()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            TogglePauseState();
        }
	}    

    public void TogglePauseState()
    {
        if (isPaused)
        {
            // Unpause game
            Time.timeScale = 1;
            pausePanel.SetActive(false);
            isPaused = false;
        }
        else
        {
            // Pause game
            Time.timeScale = 0;
            pausePanel.SetActive(true);
            isPaused = true;
        }
    }

    public void SetWinner( string name )
    {
        winner = name;

        if( victoryText != null )
        {
            victoryText.text = name + " won!";
        }
        
        StartCoroutine( GameOver( ) );
    }

    IEnumerator GameOver( )
    {
        gameGoing = false;
        yield return new WaitForSeconds( 5.0f );
        gameGoing = true;
        SceneManager.LoadScene( "Menu" );
    }
}

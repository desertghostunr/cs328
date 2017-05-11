using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // Public variables
    public GameObject blackScreenCamera;

    // Private variables
    private bool isPaused = false;
    private GameObject pausePanel;
    private GameObject defaultScreen;
    private GameObject instructionScreen;

    private GameObject victoryText;
    private string winner;
    private bool gameGoing;

    private Camera[] gameCameras;
    private bool isBlackScreen = false;

    private void Start()
    {
        pausePanel = GameObject.Find("Pause Panel");
        defaultScreen = GameObject.Find("Default");
        instructionScreen = GameObject.Find("Instructions");

        // Order of disable important
        defaultScreen.SetActive(false);
        instructionScreen.SetActive(false);
        pausePanel.SetActive(false);

        victoryText = GameObject.FindGameObjectWithTag("VictoryText");

        if( victoryText != null )
        {
            victoryText.SetActive(false);  // if only set to empty, blocks 'Resume'
        }

        gameGoing = true;

        gameCameras = Camera.allCameras;
    }

    private void Update()
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
            instructionScreen.SetActive(false);
            defaultScreen.SetActive(true);
            isPaused = true;
        }
    }

    public void GotoDefaultScreen()
    {
        instructionScreen.SetActive(false);
        defaultScreen.SetActive(true);
    }

    public void GotoInstructions()
    {
        defaultScreen.SetActive(false);
        instructionScreen.SetActive(true);
    }

    public void GotoMainMenu()
    {
        TogglePauseState();
        SceneManager.LoadScene("Menu");
    }

    public void ToggleBlackScreen()
    {
        blackScreenCamera.SetActive(!isBlackScreen);
    }

    public void SetWinner( string name )
    {
        if( gameGoing )
        {
            winner = name;
        }        

        if( victoryText != null )
        {
            victoryText.SetActive(true);
            victoryText.GetComponent<Text>().text = winner + " won!";
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

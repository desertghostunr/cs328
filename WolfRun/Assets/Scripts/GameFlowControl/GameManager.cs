﻿using System.Collections;
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
    private RotateCameraMouse mouseRotationScript;

    private AudioSource [] audioSources;

    private GameObject victoryText;
    private string winner;
    private bool gameGoing;

    private bool isBlackScreen = false;

    private void Start()
    {
        pausePanel = GameObject.Find("Pause Panel");
        defaultScreen = GameObject.Find("Default");
        instructionScreen = GameObject.Find("Instructions");

        mouseRotationScript = FindObjectOfType<RotateCameraMouse>( );

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

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
        int index;

        audioSources = FindObjectsOfType<AudioSource>( );

        if( mouseRotationScript )
        {
            mouseRotationScript.enabled = !( mouseRotationScript.enabled );
        }
        

        if (isPaused)
        {
            // Unpause game
            Time.timeScale = 1;
            pausePanel.SetActive(false);
            isPaused = false;

            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;

            for( index = 0; index < audioSources.Length; index++ )
            {
                audioSources[index].UnPause( );
            }
        }
        else
        {
            // Pause game
            Time.timeScale = 0;
            pausePanel.SetActive(true);
            instructionScreen.SetActive(false);
            defaultScreen.SetActive(true);
            isPaused = true;

            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;

            for ( index = 0; index < audioSources.Length; index++ )
            {
                audioSources[index].Pause( );
            }
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
        if (isBlackScreen) ToggleBlackScreen();
        SceneManager.LoadScene( "Menu" );
    }
}

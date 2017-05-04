using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Public variables


    // Private variables
    bool isPaused = false;
    GameObject pausePanel;

    private void Start()
    {
        pausePanel = GameObject.Find("Pause Panel");
        pausePanel.SetActive(false);
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
}


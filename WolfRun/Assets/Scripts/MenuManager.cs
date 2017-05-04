using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    private GameObject[] mainScreenObjs, howToScreenObjs;

    private void Start()
    {
        mainScreenObjs = GameObject.FindGameObjectsWithTag("MainScreen");
        howToScreenObjs = GameObject.FindGameObjectsWithTag("HowToScreen");

        ShowHowToScreen(false);
    }

    public void PlayGame()
    {
        SceneManager.LoadScene("scene1");
    }

    public void GotoHowToScreen()
    {
        ShowMainScreen(false);
        ShowHowToScreen(true);
    }

    public void GotoMainScreen()
    {
        ShowHowToScreen(false);
        ShowMainScreen(true);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    private void ShowMainScreen(bool shouldShow)
    {
        foreach (GameObject mainObj in mainScreenObjs)
        {
            mainObj.SetActive(shouldShow);
        }
    }

    private void ShowHowToScreen(bool shouldShow)
    {
        foreach (GameObject howObj in howToScreenObjs)
        {
            howObj.SetActive(shouldShow);
        }
    }
}

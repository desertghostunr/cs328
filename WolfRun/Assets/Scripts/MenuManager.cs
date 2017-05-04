using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
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
        foreach (GameObject mainObj in GameObject.FindGameObjectsWithTag("MainScreen"))
        {
            mainObj.SetActive(shouldShow);
        }
    }

    private void ShowHowToScreen(bool shouldShow)
    {
        foreach (GameObject howObj in GameObject.FindGameObjectsWithTag("HowToScreen"))
        {
            howObj.SetActive(shouldShow);
        }
    }
}

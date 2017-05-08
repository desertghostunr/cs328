using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    private GameObject[] mainScreenObjs, howToScreenObjs;
    private Text loadingText;
    private string loadingStr;

    private void Start()
    {
        mainScreenObjs = GameObject.FindGameObjectsWithTag("MainScreen");
        howToScreenObjs = GameObject.FindGameObjectsWithTag("HowToScreen");
        loadingText = GameObject.Find("Loading").GetComponent<Text>();
        loadingStr = loadingText.text;
        loadingText.text = "";

        ShowHowToScreen(false);
    }

    public void PlayGame()
    {
        loadingText.text = loadingStr;
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

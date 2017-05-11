using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    //private GameObject mainScreen, howToScreen, loadingScreen;
    //public Text loadingText;

    private void Start()
    {
        //mainScreen = GameObject.FindGameObjectWithTag("MainScreen");
        //howToScreen = GameObject.FindGameObjectWithTag("HowToScreen");
        //loadingScreen = GameObject.FindGameObjectWithTag("Loading");

        //if (loadingScreen != null)
        //{
        //    loadingText = loadingScreen.GetComponentInChildren<Text>();
        //    loadingStr = loadingText.text;
        //    loadingText.text = "";
        //}

        //ShowHowToScreen(false);
    }

    public void PlayGame()
    {
        //loadingText.text = loadingStr;
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
        //foreach (GameObject mainObj in mainScreenObjs)
        //{
        //    mainObj.SetActive(shouldShow);
        //}
    }

    private void ShowHowToScreen(bool shouldShow)
    {
        //foreach (GameObject howObj in howToScreenObjs)
        //{
        //    howObj.SetActive(shouldShow);
        //}
    }
}

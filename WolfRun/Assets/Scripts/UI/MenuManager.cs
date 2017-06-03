using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine;
using System.Collections;

public class MenuManager : MonoBehaviour
{
    private AsyncOperation loadingOperation;

    private void Start( )
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void PlayGame()
    {
        StartCoroutine( LoadGame( ) );
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    IEnumerator LoadGame( )
    {
        loadingOperation = SceneManager.LoadSceneAsync( "scene1", LoadSceneMode.Single );

        yield return loadingOperation;
    }
}

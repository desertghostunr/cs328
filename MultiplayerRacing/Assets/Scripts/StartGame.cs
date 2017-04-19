using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartGame : MonoBehaviour {

	// Use this for initialization
	void Start () {
        Cursor.visible = true;
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    public void LoadGame( )
    {
        SceneManager.LoadScene( "scene1" );
    }
}

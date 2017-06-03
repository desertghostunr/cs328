using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingBar : MonoBehaviour
{

    public string basicName = "Growing Field";
    public string endStatusSymbol = ".";
    public int maxNumberOfEndStatusSymbols = 3;

    private Text displayedText;
    private bool returnedFromCoroutine = false;
    
    private void OnEnable( )
    {

        displayedText = GetComponent<Text>( );

        displayedText.text = basicName;

        returnedFromCoroutine = true;
        
    }

    // Update is called once per frame
    void Update ()
    {
		if( returnedFromCoroutine )
        {
            StartCoroutine( ChangeSymbols( ) );
        }
	}

    IEnumerator ChangeSymbols( )
    {
        int index = 0;

        if( displayedText != null )
        {
            displayedText.text = basicName;
        }
        
        returnedFromCoroutine = false;

        for ( index = 0; index < maxNumberOfEndStatusSymbols; index++ )
        {
            if( displayedText != null )
            {
                displayedText.text += endStatusSymbol;
                yield return new WaitForSeconds( 0.5f );
            }
        }

        returnedFromCoroutine = true;
    }

}

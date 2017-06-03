using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-600)]
public class SettingsManager : MonoBehaviour
{
    // Use this for initialization
    void Start ()
    {
        ModeController modeControl;

        if( !SettingsSingleton.GetSettings( ) )
        {
            Debug.Log( "Error: no settings singleton object!" );
            return;
        }

        modeControl = FindObjectOfType<ModeController>( );
        
        if ( modeControl )
        {
            modeControl.mode = SettingsSingleton.GetSettings( ).mode;
            modeControl.aiIntelligence = SettingsSingleton.GetSettings( ).aiIntelligence;
        }

	}

    public void SetMode(int mode )
    {
        SettingsSingleton.GetSettings( ).mode = mode;
    }

    public void SetOpponentDifficulty( float difficulty )
    {
        SettingsSingleton.GetSettings( ).aiIntelligence = difficulty;
    }
}

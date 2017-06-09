using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleManager : MonoBehaviour
{
    public string prefsName;

    private Toggle m_switch;
    private bool m_changed = false;

    private void OnEnable( )
    {
        m_switch = GetComponent<Toggle>( );

        m_switch.isOn = SettingsManager.IntToBool( PlayerPrefs.GetInt( prefsName ) );
    }

    // Update is called once per frame
    void Update ()
    {
		if( m_changed )
        {
            m_switch.isOn = SettingsManager.IntToBool( PlayerPrefs.GetInt( prefsName ) );

            m_changed = false;
        }
	}

    public void OnValueSelect( )
    {
        m_changed = true;
    }
}

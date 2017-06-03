using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder( -900 )]
public class SettingsSingleton : MonoBehaviour
{

    public int mode = 0;
    public float aiIntelligence = 0;

    private static SettingsSingleton m_singleton = null;

    private void Awake( )
    {
        if ( m_singleton != null && m_singleton != this )
        {
            Destroy( gameObject );
        }
        else
        {
            m_singleton = this;

            DontDestroyOnLoad( gameObject );
        }
    }

    public static SettingsSingleton GetSettings( )
    {
        return m_singleton;
    }
}

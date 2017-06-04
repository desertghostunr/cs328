using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder( -900 )]
public class SettingsSingleton : MonoBehaviour
{
    public static readonly string tDetailDistanceStr = "tDetailDist";
    public static readonly string tDetailDensityStr = "tDetailDensity";
    public static readonly string tTreeDistanceStr = "tTreeDist";
    public static readonly string tBillboardStartStr = "tBillboardStart";
    public static readonly string tFadeLengthStr = "tFadeLen";
    public static readonly string tMaxMeshTreesStr = "tMaxMeshTrees";
    public static readonly string aiIntelligenceStr = "aiIntelligence";

    public int mode = 0;
    public float aiIntelligence = 0;

    public float tDetailDistance = 140.0f;
    public float tDetailDensity = 1.0f;
    public float tTreeDistance = 1000.0f;
    public float tBillboardStart = 50.0f;
    public float tFadeLength = 20.0f;
    public int tMaxMeshTrees = 100;

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

            LoadSettingsFromDisk( );
        }
    }

    public static SettingsSingleton GetSettings( )
    {
        return m_singleton;
    }

    public void LoadSettingsFromDisk( )
    {
        aiIntelligence = PlayerPrefs.GetFloat( aiIntelligenceStr, aiIntelligence );

        tBillboardStart = PlayerPrefs.GetFloat( tBillboardStartStr, tBillboardStart );
        tDetailDensity = PlayerPrefs.GetFloat( tDetailDensityStr, tDetailDensity );
        tDetailDistance = PlayerPrefs.GetFloat( tDetailDistanceStr, tDetailDistance );
        tFadeLength = PlayerPrefs.GetFloat( tFadeLengthStr, tFadeLength );
        tMaxMeshTrees = PlayerPrefs.GetInt( tMaxMeshTreesStr, tMaxMeshTrees );
        tTreeDistance = PlayerPrefs.GetFloat( tTreeDistanceStr, tTreeDistance );
    }
}

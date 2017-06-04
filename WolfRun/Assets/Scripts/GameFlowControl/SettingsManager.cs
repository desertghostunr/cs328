using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-600)]
public class SettingsManager : MonoBehaviour
{
    private bool m_requiresSave = false;

    // Use this for initialization
    void Start ()
    {
        int index;

        ModeController modeControl;

        Terrain[ ] terrains;

        if( !SettingsSingleton.GetSettings( ) )
        {
            Debug.Log( "Error: no settings singleton object!" );
            return;
        }

        modeControl = FindObjectOfType<ModeController>( );

        terrains = FindObjectsOfType<Terrain>( );
        
        if ( modeControl )
        {
            modeControl.mode = SettingsSingleton.GetSettings( ).mode;
            modeControl.aiIntelligence = SettingsSingleton.GetSettings( ).aiIntelligence;
        }

        for( index = 0; index < terrains.Length; index++ )
        {
            terrains[index].detailObjectDensity = SettingsSingleton.GetSettings( ).tDetailDensity;
            terrains[index].detailObjectDistance = SettingsSingleton.GetSettings( ).tDetailDistance;
            terrains[index].treeBillboardDistance = SettingsSingleton.GetSettings( ).tBillboardStart;
            terrains[index].treeDistance = SettingsSingleton.GetSettings( ).tTreeDistance;
            terrains[index].treeMaximumFullLODCount = SettingsSingleton.GetSettings( ).tMaxMeshTrees;
            terrains[index].treeCrossFadeLength = SettingsSingleton.GetSettings( ).tFadeLength;
        }
	}

    public void SetMode(int mode )
    {
        SettingsSingleton.GetSettings( ).mode = mode;
    }

    public void SetOpponentDifficulty( float difficulty )
    {
        m_requiresSave |= ( SettingsSingleton.GetSettings( ).aiIntelligence != difficulty );

        SettingsSingleton.GetSettings( ).aiIntelligence = difficulty;

        PlayerPrefs.SetFloat( SettingsSingleton.aiIntelligenceStr, SettingsSingleton.GetSettings( ).aiIntelligence );
    }

    //quality settings

    //quality preset
    public void SetQualityPreset( string name )
    {
        string[] names = QualitySettings.names;

        int index = 0;

        for( index = 0; index < names.Length; index++ )
        {
            if( name == names[ index ] )
            {
                QualitySettings.SetQualityLevel( index, true );

                break;
            }
        }
    }
    
    // 0 or less for disabled
    // 1 for enabled
    // 2 or greater for force enabled
    public void SetAnisotropicFiltering( int setting )
    {
        if( setting <= 0 )
        {
            QualitySettings.anisotropicFiltering = AnisotropicFiltering.Disable;
        }
        else if( setting >= 2 )
        {
            QualitySettings.anisotropicFiltering = AnisotropicFiltering.ForceEnable;
        }
        else
        {
            QualitySettings.anisotropicFiltering = AnisotropicFiltering.Enable;
        }
    }

    
    //0, 2, 4, or 8
    public void SetAntiAliasing( int setting )
    {
        setting = Mathf.Max( 0, Mathf.Min( setting, 8 ) );

        if( setting % 2 != 0 )
        {
            setting += 1;
        }

        if( setting % 3 == 0 )
        {
            setting = 4;
        }

        QualitySettings.antiAliasing = setting;
    }

    
    public void SetBillBoardFaceCamera( bool faceCamera )
    {
        QualitySettings.billboardsFaceCameraPosition = faceCamera;
    }

    //1, 2, 4
    public void SetBlendWeights( int setting )
    {
        if( setting <= 1 )
        {
            QualitySettings.blendWeights = BlendWeights.OneBone;
        }
        else if( setting > 2 )
        {
            QualitySettings.blendWeights = BlendWeights.FourBones;
        }
        else
        {
            QualitySettings.blendWeights = BlendWeights.TwoBones;
        }
    }


    //should be 0.0f to 1.0f, but in the Unity settings Fantastic is at 2.0f ... so 0.1f to 2.0f ...
    public void SetLodBias( float setting )
    {
        setting = Mathf.Max( 0.1f, Mathf.Min( setting, 2.0f ) );

        QualitySettings.lodBias = setting;
    }

    public void SetRealtimeReflectionProbes( bool realtimeReflection )
    {
        QualitySettings.realtimeReflectionProbes = realtimeReflection;
    }

    public void SetSoftParticles( bool softParticles )
    {
        QualitySettings.softParticles = softParticles;
    }

    public void SetSoftVegetation(  bool softVegetation )
    {
        QualitySettings.softVegetation = softVegetation;
    }

    //0 to 2
    public void SetvSyncCount( int setting )
    {
        setting = Mathf.Max( 0, Mathf.Min( setting, 2 ) );

        QualitySettings.vSyncCount = setting;
    }

    //0 to 2
    public void SetShadowQuality( int setting )
    {
        if( setting <= 0 )
        {
            QualitySettings.shadows = ShadowQuality.Disable;
        }
        else if( setting >= 2 )
        {
            QualitySettings.shadows = ShadowQuality.All;
        }
        else
        {
            QualitySettings.shadows = ShadowQuality.HardOnly;
        }
    }

    // 0 to 3
    public void SetShadowResolution( int setting )
    {
        if( setting <= 0 )
        {
            QualitySettings.shadowResolution = ShadowResolution.Low;
        }
        else if( setting >= 3 )
        {
            QualitySettings.shadowResolution = ShadowResolution.VeryHigh;
        }
        else if( setting == 2 )
        {
            QualitySettings.shadowResolution = ShadowResolution.High;
        }
        else
        {
            QualitySettings.shadowResolution = ShadowResolution.Medium;
        }
    }    

    // 0 to 1
    public void SetShadowProjection( int setting )
    {
        if( setting <= 0 )
        {
            QualitySettings.shadowProjection = ShadowProjection.CloseFit;
        }
        else
        {
            QualitySettings.shadowProjection = ShadowProjection.StableFit;
        }
    }

    // 1 to 1000
    public void SetShadowDistance( float setting )
    {
        setting = Mathf.Max( 0, Mathf.Min( setting, 1000 ) );

        QualitySettings.shadowDistance = setting;
    }     

    //0, 2, 4
    public void SetShadowCascades( int setting )
    {
        setting = Mathf.Max( 0, Mathf.Min( setting, 4 ) );

        if( setting % 2 != 0 )
        {
            setting += 1;
        }

        QualitySettings.shadowCascades = setting;
    }

    //terrain settings

    //0 to 250
    public void SetTerrainDetailDistance( float distance )
    {
        distance = Mathf.Max( 0, Mathf.Min( distance, 250 ) );

        m_requiresSave |= ( SettingsSingleton.GetSettings( ).tDetailDistance != distance );

        SettingsSingleton.GetSettings( ).tDetailDistance = distance;

        PlayerPrefs.SetFloat( SettingsSingleton.tDetailDistanceStr, SettingsSingleton.GetSettings( ).tDetailDistance );
    }

    //0.0 to 1.0
    public void SetTerrainDetailDensity( float density )
    {
        density = Mathf.Max( 0.0f, Mathf.Min( density, 1.0f ) );

        m_requiresSave |= ( SettingsSingleton.GetSettings( ).tDetailDensity != density );

        SettingsSingleton.GetSettings( ).tDetailDensity = density;

        PlayerPrefs.SetFloat( SettingsSingleton.tDetailDensityStr, SettingsSingleton.GetSettings( ).tDetailDensity );
    }

    //0 to 2000
    public void SetTerrainTreeDistance( float distance )
    {
        distance = Mathf.Max( 0, Mathf.Min( distance, 2000 ) );

        m_requiresSave |= ( SettingsSingleton.GetSettings( ).tTreeDistance != distance );

        SettingsSingleton.GetSettings( ).tTreeDistance = distance;

        PlayerPrefs.SetFloat( SettingsSingleton.tTreeDistanceStr, SettingsSingleton.GetSettings( ).tTreeDistance );
    }

    //0 to 2000
    public void SetTerrainBillboardStart( float distance )
    {
        distance = Mathf.Max( 0, Mathf.Min( distance, 2000 ) );

        m_requiresSave |= ( SettingsSingleton.GetSettings( ).tBillboardStart != distance );

        SettingsSingleton.GetSettings( ).tBillboardStart = distance;

        PlayerPrefs.SetFloat( SettingsSingleton.tBillboardStartStr, SettingsSingleton.GetSettings( ).tBillboardStart );
    }

    //0 to 200
    public void SetTerrainFadeLength( float fadeLen )
    {
        fadeLen = Mathf.Max( 0, Mathf.Min( fadeLen, 200 ) );

        m_requiresSave |= ( SettingsSingleton.GetSettings( ).tFadeLength != fadeLen );

        SettingsSingleton.GetSettings( ).tFadeLength = fadeLen;

        PlayerPrefs.SetFloat( SettingsSingleton.tFadeLengthStr, SettingsSingleton.GetSettings( ).tFadeLength );
    }

    //0 to 10000
    public void SetTerrainMaxMeshTrees( int numberOfTrees )
    {
        numberOfTrees = Mathf.Max( 0, Mathf.Min( numberOfTrees, 10000 ) );

        m_requiresSave |= ( SettingsSingleton.GetSettings( ).tMaxMeshTrees != numberOfTrees );

        SettingsSingleton.GetSettings( ).tMaxMeshTrees = numberOfTrees;

        PlayerPrefs.SetInt( SettingsSingleton.tMaxMeshTreesStr, SettingsSingleton.GetSettings( ).tMaxMeshTrees );
    }

    public void Save( )
    {
        if( m_requiresSave )
        {
            PlayerPrefs.Save( );
        }
        

        m_requiresSave = false;
    }

    public void DiscardChanges( )
    {
        SettingsSingleton.GetSettings( ).LoadSettingsFromDisk( );

        m_requiresSave = false;
    }
    

}

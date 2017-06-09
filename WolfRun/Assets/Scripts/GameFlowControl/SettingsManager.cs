using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-600)]
public class SettingsManager : MonoBehaviour
{
    public string[] qualityNames;

    public static readonly int MODE = 0;
    public static readonly float AI_INTELLIGENCE = 0;

    public static readonly float VOLUME = 1.0f;

    public static readonly float T_DETAIL_DISTANCE = 140.0f;
    public static readonly float T_DETAIL_DENSITY = 1.0f;
    public static readonly float T_TREE_DISTANCE = 1000.0f;
    public static readonly float T_BILLBOARD_START = 50.0f;
    public static readonly float T_FADE_LENGTH = 20.0f;
    public static readonly int T_MAX_MESH_TREES = 100;

    public static readonly string MODE_STR = "game-mode";
    public static readonly string AI_INTELLIGENCE_STR = "aiIntelligence";

    public static readonly string VOLUME_STR = "volume-level";

    public static readonly string QUALITY_PRESET_STR = "quality-preset";
    public static readonly string TEXTURE_QUALITY_STR = "master-texture-quality";
    public static readonly string ANISO_STR = "aniso-filter";
    public static readonly string ANTI_ALIASING_STR = "anti-aliasing";
    public static readonly string BILL_BOARD_FACE_STR = "billboard-face-bool";
    public static readonly string BLEND_WEIGHTS_STR = "blendweights";
    public static readonly string LOD_BIAS_STR = "lod-bias";
    public static readonly string REALTIME_REFLECTION_STR = "realtime-reflection-probes";
    public static readonly string SOFT_PARTICLES_STR = "soft-particles";
    public static readonly string SOFT_VEGETATION_STR = "soft-vegetation";
    public static readonly string V_SYNC_COUNT_STR = "v-sync-count";
    public static readonly string SHADOW_QUALITY_STR = "shadow-quality";
    public static readonly string SHADOW_RES_STR = "shadow-resolution";
    public static readonly string SHADOW_PROJ_STR = "shadow-projection";
    public static readonly string SHADOW_DIST_STR = "shadow-distance";
    public static readonly string SHADOW_CASCADES_STR = "shadow-cascades";


    public static readonly string T_DETAIL_DISTANCE_STR = "tDetailDist";
    public static readonly string T_DETAIL_DENSITY_STR = "tDetailDensity";
    public static readonly string T_TREE_DISTANCE_STR = "tTreeDist";
    public static readonly string T_BILLBOARD_START_STR = "tBillboardStart";
    public static readonly string T_FADE_LENGTH_STR = "tFadeLen";
    public static readonly string T_MAX_MESH_TREES_STR = "tMaxMeshTrees";
    


    private static bool m_requiresSave = false;

    // Use this for initialization
    void Start ()
    {
        ApplySettingsRunTime( );
	}

    public void ApplySettingsRunTime( )
    {
        int index;

        ModeController modeControl;

        Terrain[ ] terrains;

        modeControl = FindObjectOfType<ModeController>( );

        terrains = FindObjectsOfType<Terrain>( );

        if ( modeControl )
        {
            modeControl.mode = PlayerPrefs.GetInt( MODE_STR, MODE );
            modeControl.aiIntelligence = PlayerPrefs.GetFloat( AI_INTELLIGENCE_STR, AI_INTELLIGENCE );
        }

        for ( index = 0; index < terrains.Length; index++ )
        {
            terrains[index].detailObjectDensity = PlayerPrefs.GetFloat( T_DETAIL_DENSITY_STR, T_DETAIL_DENSITY );

            terrains[index].detailObjectDistance = PlayerPrefs.GetFloat( T_DETAIL_DISTANCE_STR, T_DETAIL_DISTANCE );

            terrains[index].treeBillboardDistance = PlayerPrefs.GetFloat( T_BILLBOARD_START_STR, T_BILLBOARD_START );

            terrains[index].treeDistance = PlayerPrefs.GetFloat( T_TREE_DISTANCE_STR, T_TREE_DISTANCE );

            terrains[index].treeMaximumFullLODCount = PlayerPrefs.GetInt( T_MAX_MESH_TREES_STR, T_MAX_MESH_TREES );

            terrains[index].treeCrossFadeLength = PlayerPrefs.GetFloat( T_FADE_LENGTH_STR, T_FADE_LENGTH );
        }

        AudioListener.volume = PlayerPrefs.GetFloat( VOLUME_STR, VOLUME );

        SetQualityPreset( PlayerPrefs.GetString( QUALITY_PRESET_STR, "Custom" ) );

        SetToSelectedPreset( );
    }

    public static void SetPlayerPrefs( int val, string playerPrefsName, int defaultVal )
    {
        m_requiresSave |= ( PlayerPrefs.GetInt( playerPrefsName, defaultVal ) == val );

        PlayerPrefs.SetInt( playerPrefsName, val );
    }

    public static void SetPlayerPrefs( float val, string playerPrefsName, float defaultVal )
    {
        m_requiresSave |= ( PlayerPrefs.GetFloat( playerPrefsName, defaultVal ) == val );

        PlayerPrefs.SetFloat( playerPrefsName, val );
    }

    public static void SetPlayerPrefs( string val, string playerPrefsName, string defaultVal )
    {
        m_requiresSave |= ( PlayerPrefs.GetString( playerPrefsName, defaultVal ) == val );

        PlayerPrefs.SetString( playerPrefsName, val );
    }

    public static int BoolToInt( bool val )
    {
        return val ? 1 : 0;
    }

    public static bool IntToBool( int val )
    {
        return val == 0 ? false : true;
    }

    //game difficulty and mode

    public void SetMode( int mode )
    {
        SetPlayerPrefs( mode, MODE_STR, MODE );        
    }

    public void SetOpponentDifficulty( float difficulty )
    {
        SetPlayerPrefs( difficulty, AI_INTELLIGENCE_STR, AI_INTELLIGENCE );
    }

    // audio
    public void SetVolume( float volume )
    {
        SetPlayerPrefs( volume, VOLUME_STR, AudioListener.volume );

        AudioListener.volume = volume;
    }


    // graphics

    //quality settings

    //quality preset
    public void SetQualityPreset( int index )
    {
        if( index > qualityNames.Length )
        {
            return;
        }

        SetQualityPreset( qualityNames[index] );
    }

    //quality preset
    public void SetQualityPreset( string qName )
    {
        string[] names = QualitySettings.names;

        int index = 0;

        for( index = 0; index < names.Length; index++ )
        {
            if( qName == names[ index ] )
            {
                SetPlayerPrefs( qName, QUALITY_PRESET_STR, names[QualitySettings.GetQualityLevel( )] );

                QualitySettings.SetQualityLevel( index, true );

                break;
            }
        }
    }

    //Texture quality 0 is best, 8 is worst, 0, 1, 2, 4
    public void SetTextureQuality( float quality )
    {
        int newVal;

        if( quality < 1 )
        {
            newVal = 0;
        }
        else if( quality < 2 )
        {
            newVal = 1;
        }
        else if( quality < 4 )
        {
            newVal = 2;
        }
        else
        {
            newVal = 4;
        }

        SetPlayerPrefs( newVal, TEXTURE_QUALITY_STR, QualitySettings.masterTextureLimit );

        QualitySettings.masterTextureLimit = newVal;
    }

    // 0 or less for disabled
    // 1 for enabled
    // 2 or greater for force enabled
    public void SetAnisotropicFiltering( float setting )
    {
        int newVal;

        if( setting <= 0 )
        {
            newVal = (int) AnisotropicFiltering.Disable;
        }
        else if( setting >= 2 )
        {
            newVal = ( int ) AnisotropicFiltering.ForceEnable;
        }
        else
        {
            newVal = ( int ) AnisotropicFiltering.Enable;
        }

        SetPlayerPrefs( newVal, ANISO_STR, ( int ) QualitySettings.anisotropicFiltering );

        QualitySettings.anisotropicFiltering = ( AnisotropicFiltering ) newVal;
    }

    
    //0, 2, 4, or 8
    public void SetAntiAliasing( float setting )
    {
        int iSetting;

        iSetting = (int) Mathf.Max( 0, Mathf.Min( setting, 8 ) );

        if( iSetting % 2 != 0 )
        {
            iSetting += 1;
        }

        if( iSetting != 0 && iSetting % 3 == 0 )
        {
            iSetting = 4;
        }

        SetPlayerPrefs( iSetting, ANTI_ALIASING_STR, QualitySettings.antiAliasing );

        QualitySettings.antiAliasing = iSetting;
    }

    
    public void SetBillBoardFaceCamera( bool faceCamera )
    {
        SetPlayerPrefs( BoolToInt( faceCamera ), BILL_BOARD_FACE_STR, BoolToInt( QualitySettings.billboardsFaceCameraPosition ) );

        QualitySettings.billboardsFaceCameraPosition = faceCamera;
    }

    //1, 2, 4
    public void SetBlendWeights( float setting )
    {
        int newVal;

        if( setting <= 1 )
        {
            newVal = ( int ) BlendWeights.OneBone;
        }
        else if( setting > 2 )
        {
            newVal = ( int ) BlendWeights.FourBones;
        }
        else
        {
            newVal = ( int ) BlendWeights.TwoBones;
        }

        SetPlayerPrefs( newVal, BLEND_WEIGHTS_STR, ( int ) QualitySettings.blendWeights );

        QualitySettings.blendWeights = ( BlendWeights ) newVal;
    }


    //should be 0.0f to 1.0f, but in the Unity settings Fantastic is at 2.0f ... so 0.1f to 2.0f ...
    public void SetLodBias( float setting )
    {
        setting = Mathf.Max( 0.1f, Mathf.Min( setting, 2.0f ) );

        SetPlayerPrefs( setting, LOD_BIAS_STR, QualitySettings.lodBias );

        QualitySettings.lodBias = setting;
    }

    public void SetRealtimeReflectionProbes( bool realtimeReflection )
    {
        SetPlayerPrefs( BoolToInt( realtimeReflection ), 
                        REALTIME_REFLECTION_STR, 
                        BoolToInt( QualitySettings.realtimeReflectionProbes ) );

        QualitySettings.realtimeReflectionProbes = realtimeReflection;
    }

    public void SetSoftParticles( bool softParticles )
    {
        SetPlayerPrefs( BoolToInt( softParticles ), 
                        SOFT_PARTICLES_STR, 
                        BoolToInt( QualitySettings.softParticles ) );

        QualitySettings.softParticles = softParticles;
    }

    public void SetSoftVegetation(  bool softVegetation )
    {
        SetPlayerPrefs( BoolToInt( softVegetation ), 
                        SOFT_VEGETATION_STR, 
                        BoolToInt( QualitySettings.softVegetation ) );

        QualitySettings.softVegetation = softVegetation;
    }

    //0 to 2
    public void SetvSyncCount( float setting )
    {
        setting = Mathf.Max( 0, Mathf.Min( setting, 2 ) );

        SetPlayerPrefs( ( int ) setting, V_SYNC_COUNT_STR, QualitySettings.vSyncCount );

        QualitySettings.vSyncCount = (int) setting;
    }

    //0 to 2
    public void SetShadowQuality( float setting )
    {
        int newVal;

        if( setting <= 0 )
        {
            newVal = ( int ) ShadowQuality.Disable;
        }
        else if( setting >= 2 )
        {
            newVal = ( int ) ShadowQuality.All;
        }
        else
        {
            newVal = ( int ) ShadowQuality.HardOnly;
        }

        SetPlayerPrefs( newVal, SHADOW_QUALITY_STR, ( int ) QualitySettings.shadows );

        QualitySettings.shadows = ( ShadowQuality ) newVal;
    }

    // 0 to 3
    public void SetShadowResolution( float setting )
    {
        int newVal;

        if( setting <= 0 )
        {
            newVal = ( int ) ShadowResolution.Low;
        }
        else if( setting >= 3 )
        {
            newVal = ( int ) ShadowResolution.VeryHigh;
        }
        else if( setting == 2 )
        {
            newVal = ( int ) ShadowResolution.High;
        }
        else
        {
            newVal = ( int ) ShadowResolution.Medium;
        }

        SetPlayerPrefs( newVal, SHADOW_RES_STR, ( int ) QualitySettings.shadowResolution );

        QualitySettings.shadowResolution = ( ShadowResolution ) newVal;
    }    

    // 0 to 1
    public void SetShadowProjection( float setting )
    {
        if( setting <= 0 )
        {
            SetPlayerPrefs( ( int ) ShadowProjection.CloseFit, 
                            SHADOW_PROJ_STR, 
                            ( int ) QualitySettings.shadowProjection );

            QualitySettings.shadowProjection = ShadowProjection.CloseFit;
        }
        else
        {
            SetPlayerPrefs( ( int ) ShadowProjection.StableFit,
                            SHADOW_PROJ_STR,
                            ( int ) QualitySettings.shadowProjection );

            QualitySettings.shadowProjection = ShadowProjection.StableFit;
        }
    }

    // 1 to 1000
    public void SetShadowDistance( float setting )
    {
        setting = Mathf.Max( 1, Mathf.Min( setting, 1000 ) );

        SetPlayerPrefs( setting, SHADOW_DIST_STR, QualitySettings.shadowDistance );

        QualitySettings.shadowDistance = setting;
    }     

    //0, 2, 4
    public void SetShadowCascades( float setting )
    {
        int iSetting;
        iSetting = (int) Mathf.Max( 0, Mathf.Min( setting, 4 ) );

        if( iSetting % 2 != 0 )
        {
            iSetting += 1;
        }

        SetPlayerPrefs( iSetting, SHADOW_CASCADES_STR, QualitySettings.shadowCascades );

        QualitySettings.shadowCascades = iSetting;
    }

    //terrain settings

    //0 to 250
    public void SetTerrainDetailDistance( float distance )
    {
        distance = Mathf.Max( 0, Mathf.Min( distance, 250 ) );

        SetPlayerPrefs( distance, T_DETAIL_DISTANCE_STR, T_DETAIL_DISTANCE );
    }

    //0.0 to 1.0
    public void SetTerrainDetailDensity( float density )
    {
        density = Mathf.Max( 0.0f, Mathf.Min( density, 1.0f ) );

        SetPlayerPrefs( density, T_DETAIL_DENSITY_STR, T_DETAIL_DENSITY );
    }

    //0 to 2000
    public void SetTerrainTreeDistance( float distance )
    {
        distance = Mathf.Max( 0, Mathf.Min( distance, 2000 ) );

        SetPlayerPrefs( distance, T_TREE_DISTANCE_STR, T_TREE_DISTANCE );
    }

    //0 to 2000
    public void SetTerrainBillboardStart( float distance )
    {
        distance = Mathf.Max( 0, Mathf.Min( distance, 2000 ) );

        SetPlayerPrefs( distance, T_BILLBOARD_START_STR, T_BILLBOARD_START );
    }

    //0 to 200
    public void SetTerrainFadeLength( float fadeLen )
    {
        fadeLen = Mathf.Max( 0, Mathf.Min( fadeLen, 200 ) );
        
        SetPlayerPrefs( fadeLen, T_FADE_LENGTH_STR, T_FADE_LENGTH );
    }

    //0 to 10000
    public void SetTerrainMaxMeshTrees( float numberOfTrees )
    {
        numberOfTrees = Mathf.Max( 0, Mathf.Min( numberOfTrees, 10000 ) );

        SetPlayerPrefs( ( int ) numberOfTrees, T_MAX_MESH_TREES_STR, T_MAX_MESH_TREES );
    }

    public void Save( )
    {
        if( m_requiresSave )
        {
            PlayerPrefs.Save( );
        }       

        m_requiresSave = false;
    }    
    
    public void SetToSelectedPreset( )
    {
        if ( QualitySettings.names[QualitySettings.GetQualityLevel( )] == "Custom" )
        {
            SetTextureQuality( PlayerPrefs.GetInt( TEXTURE_QUALITY_STR, QualitySettings.masterTextureLimit ) );

            SetAnisotropicFiltering( PlayerPrefs.GetInt( ANISO_STR, ( int ) QualitySettings.anisotropicFiltering ) );

            SetAntiAliasing( PlayerPrefs.GetInt( ANTI_ALIASING_STR, QualitySettings.antiAliasing ) );

            SetBillBoardFaceCamera( IntToBool( PlayerPrefs.GetInt( BILL_BOARD_FACE_STR, BoolToInt( QualitySettings.billboardsFaceCameraPosition ) ) ) );

            SetBlendWeights( PlayerPrefs.GetInt( BLEND_WEIGHTS_STR, ( int ) QualitySettings.blendWeights ) );

            SetLodBias( PlayerPrefs.GetFloat( LOD_BIAS_STR, QualitySettings.lodBias ) );

            SetRealtimeReflectionProbes( IntToBool( PlayerPrefs.GetInt( REALTIME_REFLECTION_STR, BoolToInt( QualitySettings.realtimeReflectionProbes ) ) ) );

            SetSoftParticles( IntToBool( PlayerPrefs.GetInt( SOFT_PARTICLES_STR, BoolToInt( QualitySettings.softParticles ) ) ) );

            SetSoftVegetation( IntToBool( PlayerPrefs.GetInt( SOFT_VEGETATION_STR, BoolToInt( QualitySettings.softVegetation ) ) ) );

            SetvSyncCount( PlayerPrefs.GetInt( V_SYNC_COUNT_STR, QualitySettings.vSyncCount ) );

            SetShadowQuality( PlayerPrefs.GetInt( SHADOW_QUALITY_STR, ( int ) QualitySettings.shadows ) );

            SetShadowResolution( PlayerPrefs.GetInt( SHADOW_RES_STR, ( int ) QualitySettings.shadowResolution ) );

            SetShadowProjection( PlayerPrefs.GetInt( SHADOW_PROJ_STR, ( int ) QualitySettings.shadowProjection ) );

            SetShadowDistance( PlayerPrefs.GetFloat( SHADOW_DIST_STR, QualitySettings.shadowDistance ) );

            SetShadowCascades( PlayerPrefs.GetInt( SHADOW_CASCADES_STR, QualitySettings.shadowCascades ) );
        }
        else
        {
            SetTextureQuality( QualitySettings.masterTextureLimit );
            SetAnisotropicFiltering( ( float ) QualitySettings.anisotropicFiltering );
            SetAntiAliasing( QualitySettings.antiAliasing );
            SetBillBoardFaceCamera( QualitySettings.billboardsFaceCameraPosition );
            SetBlendWeights( ( float ) QualitySettings.blendWeights );
            SetLodBias( QualitySettings.lodBias );
            SetRealtimeReflectionProbes( QualitySettings.realtimeReflectionProbes );
            SetSoftParticles( QualitySettings.softParticles );
            SetSoftVegetation( QualitySettings.softVegetation );
            SetvSyncCount( QualitySettings.vSyncCount );
            SetShadowQuality( ( float ) QualitySettings.shadows );
            SetShadowResolution( ( float ) QualitySettings.shadowResolution );
            SetShadowProjection( ( float ) QualitySettings.shadowProjection );
            SetShadowDistance( QualitySettings.shadowDistance );
            SetShadowCascades( QualitySettings.shadowCascades );
        }
        
        //SetTerrainDetailDistance( PlayerPrefs.GetFloat( T_DETAIL_DISTANCE_STR, T_DETAIL_DISTANCE ) );
        //SetTerrainDetailDensity( PlayerPrefs.GetFloat( T_DETAIL_DENSITY_STR, T_DETAIL_DENSITY ) );
        //SetTerrainTreeDistance( PlayerPrefs.GetFloat( T_TREE_DISTANCE_STR, T_TREE_DISTANCE ) );
        //SetTerrainBillboardStart( PlayerPrefs.GetFloat( T_BILLBOARD_START_STR, T_BILLBOARD_START ) );
        //SetTerrainFadeLength( PlayerPrefs.GetFloat( T_FADE_LENGTH_STR, T_FADE_LENGTH ) );
        //SetTerrainMaxMeshTrees( PlayerPrefs.GetInt( T_MAX_MESH_TREES_STR, T_MAX_MESH_TREES ) );
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QualityPresetDropDown : MonoBehaviour
{

    public string customName = "Custom";
    
    private SliderManager[] m_qualitySliderManagers;

    private Toggle[] m_toggles;
    private ToggleManager[] m_toggleManagers;

    private Dropdown m_dropDown;

    

    private void OnEnable( )
    {
        List<Dropdown.OptionData> options;
        int index = 0;

        m_dropDown = GetComponentInChildren<Dropdown>( );
        
        m_qualitySliderManagers = GetComponentsInChildren<SliderManager>( );

        m_toggles = GetComponentsInChildren<Toggle>( );
        m_toggleManagers = GetComponentsInChildren<ToggleManager>( );

        options = m_dropDown.options;

        for( index = 0; index < options.Count; index++ )
        {
            if( QualitySettings.names[QualitySettings.GetQualityLevel( )] == options[ index ].text )
            {
                m_dropDown.value = index;
                break;
            }
        }

        OnOptionChange( );
    }

    public void OnOptionChange( )
    {
        int index;
        bool interactable;

        //sliders

        interactable = ( m_dropDown.options[m_dropDown.value].text == customName );

        for ( index  = 0; index < m_qualitySliderManagers.Length; index++ )
        {
            m_qualitySliderManagers[index].OnSliderMove( );

            if( m_qualitySliderManagers[index].qualitySetting )
            {
                m_qualitySliderManagers[index].SetInteractable( interactable );
            }
            else
            {
                m_qualitySliderManagers[index].SetInteractable( true );
            }
            
        }

        //toggles
        for ( index = 0; index < m_toggleManagers.Length; index++ )
        {
            m_toggleManagers[index].OnValueSelect( );
        }

        interactable = ( m_dropDown.options[m_dropDown.value].text == customName );

        for ( index = 0; index < m_toggles.Length; index++ )
        {
            m_toggles[index].interactable = interactable;
        }

    }

}

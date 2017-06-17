using UnityEngine;
using UnityEngine.UI;

public class SliderManager : MonoBehaviour
{
    public string prefsName;

    public bool isInt = false;

    public bool qualitySetting = false;

    public bool interactabilityCanBeModified = true;

    private Slider m_slider;    

    private bool m_changed = false;

    private Text m_sliderText;

    private void OnEnable( )
    {
        m_slider = GetComponent<Slider>( );

        m_slider.value = isInt ? PlayerPrefs.GetInt( prefsName ) : PlayerPrefs.GetFloat( prefsName );

        m_sliderText = GetComponentInChildren<Text>( );

        if ( m_sliderText )
        {
            m_sliderText.text = m_slider.value.ToString( isInt ? "0" : "0.0" );
        }
    }

    private void Update( )
    {
        if ( m_slider && m_changed )
        {
            m_slider.value = isInt ? PlayerPrefs.GetInt( prefsName ) : PlayerPrefs.GetFloat( prefsName );

            if( m_sliderText )
            {
                m_sliderText.text = m_slider.value.ToString( isInt ? "0" : "0.0" );
            }
            

            m_changed = false;
        }
    }

    
    public void OnSliderMove()
    {
        m_changed = true;
    }

    public void SetInteractable( bool interactable )
    {
        if ( m_slider && interactabilityCanBeModified )
        {
            m_slider.interactable = interactable;
        }
    }

    
}

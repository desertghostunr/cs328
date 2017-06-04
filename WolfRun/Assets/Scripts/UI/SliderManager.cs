using UnityEngine;
using UnityEngine.UI;

public class SliderManager : MonoBehaviour
{
    public string playerPrefsName;

    public bool isInt = false;

    private Slider m_slider;

    private void OnEnable( )
    {
        m_slider = GetComponent<Slider>( );

        m_slider.value = isInt ? PlayerPrefs.GetInt( playerPrefsName ) : PlayerPrefs.GetFloat( playerPrefsName );
        
    }
}

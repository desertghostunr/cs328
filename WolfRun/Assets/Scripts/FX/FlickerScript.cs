using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Light))]
public class FlickerScript : MonoBehaviour
{
    public float flickerRate = 0.5f;
    public float brightnessDelta = 0.5f;

    public float offsetDelta = 0.25f;

    private Light m_light;

    private float maxLightIntensity;
    private float minLightIntensity;

    private float timeAccumulated;

    private float randomOffset;

	// Use this for initialization
	void Start ()
    {
        m_light = GetComponent<Light>( );

        maxLightIntensity = m_light.intensity;

        minLightIntensity = ( 1.0f - brightnessDelta ) * maxLightIntensity;

        timeAccumulated = 0;

        randomOffset = Random.Range( 0.0f, maxLightIntensity * offsetDelta );
    }
	
	// Update is called once per frame
	void Update ()
    {
        timeAccumulated += Time.deltaTime;
	
        if( timeAccumulated <= flickerRate )
        {
            
            m_light.intensity = Mathf.SmoothStep( minLightIntensity, 
                                                  maxLightIntensity + randomOffset, 
                                                  timeAccumulated / flickerRate );
        }
        else
        {
            randomOffset = Random.Range( 0.0f, maxLightIntensity * offsetDelta );
            timeAccumulated = 0;
        }
	}
}
